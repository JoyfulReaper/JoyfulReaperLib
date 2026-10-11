# JoyfulReaperLib

A collection of lightweight .NET helpers and optional packages for SQLite storage, web statistics, hosted TCP servers, Mission Control events, and ntfy notifications.

## Packages

| Package | Description |
| --- | --- |
| [JoyfulReaperLib](https://www.nuget.org/packages/JoyfulReaperLib) | Shared helpers for text, numbers, collections, serialization, validation, encryption, console output, and other common tasks. |
| [JoyfulReaperLib.Sqlite](https://www.nuget.org/packages/JoyfulReaperLib.Sqlite) | Shared SQLite provider initialization, database initialization, and connection-string path handling. |
| [JoyfulReaperLib.Caching.Sqlite](https://www.nuget.org/packages/JoyfulReaperLib.Caching.Sqlite) | A SQLite-backed implementation of `IDistributedCache`. |
| [JoyfulReaperLib.WebStats.Sqlite](https://www.nuget.org/packages/JoyfulReaperLib.WebStats.Sqlite) | SQLite-backed hit counting and unique-visitor statistics. |
| [JoyfulReaperLib.TcpServer](https://www.nuget.org/packages/JoyfulReaperLib.TcpServer) | Generic Host infrastructure for bounded, dependency-injected TCP servers. |
| [JoyfulReaperLib.MissionControl](https://www.nuget.org/packages/JoyfulReaperLib.MissionControl) | A best-effort HTTP client for publishing application events to Mission Control. |
| [JoyfulReaperLib.Ntfy](https://www.nuget.org/packages/JoyfulReaperLib.Ntfy) | An HTTP client for publishing notifications to hosted or self-hosted ntfy servers. |

`JoyfulReaperLib` is the lightweight base package. SQLite features, TCP server hosting, Mission Control integration, and ntfy support live in separate optional packages so applications only take the dependencies they need.

## Installation

All seven packages are available on NuGet.org. From your application's project directory, install only the packages you need:

```bash
dotnet add package JoyfulReaperLib
dotnet add package JoyfulReaperLib.Sqlite
dotnet add package JoyfulReaperLib.Caching.Sqlite
dotnet add package JoyfulReaperLib.WebStats.Sqlite
dotnet add package JoyfulReaperLib.TcpServer
dotnet add package JoyfulReaperLib.MissionControl
dotnet add package JoyfulReaperLib.Ntfy
```

## Using the projects locally

For local development or direct source usage, clone this repository and add project references from your application to the projects it uses. For normal installation, use the NuGet packages above. Replace `path/to/YourApp.csproj` with your application's project path:

```bash
dotnet add path/to/YourApp.csproj reference JoyfulReaperLib/JoyfulReaperLib.csproj
dotnet add path/to/YourApp.csproj reference JoyfulReaperLib.Sqlite/JoyfulReaperLib.Sqlite.csproj
dotnet add path/to/YourApp.csproj reference JoyfulReaperLib.Caching.Sqlite/JoyfulReaperLib.Caching.Sqlite.csproj
dotnet add path/to/YourApp.csproj reference JoyfulReaperLib.WebStats.Sqlite/JoyfulReaperLib.WebStats.Sqlite.csproj
dotnet add path/to/YourApp.csproj reference JoyfulReaperLib.TcpServer/JoyfulReaperLib.TcpServer.csproj
dotnet add path/to/YourApp.csproj reference JoyfulReaperLib.MissionControl/JoyfulReaperLib.MissionControl.csproj
dotnet add path/to/YourApp.csproj reference JoyfulReaperLib.Ntfy/JoyfulReaperLib.Ntfy.csproj
```

Only add the references your application needs. `JoyfulReaperLib.Sqlite` is referenced transitively by the SQLite caching and web stats projects. Reference it directly when using `SqliteProviderInitializer`, `SqliteDatabaseInitializer`, or `SqliteConnectionStringHelper` in application code.

## JoyfulReaperLib

The base package contains dependency-free utility APIs, including:

- string reversal, palindrome checks, text analysis, and greedy line wrapping;
- numeric helpers, base conversion, Fibonacci sequences, and Luhn validation;
- random item selection for arrays and lists;
- JSON byte-array serialization;
- URL and IP address validation and ingress-network classification;
- currency, enum, console, Caesar cipher, and Vigenere cipher helpers.

### Text helpers

```csharp
using JoyfulReaperLib.JRText;

string reversed = StringHelper.Reverse("Joyful");
bool palindrome = StringHelper.IsPalindrome("neveroddoreven");
string? optionalValue = StringHelper.AssignNullIfEmpty("   ");

var wrapper = new GreedyWrap(
    lineWidth: 30,
    wrapWordsLongerThanLineWidth: true)
{
    TabWidth = 4
};

string wrapped = wrapper.LineWrap(
    "A long line of text that should be wrapped for display.");
```

`StringHelper.VowelAnalysis(...)` also returns the vowel count while reporting consonants, whitespace, numbers, and unknown characters through `out` parameters.

### Collections and enums

```csharp
using JoyfulReaperLib.JRArray;
using JoyfulReaperLib.JREnums;
using JoyfulReaperLib.JRLists;

string randomFromArray = new[] { "red", "green", "blue" }.RandomItem();
int randomFromList = new List<int> { 10, 20, 30 }.RandomItem();

DayOfWeek randomDay = EnumHelper.RandomEnumValue<DayOfWeek>();
bool validDay = EnumHelper.EnumValueIsValid(DayOfWeek.Monday);
(long min, long max) = EnumHelper.GetEnumMinMax<DayOfWeek>();
```

Random item selection throws when the source collection is empty.

### Numbers and algorithms

```csharp
using JoyfulReaperLib.JRAlgorithms;
using JoyfulReaperLib.JRMath;
using JoyfulReaperLib.JRNumbers;

int digits = NumberHelper.NumberOfDigits(12345);
string binary = BaseConverter.DecimalToBinary(42);
int decimalValue = BaseConverter.BinaryToDecimal("101010");

long[] sequence = Fibonacci.FibonacciSequence(
    first: 0,
    second: 1,
    term: 10);

string completedNumber = Luhn.LuhnCreate("7992739871", out int checkDigit);
bool valid = Luhn.LuhnValidate(completedNumber);
string cardNumber = "4111111111111111";
bool validVisa = Luhn.LuhnValidate(cardNumber, Luhn.CheckType.Visa);
```

### JSON byte-array serialization

```csharp
using JoyfulReaperLib.JRSerialization;

var original = new Dictionary<string, int>
{
    ["apples"] = 3,
    ["oranges"] = 2
};

byte[]? bytes = JsonByteArraySerializer.SerializeToUtf8Bytes(original);
Dictionary<string, int>? restored =
    JsonByteArraySerializer.DeserializeFromUtf8Bytes<Dictionary<string, int>>(bytes);
```

Null values serialize to `null`, and null or empty byte arrays deserialize to the default value for the requested type.

### Networking helpers

```csharp
using System.Net;
using JoyfulReaperLib.JRNet;

bool validUrl = UrlValidator.ValidateUrl("https://example.com/status");

IPAddress anyAddress = IPAddressUtils.ParseListenAddress("*");
IPAddress loopback = IPAddressUtils.ParseListenAddress("127.0.0.1");
```

`ParseListenAddress` maps `*`, `+`, and `0.0.0.0` to `IPAddress.Any`, and maps `::` to `IPAddress.IPv6Any`.

`IngressNetworkClassifier` returns `"public"`, `"dn42"`, `"i2p"`, `"yggdrasil"`, or `"unknown"` from a hostname. DN42 and I2P are recognized automatically; applications supply their public and Yggdrasil rules:

```csharp
using JoyfulReaperLib.JRNet;

var network = IngressNetworkClassifier.FromHost(
    "api.ygg.kgivler.com",
    publicHosts: ["status.example.net"],
    publicHostSuffixes: ["kgivler.com"],
    yggdrasilHostSuffixes: ["ygg.kgivler.com"]);
// network == "yggdrasil"

var i2pNetwork = IngressNetworkClassifier.FromHost("example.i2p");
// i2pNetwork == "i2p" (no configuration needed)
```

Matching ignores case, surrounding whitespace, and trailing DNS dots in both hostnames and rules. Exact public rules match only the named host. Configured suffixes match the domain itself and its subdomains at DNS-label boundaries: `kgivler.com` matches `api.kgivler.com`, but not `notkgivler.com`. An optional leading dot on a suffix has the same meaning and still includes the root domain.

The built-in `*.dn42` and `*.i2p` rules run first (bare `dn42` and `i2p` do not match), followed by configured Yggdrasil rules, then public rules. Built-in network suffixes take precedence even over overlapping public or Yggdrasil rules. Unmatched, null, or blank hostnames return `"unknown"`; omitted collections and null, blank, or dot-only rule entries are ignored. Pass hostname strings without schemes, ports, or paths. This helper performs no DNS lookup or network verification.

### Currency helpers

```csharp
using JoyfulReaperLib.JRCurrency;

List<CurrencyUnit> coins = CurrencyHelper.GetUSDCommonCoins();
List<CurrencyUnit> change = CurrencyHelper.CalculateChange(0.41m, coins);

foreach (CurrencyUnit unit in change)
{
    Console.WriteLine($"{unit.Quantity} {unit.PluralName}");
}
```

`CalculateChange` uses a greedy algorithm and throws if the requested amount cannot be represented by the supplied units.

### Classical ciphers

```csharp
using JoyfulReaperLib.JREncryption;

string caesar = CipherHelper.CaesarEncipher("Meet at noon", key: 3);
string original = CipherHelper.CaesarDecipher(caesar, key: 3);

string vigenere = CipherHelper.VigenereCipher(
    text: "ATTACKATDAWN",
    key: "LEMON",
    dir: CipherHelper.Direction.Encipher);
```

These are educational classical ciphers and are not suitable for passwords, secrets, or other security-sensitive data.

### Console output

```csharp
using JoyfulReaperLib.JRConsole;

ConsoleHelper.ColorWriteLine(ConsoleColor.Green, "Operation completed.");

ConsoleHelper.MulticolorWriteLine(
    new List<ConsoleColor>
    {
        ConsoleColor.Cyan,
        ConsoleColor.Magenta
    },
    "Alternating colors");

int choice = ConsoleHelper.GetValidInt(
    prompt: "Choose a value: ",
    min: 1,
    max: 10);
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

## JoyfulReaperLib.TcpServer

This package hosts a bounded TCP listener in a .NET Generic Host. Each accepted connection is handled in its own dependency-injection scope by an `ITcpConnectionHandler`.

Define an options type and connection handler:

```csharp
using JoyfulReaperLib.TcpServer;

public sealed class EchoServerOptions : ITcpServerOptions
{
    public string ListenAddress { get; init; } = "127.0.0.1";
    public bool DualMode { get; init; } = false;
    public int Port { get; init; } = 7000;
    public int MaxConcurrentConnections { get; init; } = 20;
    public ConnectionLimitBehavior ConnectionLimitBehavior { get; init; } =
        ConnectionLimitBehavior.Wait;
}

public sealed class EchoConnectionHandler : ITcpConnectionHandler
{
    public async ValueTask HandleAsync(
        TcpConnectionContext context,
        CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[4096];
        int bytesRead;

        while ((bytesRead = await context.Stream.ReadAsync(
            buffer,
            cancellationToken)) > 0)
        {
            await context.Stream.WriteAsync(
                buffer.AsMemory(0, bytesRead),
                cancellationToken);
        }
    }
}
```

Register the options and server with the host:

```csharp
builder.Services
    .AddOptions<EchoServerOptions>()
    .Bind(builder.Configuration.GetSection("EchoServer"));

builder.Services.AddTcpServer<EchoConnectionHandler, EchoServerOptions>();
```

Set `ConnectionLimitBehavior` to `Wait` to delay accepting another connection until capacity is available, or `Reject` to accept and immediately close excess connections.

Listener address-family configurations are:

- IPv4 only: `ListenAddress = "0.0.0.0"`, `DualMode = false`
- IPv6 only: `ListenAddress = "::"`, `DualMode = false`
- IPv4 and IPv6: `ListenAddress = "::"`, `DualMode = true`

Dual mode uses one IPv6 socket and accepts IPv4 clients as IPv4-mapped IPv6 connections. It is only valid with the IPv6 wildcard address `::`.

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
