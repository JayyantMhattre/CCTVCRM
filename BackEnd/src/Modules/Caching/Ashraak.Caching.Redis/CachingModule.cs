using Ashraak.Caching.Abstractions;
using Ashraak.Caching.Redis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Ashraak.Caching.Redis;

/// <summary>
/// DI registration entry point for the Caching module.
/// Wires the Redis <see cref="ConnectionMultiplexer"/>, L1 memory cache,
/// <see cref="ICacheService"/>, and <see cref="IDistributedLockService"/>
/// plus higher-level facades for sessions and invalidation strategy.
/// </summary>
/// <remarks>
/// Called once from <c>Ashraak.Api.Program</c> before any module that
/// depends on <see cref="ICacheService"/> is registered.
/// The <see cref="ConnectionMultiplexer"/> is intentionally registered as a singleton
/// because the multiplexed connection is thread-safe and intended to be shared for the
/// application's lifetime.
/// </remarks>
public static class CachingModule
{
    /// <summary>
    /// Registers all Caching module services into the DI container.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="configuration">
    /// Application configuration used to read the <c>ConnectionStrings:Redis</c> value
    /// and <c>ASPNETCORE_ENVIRONMENT</c> for cache key prefixing.
    /// </param>
    /// <returns>The same <paramref name="services"/> for chaining.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the <c>ConnectionStrings:Redis</c> configuration value is absent.
    /// </exception>
    public static IServiceCollection AddCachingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis connection string 'Redis' is required.");

        var multiplexer = ConnectionMultiplexer.Connect(connectionString);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);

        services.AddMemoryCache();

        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<IDistributedLockService, RedisDistributedLockService>();
        services.AddSingleton<ISessionCacheService, SessionCacheService>();
        services.AddSingleton<ICacheInvalidationService, CacheInvalidationService>();

        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";
        CacheKeyBuilder.SetEnvironment(environment.ToLowerInvariant());

        return services;
    }
}
