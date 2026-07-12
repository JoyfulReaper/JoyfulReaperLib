namespace JoyfulReaperLib.MissionControl;

internal sealed record PublishEventRequest<TPayload>(
    Guid EventId,
    string EventType,
    int SchemaVersion,
    DateTimeOffset OccurredAt,
    string? CorrelationId,
    TPayload Payload);