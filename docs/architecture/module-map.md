# Module Map

## Backend modules

| Module | Projects | Storage | HTTP routes | Contract surface |
|--------|----------|---------|-------------|------------------|
| **Host** | `Ashraak.Api` | — | Health, versioning wrapper | `ICurrentUser`, `ITenantContext` |
| **SharedKernel** | `Ashraak.SharedKernel` | — | — | DDD primitives, `Result`, outbox types |
| **Contracts** | `Ashraak.SharedKernel.Contracts` | — | — | `ITenantService`, `IUserService`, `IAuditService`, events |
| **BuildingBlocks** | Application, EventBus, Infrastructure, Data.* | — | — | Base classes, behaviors (partially used) |
| **Caching** | Abstractions, Redis | Redis + memory | None | `ICacheService`, locks, sessions |
| **Auth** | Domain, Application, Infrastructure, Api | PostgreSQL `auth` | `/api/v1/auth/*`, `/connect/token`, `/api/auth/sso/*` | `IAuthPermissionChecker` |
| **Tenant** | Domain, Application, Infrastructure, Api | PostgreSQL `tenant` | `/api/v1/tenants/*` | `ITenantService` |
| **Users** | Domain, Application, Infrastructure, Api | PostgreSQL `users` | `/api/v1/users/*` | `IUserService` |
| **Audit** | Domain, Application, Infrastructure, Api | MongoDB `audit_entries` | `/api/v1/audit-logs` (stub GET) | `IAuditService` |
| **Notifications** | Domain, Application, Infrastructure, Api | — | `/api/v1/notifications/health` | `INotificationService` |
| **Files** | Domain, Application, Infrastructure, Api | PostgreSQL `files` | `/api/v1/files` | `IFileStorage` |
| **Webhooks** | Domain, Application, Infrastructure, Api | PostgreSQL `webhooks` | `/api/v1/webhooks/*` | `IWebhookPublisher`, `IWebhookSubscriptionRepository` |

## Platform capabilities (not vertical modules)

| Capability | Docs | Backend | Web | Mobile | Status |
|------------|------|---------|-----|--------|--------|
| **Outbox** | [platform/outbox](../platform/outbox/README.md) | Scaffold | N/A | N/A | Partial |
| **Webhooks** | [modules/webhooks](../modules/webhooks/README.md) | **Done (W1)** | Planned | Planned (read-only W5) | Subscriptions + catalog; delivery W2 |

Webhooks are a **reusable platform capability** — any module may publish catalog events; external systems subscribe. See [ADR-Webhook-0001](../adr/ADR-Webhook-0001-webhook-platform-architecture.md).

## Dependency graph (allowed references)

```mermaid
flowchart TD
    Host[Ashraak.Api]
    SK[SharedKernel]
    SC[SharedKernel.Contracts]
    BB[BuildingBlocks.*]
    Cache[Caching]
    Auth[Auth]
    Tenant[Tenant]
    Users[Users]
    Audit[Audit]

    Host --> Auth & Tenant & Users & Audit & Cache
    Auth --> SC & SK & Cache
    Tenant --> SC & SK
    Users --> SC & SK
    Audit --> SC & SK
    Auth.Infrastructure --> BB.Infrastructure
    Tenant.Infrastructure --> BB.Infrastructure
    Users.Infrastructure --> BB.Infrastructure

    Auth -.->|ITenantService| SC
    Users -.->|events| SC
    Audit -.->|IAuditService| SC
```

Solid arrows: project references. Dotted: runtime via contracts/DI only.

## Frontend feature map

| Folder | Pages | Backend APIs |
|--------|-------|--------------|
| `modules/auth` | Login, Register | `/connect/token`, `/api/v1/auth/register` |
| `modules/dashboard` | Dashboard | — |
| `modules/tenant` | Profile, Settings (stub) | `/api/v1/tenants/current` |
| `modules/users` | List, Profile | `/api/v1/users/*` |
| `modules/audit` | Audit log | `/api/v1/audit-logs` |

## Docker services

| Service | Used by |
|---------|---------|
| postgres | Auth, Tenant, Users |
| redis | Caching |
| mongodb | Audit |
| seq | Serilog |
| rabbitmq | **Not connected to API** (future) |

## Documentation index

| Module | Docs path |
|--------|-----------|
| SharedKernel | [modules/shared-kernel](../modules/shared-kernel/README.md) |
| BuildingBlocks | [modules/building-blocks](../modules/building-blocks/README.md) |
| Host | [modules/host](../modules/host/README.md) |
| Auth | [modules/auth](../modules/auth/README.md) |
| Tenant | [modules/tenant](../modules/tenant/README.md) |
| Users | [modules/users](../modules/users/README.md) |
| Audit | [modules/audit](../modules/audit/README.md) |
| Caching | [modules/caching](../modules/caching/README.md) |
| Webhooks (platform) | [modules/webhooks](../modules/webhooks/README.md) |
