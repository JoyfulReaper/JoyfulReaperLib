using JoyfulReaperLib.JRNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace JoyfulReaperLib.TcpServer;

/// <summary>
/// Hosts a bounded TCP listener and dispatches accepted connections to a
/// protocol-specific connection handler.
/// </summary>
public sealed class TcpServerHostedService<THandler, TOptions>
    : BackgroundService
    where THandler : class, ITcpConnectionHandler
    where TOptions : class, ITcpServerOptions
{
    private readonly ILogger<TcpServerHostedService<THandler, TOptions>> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TOptions _options;
    private readonly SemaphoreSlim _connectionLimit;
    private readonly ConcurrentDictionary<long, Task> _activeConnections = new();

    private TcpListener? _listener;
    private volatile bool _stopRequested;
    private long _nextConnectionId;

    public TcpServerHostedService(
        ILogger<TcpServerHostedService<THandler, TOptions>> logger,
        IServiceScopeFactory scopeFactory,
        IOptions<TOptions> options)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(scopeFactory);
        ArgumentNullException.ThrowIfNull(options);

        _logger = logger;
        _scopeFactory = scopeFactory;
        _options = options.Value;

        ValidateOptions(_options);

        _connectionLimit = new SemaphoreSlim(
            _options.MaxConcurrentConnections,
            _options.MaxConcurrentConnections);
    }

    /// <summary>
    /// Gets the endpoint on which the server is currently listening.
    /// </summary>
    public IPEndPoint? BoundEndpoint { get; private set; }

    /// <inheritdoc />
    public override async Task StartAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IPAddress listenAddress =
            IPAddressUtils.ParseListenAddress(
                _options.ListenAddress);

        var listener = new TcpListener(
            listenAddress,
            _options.Port);

        try
        {
            listener.Start();

            _listener = listener;
            BoundEndpoint =
                (IPEndPoint)listener.LocalEndpoint;

            _logger.LogInformation(
                "TCP server for {HandlerType} started on {Address}:{Port}.",
                typeof(THandler).Name,
                BoundEndpoint.Address,
                BoundEndpoint.Port);

            await base.StartAsync(
                cancellationToken);
        }
        catch
        {
            listener.Stop();

            _listener = null;
            BoundEndpoint = null;

            throw;
        }
    }

    /// <inheritdoc />
    /// <inheritdoc />
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        TcpListener listener =
            _listener ??
            throw new InvalidOperationException(
                "The TCP listener was not initialized before execution.");

        try
        {
            while (!_stopRequested &&
                   !stoppingToken.IsCancellationRequested)
            {
                TcpClient? client = null;
                bool permitAcquired = false;

                try
                {
                    if (_options.ConnectionLimitBehavior is
                        ConnectionLimitBehavior.Wait)
                    {
                        // Allow the operating system's listen backlog to hold
                        // pending connections until the application has room.
                        await _connectionLimit.WaitAsync(stoppingToken);
                        permitAcquired = true;

                        client = await listener.AcceptTcpClientAsync(
                            stoppingToken);
                    }
                    else
                    {
                        client = await listener.AcceptTcpClientAsync(
                            stoppingToken);

                        permitAcquired = _connectionLimit.Wait(0);

                        if (!permitAcquired)
                        {
                            _logger.LogDebug(
                                "Rejected TCP connection from {Remote} because all {MaximumConnections} connection slots are occupied.",
                                client.Client.RemoteEndPoint,
                                _options.MaxConcurrentConnections);

                            client.Dispose();
                            client = null;

                            continue;
                        }
                    }

                    long connectionId =
                        Interlocked.Increment(ref _nextConnectionId);

                    DateTimeOffset acceptedAt =
                        DateTimeOffset.UtcNow;

                    Task connectionTask = HandleClientAsync(
                        connectionId,
                        client,
                        acceptedAt,
                        stoppingToken);

                    client = null;
                    permitAcquired = false;

                    TrackConnection(
                        connectionId,
                        connectionTask);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested ||
                          _stopRequested)
                {
                    client?.Dispose();

                    if (permitAcquired)
                    {
                        _connectionLimit.Release();
                    }

                    break;
                }
                catch (SocketException)
                    when (stoppingToken.IsCancellationRequested ||
                          _stopRequested)
                {
                    client?.Dispose();

                    if (permitAcquired)
                    {
                        _connectionLimit.Release();
                    }

                    break;
                }
                catch (ObjectDisposedException)
                    when (stoppingToken.IsCancellationRequested ||
                          _stopRequested)
                {
                    client?.Dispose();

                    if (permitAcquired)
                    {
                        _connectionLimit.Release();
                    }

                    break;
                }
                catch
                {
                    client?.Dispose();

                    if (permitAcquired)
                    {
                        _connectionLimit.Release();
                    }

                    throw;
                }
            }
        }
        finally
        {
            listener.Stop();

            await DrainConnectionsAsync();

            BoundEndpoint = null;
            _listener = null;

            _logger.LogInformation(
                "TCP server for {HandlerType} stopped.",
                typeof(THandler).Name);
        }
    }

    private async Task HandleClientAsync(
        long connectionId,
        TcpClient client,
        DateTimeOffset acceptedAt,
        CancellationToken stoppingToken)
    {
        try
        {
            using (client)
            {
                client.NoDelay = true;

                EndPoint? remoteEndpoint =
                    client.Client.RemoteEndPoint;

                EndPoint? localEndpoint =
                    client.Client.LocalEndPoint;

                await using NetworkStream stream =
                    client.GetStream();

                await using AsyncServiceScope scope =
                    _scopeFactory.CreateAsyncScope();

                THandler handler =
                    scope.ServiceProvider
                        .GetRequiredService<THandler>();

                var context = new TcpConnectionContext(
                    ConnectionId: connectionId,
                    Stream: stream,
                    RemoteEndPoint: remoteEndpoint,
                    LocalEndPoint: localEndpoint,
                    AcceptedAt: acceptedAt);

                await handler.HandleAsync(
                    context,
                    stoppingToken);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug(
                "TCP connection {ConnectionId} was cancelled during server shutdown.",
                connectionId);
        }
        finally
        {
            _connectionLimit.Release();
        }
    }

    private void TrackConnection(
        long connectionId,
        Task connectionTask)
    {
        _activeConnections[connectionId] =
            connectionTask;

        _ = connectionTask.ContinueWith(
            completedTask =>
            {
                _activeConnections.TryRemove(
                    connectionId,
                    out _);

                if (completedTask.IsFaulted)
                {
                    _logger.LogError(
                        completedTask.Exception,
                        "TCP connection {ConnectionId} handled by {HandlerType} failed unexpectedly.",
                        connectionId,
                        typeof(THandler).Name);
                }
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }

    private async Task DrainConnectionsAsync()
    {
        Task[] remainingConnections =
            _activeConnections.Values.ToArray();

        if (remainingConnections.Length == 0)
        {
            return;
        }

        try
        {
            await Task.WhenAll(remainingConnections);
        }
        catch
        {
            // Individual failures are logged by TrackConnection.
        }
    }

    /// <inheritdoc />
    public override Task StopAsync(
        CancellationToken cancellationToken)
    {
        _stopRequested = true;
        _listener?.Stop();

        return base.StopAsync(cancellationToken);
    }

    private static void ValidateOptions(
        TOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ListenAddress))
        {
            throw new InvalidOperationException(
                "The TCP listen address must not be empty.");
        }

        if (options.Port is < 0 or > 65535)
        {
            throw new InvalidOperationException(
                "The TCP port must be between 0 and 65535.");
        }

        if (options.MaxConcurrentConnections <= 0)
        {
            throw new InvalidOperationException(
                "The maximum concurrent connection count must be positive.");
        }

        if (!Enum.IsDefined(
                typeof(ConnectionLimitBehavior),
                options.ConnectionLimitBehavior))
        {
            throw new InvalidOperationException(
                "The configured connection-limit behavior is invalid.");
        }
    }
}