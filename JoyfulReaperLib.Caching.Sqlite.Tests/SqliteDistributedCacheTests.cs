using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Data.Sqlite;

namespace JoyfulReaperLib.Caching.Sqlite.Tests;

[TestClass]
public class SqliteDistributedCacheTests
{
    [TestMethod]
    public async Task SetAsync_ThenGetAsync_ReturnsStoredBytes()
    {
        using var fixture = new SqliteCacheFixture();
        var cache = fixture.CreateCache("cache.db");
        var value = new byte[] { 1, 2, 3, 4 };

        await cache.SetAsync("alpha", value, new DistributedCacheEntryOptions());
        var result = await cache.GetAsync("alpha");

        CollectionAssert.AreEqual(value, result);
    }

    [TestMethod]
    public async Task GetAsync_MissingKey_ReturnsNull()
    {
        using var fixture = new SqliteCacheFixture();
        var cache = fixture.CreateCache("cache.db");

        var result = await cache.GetAsync("missing");

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task RemoveAsync_RemovesExistingKey()
    {
        using var fixture = new SqliteCacheFixture();
        var cache = fixture.CreateCache("cache.db");

        await cache.SetAsync("alpha", new byte[] { 7 }, new DistributedCacheEntryOptions());
        await cache.RemoveAsync("alpha");

        Assert.IsNull(await cache.GetAsync("alpha"));
    }

    [TestMethod]
    public async Task AbsoluteExpirationRelativeToNow_ExpiresEntry()
    {
        using var fixture = new SqliteCacheFixture();
        var cache = fixture.CreateCache("absolute.db");

        await cache.SetAsync(
            "alpha",
            new byte[] { 7 },
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(250)
            });

        await Task.Delay(700);

        Assert.IsNull(await cache.GetAsync("alpha"));
    }

    [TestMethod]
    public async Task SlidingExpiration_GetAsyncRefreshesEntryLifetime()
    {
        using var fixture = new SqliteCacheFixture();
        var cache = fixture.CreateCache("sliding.db");

        await cache.SetAsync(
            "alpha",
            new byte[] { 7 },
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(1)
            });

        await Task.Delay(500);
        Assert.IsNotNull(await cache.GetAsync("alpha"));

        await Task.Delay(700);
        Assert.IsNotNull(await cache.GetAsync("alpha"));

        await Task.Delay(1200);
        Assert.IsNull(await cache.GetAsync("alpha"));
    }

    [TestMethod]
    public async Task RelativeDataSource_UsesBasePath()
    {
        using var fixture = new SqliteCacheFixture();
        var cache = fixture.CreateCache("relative-cache.db");
        var expectedPath = Path.Combine(fixture.BasePath, "relative-cache.db");

        await cache.SetAsync("alpha", new byte[] { 1 }, new DistributedCacheEntryOptions());

        Assert.IsTrue(File.Exists(expectedPath), $"Expected database at {expectedPath}");
    }

    [TestMethod]
    public void ProviderInitialization_CanBeCalledMultipleTimes()
    {
        SqliteProviderInitializer.Initialize();
        SqliteProviderInitializer.Initialize();
    }

    [TestMethod]
    public void SyncMethods_DelegateSuccessfully()
    {
        using var fixture = new SqliteCacheFixture();
        var cache = fixture.CreateCache("sync.db");
        var value = new byte[] { 9, 8, 7 };

        cache.Set("alpha", value, new DistributedCacheEntryOptions());
        var result = cache.Get("alpha");
        cache.Remove("alpha");

        CollectionAssert.AreEqual(value, result);
        Assert.IsNull(cache.Get("alpha"));
    }

    [TestMethod]
    public void AddJoyfulReaperSqliteDistributedCache_RegistersDistributedCache()
    {
        var services = new ServiceCollection();

        services.AddJoyfulReaperSqliteDistributedCache(options =>
        {
            options.ConnectionString = "Data Source=service-cache.db";
            options.BasePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        });

        using var provider = services.BuildServiceProvider();
        var cache = provider.GetRequiredService<IDistributedCache>();
        var options = provider.GetRequiredService<IOptions<SqliteDistributedCacheOptions>>();

        Assert.IsInstanceOfType<SqliteDistributedCache>(cache);
        Assert.AreEqual("Data Source=service-cache.db", options.Value.ConnectionString);
    }

    private sealed class SqliteCacheFixture : IDisposable
    {
        public SqliteCacheFixture()
        {
            BasePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(BasePath);
            SqliteProviderInitializer.Initialize();
        }

        public string BasePath { get; }

        public SqliteDistributedCache CreateCache(string dataSource)
        {
            var options = Options.Create(new SqliteDistributedCacheOptions
            {
                ConnectionString = $"Data Source={dataSource};Pooling=False",
                BasePath = BasePath
            });

            return new SqliteDistributedCache(options);
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
