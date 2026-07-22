using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;

namespace JoyfulReaperLib.TcpServer.Tests;

public sealed class TcpServerHostedServiceTests
{
    [Fact]
    public async Task AfterCloseWork_RunsAfterClientHasBeenClosed()
    {
        var probe = new AfterCloseProbe();

        using ServiceProvider provider =
            BuildProvider<AfterCloseHandler>(
                services => services.AddSingleton(probe));

        using TcpServerHostedService<AfterCloseHandler, TestOptions> server =
            CreateServer<AfterCloseHandler>(
                provider,
                ValidOptions);

        try
        {
            await server.StartAsync(
                CancellationToken.None);

            IPEndPoint endpoint =
                Assert.IsType<IPEndPoint>(
                    server.BoundEndpoint);

            using var client = new TcpClient();

            await client.ConnectAsync(
                IPAddress.Loopback,
                endpoint.Port);

            await probe.AfterCloseStarted.Task.WaitAsync(
                TimeSpan.FromSeconds(5));

            NetworkStream stream =
                client.GetStream();

            byte[] buffer = new byte[1];

            using var readTimeout =
                new CancellationTokenSource(
                    TimeSpan.FromSeconds(5));

            try
            {
                int bytesRead =
                    await stream.ReadAsync(
                        buffer,
                        readTimeout.Token);

                Assert.Equal(0, bytesRead);
            }
            catch (IOException)
            {
                // A connection reset also proves that the socket was closed.
            }

            Assert.False(
                probe.ReleaseAfterClose.Task.IsCompleted);
        }
        finally
        {
            probe.ReleaseAfterClose.TrySetResult(true);

            await server.StopAsync(
                CancellationToken.None);
        }
    }

    [Fact]
    public async Task AfterCloseWork_DoesNotHoldConnectionSlot()
    {
        var probe = new AfterCloseProbe();

        TestOptions options = ValidOptions with
        {
            MaxConcurrentConnections = 1,
            ConnectionLimitBehavior =
                ConnectionLimitBehavior.Wait
        };

        using ServiceProvider provider =
            BuildProvider<AfterCloseHandler>(
                services => services.AddSingleton(probe));

        using TcpServerHostedService<AfterCloseHandler, TestOptions> server =
            CreateServer<AfterCloseHandler>(
                provider,
                options);

        try
        {
            await server.StartAsync(
                CancellationToken.None);

            IPEndPoint endpoint =
                Assert.IsType<IPEndPoint>(
                    server.BoundEndpoint);

            using var firstClient = new TcpClient();

            await firstClient.ConnectAsync(
                IPAddress.Loopback,
                endpoint.Port);

            await probe.AfterCloseStarted.Task.WaitAsync(
                TimeSpan.FromSeconds(5));

            Assert.False(
                probe.ReleaseAfterClose.Task.IsCompleted);

            using var secondClient = new TcpClient();

            await secondClient.ConnectAsync(
                IPAddress.Loopback,
                endpoint.Port);

            await probe.SecondConnectionHandled.Task.WaitAsync(
                TimeSpan.FromSeconds(5));

            Assert.Equal(
                2,
                Volatile.Read(ref probe.InvocationCount));
        }
        finally
        {
            probe.ReleaseAfterClose.TrySetResult(true);

            await server.StopAsync(
                CancellationToken.None);
        }
    }

