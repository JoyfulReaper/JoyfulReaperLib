using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace JoyfulReaperLib.Ntfy.Tests;

[TestClass]
public sealed class ServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddJoyfulReaperNtfy_WithConfigureAction_RegistersPublisher()
    {
        var services = new ServiceCollection();
        services.AddJoyfulReaperNtfy(SetValidOptions);

        using var provider = services.BuildServiceProvider();
        var publisher = provider.GetRequiredService<INtfyPublisher>();

        Assert.IsInstanceOfType<NtfyPublisher>(publisher);
    }

    [TestMethod]
    public void AddJoyfulReaperNtfy_AppliesConfiguredTimeoutToTypedHttpClient()
    {
        var services = new ServiceCollection();
        var expectedTimeout = TimeSpan.FromSeconds(37);
        TimeSpan? observedTimeout = null;
        services.AddJoyfulReaperNtfy(options =>
        {
            SetValidOptions(options);
            options.Timeout = expectedTimeout;
        });
        services.ConfigureAll<HttpClientFactoryOptions>(options =>
            options.HttpClientActions.Add(client => observedTimeout = client.Timeout));

        using var provider = services.BuildServiceProvider();
        _ = provider.GetRequiredService<INtfyPublisher>();

        Assert.AreEqual(expectedTimeout, observedTimeout);
    }

    [TestMethod]
    [DataRow("relative", "topic", 10)]
    [DataRow("ftp://ntfy.example.test", "topic", 10)]
    [DataRow("https://ntfy.example.test", "", 10)]
    [DataRow("https://ntfy.example.test", "topic", 0)]
    [DataRow("https://ntfy.example.test", "topic", -1)]
    public void AddJoyfulReaperNtfy_WithInvalidOptions_FailsValidation(
        string serverUrl,
        string topic,
        int timeoutSeconds)
    {
        var services = new ServiceCollection();
        services.AddJoyfulReaperNtfy(options =>
        {
            options.ServerUrl = new Uri(serverUrl, UriKind.RelativeOrAbsolute);
            options.Topic = topic;
            options.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        });

        using var provider = services.BuildServiceProvider();

        Assert.ThrowsExactly<OptionsValidationException>(
            () => _ = provider.GetRequiredService<IOptions<NtfyOptions>>().Value);
    }

    [TestMethod]
    public void AddJoyfulReaperNtfy_WithConfiguration_BindsNtfySection()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Ntfy:ServerUrl"] = "https://configured.example.test/",
            ["Ntfy:Topic"] = "configured-topic",
            ["Ntfy:AccessToken"] = "tk_configured",
            ["Ntfy:Timeout"] = "00:00:23"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        var services = new ServiceCollection();

        services.AddJoyfulReaperNtfy(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<NtfyOptions>>().Value;
        Assert.AreEqual(new Uri("https://configured.example.test/"), options.ServerUrl);
        Assert.AreEqual("configured-topic", options.Topic);
        Assert.AreEqual("tk_configured", options.AccessToken);
        Assert.AreEqual(TimeSpan.FromSeconds(23), options.Timeout);
        Assert.IsInstanceOfType<NtfyPublisher>(provider.GetRequiredService<INtfyPublisher>());
    }

    private static void SetValidOptions(NtfyOptions options)
    {
        options.ServerUrl = new Uri("https://ntfy.example.test/");
        options.Topic = "test-topic";
    }
}
