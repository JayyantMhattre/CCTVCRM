# Tenant — Architecture

## Project layout

```
BackEnd/src/Modules/Tenant/
├── TENANT_MODULE_STATUS.md
├── Ashraak.Tenant.Domain/
│   ├── Aggregates/Tenant/         Tenant, TenantId, domain events
│   ├── Enums/TenantPlan.cs        TenantPlan, TenantStatus
│   ├── ValueObjects/              TenantSettings, Subscription
│   └── Repositories/ITenantRepository.cs
├── Ashraak.Tenant.Application/
│   ├── Commands/ProvisionTenant/
│   ├── Queries/GetTenant/
│   └── Behaviors/ValidationPipelineBehavior.cs  (unregistered)
├── Ashraak.Tenant.Infrastructure/
│   ├── TenantModule.cs
│   ├── Persistence/TenantDbContext.cs
│   ├── Persistence/Configurations/TenantConfiguration.cs
│   ├── Persistence/Repositories/TenantRepository.cs
│   └── Services/TenantService.cs
└── Ashraak.Tenant.Api/
    └── Endpoints/TenantEndpoints.cs
```

## Domain model

**Aggregate:** `Tenant` (`Aggregates/Tenant/Tenant.cs`)

| Value object | Contents |
|--------------|----------|
| `TenantSettings` | Locale, timezone, password policy, MFA, session timeout |
| `Subscription` | Plan, seat limit, storage limit, expiry |
| `TenantId` | Strongly typed identifier |

**Enums:** `TenantPlan` (Free, Pro, Enterprise), `TenantStatus` (Active, Suspended, Deleted)

### Domain events (module-local)

| Event | Trigger |
|-------|---------|
| `TenantCreatedDomainEvent` | `Tenant.Create()` |
| `TenantSuspendedDomainEvent` | `Tenant.Suspend()` |
| `TenantPlanChangedDomainEvent` | `Tenant.ChangePlan()` |
| `TenantDeletedDomainEvent` | `Tenant.Delete()` |

## Persistence

**DbContext:** `TenantDbContext`

| Feature | Detail |
|---------|--------|
| Schema | `tenant` |
| Tables | `tenant.tenants`, `tenant.outbox_messages` |
| `IUnitOfWork` | Implemented directly |
| Query filter | None on Tenant entity |
| `ITenantContext` | Injected but not used for global filter |
| Interceptors | All DI `IInterceptor` (Audit) |

**Configuration:** `TenantConfiguration.cs` — owned types for settings/subscription, unique index on `slug`.

## TenantService (cross-module facade)

File: `Infrastructure/Services/TenantService.cs`

Implements `ITenantService`:

| Method | Caching |
|--------|---------|
| `GetTenantAsync` | Yes — 5 min, key `ForEntity(tenantId, "tenant", "config", tenantId)` |
| `IsActiveAsync` | Uses cached tenant |
| `GetFeatureFlagAsync` | Yes — key `ForFeatureFlags(tenantId)` |
| `GetSeatLimitAsync` | **No cache** — direct DB |

## Endpoint authorization

- `POST /tenants` — anonymous (provisioning)
- `GET /tenants/{id}`, `GET /tenants/current` — authenticated; cross-tenant forbidden when `ITenantContext.TenantId` differs

No `TenantAdmin` policy applied to endpoints today.
