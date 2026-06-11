using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Ashraak.Infrastructure.Shared.Cache;

/// <summary>
/// Wraps ConnectionMultiplexer so a single multiplexed TCP connection is shared
/// across all Redis callers for the lifetime of the application.
/// Configured with retry policy and connection resilience options.
/// </summary>
public sealed class RedisConnectionFactory : IRedisConnectionFactory, IDisposable
{
    private readonly ConnectionMultiplexer _multiplexer;

    public RedisConnectionFactory(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' is not configured.");

        var options = ConfigurationOptions.Parse(connectionString);
        options.AbortOnConnectFail = false;
        options.ConnectRetry = 3;
        options.ReconnectRetryPolicy = new LinearRetry(1000);

        _multiplexer = ConnectionMultiplexer.Connect(options);
    }

    public IDatabase GetDatabase(int db = -1) => _multiplexer.GetDatabase(db);

    public IServer GetServer(string host, int port) => _multiplexer.GetServer(host, port);

    public void Dispose() => _multiplexer.Dispose();
}
