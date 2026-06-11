# Tenant — Events

## Domain events (aggregate)

Raised in `Ashraak.Tenant.Domain/Aggregates/Tenant/`:

| Domain event | Trigger |
|--------------|---------|
| `TenantCreatedDomainEvent` | `Tenant.Create()` |
| `TenantSuspendedDomainEvent` | `Tenant.Suspend()` |
| `TenantPlanChangedDomainEvent` | `Tenant.ChangePlan()` |
| `TenantDeletedDomainEvent` | `Tenant.Delete()` |

**Not persisted to outbox** — `TenantDbContext` does not inherit `BaseDbContext`.

## Contract events (SharedKernel.Contracts)

Path: `Shared/Ashraak.SharedKernel.Contracts/Tenant/Events/`

| Contract event | Intended source | Published today |
|----------------|-----------------|-----------------|
| `TenantProvisionedEvent` | Map from `TenantCreatedDomainEvent` | **No** |
| `TenantDeletedEvent` | Map from `TenantDeletedDomainEvent` | **No** |
| `TenantSuspendedEvent` | Map from suspended domain event | **No** |
| `TenantPlanChangedEvent` | Map from plan changed domain event | **No** |

## Consumed events

Tenant module has **no** `INotificationHandler` implementations.

## Downstream impact

**Users module** listens for `TenantDeletedEvent` (`TenantDeletedEventHandler`) — but Tenant never publishes the contract event today.

**Gap:** Domain event `TenantDeletedDomainEvent` ≠ contract `TenantDeletedEvent`. Users handler will not run until contract events are published (via outbox mapper or direct `IPublisher.Publish`).

## Audit capture

When domain/contract events are published via MediatR, `DomainEventAuditHandler` logs them.

EF changes to `Tenant` entity are captured by `AuditEntityChangeInterceptor` on save.

## Provision flow (today)

```
POST /api/v1/tenants
  → ProvisionTenantCommandHandler
  → Tenant.Create() raises TenantCreatedDomainEvent
  → SaveChangesAsync
  → (no IPublisher.Publish of TenantProvisionedEvent)
```

## Future outbox flow

```
TenantCreatedDomainEvent
  → BaseDbContext outbox write
  → OutboxProcessor
  → TenantProvisionedEvent
  → downstream handlers
```

See [Building Blocks events](../building-blocks/events.md).
