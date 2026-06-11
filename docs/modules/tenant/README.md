# Tenant Module

Multi-tenant organization management: provisioning, plan/subscription, settings, and cross-module tenant queries via `ITenantService`.

**Source:** `BackEnd/src/Modules/Tenant/`

## Projects

| Layer | Project |
|-------|---------|
| Domain | `Ashraak.Tenant.Domain` |
| Application | `Ashraak.Tenant.Application` |
| Infrastructure | `Ashraak.Tenant.Infrastructure` |
| Api | `Ashraak.Tenant.Api` |

## Key facts

- PostgreSQL schema `tenant`
- **`ITenantService`** implemented here — consumed by Auth, Users, and host
- Tenant config and feature flags cached in Redis (5 min TTL)
- **`ValidationPipelineBehavior`** exists but is **not registered** in DI
- Outbox `DbSet` present; **no `BaseDbContext` inheritance**

## Module documentation

- [Architecture](./architecture.md)
- [Registration](./registration.md)
- [API](./api.md)
- [Events](./events.md)
- [Extending](./extending.md)
- [Operations](./operations.md)

## Dependencies

- [Caching](../caching/README.md) — `ICacheService` for tenant config
- [Shared Kernel](../shared-kernel/README.md) — `ITenantService` contract
