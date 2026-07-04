# JoyfulReaperLib
A library of code that I have found helpful!

Packages:

- `JoyfulReaperLib`
- `JoyfulReaperLib.Sqlite`
- `JoyfulReaperLib.Caching.Sqlite`
- `JoyfulReaperLib.WebStats.Sqlite`

Install:

```bash
dotnet add package JoyfulReaperLib.Caching.Sqlite
```

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
