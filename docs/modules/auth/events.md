# Auth — Events

## Published events

Contract events in `Ashraak.SharedKernel.Contracts/Auth/Events/`:

| Event | Published today | Publisher | Payload (summary) |
|-------|-----------------|-----------|-------------------|
| `UserRegisteredEvent` | **Yes** | `RegisterUserCommandHandler` | TenantId, UserId, Email, DisplayName |
| `UserLoggedInEvent` | No | — | Defined, not emitted |
| `UserPasswordChangedEvent` | No | — | Defined, not emitted |
| `RoleAssignedEvent` | No | — | Defined, not emitted |
| `TokenRevokedEvent` | No | — | Defined, not emitted |

### UserRegisteredEvent flow

```
RegisterUserCommandHandler
  → AuthUser persisted
  → SaveChangesAsync (AuthDbContext)
  → IPublisher.Publish(UserRegisteredEvent)
  → UserRegisteredEventHandler (Users module)
  → CreateUserProfileCommand
```

File: `Ashraak.Auth.Application/Commands/RegisterUser/RegisterUserCommandHandler.cs`

Delivery: **synchronous in-process MediatR** — not outbox, not RabbitMQ.

## Domain events (AuthUser aggregate)

Raised on aggregate state changes in `Ashraak.Auth.Domain/Aggregates/AuthUser/`:

- Registration, login recorded, password changed, etc.

**Not auto-serialized to outbox** — AuthDbContext does not inherit `BaseDbContext`.

## Consumed events

Auth module has **no** `INotificationHandler` implementations for external events.

## Audit capture

All published contract/domain events are observed by Audit's `DomainEventAuditHandler` when they extend `DomainEvent`.

Login flow does not publish `UserLoggedInEvent` today — login is audited via:
- `AuditApiCallMiddleware` (HTTP)
- Potential EF changes on `AuthUser` (interceptor)

## IDomainEventPublisher

Auth is the **only module** registering `IDomainEventPublisher` → `DomainEventPublisher`.

Provides optional immediate MediatR dispatch abstraction. Register handler uses `IPublisher` directly.

## Future: outbox integration

To make `UserRegisteredEvent` durable:

1. Inherit `BaseDbContext` in `AuthDbContext`
2. Deploy `OutboxProcessorBase` subclass for `auth` schema
3. Optionally remove direct `IPublisher.Publish` from handler

See [Building Blocks events](../building-blocks/events.md).

## Event files

```
BackEnd/src/Shared/Ashraak.SharedKernel.Contracts/Auth/Events/
├── UserRegisteredEvent.cs
├── UserLoggedInEvent.cs
├── UserPasswordChangedEvent.cs
├── RoleAssignedEvent.cs
└── TokenRevokedEvent.cs
```
