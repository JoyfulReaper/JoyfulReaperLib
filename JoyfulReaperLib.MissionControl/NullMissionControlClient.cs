using System.Text.Json.Serialization.Metadata;

namespace JoyfulReaperLib.MissionControl;

public sealed class NullMissionControlClient
    : IMissionControlClient
{
    public static NullMissionControlClient Instance { get; } =
        new();

    private NullMissionControlClient()
    {
    }

    public Task<bool> TryPublishAsync<TPayload>(string eventType, TPayload payload, JsonTypeInfo<TPayload> payloadTypeInfo, DateTimeOffset occurredAt, string? correlationId = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }
}