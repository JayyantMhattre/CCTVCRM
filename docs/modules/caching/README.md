# Caching Module

Two-tier caching (L1 in-memory + L2 Redis) with tenant-scoped keys, session storage, cache invalidation, and distributed locks.

**Source:** `BackEnd/src/Modules/Caching/`

## Projects

| Project | Path | Purpose |
|---------|------|---------|
| `Ashraak.Caching.Abstractions` | `Caching/Ashraak.Caching.Abstractions/` | Interfaces + `CacheKeyBuilder` (no Redis dep) |
| `Ashraak.Caching.Redis` | `Caching/Ashraak.Caching.Redis/` | L1+L2 implementation + DI |

## Key facts

- **Layer 0** = first module registered in host (other modules depend on `ICacheService`)
- **L1:** `IMemoryCache`, 60s default TTL
- **L2:** Redis via `StackExchange.Redis`, 30 min default TTL
- **Active consumers:** Auth (permissions, sessions), Tenant (config, feature flags)
- **Registered but unused:** `ICacheInvalidationService`, `IDistributedLockService`

## Module documentation

- [Architecture](./architecture.md)
- [Registration](./registration.md)
- [API](./api.md)
- [Events](./events.md)
- [Extending](./extending.md)
- [Operations](./operations.md)

## Dependencies

- Redis required at startup — missing connection string throws
- Host output cache also uses same Redis connection
