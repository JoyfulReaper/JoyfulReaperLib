using System.Net;

namespace JoyfulReaperLib.TcpServer;

/// <summary>
/// Contains the infrastructure-owned state associated with an accepted TCP
/// connection.
/// </summary>
public sealed record TcpConnectionContext(
    long ConnectionId,
    Stream Stream,
    EndPoint? RemoteEndPoint,
    EndPoint? LocalEndPoint,
    DateTimeOffset AcceptedAt);