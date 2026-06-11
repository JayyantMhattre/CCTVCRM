# Caching Module Status

> **Canonical:** [docs/modules/caching/](../../../docs/modules/caching/README.md)

This document validates the Caching module against the requested scope and records incremental updates made in this phase.

## Requirement Coverage

### 1) Abstraction (`ICacheService`)

Implemented and retained:

- `ICacheService` in `Ashraak.Caching.Abstractions`
- Generic read/write/get-or-set/remove/remove-by-prefix/exists operations

Additional domain-oriented abstractions added:

- `ISessionCacheService`
- `ICacheInvalidationService`

### 2) Memory + Redis Implementation

Implemented and retained:

- `RedisCacheService`
  - L1: in-process memory cache (`IMemoryCache`)
  - L2: Redis (`StackExchange.Redis`)

### 3) Use Cases

- **Permissions**
  - `AuthPermissionChecker` caches permission snapshots with `CacheKeyBuilder.ForPermissions(...)`.
- **Sessions** (added integration)
  - `AuthEndpoints` token issuance now writes session metadata via `ISessionCacheService`.
  - Session keys use `CacheKeyBuilder.ForSession(...)`.
- **Config**
  - `TenantService` caches tenant configuration and feature flags.
  - Keys use entity/config and feature-flag cache keys.

### 4) Cache Invalidation Strategy

Added explicit strategy service:

- `ICacheInvalidationService`
- `CacheInvalidationService`

Provided invalidation paths:

- user permission cache segment
- user session cache segment
- tenant config/feature flags segment
- module-prefix fallback invalidation

## Interfaces (Output)

- `ICacheService`
- `IDistributedLockService` / `IDistributedLock`
- `ISessionCacheService` (new)
- `ICacheInvalidationService` (new)

## Implementation Structure (Output)

```text
Modules/Caching
  Ashraak.Caching.Abstractions/
    ICacheService.cs
    IDistributedLockService.cs
    CacheKeyBuilder.cs
    ISessionCacheService.cs            (new)
    ICacheInvalidationService.cs       (new)

  Ashraak.Caching.Redis/
    CachingModule.cs
    Services/
      RedisCacheService.cs
      RedisDistributedLockService.cs
      SessionCacheService.cs           (new)
      CacheInvalidationService.cs      (new)
```

## Integration Points (Output)

- Host startup (`Program.cs`)
  - `AddCachingModule(...)` registers cache core + new facades.
- Auth module
  - Permission cache in `AuthPermissionChecker`.
  - Session cache write on token issuance in `AuthEndpoints`.
- Tenant module
  - Config/feature-flag caching in `TenantService`.

## Invalidation Strategy Summary

- Fine-grained invalidation (`RemoveAsync`) for permissions/session keys.
- Segment invalidation (`RemoveByPrefixAsync`) for module-level tenant caches.
- Tenant config invalidation combines targeted key removal + module-prefix fallback.
