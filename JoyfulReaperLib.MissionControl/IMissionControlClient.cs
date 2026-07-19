using System.Text.Json.Serialization.Metadata;

namespace JoyfulReaperLib.MissionControl;

public interface IMissionControlClient
{
    Task<bool> TryPublishAsync<TPayload>(
        string eventType,
        TPayload payload,
        JsonTypeInfo<TPayload> payloadTypeInfo,
        DateTimeOffset occurredAt,
        string? correlationId = null,
        CancellationToken cancellationToken = default);
}