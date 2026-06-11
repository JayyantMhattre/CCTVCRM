using Ashraak.Infrastructure.Shared.Cache;
using Ashraak.Infrastructure.Shared.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Infrastructure.Shared.Extensions;

/// <summary>
/// DI extension that registers services shared by all modules:
/// the PostgreSQL connection factory and the Redis connection factory.
/// </summary>
/// <remarks>
/// Call this once from <c>Ashraak.Api.Program</c> before registering individual modules.
/// Module infrastructure projects depend on <see cref="IDatabaseConnectionFactory"/> and
/// <see cref="IRedisConnectionFactory"/> (both resolved from DI) for their Dapper read queries.
/// </remarks>
public static class SharedInfrastructureExtensions
{
    /// <summary>
    /// Registers <see cref="NpgsqlConnectionFactory"/> and <see cref="RedisConnectionFactory"/>
    /// as their respective interface implementations.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <returns>The same <paramref name="services"/> for chaining.</returns>
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseConnectionFactory, NpgsqlConnectionFactory>();
        services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();

        return services;
    }
}
