using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace JoyfulReaperLib.MissionControl;

public sealed class MissionControlClient(
    IHttpClientFactory httpClientFactory,
    IOptions<MissionControlClientOptions> options,
    ILogger<MissionControlClient> logger)
    : IMissionControlClient
{
    private readonly MissionControlClientOptions _options =
        options.Value;

    public async Task<bool> TryPublishAsync<TPayload>(
        string eventType,
        TPayload payload,
        JsonTypeInfo<TPayload> payloadTypeInfo,
        DateTimeOffset occurredAt,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(payloadTypeInfo);

        if (!_options.Enabled)
        {
            return false;
        }

        try
        {
            JsonElement serializedPayload =
                JsonSerializer.SerializeToElement(
                    payload,
                    payloadTypeInfo);

            var request = new PublishEventRequest(
                EventId: Guid.NewGuid(),
                EventType: eventType,
                SchemaVersion: 1,
                OccurredAt: occurredAt,
                CorrelationId: correlationId,
                Payload: serializedPayload);

            using var message = new HttpRequestMessage(
                HttpMethod.Post,
                "api/events");

            message.Headers.TryAddWithoutValidation(
                MissionControlClientOptions.ApiKeyHeaderName,
                _options.ApiKey);

            message.Content = JsonContent.Create(
                request,
                MissionControlJsonContext.Default
                    .PublishEventRequest);

            var client = httpClientFactory.CreateClient(
                MissionControlClientOptions.HttpClientName);

            using var response = await client.SendAsync(
                message,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                logger.LogDebug(
                    "Published Mission Control event {EventType}",
                    eventType);

                return true;
            }

            string responseBody =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            logger.LogWarning(
                "Mission Control rejected event {EventType} with HTTP status {StatusCode}. Response: {ResponseBody}",
                eventType,
                (int)response.StatusCode,
                responseBody);

            return false;
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            return false;
        }
        catch (OperationCanceledException exception)
        {
            logger.LogWarning(
                exception,
                "Mission Control event {EventType} timed out",
                eventType);

            return false;
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(
                exception,
                "Mission Control was unavailable while publishing {EventType}",
                eventType);

            return false;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unexpected error publishing Mission Control event {EventType}",
                eventType);

            return false;
        }
    }
}