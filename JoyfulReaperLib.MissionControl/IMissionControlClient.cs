namespace JoyfulReaperLib.MissionControl;

public interface IMissionControlClient
{
    Task<bool> TryPublishAsync<TPayload>(
        string eventType,
        TPayload payload,
        DateTimeOffset occurredAt,
        string? correlationId = null,
        CancellationToken cancellationToken = default);
}