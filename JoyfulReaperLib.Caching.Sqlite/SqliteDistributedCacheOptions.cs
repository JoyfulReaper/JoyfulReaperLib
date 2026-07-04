namespace JoyfulReaperLib.Caching.Sqlite;

public sealed class SqliteDistributedCacheOptions
{
    public required string ConnectionString { get; set; }

    public string? BasePath { get; set; }
}
