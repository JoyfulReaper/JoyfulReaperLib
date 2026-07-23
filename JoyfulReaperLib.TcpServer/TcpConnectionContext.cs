using System.Net;

namespace JoyfulReaperLib.TcpServer;

/// <summary>
/// Contains the infrastructure-owned state associated with an accepted TCP
/// connection.
/// </summary>
public sealed class TcpConnectionContext
{
    private Func<CancellationToken, ValueTask>? _afterClose;

    public TcpConnectionContext(
        long connectionId,
        Stream stream,
        EndPoint? remoteEndPoint,
        EndPoint? localEndPoint,
        DateTimeOffset acceptedAt)
    {
        ArgumentNullException.ThrowIfNull(stream);

        ConnectionId = connectionId;
        Stream = stream;
        RemoteEndPoint = remoteEndPoint;
        LocalEndPoint = localEndPoint;
        AcceptedAt = acceptedAt;
    }

    public long ConnectionId { get; }

    public Stream Stream { get; }

    public EndPoint? RemoteEndPoint { get; }

    public EndPoint? LocalEndPoint { get; }

    public DateTimeOffset AcceptedAt { get; }

    /// <summary>
    /// Registers work to run after the network stream and client have been
    /// disposed and the connection slot has been released.
    /// </summary>
    /// <remarks>
    /// The callback must not use <see cref="Stream"/> because the connection
    /// is already closed when it runs.
    /// </remarks>
    public void RegisterAfterClose(
        Func<CancellationToken, ValueTask> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        if (Interlocked.CompareExchange(ref _afterClose, callback, null) is not null)
        {
            throw new InvalidOperationException(
                "After-close work has already been registered for this connection.");
        }
    }

    internal ValueTask ExecuteAfterCloseAsync(
        CancellationToken cancellationToken)
    {
        Func<CancellationToken, ValueTask>? callback = Volatile.Read(ref _afterClose);

        return callback is null
            ? ValueTask.CompletedTask
            : callback(cancellationToken);
    }
}