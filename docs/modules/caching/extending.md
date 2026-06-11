# Caching — Extending

## Use ICacheService in a new module

1. Add project reference to `Ashraak.Caching.Abstractions` only (not Redis)
2. Inject `ICacheService` in Infrastructure service
3. Build keys with `CacheKeyBuilder`:

```csharp
var key = CacheKeyBuilder.ForEntity(tenantId, "mymodule", "entity", entityId);
await _cache.GetOrSetAsync(key, () => _repo.GetAsync(id), TimeSpan.FromMinutes(5), ct);
```

4. Ensure Caching remains Layer 0 in `AddModules` — no change needed if already first

## Wire event-driven invalidation

In event handler after state change:

```csharp
public class RoleAssignedEventHandler(
    ICacheInvalidationService cacheInvalidation) : INotificationHandler<RoleAssignedEvent>
{
    public async Task Handle(RoleAssignedEvent e, CancellationToken ct)
    {
        await cacheInvalidation.InvalidatePermissionsAsync(e.TenantId, e.UserId);
    }
}
```

Inject `ICacheInvalidationService` — already registered in Caching module.

## Use distributed locks

For concurrent operations (seat allocation, slug generation):

```csharp
await using var lockHandle = await _lockService.AcquireLockAsync("provision-slug", tenantId, ct: ct);
if (!lockHandle.IsAcquired) return Result.Failure(Error.Conflict("Could not acquire lock"));
// critical section
```

`IDistributedLockService` registered but unused — safe to adopt.

## Add rate limiting

Use `CacheKeyBuilder.ForRateLimit(tenantId, endpoint)` with Redis INCR pattern — key builder exists; middleware not implemented.

## Custom TTL strategy

Pass explicit expiry to `SetAsync` / `GetOrSetAsync`:

```csharp
await _cache.SetAsync(key, value, TimeSpan.FromHours(8), ct);
```

L1 still capped at 60s on set.

## Replace ConnectionMultiplexer setup

For production resilience, extend `CachingModule.cs`:

```csharp
var options = ConfigurationOptions.Parse(connectionString);
options.AbortOnConnectFail = false;
options.ConnectRetry = 3;
var multiplexer = ConnectionMultiplexer.Connect(options);
```

Or adopt `RedisConnectionFactory` from Infrastructure.Shared.

## Add cache stampede protection

Extend `GetOrSetAsync` with per-key semaphores or use distributed lock around factory execution for hot keys.

## Monitoring hooks

Add `IConnectionMultiplexer` event handlers for connection failures:

```csharp
multiplexer.ConnectionFailed += (_, e) => _logger.LogError(...);
```

## Module boundaries

- Feature modules: Abstractions reference only
- Only host references `Ashraak.Caching.Redis`
- Do not embed Redis types in module public APIs

## Testing

Mock `ICacheService` in unit tests. Integration tests need Redis container or `Testcontainers.Redis`.
