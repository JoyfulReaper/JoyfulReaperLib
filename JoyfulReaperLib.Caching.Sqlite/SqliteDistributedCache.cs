using System.Data;
using System.Globalization;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace JoyfulReaperLib.Caching.Sqlite;

public sealed class SqliteDistributedCache : IDistributedCache
{
    private readonly string _connectionString;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private bool _initialized;

    public SqliteDistributedCache(IOptions<SqliteDistributedCacheOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        SqliteProviderInitializer.Initialize();

        _connectionString = ResolveConnectionString(options.Value);
    }

    public byte[]? Get(string key)
        => GetAsync(key).GetAwaiter().GetResult();

    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        await EnsureInitializedAsync(token);

        await using var connection = CreateConnection();
        await connection.OpenAsync(token);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Value, ExpiresAtTimeUtc, SlidingExpirationInSeconds, AbsoluteExpirationUtc
            FROM CacheEntries
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$id", key);

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, token);
        if (!await reader.ReadAsync(token))
        {
            return null;
        }

        var expiresAt = ReadNullableDateTimeOffset(reader, 1);
        if (IsExpired(expiresAt))
        {
            await reader.DisposeAsync();
            await DeleteEntryAsync(connection, key, token);
            return null;
        }

        var value = (byte[])reader["Value"];
        var slidingExpiration = reader.IsDBNull(2)
            ? (TimeSpan?)null
            : TimeSpan.FromSeconds(reader.GetInt64(2));
        var absoluteExpiration = ReadNullableDateTimeOffset(reader, 3);

        await reader.DisposeAsync();

        if (slidingExpiration.HasValue)
        {
            await RefreshEntryAsync(connection, key, absoluteExpiration, slidingExpiration.Value, token);
        }

        return value;
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        => SetAsync(key, value, options).GetAwaiter().GetResult();

    public async Task SetAsync(
        string key,
        byte[] value,
        DistributedCacheEntryOptions options,
        CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(options);

        await EnsureInitializedAsync(token);

        var now = DateTimeOffset.UtcNow;
        var absoluteExpiration = GetAbsoluteExpiration(now, options);
        var slidingExpiration = options.SlidingExpiration;
        var expiresAt = GetEffectiveExpiration(now, absoluteExpiration, slidingExpiration);
        var slidingExpirationSeconds = slidingExpiration.HasValue
            ? (object)(long)Math.Ceiling(slidingExpiration.Value.TotalSeconds)
            : DBNull.Value;

        await using var connection = CreateConnection();
        await connection.OpenAsync(token);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO CacheEntries (
                Id,
                Value,
                ExpiresAtTimeUtc,
                SlidingExpirationInSeconds,
                AbsoluteExpirationUtc)
            VALUES (
                $id,
                $value,
                $expiresAt,
                $slidingExpirationSeconds,
                $absoluteExpiration)
            ON CONFLICT(Id) DO UPDATE SET
                Value = excluded.Value,
                ExpiresAtTimeUtc = excluded.ExpiresAtTimeUtc,
                SlidingExpirationInSeconds = excluded.SlidingExpirationInSeconds,
                AbsoluteExpirationUtc = excluded.AbsoluteExpirationUtc;
            """;
        command.Parameters.AddWithValue("$id", key);
        command.Parameters.Add("$value", SqliteType.Blob).Value = value;
        command.Parameters.AddWithValue("$expiresAt", ToDbValue(expiresAt));
        command.Parameters.AddWithValue("$slidingExpirationSeconds", slidingExpirationSeconds);
        command.Parameters.AddWithValue("$absoluteExpiration", ToDbValue(absoluteExpiration));

        await command.ExecuteNonQueryAsync(token);
        await DeleteExpiredEntriesAsync(connection, token);
    }

    public void Refresh(string key)
        => RefreshAsync(key).GetAwaiter().GetResult();

    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        await EnsureInitializedAsync(token);

        await using var connection = CreateConnection();
        await connection.OpenAsync(token);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT ExpiresAtTimeUtc, SlidingExpirationInSeconds, AbsoluteExpirationUtc
            FROM CacheEntries
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$id", key);

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, token);
        if (!await reader.ReadAsync(token))
        {
            return;
        }

        var expiresAt = ReadNullableDateTimeOffset(reader, 0);
        if (IsExpired(expiresAt))
        {
            await reader.DisposeAsync();
            await DeleteEntryAsync(connection, key, token);
            return;
        }

        if (reader.IsDBNull(1))
        {
            return;
        }

        var slidingExpiration = TimeSpan.FromSeconds(reader.GetInt64(1));
        var absoluteExpiration = ReadNullableDateTimeOffset(reader, 2);

        await reader.DisposeAsync();
        await RefreshEntryAsync(connection, key, absoluteExpiration, slidingExpiration, token);
    }

    public void Remove(string key)
        => RemoveAsync(key).GetAwaiter().GetResult();

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        await EnsureInitializedAsync(token);

        await using var connection = CreateConnection();
        await connection.OpenAsync(token);

        await DeleteEntryAsync(connection, key, token);
    }

    private async Task EnsureInitializedAsync(CancellationToken token)
    {
        if (_initialized)
        {
            return;
        }

        await _initializationLock.WaitAsync(token);
        try
        {
            if (_initialized)
            {
                return;
            }

            await using var connection = CreateConnection();
            await connection.OpenAsync(token);
            await CreateSchemaAsync(connection, token);
            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    private static async Task CreateSchemaAsync(SqliteConnection connection, CancellationToken token)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            PRAGMA journal_mode = WAL;
            PRAGMA synchronous = NORMAL;

            CREATE TABLE IF NOT EXISTS CacheEntries (
                Id TEXT NOT NULL PRIMARY KEY,
                Value BLOB NOT NULL,
                ExpiresAtTimeUtc TEXT NULL,
                SlidingExpirationInSeconds INTEGER NULL,
                AbsoluteExpirationUtc TEXT NULL
            );

            CREATE INDEX IF NOT EXISTS IX_CacheEntries_ExpiresAtTimeUtc
            ON CacheEntries (ExpiresAtTimeUtc);
            """;

        await command.ExecuteNonQueryAsync(token);
    }

    private static async Task DeleteExpiredEntriesAsync(SqliteConnection connection, CancellationToken token)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            DELETE FROM CacheEntries
            WHERE ExpiresAtTimeUtc IS NOT NULL
              AND ExpiresAtTimeUtc <= $utcNow;
            """;
        command.Parameters.AddWithValue("$utcNow", ToDbValue(DateTimeOffset.UtcNow));

        await command.ExecuteNonQueryAsync(token);
    }

    private static async Task DeleteEntryAsync(SqliteConnection connection, string key, CancellationToken token)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM CacheEntries WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", key);

        await command.ExecuteNonQueryAsync(token);
    }

    private static async Task RefreshEntryAsync(
        SqliteConnection connection,
        string key,
        DateTimeOffset? absoluteExpiration,
        TimeSpan slidingExpiration,
        CancellationToken token)
    {
        var newExpiration = DateTimeOffset.UtcNow.Add(slidingExpiration);
        if (absoluteExpiration.HasValue && absoluteExpiration.Value < newExpiration)
        {
            newExpiration = absoluteExpiration.Value;
        }

        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE CacheEntries
            SET ExpiresAtTimeUtc = $expiresAt
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$id", key);
        command.Parameters.AddWithValue("$expiresAt", ToDbValue(newExpiration));

        await command.ExecuteNonQueryAsync(token);
    }

    private SqliteConnection CreateConnection()
        => new(_connectionString);

    private static string ResolveConnectionString(SqliteDistributedCacheOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new InvalidOperationException("SQLite cache connection string must include a Data Source.");
        }

        var builder = new SqliteConnectionStringBuilder(options.ConnectionString);
        if (string.IsNullOrWhiteSpace(builder.DataSource))
        {
            throw new InvalidOperationException("SQLite cache connection string must include a Data Source.");
        }

        if (IsSpecialDataSource(builder.DataSource))
        {
            return builder.ToString();
        }

        if (!Path.IsPathRooted(builder.DataSource))
        {
            var basePath = ResolveBasePath(options.BasePath);
            Directory.CreateDirectory(basePath);
            builder.DataSource = Path.GetFullPath(Path.Combine(basePath, builder.DataSource));
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

    private static DateTimeOffset? GetAbsoluteExpiration(
        DateTimeOffset now,
        DistributedCacheEntryOptions options)
    {
        if (options.AbsoluteExpirationRelativeToNow.HasValue)
        {
            return now.Add(options.AbsoluteExpirationRelativeToNow.Value);
        }

        return options.AbsoluteExpiration;
    }

    private static DateTimeOffset? GetEffectiveExpiration(
        DateTimeOffset now,
        DateTimeOffset? absoluteExpiration,
        TimeSpan? slidingExpiration)
    {
        if (!slidingExpiration.HasValue)
        {
            return absoluteExpiration;
        }

        var slidingExpiresAt = now.Add(slidingExpiration.Value);
        if (!absoluteExpiration.HasValue || slidingExpiresAt < absoluteExpiration.Value)
        {
            return slidingExpiresAt;
        }

        return absoluteExpiration;
    }

    private static DateTimeOffset? ReadNullableDateTimeOffset(SqliteDataReader reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal))
        {
            return null;
        }

        return DateTimeOffset.ParseExact(
            reader.GetString(ordinal),
            "O",
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind);
    }

    private static object ToDbValue(DateTimeOffset? value)
        => value?.ToString("O", CultureInfo.InvariantCulture) ?? (object)DBNull.Value;

    private static bool IsExpired(DateTimeOffset? expiresAt)
        => expiresAt.HasValue && expiresAt.Value <= DateTimeOffset.UtcNow;
}
