using JoyfulReaperLib.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace JoyfulReaperLib.WebStats.Sqlite;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJoyfulReaperSqliteHitCounter(
        this IServiceCollection services,
        Action<SqliteHitCounterOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        SqliteProviderInitializer.Initialize();

        services
            .AddOptions<SqliteHitCounterOptions>()
            .Configure(configure);

        services.AddSingleton<IHitCounter, SqliteHitCounter>();

        return services;
    }
}
