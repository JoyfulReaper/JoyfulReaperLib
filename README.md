# JoyfulReaperLib
A library of code that I have found helpful!

Packages:

- `JoyfulReaperLib`
- `JoyfulReaperLib.Sqlite`
- `JoyfulReaperLib.Caching.Sqlite`
- `JoyfulReaperLib.WebStats.Sqlite`
- `JoyfulReaperLib.MissionControl`
- `JoyfulReaperLib.Ntfy`

Base package:

`JoyfulReaperLib` is the lightweight base package for shared utility helpers. Optional SQLite-backed caching, provider initialization, and web stats features live in the separate `JoyfulReaperLib.Sqlite`, `JoyfulReaperLib.Caching.Sqlite`, and `JoyfulReaperLib.WebStats.Sqlite` packages. ntfy support lives in its own optional `JoyfulReaperLib.Ntfy` package.

Install:

```bash
dotnet add package JoyfulReaperLib
dotnet add package JoyfulReaperLib.Caching.Sqlite
dotnet add package JoyfulReaperLib.WebStats.Sqlite
dotnet add package JoyfulReaperLib.Ntfy
```

`JoyfulReaperLib.Sqlite` is usually pulled in transitively unless your app directly uses `SqliteProviderInitializer` or `SqliteConnectionStringHelper`.

`JoyfulReaperLib` is the lightweight base package with no SQLite dependencies.
`JoyfulReaperLib.Sqlite` provides shared SQLite provider initialization for optional SQLite-based packages.
`JoyfulReaperLib.Caching.Sqlite` provides a SQLite-backed `IDistributedCache`.
`JoyfulReaperLib.WebStats.Sqlite` provides SQLite-backed reusable web stats and hit counting.
`JoyfulReaperLib.MissionControl` provides a best-effort HTTP client for publishing application events to Mission Control.
`JoyfulReaperLib.Ntfy` publishes notifications to hosted or self-hosted ntfy servers. It supports bearer-token authentication, titles, priorities, tags, click URLs, cancellation, configurable timeouts, and dependency injection.

Caching:

```csharp
services.AddJoyfulReaperSqliteDistributedCache(options =>
{
    options.ConnectionString = "Data Source=cache.db";
});
```

Web stats:

```csharp
services.AddJoyfulReaperSqliteHitCounter(options =>
{
    options.ConnectionString = "Data Source=app-stats.db";
});
```

```csharp
var stats = await hitCounter.RecordHitAsync(visitorKey);
```

## Ntfy notifications

### Registration

```csharp
builder.Services.AddJoyfulReaperNtfy(builder.Configuration);
```

### Configuration

```json
{
  "Ntfy": {
    "ServerUrl": "https://ntfy.example.com",
    "Topic": "application-alerts",
    "Timeout": "00:00:10"
  }
}
```

The access token should normally be supplied through an environment variable rather than committed to configuration:

```text
Ntfy__AccessToken=tk_your_token_here
```

`ServerUrl` should contain only the ntfy server root. The topic is configured separately through `NtfyOptions.Topic`. Invalid server URL, topic, and timeout configuration is validated during application startup.

### Publishing

```csharp
public sealed class AlertService(INtfyPublisher publisher)
{
    public Task SendAsync(CancellationToken cancellationToken)
    {
        return publisher.PublishAsync(
            new NtfyMessage
            {
                Title = "Application alert",
                Message = "The application requires attention.",
                Priority = NtfyPriority.High,
                Tags = ["warning", "computer"],
                ClickUrl = new Uri("https://example.com/status")
            },
            cancellationToken);
    }
}
```
