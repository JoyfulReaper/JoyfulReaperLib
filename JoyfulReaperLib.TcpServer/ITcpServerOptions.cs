namespace JoyfulReaperLib.TcpServer;

/// <summary>
/// Defines the configuration required by a hosted TCP server.
/// </summary>
public interface ITcpServerOptions
{
    /// <summary>
    /// Gets the local IP address on which the server listens.
    /// </summary>
    string ListenAddress { get; }

    /// <summary>
    /// Gets the TCP port on which the server listens.
    /// </summary>
    int Port { get; }

    /// <summary>
    /// Gets the maximum number of connections that may be processed
    /// concurrently.
    /// </summary>
    int MaxConcurrentConnections { get; }

    /// <summary>
    /// Gets the behavior used when the concurrent connection limit
    /// has been reached.
    /// </summary>
    ConnectionLimitBehavior ConnectionLimitBehavior { get; }
}