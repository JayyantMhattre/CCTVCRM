# Caching — Registration

Entry point: `BackEnd/src/Modules/Caching/Ashraak.Caching.Redis/CachingModule.cs`

```csharp
public static IServiceCollection AddCachingModule(this IServiceCollection services, IConfiguration configuration)
```

Called first in host `ModuleExtensions.AddModules` — **Layer 0**.

## Redis connection

```csharp
var connectionString = configuration.GetConnectionString("Redis")
    ?? throw new InvalidOperationException("Redis connection string 'Redis' is required.");

var multiplexer = ConnectionMultiplexer.Connect(connectionString);
services.AddSingleton<IConnectionMultiplexer>(multiplexer);
```

Uses raw `ConnectionMultiplexer.Connect` — no custom `ConfigurationOptions`.

Default database: `-1` (Redis default).

## Service registrations

| Service | Implementation | Lifetime |
|---------|----------------|----------|
| `IConnectionMultiplexer` | Connected multiplexer | Singleton |
| `IMemoryCache` | `AddMemoryCache()` | Singleton |
| `ICacheService` | `RedisCacheService` | Singleton |
| `IDistributedLockService` | `RedisDistributedLockService` | Singleton |
| `ISessionCacheService` | `SessionCacheService` | Singleton |
| `ICacheInvalidationService` | `CacheInvalidationService` | Singleton |

All implementations are `internal sealed` in Redis project.

## Startup side effect

```csharp
CacheKeyBuilder.SetEnvironment(
    (configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development").ToLowerInvariant());
```

## Host reference

Only `Ashraak.Api` references `Ashraak.Caching.Redis`.

Feature modules reference **`Ashraak.Caching.Abstractions` only**:

- `Ashraak.Auth.Infrastructure`, `Ashraak.Auth.Api`
- `Ashraak.Tenant.Infrastructure`
- `Ashraak.Users.Infrastructure` (no usage yet)

## Configuration keys

| Key | Required | Purpose |
|-----|----------|---------|
| `ConnectionStrings:Redis` | **Yes** | Redis connection (throws if missing) |
| `ASPNETCORE_ENVIRONMENT` | No | Cache key env prefix (default `development`) |

Local default: `localhost:6379`

Docker: `ConnectionStrings__Redis` with password, timeouts:

```
redis:6379,password=...,abortConnect=false,connectTimeout=5000,syncTimeout=5000
```

## Health check

Host registers Redis health check on `/health/ready` with tag `ready`.

## Packages (`Ashraak.Caching.Redis.csproj`)

| Package | Used |
|---------|------|
| `StackExchange.Redis` | Yes |
| `Microsoft.Extensions.Caching.Memory` | Yes |
| `Microsoft.Extensions.Caching.StackExchangeRedis` | Referenced, not used in CachingModule |

## Not registered here

- Host output cache — separate registration in `Program.cs`
- `IRedisConnectionFactory` from Infrastructure.Shared — unused
