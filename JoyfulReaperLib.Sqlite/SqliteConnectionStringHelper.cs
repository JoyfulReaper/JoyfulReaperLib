using Microsoft.Data.Sqlite;

namespace JoyfulReaperLib.Sqlite;

public static class SqliteConnectionStringHelper
{
    public static string Resolve(string connectionString, string? basePath = null)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("SQLite connection string must include a Data Source.");
        }

        var builder = new SqliteConnectionStringBuilder(connectionString);
        if (string.IsNullOrWhiteSpace(builder.DataSource))
        {
            throw new InvalidOperationException("SQLite connection string must include a Data Source.");
        }

        if (IsSpecialDataSource(builder.DataSource))
        {
            return builder.ToString();
        }

        if (!Path.IsPathRooted(builder.DataSource))
        {
            var resolvedBasePath = ResolveBasePath(basePath);
            Directory.CreateDirectory(resolvedBasePath);
            builder.DataSource = Path.GetFullPath(Path.Combine(resolvedBasePath, builder.DataSource));
            return builder.ToString();
        }

        builder.DataSource = Path.GetFullPath(builder.DataSource);
        var directory = Path.GetDirectoryName(builder.DataSource);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return builder.ToString();
    }

    private static string ResolveBasePath(string? basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
        {
            return Path.Combine(AppContext.BaseDirectory, "Data");
        }

        return Path.IsPathRooted(basePath)
            ? Path.GetFullPath(basePath)
            : Path.GetFullPath(basePath, AppContext.BaseDirectory);
    }

    private static bool IsSpecialDataSource(string dataSource)
        => string.Equals(dataSource, ":memory:", StringComparison.OrdinalIgnoreCase)
           || dataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase);
}
