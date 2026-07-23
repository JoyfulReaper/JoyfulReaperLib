/*
 * JoyfulReaperLibrary
 * Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

namespace JoyfulReaperLib.TcpServer;

/// <summary>
/// Handles one accepted TCP connection.
/// </summary>
public interface ITcpConnectionHandler
{
    /// <summary>
    /// Processes an accepted connection.
    /// </summary>
    /// <param name="context">
    /// Information and stream associated with the accepted connection.
    /// </param>
    /// <param name="cancellationToken">
    /// Signals that the server is shutting down.
    /// </param>
    ValueTask HandleAsync(
        TcpConnectionContext context,
        CancellationToken cancellationToken);
}