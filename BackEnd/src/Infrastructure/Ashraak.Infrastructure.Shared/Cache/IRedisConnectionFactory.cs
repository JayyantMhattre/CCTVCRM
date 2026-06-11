using StackExchange.Redis;

namespace Ashraak.Infrastructure.Shared.Cache;

/// <summary>
/// Abstraction over the StackExchange.Redis multiplexer.
/// Allows modules to obtain a Redis database without taking a hard dependency
/// on connection string configuration details.
/// </summary>
public interface IRedisConnectionFactory
{
    IDatabase GetDatabase(int db = -1);
    IServer GetServer(string host, int port);
}
