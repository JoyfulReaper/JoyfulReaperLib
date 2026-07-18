using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JoyfulReaperLib.Ntfy;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJoyfulReaperNtfy(
        this IServiceCollection services,
        Action<NtfyOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services
            .AddOptions<NtfyOptions>()
            .Configure(configure)
            .Validate(
                options =>
                    options.ServerUrl is { IsAbsoluteUri: true } &&
                    (options.ServerUrl.Scheme == Uri.UriSchemeHttp ||
                     options.ServerUrl.Scheme == Uri.UriSchemeHttps),
                $"{nameof(NtfyOptions.ServerUrl)} must be an absolute HTTP or HTTPS URL.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Topic),
                $"{nameof(NtfyOptions.Topic)} cannot be empty.")
            .Validate(
                options => options.Timeout > TimeSpan.Zero,
                $"{nameof(NtfyOptions.Timeout)} must be greater than zero.")
            .ValidateOnStart();

        services.AddHttpClient<INtfyPublisher, NtfyPublisher>(
            static (serviceProvider, httpClient) =>
            {
                var options = serviceProvider
                    .GetRequiredService<IOptions<NtfyOptions>>()
                    .Value;

                httpClient.Timeout = options.Timeout;
            });

        return services;
    }

    public static IServiceCollection AddJoyfulReaperNtfy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return services.AddJoyfulReaperNtfy(options =>
        {
            configuration
                .GetSection(NtfyOptions.SectionName)
                .Bind(options);
        });
    }
}