    [Fact]
    public void Constructor_RejectsEmptyListenAddress()
    {
        var options = ValidOptions with
        {
            ListenAddress = " "
        };

        using ServiceProvider provider =
            BuildProvider<RecordingHandler>(
                services => services.AddSingleton(new ConnectionProbe()));

        Assert.Throws<InvalidOperationException>(
            () => CreateServer<RecordingHandler>(
                provider,
                options));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(65536)]
    public void Constructor_RejectsInvalidPort(
        int port)
    {
        var options = ValidOptions with
        {
            Port = port
        };

        using ServiceProvider provider =
            BuildProvider<RecordingHandler>(
                services => services.AddSingleton(new ConnectionProbe()));

        Assert.Throws<InvalidOperationException>(
            () => CreateServer<RecordingHandler>(
                provider,
                options));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_RejectsInvalidConnectionLimit(
        int maximumConnections)
    {
        var options = ValidOptions with
        {
            MaxConcurrentConnections = maximumConnections
        };

        using ServiceProvider provider =
            BuildProvider<RecordingHandler>(
                services => services.AddSingleton(new ConnectionProbe()));

        Assert.Throws<InvalidOperationException>(
            () => CreateServer<RecordingHandler>(
                provider,
                options));
    }

    [Fact]
    public void Constructor_RejectsUnknownLimitBehavior()
    {
        var options = ValidOptions with
        {
            ConnectionLimitBehavior =
                (ConnectionLimitBehavior)999
        };

        using ServiceProvider provider =
            BuildProvider<RecordingHandler>(
                services => services.AddSingleton(new ConnectionProbe()));

        Assert.Throws<InvalidOperationException>(
            () => CreateServer<RecordingHandler>(
                provider,
                options));
    }

    [Fact]
    public async Task Server_DispatchesAcceptedConnectionToHandler()
    {
        var probe = new ConnectionProbe();

        using ServiceProvider provider =
            BuildProvider<RecordingHandler>(
                services => services.AddSingleton(probe));

        using TcpServerHostedService<RecordingHandler, TestOptions> server =
            CreateServer<RecordingHandler>(
                provider,
                ValidOptions);

        try
        {
            await server.StartAsync(
                CancellationToken.None);

            IPEndPoint endpoint =
                Assert.IsType<IPEndPoint>(
                    server.BoundEndpoint);

            using var client = new TcpClient();

            await client.ConnectAsync(
                IPAddress.Loopback,
                endpoint.Port);

            ObservedConnection observed =
                await probe.Accepted.Task.WaitAsync(
                    TimeSpan.FromSeconds(5));

            Assert.Equal(1, observed.ConnectionId);
            Assert.Equal(
                endpoint.Port,
                observed.LocalEndpoint.Port);

            Assert.True(observed.StreamWasReadable);
            Assert.NotNull(observed.RemoteEndpoint);
        }
        finally
        {
            await server.StopAsync(
                CancellationToken.None);
        }
    }

    [Fact]
    public async Task StopAsync_CancelsAndDrainsActiveHandler()
    {
        var probe = new BlockingProbe();

        using ServiceProvider provider =
            BuildProvider<BlockingHandler>(
                services => services.AddSingleton(probe));

        using TcpServerHostedService<BlockingHandler, TestOptions> server =
            CreateServer<BlockingHandler>(
                provider,
                ValidOptions);

        await server.StartAsync(
            CancellationToken.None);

        IPEndPoint endpoint =
            Assert.IsType<IPEndPoint>(
                server.BoundEndpoint);

        using var client = new TcpClient();

        await client.ConnectAsync(
            IPAddress.Loopback,
            endpoint.Port);

        await probe.Started.Task.WaitAsync(
            TimeSpan.FromSeconds(5));

        using var stopTimeout =
            new CancellationTokenSource(
                TimeSpan.FromSeconds(5));

        await server.StopAsync(
            stopTimeout.Token);

        bool cancellationWasRequested =
            await probe.Finished.Task.WaitAsync(
                TimeSpan.FromSeconds(5));

        Assert.True(cancellationWasRequested);
    }

    [Fact]
    public async Task RejectBehavior_ClosesConnectionWhenServerIsFull()
    {
        var probe = new BlockingProbe();

        TestOptions options = ValidOptions with
        {
            MaxConcurrentConnections = 1,
            ConnectionLimitBehavior =
                ConnectionLimitBehavior.Reject
        };

        using ServiceProvider provider =
            BuildProvider<BlockingHandler>(
                services => services.AddSingleton(probe));

        using TcpServerHostedService<BlockingHandler, TestOptions> server =
            CreateServer<BlockingHandler>(
                provider,
                options);

        try
        {
            await server.StartAsync(
                CancellationToken.None);

            IPEndPoint endpoint =
                Assert.IsType<IPEndPoint>(
                    server.BoundEndpoint);

            using var firstClient =
                new TcpClient();

            await firstClient.ConnectAsync(
                IPAddress.Loopback,
                endpoint.Port);

            await probe.Started.Task.WaitAsync(
                TimeSpan.FromSeconds(5));

            using var secondClient =
                new TcpClient();

            await secondClient.ConnectAsync(
                IPAddress.Loopback,
                endpoint.Port);

            NetworkStream secondStream =
                secondClient.GetStream();

            byte[] buffer = new byte[1];

            using var rejectionTimeout =
                new CancellationTokenSource(
                    TimeSpan.FromSeconds(5));

            try
            {
                int bytesRead =
                    await secondStream.ReadAsync(
                        buffer,
                        rejectionTimeout.Token);

                Assert.Equal(0, bytesRead);
            }
            catch (IOException)
            {
                // A reset rather than a graceful EOF is also a valid
                // immediate rejection.
            }

            Assert.Equal(
                1,
                Volatile.Read(ref probe.InvocationCount));
        }
        finally
        {
            await server.StopAsync(
                CancellationToken.None);
        }
    }

    private static TcpServerHostedService<THandler, TestOptions>
        CreateServer<THandler>(
            ServiceProvider provider,
            TestOptions options)
        where THandler : class, ITcpConnectionHandler
    {
        return new TcpServerHostedService<THandler, TestOptions>(
            NullLogger<
                TcpServerHostedService<THandler, TestOptions>>
                .Instance,
            provider.GetRequiredService<IServiceScopeFactory>(),
            Options.Create(options));
    }

    private static ServiceProvider BuildProvider<THandler>(
        Action<IServiceCollection> configureServices)
        where THandler : class, ITcpConnectionHandler
    {
        var services =
            new ServiceCollection();

        configureServices(services);

        services.AddScoped<THandler>();

        return services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
    }

    private static TestOptions ValidOptions =>
        new()
        {
            ListenAddress = "127.0.0.1",
            Port = 0,
            MaxConcurrentConnections = 4,
            ConnectionLimitBehavior =
                ConnectionLimitBehavior.Wait
        };

    private sealed record TestOptions
        : ITcpServerOptions
    {
        public required string ListenAddress { get; init; }

        public required int Port { get; init; }

        public required int MaxConcurrentConnections { get; init; }

        public required ConnectionLimitBehavior
            ConnectionLimitBehavior
        { get; init; }
    }

    private sealed class ConnectionProbe
    {
        public TaskCompletionSource<ObservedConnection> Accepted
        { get; } =
            new(
                TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private sealed record ObservedConnection(
        long ConnectionId,
        IPEndPoint LocalEndpoint,
        IPEndPoint? RemoteEndpoint,
        bool StreamWasReadable);

    private sealed class RecordingHandler(
        ConnectionProbe probe)
        : ITcpConnectionHandler
    {
        public ValueTask HandleAsync(
            TcpConnectionContext context,
            CancellationToken cancellationToken)
        {
            var localEndpoint =
                Assert.IsType<IPEndPoint>(
                    context.LocalEndPoint);

            var remoteEndpoint =
                context.RemoteEndPoint as IPEndPoint;

            probe.Accepted.TrySetResult(
                new ObservedConnection(
                    context.ConnectionId,
                    localEndpoint,
                    remoteEndpoint,
                    context.Stream.CanRead));

            return ValueTask.CompletedTask;
        }
    }

    private sealed class BlockingProbe
    {
        public TaskCompletionSource<bool> Started
        { get; } =
            new(
                TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> Finished
        { get; } =
            new(
                TaskCreationOptions.RunContinuationsAsynchronously);

        public int InvocationCount;
    }

    private sealed class BlockingHandler(
        BlockingProbe probe)
        : ITcpConnectionHandler
    {
        public async ValueTask HandleAsync(
            TcpConnectionContext context,
            CancellationToken cancellationToken)
        {
            Interlocked.Increment(
                ref probe.InvocationCount);

            probe.Started.TrySetResult(true);

            try
            {
                await Task.Delay(
                    Timeout.InfiniteTimeSpan,
                    cancellationToken);
            }
            finally
            {
                probe.Finished.TrySetResult(
                    cancellationToken.IsCancellationRequested);
            }
        }
    }

    private sealed class AfterCloseProbe
    {
        public TaskCompletionSource<bool> AfterCloseStarted
        { get; } =
            new(
                TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> ReleaseAfterClose
        { get; } =
            new(
                TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> SecondConnectionHandled
        { get; } =
            new(
                TaskCreationOptions.RunContinuationsAsynchronously);

        public int InvocationCount;

        public async ValueTask WaitForReleaseAsync(
            CancellationToken cancellationToken)
        {
            AfterCloseStarted.TrySetResult(true);

            await ReleaseAfterClose.Task.WaitAsync(
                cancellationToken);
        }
    }

    private sealed class AfterCloseHandler(
        AfterCloseProbe probe)
        : ITcpConnectionHandler
    {
        public ValueTask HandleAsync(
            TcpConnectionContext context,
            CancellationToken cancellationToken)
        {
            int invocation =
                Interlocked.Increment(
                    ref probe.InvocationCount);

            if (invocation == 1)
            {
                context.RegisterAfterClose(
                    probe.WaitForReleaseAsync);
            }
            else if (invocation == 2)
            {
                probe.SecondConnectionHandled.TrySetResult(true);
            }

            return ValueTask.CompletedTask;
        }
    }
}