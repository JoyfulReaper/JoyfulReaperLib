namespace JoyfulReaperLib.MissionControl;

public sealed class NullMissionControlClient
    : IMissionControlClient
{
    public static NullMissionControlClient Instance { get; } =
        new();

    private NullMissionControlClient()
    {
    }

    public Task<bool> TryPublishAsync<TPayload>(
        string eventType,
        TPayload payload,
        DateTimeOffset occurredAt,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }
}