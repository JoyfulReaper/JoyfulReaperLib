using JoyfulReaperLib.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JoyfulReaperLib.WebStats.Sqlite.Tests;

[TestClass]
public class SqliteHitCounterTests
{
    [TestMethod]
    public async Task GetHitCountsAsync_EmptyDatabase_ReturnsZeroCounts()
    {
        using var fixture = new SqliteHitCounterFixture();
        var counter = fixture.CreateCounter("empty.db");

        var result = await counter.GetHitCountsAsync();

        Assert.AreEqual(0, result.TotalHits);
        Assert.AreEqual(0, result.UniqueVisitors);
    }

    [TestMethod]
    public async Task RecordHitAsync_OneVisitor_ReturnsOneHitAndOneVisitor()
    {
        using var fixture = new SqliteHitCounterFixture();
        var counter = fixture.CreateCounter("single.db");

        var result = await counter.RecordHitAsync("visitor-a");

        Assert.AreEqual(1, result.TotalHits);
        Assert.AreEqual(1, result.UniqueVisitors);
    }

    [TestMethod]
    public async Task RecordHitAsync_SameVisitorTwice_ReturnsTwoHitsAndOneVisitor()
    {
        using var fixture = new SqliteHitCounterFixture();
        var counter = fixture.CreateCounter("repeat.db");

        await counter.RecordHitAsync("visitor-a");
        var result = await counter.RecordHitAsync("visitor-a");

        Assert.AreEqual(2, result.TotalHits);
        Assert.AreEqual(1, result.UniqueVisitors);
    }

    [TestMethod]
    public async Task RecordHitAsync_TwoVisitors_ReturnsTwoHitsAndTwoVisitors()
    {
        using var fixture = new SqliteHitCounterFixture();
        var counter = fixture.CreateCounter("multi.db");

        await counter.RecordHitAsync("visitor-a");
        var result = await counter.RecordHitAsync("visitor-b");

        Assert.AreEqual(2, result.TotalHits);
        Assert.AreEqual(2, result.UniqueVisitors);
    }

    [TestMethod]
    public async Task RecordHitAsync_RepeatHit_UpdatesLastSeen()
    {
        using var fixture = new SqliteHitCounterFixture();
        var counter = fixture.CreateCounter("last-seen.db");
        const string visitorKey = "visitor-a";

        await counter.RecordHitAsync(visitorKey);
        var firstLastSeen = await fixture.GetLastSeenAsync("last-seen.db", visitorKey);

        await Task.Delay(20);
        await counter.RecordHitAsync(visitorKey);
        var secondLastSeen = await fixture.GetLastSeenAsync("last-seen.db", visitorKey);

        Assert.IsNotNull(firstLastSeen);
        Assert.IsNotNull(secondLastSeen);
        Assert.IsTrue(secondLastSeen > firstLastSeen);
    }

    [TestMethod]
    public async Task RelativeDataSource_UsesBasePath()
    {
        using var fixture = new SqliteHitCounterFixture();
        var counter = fixture.CreateCounterWithBasePath("Data Source=stats.db;Pooling=False");
        var expectedPath = Path.Combine(fixture.BasePath, "stats.db");

        await counter.RecordHitAsync("visitor-a");

        Assert.IsTrue(File.Exists(expectedPath), $"Expected database at {expectedPath}");
    }

    [TestMethod]
    public void AddJoyfulReaperSqliteHitCounter_RegistersIHitCounter()
    {
        var services = new ServiceCollection();

        services.AddJoyfulReaperSqliteHitCounter(options =>
        {
            options.ConnectionString = "Data Source=service-stats.db";
        });

        using var provider = services.BuildServiceProvider();
        var counter = provider.GetRequiredService<IHitCounter>();
        var options = provider.GetRequiredService<IOptions<SqliteHitCounterOptions>>();

        Assert.IsInstanceOfType<SqliteHitCounter>(counter);
        Assert.AreEqual("Data Source=service-stats.db", options.Value.ConnectionString);
    }

    [TestMethod]
    public void ProviderInitialization_CanBeCalledMultipleTimes()
    {
        SqliteProviderInitializer.Initialize();
        SqliteProviderInitializer.Initialize();
    }

    private sealed class SqliteHitCounterFixture : IDisposable
    {
        public SqliteHitCounterFixture()
        {
            BasePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(BasePath);
            SqliteProviderInitializer.Initialize();
        }

        public string BasePath { get; }

        public SqliteHitCounter CreateCounter(string dataSource)
        {
            var options = Options.Create(new SqliteHitCounterOptions
            {
                ConnectionString = $"Data Source={Path.Combine(BasePath, dataSource)};Pooling=False"
            });

            return new SqliteHitCounter(options);
        }

        public SqliteHitCounter CreateCounterWithBasePath(string connectionString)
        {
            var options = Options.Create(new SqliteHitCounterOptions
            {
                ConnectionString = connectionString,
                BasePath = BasePath
            });

            return new SqliteHitCounter(options);
        }

        public async Task<DateTimeOffset?> GetLastSeenAsync(string dataSource, string visitorKey)
        {
            var connectionString = $"Data Source={Path.Combine(BasePath, dataSource)};Pooling=False";
            await using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT LastSeen FROM Visitors WHERE IpAddress = $visitorKey;";
            command.Parameters.AddWithValue("$visitorKey", visitorKey);

            var result = await command.ExecuteScalarAsync();
            if (result is null or DBNull)
            {
                return null;
            }

            return DateTimeOffset.Parse((string)result);
        }

        public void Dispose()
        {
            SqliteConnection.ClearAllPools();

            if (Directory.Exists(BasePath))
            {
                for (var attempt = 0; attempt < 5; attempt++)
                {
                    try
                    {
                        Directory.Delete(BasePath, recursive: true);
                        break;
                    }
                    catch (IOException) when (attempt < 4)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
