namespace JoyfulReaperLib.TcpServer;

/// <summary>
/// Defines how a TCP server behaves when every connection slot is occupied.
/// </summary>
public enum ConnectionLimitBehavior
{
    /// <summary>
    /// Waits for an available connection slot before accepting another
    /// connection.
    /// </summary>
    Wait,

    /// <summary>
    /// Accepts and immediately closes a connection when no processing slot
    /// is available.
    /// </summary>
    Reject
}