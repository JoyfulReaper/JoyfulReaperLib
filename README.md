# JoyfulReaperLib
A library of code that I have found helpful!

Optional package: `JoyfulReaperLib.Caching.Sqlite`

Install:

```bash
dotnet add package JoyfulReaperLib.Caching.Sqlite
```

This package provides a SQLite-backed `IDistributedCache`.
It does not register HybridCache.

```csharp
services.AddJoyfulReaperSqliteDistributedCache(options =>
{
    options.ConnectionString = "Data Source=cache.db";
});
```
