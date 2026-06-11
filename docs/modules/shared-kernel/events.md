# Shared Kernel — Events

All contract events extend `DomainEvent` → `IDomainEvent` → `MediatR.INotification`. They are dispatched in-process via MediatR, **not** via `IEventBus` or RabbitMQ.

**Path prefix:** `BackEnd/src/Shared/Ashraak.SharedKernel.Contracts/`

## Auth events

| Event | Path | Typical publisher | Known handlers |
|-------|------|-------------------|----------------|
| `UserRegisteredEvent` | `Auth/Events/UserRegisteredEvent.cs` | `RegisterUserCommandHandler` (Auth) | `UserRegisteredEventHandler` (Users) |
| `UserLoggedInEvent` | `Auth/Events/UserLoggedInEvent.cs` | Not published yet | — |
| `UserPasswordChangedEvent` | `Auth/Events/UserPasswordChangedEvent.cs` | Not published yet | — |
| `RoleAssignedEvent` | `Auth/Events/RoleAssignedEvent.cs` | Not published yet | — |
| `TokenRevokedEvent` | `Auth/Events/TokenRevokedEvent.cs` | Not published yet | — |

### UserRegisteredEvent (working flow)

```
POST /api/v1/auth/register
  → RegisterUserCommandHandler
  → SaveChangesAsync (AuthDbContext)
  → IPublisher.Publish(UserRegisteredEvent)
  → UserRegisteredEventHandler (Users)
  → CreateUserProfileCommand
```

File: `BackEnd/src/Modules/Auth/Ashraak.Auth.Application/Commands/RegisterUser/RegisterUserCommandHandler.cs`

## Tenant events

| Event | Path | Intended source | Known handlers |
|-------|------|-----------------|----------------|
| `TenantProvisionedEvent` | `Tenant/Events/TenantProvisionedEvent.cs` | Outbox promotion from `TenantCreatedDomainEvent` | None |
| `TenantDeletedEvent` | `Tenant/Events/TenantDeletedEvent.cs` | Outbox promotion from `TenantDeletedDomainEvent` | `TenantDeletedEventHandler` (Users) |
| `TenantSuspendedEvent` | `Tenant/Events/TenantSuspendedEvent.cs` | Not published yet | — |
| `TenantPlanChangedEvent` | `Tenant/Events/TenantPlanChangedEvent.cs` | Not published yet | — |

**Gap:** Tenant aggregate raises `TenantDeletedDomainEvent` (module-local), but Users handler listens to `TenantDeletedEvent` (contract). No mapper bridges these types today — the handler will not fire until contract events are published.

## Users events

| Event | Path | Status |
|-------|------|--------|
| `UserCreatedEvent` | `Users/Events/UserCreatedEvent.cs` | Not published yet |
| `UserDeletedEvent` | `Users/Events/UserDeletedEvent.cs` | Not published yet |
| `UserDeactivatedEvent` | `Users/Events/UserDeactivatedEvent.cs` | Not published yet |
| `UserInvitedEvent` | `Users/Events/UserInvitedEvent.cs` | Not published yet |

## Domain events vs contract events

Module aggregates raise **domain events** (in `*.Domain/Events/`). Contract events in SharedKernel.Contracts are the **cross-module public surface**.

Intended flow (not fully wired):

```
Domain event on aggregate
  → BaseDbContext.SerializeDomainEventsToOutbox (same transaction)
  → OutboxProcessorBase (Quartz job)
  → Map to contract event
  → MediatR IPublisher
```

Current flow for Auth→Users:

```
Handler publishes contract event directly after SaveChangesAsync
```

## Audit capture of all domain events

`DomainEventAuditHandler` (`BackEnd/src/Modules/Audit/Ashraak.Audit.Application/EventHandlers/DomainEventAuditHandler.cs`) implements `INotificationHandler<IDomainEvent>` and logs every published domain/contract event to MongoDB via `IAuditService`.

## Integration events (not in Shared Kernel)

`IIntegrationEvent` lives in `Ashraak.BuildingBlocks.EventBus` for future RabbitMQ/MassTransit. Contract events do **not** implement it. See [Building Blocks events](../building-blocks/events.md).
