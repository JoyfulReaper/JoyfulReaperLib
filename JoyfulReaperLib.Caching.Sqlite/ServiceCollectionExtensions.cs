using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace JoyfulReaperLib.Caching.Sqlite;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJoyfulReaperSqliteDistributedCache(
        this IServiceCollection services,
        Action<SqliteDistributedCacheOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        SqliteProviderInitializer.Initialize();

        services
            .AddOptions<SqliteDistributedCacheOptions>()
            .Configure(configure);

        services.AddSingleton<IDistributedCache, SqliteDistributedCache>();

        return services;
    }
}
