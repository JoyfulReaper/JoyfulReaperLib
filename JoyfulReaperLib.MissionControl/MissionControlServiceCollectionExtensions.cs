using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JoyfulReaperLib.MissionControl;

public static class MissionControlServiceCollectionExtensions
{
    public static IServiceCollection AddMissionControlClient(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configurationSection);

        services
            .AddOptions<MissionControlClientOptions>()
            .Bind(configurationSection)
            .Validate(
                options =>
                    Uri.TryCreate(
                        options.BaseUrl,
                        UriKind.Absolute,
                        out var uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp ||
                     uri.Scheme == Uri.UriSchemeHttps),
                "MissionControl:BaseUrl must be an absolute HTTP or HTTPS URL.")
            .Validate(
                options =>
                    !options.Enabled ||
                    options.ApiKey.Length >= 32,
                "MissionControl:ApiKey must contain at least 32 characters when enabled.")
            .Validate(
                options =>
                    options.TimeoutMilliseconds
                        is >= 100 and <= 10_000,
                "MissionControl:TimeoutMilliseconds must be between 100 and 10000.")
            .ValidateOnStart();

        services.AddHttpClient(
            MissionControlClientOptions.HttpClientName,
            (serviceProvider, client) =>
            {
                var options = serviceProvider
                    .GetRequiredService<
                        IOptions<MissionControlClientOptions>>()
                    .Value;

                if (!string.IsNullOrWhiteSpace(options.CloudflareAccessClientId))
                {
                    client.DefaultRequestHeaders.Add(
                        "CF-Access-Client-Id",
                        options.CloudflareAccessClientId);
                }

                if (!string.IsNullOrWhiteSpace(options.CloudflareAccessClientSecret))
                {
                    client.DefaultRequestHeaders.Add(
                        "CF-Access-Client-Secret",
                        options.CloudflareAccessClientSecret);
                }

                client.BaseAddress =
                    new Uri(options.BaseUrl, UriKind.Absolute);

                client.Timeout =
                    TimeSpan.FromMilliseconds(
                        options.TimeoutMilliseconds);
            });

        services.AddSingleton<
            IMissionControlClient,
            MissionControlClient>();

        return services;
    }
}