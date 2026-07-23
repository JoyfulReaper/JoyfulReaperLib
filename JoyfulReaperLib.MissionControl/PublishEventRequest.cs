using System.Text.Json;

namespace JoyfulReaperLib.MissionControl;

internal sealed record PublishEventRequest(
    Guid EventId,
    string EventType,
    int SchemaVersion,
    DateTimeOffset OccurredAt,
    string? CorrelationId,
    JsonElement Payload);