using System.Net;

namespace JoyfulReaperLib.Ntfy.Tests;

internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _responseFactory;

    public FakeHttpMessageHandler(
        HttpStatusCode statusCode = HttpStatusCode.OK)
        : this((_, _) => Task.FromResult(new HttpResponseMessage(statusCode)))
    {
    }

    public FakeHttpMessageHandler(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responseFactory)
    {
        _responseFactory = responseFactory;
    }

    public HttpRequestMessage? Request { get; private set; }

    public string? RequestBody { get; private set; }

    public CancellationToken CancellationToken { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Request = request;
        CancellationToken = cancellationToken;
        RequestBody = request.Content is null
            ? null
            : await request.Content.ReadAsStringAsync(cancellationToken);

        return await _responseFactory(request, cancellationToken);
    }
}
