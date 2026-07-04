# JoyfulReaperLib
A library of code that I have found helpful!

Optional SQLite distributed cache package:

```csharp
services.AddJoyfulReaperSqliteDistributedCache(options =>
{
    options.ConnectionString = "Data Source=cache.db";
});
```
