using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json;

namespace JoyfulReaperLib.Ntfy.Tests;

[TestClass]
public sealed class NtfyPublisherTests
{
    private static readonly Uri ServerUrl = new("https://ntfy.example.test/");

    [TestMethod]
    public async Task PublishAsync_SendsPostToConfiguredServerUrl()
    {
        var handler = new FakeHttpMessageHandler();
        var publisher = CreatePublisher(handler);

        await publisher.PublishAsync(new NtfyMessage { Message = "Hello" });

        Assert.IsNotNull(handler.Request);
        Assert.AreEqual(HttpMethod.Post, handler.Request.Method);
        Assert.AreEqual(ServerUrl, handler.Request.RequestUri);
    }

    [TestMethod]
    public async Task PublishAsync_SerializesConfiguredPayload()
    {
        var handler = new FakeHttpMessageHandler();
        var publisher = CreatePublisher(handler, topic: "alerts");
        var clickUrl = new Uri("https://example.test/details?id=42");

        await publisher.PublishAsync(new NtfyMessage
        {
            Message = "Build completed",
            Title = "CI",
            Priority = NtfyPriority.High,
            Tags = ["white_check_mark", "build"],
            ClickUrl = clickUrl
        });

        using var payload = JsonDocument.Parse(handler.RequestBody!);
        var root = payload.RootElement;

        Assert.AreEqual("alerts", root.GetProperty("topic").GetString());
        Assert.AreEqual("Build completed", root.GetProperty("message").GetString());
        Assert.AreEqual("CI", root.GetProperty("title").GetString());
        Assert.AreEqual(4, root.GetProperty("priority").GetInt32());
        CollectionAssert.AreEqual(
            new[] { "white_check_mark", "build" },
            root.GetProperty("tags").EnumerateArray().Select(tag => tag.GetString()).ToArray());
        Assert.AreEqual(clickUrl.AbsoluteUri, root.GetProperty("click").GetString());
    }

    [TestMethod]
    public async Task PublishAsync_WithAccessToken_SendsBearerAuthorizationHeader()
    {
        var handler = new FakeHttpMessageHandler();
        var publisher = CreatePublisher(handler, accessToken: "tk_exact-token");

        await publisher.PublishAsync(new NtfyMessage { Message = "Hello" });

        Assert.IsNotNull(handler.Request);
        Assert.IsNotNull(handler.Request.Headers.Authorization);
        Assert.AreEqual("Bearer", handler.Request.Headers.Authorization.Scheme);
        Assert.AreEqual("tk_exact-token", handler.Request.Headers.Authorization.Parameter);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public async Task PublishAsync_WithoutAccessToken_OmitsAuthorizationHeader(string? accessToken)
    {
        var handler = new FakeHttpMessageHandler();
        var publisher = CreatePublisher(handler, accessToken: accessToken);

        await publisher.PublishAsync(new NtfyMessage { Message = "Hello" });

        Assert.IsNotNull(handler.Request);
        Assert.IsNull(handler.Request.Headers.Authorization);
    }

    [TestMethod]
    public async Task PublishAsync_WithNullOptionalValues_OmitsThemFromJson()
    {
        var handler = new FakeHttpMessageHandler();
        var publisher = CreatePublisher(handler);

        await publisher.PublishAsync(new NtfyMessage
        {
            Message = "Only required values",
            Title = null,
            ClickUrl = null,
            Tags = []
        });

        using var payload = JsonDocument.Parse(handler.RequestBody!);
        var root = payload.RootElement;

        Assert.IsFalse(root.TryGetProperty("title", out _));
        Assert.IsFalse(root.TryGetProperty("click", out _));
        Assert.IsFalse(root.TryGetProperty("tags", out _));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public async Task PublishAsync_WithEmptyMessage_ThrowsArgumentException(string message)
    {
        var publisher = CreatePublisher(new FakeHttpMessageHandler());
        var notification = new NtfyMessage { Message = message };

        await Assert.ThrowsExactlyAsync<ArgumentException>(
            () => publisher.PublishAsync(notification));
    }

    [TestMethod]
    public async Task PublishAsync_WithNullNotification_ThrowsArgumentNullException()
    {
        var publisher = CreatePublisher(new FakeHttpMessageHandler());

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => publisher.PublishAsync(null!));
    }

    [TestMethod]
    public async Task PublishAsync_WithNonSuccessResponse_ThrowsHttpRequestException()
    {
        var handler = new FakeHttpMessageHandler(HttpStatusCode.BadGateway);
        var publisher = CreatePublisher(handler);

        await Assert.ThrowsExactlyAsync<HttpRequestException>(
            () => publisher.PublishAsync(new NtfyMessage { Message = "Hello" }));
    }

    [TestMethod]
    public async Task PublishAsync_WhenCancelled_PropagatesCancellationToHandler()
    {
        var handlerEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var handler = new FakeHttpMessageHandler(async (_, cancellationToken) =>
        {
            handlerEntered.SetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        var publisher = CreatePublisher(handler);
        using var cancellationSource = new CancellationTokenSource();

        var publishTask = publisher.PublishAsync(
            new NtfyMessage { Message = "Hello" },
            cancellationSource.Token);
        await handlerEntered.Task;
        cancellationSource.Cancel();

        await Assert.ThrowsExactlyAsync<TaskCanceledException>(() => publishTask);
        Assert.IsTrue(handler.CancellationToken.IsCancellationRequested);
    }

    [TestMethod]
    [DataRow(NtfyPriority.Min, 1)]
    [DataRow(NtfyPriority.Low, 2)]
    [DataRow(NtfyPriority.Default, 3)]
    [DataRow(NtfyPriority.High, 4)]
    [DataRow(NtfyPriority.Max, 5)]
    public async Task PublishAsync_MapsPriorityToNtfyNumericValue(
        NtfyPriority priority,
        int expectedValue)
    {
        var handler = new FakeHttpMessageHandler();
        var publisher = CreatePublisher(handler);

        await publisher.PublishAsync(new NtfyMessage
        {
            Message = "Hello",
            Priority = priority
        });

        using var payload = JsonDocument.Parse(handler.RequestBody!);
        Assert.AreEqual(expectedValue, payload.RootElement.GetProperty("priority").GetInt32());
    }

    private static NtfyPublisher CreatePublisher(
        FakeHttpMessageHandler handler,
        string topic = "test-topic",
        string? accessToken = null)
    {
        var httpClient = new HttpClient(handler);
        var options = Options.Create(new NtfyOptions
        {
            ServerUrl = ServerUrl,
            Topic = topic,
            AccessToken = accessToken
        });

        return new NtfyPublisher(httpClient, options);
    }
}
