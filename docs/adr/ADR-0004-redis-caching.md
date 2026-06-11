# ADR-0004: Redis Caching Layer

**Status:** Accepted  
**Date:** 2026-05-01

---

## Context

Permission checks and tenant metadata are read on every authenticated request. Database round-trips per request do not scale and complicate Auth/Tenant modules.

---

## Decision

Introduce a dedicated **Caching module** (Layer 0) providing:

- `ICacheService` — L1 memory + L2 Redis
- `IDistributedLockService` — Redis RedLock-style operations
- `ISessionCacheService` — session snapshots after token issue
- `CacheKeyBuilder` — tenant-scoped key conventions

Register Caching **before** Auth in `ModuleExtensions`.

---

## Rationale

- Centralized cache semantics and key naming (`{tenantId}:...`)
- Auth and Tenant remain free of StackExchange.Redis direct references
- Template users get production-ready cache patterns

---

## Consequences

**Positive:** Faster permission resolution; session cache for revocation checks (when extended).

**Negative:** Redis becomes readiness dependency; cache invalidation must be explicit on role/tenant changes.

**Note:** `CacheKeyBuilder.ForRateLimit` exists; rate limit middleware not implemented.

---

## Alternatives considered

| Alternative | Rejected because |
|-------------|------------------|
| IMemoryCache only | No multi-instance consistency |
| EF second-level cache | Module isolation complexity |
| Cache inside Auth module | Violates Layer 0 shared infra pattern |

---

## References

- [modules/caching/README.md](../modules/caching/README.md)
- `CachingModule.cs`, `RedisCacheService.cs`
