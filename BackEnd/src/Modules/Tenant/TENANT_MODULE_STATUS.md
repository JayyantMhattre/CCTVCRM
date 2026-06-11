# Tenant Module Status

> **Canonical:** [docs/modules/tenant/](../../../docs/modules/tenant/README.md)

This document validates the Tenant module against the current modular SaaS specification and records incremental updates made in this phase.

## Coverage Check

### Tenant Entity

Implemented as aggregate root `Tenant` with:

- identity (`TenantId`)
- business fields (`Name`, `Slug`, `CustomDomain`)
- lifecycle (`Status`)
- subscription plan (`Plan`, `Subscription`)
- timestamps

### Tenant Settings

Implemented as value object `TenantSettings` with:

- locale
- timezone
- password minimum length policy
- MFA requirement flag
- session timeout policy

Settings are mapped via EF owned type in `TenantConfiguration`.

### Tenant Isolation Strategy

Current strategy is layered:

1. Request-level tenant resolution middleware (host + auth API middleware).
2. Contract-level tenant checks via `ITenantService.IsActiveAsync`.
3. Endpoint-level resolver enforcement:
   - `GET /api/tenants/{tenantId}` now forbids cross-tenant reads when resolved tenant differs.
   - `GET /api/tenants/current` reads directly from resolved tenant context.

### Tenant Resolver Integration

`ITenantContext` is now actively consumed by Tenant API endpoints to enforce request tenant scope, not only by lower-level infrastructure.

## Entities

- `Tenant` (aggregate root)
- `TenantSettings` (value object)
- `Subscription` (value object)

## Interfaces

- Domain: `ITenantRepository`
- Cross-module contract: `ITenantService`
- Shared runtime context: `ITenantContext`

## DB Design

Schema: `tenant`

Primary table: `tenant.tenants`

- scalar columns: name, slug, plan, status, custom_domain, timestamps
- owned settings columns: `settings_*`
- owned subscription columns: `subscription_*`
- unique index on `slug`

Outbox (scaffold):

- `OutboxMessages` DbSet prepared; **no hosted outbox processor** — cross-module events use synchronous MediatR where implemented. See [docs/architecture/outbox.md](../../../docs/architecture/outbox.md).
