# JoyfulReaperLib

A collection of lightweight .NET helpers and optional packages for SQLite storage, web statistics, Mission Control events, and ntfy notifications.

## Packages

| Package | Description |
| --- | --- |
| `JoyfulReaperLib` | Shared helpers for text, numbers, collections, serialization, validation, encryption, console output, and other common tasks. |
| `JoyfulReaperLib.Sqlite` | Shared SQLite provider initialization, database initialization, and connection-string path handling. |
| `JoyfulReaperLib.Caching.Sqlite` | A SQLite-backed implementation of `IDistributedCache`. |
| `JoyfulReaperLib.WebStats.Sqlite` | SQLite-backed hit counting and unique-visitor statistics. |
| `JoyfulReaperLib.MissionControl` | A best-effort HTTP client for publishing application events to Mission Control. |
| `JoyfulReaperLib.Ntfy` | An HTTP client for publishing notifications to hosted or self-hosted ntfy servers. |

`JoyfulReaperLib` is the lightweight base package. SQLite features, Mission Control integration, and ntfy support live in separate optional packages so applications only take the dependencies they need.

## Installation

Install the packages your application uses:

```bash
dotnet add package JoyfulReaperLib
dotnet add package JoyfulReaperLib.Sqlite
dotnet add package JoyfulReaperLib.Caching.Sqlite
dotnet add package JoyfulReaperLib.WebStats.Sqlite
dotnet add package JoyfulReaperLib.MissionControl
dotnet add package JoyfulReaperLib.Ntfy
```

`JoyfulReaperLib.Sqlite` is pulled in transitively by the SQLite caching and web stats packages. Install it directly when using `SqliteProviderInitializer`, `SqliteDatabaseInitializer`, or `SqliteConnectionStringHelper` in application code.

## JoyfulReaperLib

The base package contains dependency-free utility APIs, including:

- string reversal, palindrome checks, text analysis, and greedy line wrapping;
- numeric helpers, base conversion, Fibonacci sequences, and Luhn validation;
- random item selection for arrays and lists;
- JSON byte-array serialization;
- URL and IP address validation;
- currency, enum, console, Caesar cipher, and Vigenere cipher helpers.

Example:

```csharp
using JoyfulReaperLib.JRNumbers;
using JoyfulReaperLib.JRSerialization;
using JoyfulReaperLib.JRText;

string reversed = StringHelper.Reverse("Joyful");
string binary = BaseConverter.DecimalToBinary(42);

byte[]? bytes = JsonByteArraySerializer.SerializeToUtf8Bytes(new
{
    Name = "example",
    Count = 2
});
```

## JoyfulReaperLib.Sqlite

This package configures the bundled SQLite provider and provides helpers for resolving database paths and initializing a database schema.

```csharp
using JoyfulReaperLib.Sqlite;

string connectionString = SqliteDatabaseInitializer.Initialize(
    dbFileName: "application.db",
    schemaSql: """
        CREATE TABLE IF NOT EXISTS Settings (
            Key TEXT PRIMARY KEY,
            Value TEXT NOT NULL
        );
        """,
    basePath: "Data");
```

`SqliteProviderInitializer.Initialize()` is safe to call more than once. `SqliteConnectionStringHelper.Resolve(...)` leaves `:memory:` and `file:` data sources unchanged. Relative file paths use the supplied base path, or the application's `Data` directory when no base path is supplied.

## JoyfulReaperLib.Caching.Sqlite

This package registers a SQLite-backed `IDistributedCache` with support for absolute and sliding expiration.

### Registration

```csharp
using JoyfulReaperLib.Caching.Sqlite;

builder.Services.AddJoyfulReaperSqliteDistributedCache(options =>
{
    options.ConnectionString = "Data Source=cache.db";
    options.BasePath = "Data";
});
```

Provider initialization and database creation are handled automatically.

### Usage

```csharp
using Microsoft.Extensions.Caching.Distributed;

await cache.SetStringAsync(
    "status",
    "ready",
    new DistributedCacheEntryOptions
    {
        SlidingExpiration = TimeSpan.FromMinutes(10)
    },
    cancellationToken);

string? status = await cache.GetStringAsync("status", cancellationToken);
```

## JoyfulReaperLib.WebStats.Sqlite

This package provides an `IHitCounter` that tracks total hits and unique visitor keys in SQLite.

### Registration

```csharp
using JoyfulReaperLib.WebStats.Sqlite;

builder.Services.AddJoyfulReaperSqliteHitCounter(options =>
{
    options.ConnectionString = "Data Source=web-stats.db";
    options.BasePath = "Data";
});
```

Provider initialization and database creation are handled automatically.

### Usage

```csharp
HitCountStats stats = await hitCounter.RecordHitAsync(
    visitorKey,
    cancellationToken);

Console.WriteLine($"{stats.TotalHits} hits from {stats.UniqueVisitors} visitors");

HitCountStats current = await hitCounter.GetHitCountsAsync(cancellationToken);
```

The application chooses the visitor key. Avoid storing personal information unless your application's privacy requirements allow it.

## JoyfulReaperLib.MissionControl

This package publishes structured application events to the Mission Control `api/events` endpoint. Publishing is best effort: `TryPublishAsync` returns `false` when the client is disabled, the request is cancelled, the request times out, or the server cannot accept the event.

### Registration

```csharp
using JoyfulReaperLib.MissionControl;

builder.Services.AddMissionControlClient(
    builder.Configuration.GetSection(MissionControlClientOptions.SectionName));
```

### Configuration

```json
{
  "MissionControl": {
    "Enabled": true,
    "BaseUrl": "https://mission-control.example.com/",
    "TimeoutMilliseconds": 1000
  }
}
```

Supply the API key through an environment variable rather than committing it:

```text
MissionControl__ApiKey=replace_with_at_least_32_characters
```

Optional Cloudflare Access credentials can be provided through `MissionControl__CloudflareAccessClientId` and `MissionControl__CloudflareAccessClientSecret`.

When enabled, the base URL must be an absolute HTTP or HTTPS URL, the API key must contain at least 32 characters, and the timeout must be between 100 and 10,000 milliseconds. These settings are validated during application startup.

### Publishing

```csharp
bool published = await missionControlClient.TryPublishAsync(
    eventType: "application.started",
    payload: new { Environment = "Production" },
    occurredAt: DateTimeOffset.UtcNow,
    correlationId: null,
    cancellationToken: cancellationToken);
```

## JoyfulReaperLib.Ntfy

This package publishes notifications to hosted or self-hosted ntfy servers. It supports bearer-token authentication, titles, priorities, tags, click URLs, cancellation, configurable timeouts, and dependency injection.

### Registration

```csharp
using JoyfulReaperLib.Ntfy;

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

Supply the access token through an environment variable rather than committing it:

```text
Ntfy__AccessToken=tk_your_token_here
```

`ServerUrl` should contain only the ntfy server root. The topic is configured separately through `NtfyOptions.Topic`. Invalid server URL, topic, and timeout settings are validated during application startup.

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

Priority values map directly to ntfy priorities: `Min`, `Low`, `Default`, `High`, and `Max` map to numeric values 1 through 5.

## Development

Build and test the complete solution with:

```powershell
dotnet build JoyfulReaperLibrary.sln
dotnet test JoyfulReaperLibrary.sln
```

## License

JoyfulReaperLib is available under the [MIT License](LICENSE).
