namespace JoyfulReaperLib.WebStats.Sqlite;

public sealed class SqliteHitCounterOptions
{
    public required string ConnectionString { get; set; }

    public string? BasePath { get; set; }
}
