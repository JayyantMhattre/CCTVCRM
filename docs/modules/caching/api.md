# Caching — API Reference

## ICacheService

Path: `Ashraak.Caching.Abstractions/ICacheService.cs`  
Implementation: `RedisCacheService.cs`

| Method | Behavior |
|--------|----------|
| `GetAsync<T>(string key, CancellationToken ct)` | L1 → L2 read, JSON deserialize |
| `GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry, ct)` | Cache-aside |
| `SetAsync<T>(string key, T value, TimeSpan? expiry, ct)` | Write L2 then L1 |
| `RemoveAsync(string key, ct)` | Delete L2 + L1 |
| `RemoveByPrefixAsync(string prefix, ct)` | SCAN keys, bulk delete |
| `ExistsAsync(string key, ct)` | L2 key exists check |

**Default L2 expiry:** 30 minutes (implementation constant)  
**Default L1 expiry:** 60 seconds (capped by L2 expiry on set)

## ISessionCacheService

Path: `Abstractions/ISessionCacheService.cs`  
Implementation: `SessionCacheService.cs`

| Method | Purpose |
|--------|---------|
| `GetSessionAsync(tenantId, userId)` | Read session |
| `SetSessionAsync(tenantId, userId, entry, expiry)` | Write session |
| `RemoveSessionAsync(tenantId, userId)` | Delete session |

**SessionCacheEntry record:** TenantId, UserId, Roles, Permissions, IssuedOnUtc

## ICacheInvalidationService

Path: `Abstractions/ICacheInvalidationService.cs`  
Implementation: `CacheInvalidationService.cs`

| Method | Invalidates |
|--------|-------------|
| `InvalidatePermissionsAsync(tenantId, userId)` | `ForPermissions` key |
| `InvalidateSessionAsync(tenantId, userId)` | `ForSession` key |
| `InvalidateTenantConfigAsync(tenantId)` | Config + feature flags + tenant prefix |
| `InvalidateModulePrefixAsync(tenantId, module)` | `ForTenantPrefix` |

**No callers in codebase yet.**

## IDistributedLockService

Path: `Abstractions/IDistributedLockService.cs`  
Implementation: `RedisDistributedLockService.cs`

| Method | Behavior |
|--------|----------|
| `AcquireLockAsync(resource, tenantId, waitTime?, ct)` | Returns `IDistributedLock` |

**IDistributedLock:** `IsAcquired`, `ReleaseAsync()`

**No callers in codebase yet.**

## CacheKeyBuilder

Path: `Abstractions/CacheKeyBuilder.cs` — static, no interface

| Method | Returns |
|--------|---------|
| `SetEnvironment(string env)` | void — call once at startup |
| `ForEntity(tenantId, module, entity, id)` | Full key string |
| `ForTenantPrefix(tenantId, module)` | Prefix for scan/delete |
| `ForSession(tenantId, userId)` | Session key |
| `ForPermissions(tenantId, userId)` | Permissions key |
| `ForFeatureFlags(tenantId)` | Feature flags key |
| `ForRateLimit(tenantId, endpoint)` | Rate limit key (no env) |
| `ForDistributedLock(resource, tenantId)` | Lock key |

## Usage examples from modules

### Auth permissions (10 min TTL)

```csharp
var key = CacheKeyBuilder.ForPermissions(tenantId, userId);
await _cache.GetOrSetAsync(key, () => LoadFromDbAsync(...), TimeSpan.FromMinutes(10), ct);
```

File: `AuthPermissionChecker.cs`

### Auth session (8 hours)

```csharp
await _sessionCache.SetSessionAsync(tenantId, userId, entry, TimeSpan.FromHours(8));
```

File: `AuthEndpoints.cs`

### Tenant config (5 min TTL)

```csharp
var key = CacheKeyBuilder.ForEntity(tenantId, "tenant", "config", tenantId);
await _cache.GetOrSetAsync(key, () => LoadTenantAsync(...), TimeSpan.FromMinutes(5), ct);
```

File: `TenantService.cs`
