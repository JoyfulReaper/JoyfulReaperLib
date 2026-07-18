using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JoyfulReaperLib.Ntfy;

internal sealed class NtfyPublisher : INtfyPublisher
{
    private static readonly JsonSerializerOptions JsonOptions =
        new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

    private readonly HttpClient _httpClient;
    private readonly NtfyOptions _options;

    public NtfyPublisher(
        HttpClient httpClient,
        IOptions<NtfyOptions> options

    )
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task PublishAsync(
        NtfyMessage notification,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (string.IsNullOrWhiteSpace(notification.Message))
        {
            throw new ArgumentException(
                "Notification message cannot be empty.",
                nameof(notification.Message)
            );
        }

        var payload = new NtfyPublishRequest
        {
            Topic = _options.Topic,
            Message = notification.Message,
            Title = notification.Title,
            Priority = (int)notification.Priority,
            Tags = notification.Tags.Count > 0 ? notification.Tags : null,
            Click = notification.ClickUrl?.AbsoluteUri
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            _options.ServerUrl)
        {
            Content = JsonContent.Create(payload, options: JsonOptions)
        };

        if(!string.IsNullOrWhiteSpace(_options.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _options.AccessToken
            );
        }

        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    private sealed record NtfyPublishRequest
    {
        [JsonPropertyName("topic")]
        public required string Topic { get; init; }

        [JsonPropertyName("message")]
        public required string Message { get; init; }

        [JsonPropertyName("title")]
        public string? Title { get; init; }

        [JsonPropertyName("priority")]
        public int Priority { get; init; }

        [JsonPropertyName("tags")]
        public IReadOnlyCollection<string>? Tags { get; init; }

        [JsonPropertyName("click")]
        public string? Click { get; init; }
    }
}

