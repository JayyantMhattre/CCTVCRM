# Caching — Events

The Caching module does **not** publish or consume domain or integration events.

## Relationship to event-driven architecture

Caching is **infrastructure support** — it reacts to module operations, not to MediatR notifications.

| Concern | Mechanism |
|---------|-----------|
| Cache population | On-demand via `GetOrSetAsync` in service/endpoints |
| Cache invalidation | TTL expiry today; `ICacheInvalidationService` available for event-driven invalidation (not wired) |
| Session lifecycle | Set on token issuance; no event on logout/revoke yet |

## Recommended invalidation on events (not implemented)

When modules publish events, handlers could call:

```csharp
await _cacheInvalidation.InvalidatePermissionsAsync(tenantId, userId);
await _cacheInvalidation.InvalidateTenantConfigAsync(tenantId);
```

Example mapping (future):

| Event | Invalidation |
|-------|--------------|
| `RoleAssignedEvent` | `InvalidatePermissionsAsync` |
| `TenantPlanChangedEvent` | `InvalidateTenantConfigAsync` |
| `TokenRevokedEvent` | `InvalidateSessionAsync` |

`ICacheInvalidationService` is registered but **no module injects it yet**.

## Outbox / RabbitMQ

Caching has no interaction with outbox processor or `IEventBus`. Redis is not used as an event stream.

## Audit

Cache operations are not audited. Redis misses/hits appear only in application logs if added manually.

## Host output cache

HTTP response caching via `UseOutputCache()` is separate from `ICacheService` — invalidates by TTL (30s base policy), not by domain events.

## Summary

| Event system | Caching involvement |
|--------------|---------------------|
| MediatR domain events | None (invalidation not hooked up) |
| Integration events / RabbitMQ | None |
| Outbox | None |

See module-specific [events.md](../auth/events.md) for when cache **should** be invalidated after future wiring.
