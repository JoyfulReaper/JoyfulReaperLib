using Microsoft.Data.Sqlite;

namespace JoyfulReaperLib.Sqlite;

public static class SqliteDatabaseInitializer
{
    public static string Initialize(
        string dbFileName,
        string schemaSql,
        string? basePath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dbFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(schemaSql);

        SqliteProviderInitializer.Initialize();

        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dbFileName,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ToString();

        connectionString = SqliteConnectionStringHelper.Resolve(connectionString, basePath);

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = schemaSql;
        command.ExecuteNonQuery();

        return connectionString;
    }
}