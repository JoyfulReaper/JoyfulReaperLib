# JoyfulReaperLib
A library of code that I have found helpful!

Packages:

- `JoyfulReaperLib`
- `JoyfulReaperLib.Sqlite`
- `JoyfulReaperLib.Caching.Sqlite`
- `JoyfulReaperLib.WebStats.Sqlite`

Base package:

`JoyfulReaperLib` is the lightweight base package for shared utility helpers. Optional SQLite-backed caching, provider initialization, and web stats features live in the separate `JoyfulReaperLib.Sqlite`, `JoyfulReaperLib.Caching.Sqlite`, and `JoyfulReaperLib.WebStats.Sqlite` packages.

Install:

```bash
dotnet add package JoyfulReaperLib
dotnet add package JoyfulReaperLib.Caching.Sqlite
dotnet add package JoyfulReaperLib.WebStats.Sqlite
```

`JoyfulReaperLib.Sqlite` is usually pulled in transitively unless your app directly uses `SqliteProviderInitializer` or `SqliteConnectionStringHelper`.

`JoyfulReaperLib` is the lightweight base package with no SQLite dependencies.
`JoyfulReaperLib.Sqlite` provides shared SQLite provider initialization for optional SQLite-based packages.
`JoyfulReaperLib.Caching.Sqlite` provides a SQLite-backed `IDistributedCache`.
`JoyfulReaperLib.WebStats.Sqlite` provides SQLite-backed reusable web stats and hit counting.

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
