# Users — Events

## Published domain events

| Event | Trigger | Contract mapped |
|-------|---------|-----------------|
| `UserProfileCreatedDomainEvent` | `UserProfile.Create()` | `UserCreatedEvent` — **not published** |
| `UserDeactivatedDomainEvent` | `UserProfile.Deactivate()` | `UserDeactivatedEvent` — **not published** |

Contract events defined in `SharedKernel.Contracts/Users/Events/` but not emitted via `IPublisher` today.

## Consumed events

### UserRegisteredEvent (working)

| Property | Value |
|----------|-------|
| Source | Auth module |
| Handler | `UserRegisteredEventHandler` |
| Action | Sends `CreateUserProfileCommand` |

```
UserRegisteredEvent (TenantId, UserId, Email, DisplayName)
  → CreateUserProfileCommand
  → UserProfile.Create() with Id = UserId
  → SaveChangesAsync
```

File: `Ashraak.Users.Application/EventHandlers/UserRegisteredEventHandler.cs`

Delivery: synchronous MediatR in same request as registration.

### TenantDeletedEvent (not triggered)

| Property | Value |
|----------|-------|
| Source | Tenant module (intended) |
| Handler | `TenantDeletedEventHandler` |
| Action | Load all profiles for tenant, `Deactivate()`, save |

**Gap:** Tenant module raises `TenantDeletedDomainEvent` but never publishes contract `TenantDeletedEvent`. Handler exists but never runs.

File: `Ashraak.Users.Application/EventHandlers/TenantDeletedEventHandler.cs`

## Outbox

`UsersDbContext` has `DbSet<OutboxMessage>` — scaffold only, no `BaseDbContext`.

## Audit capture

- Profile creates/updates → `AuditEntityChangeInterceptor`
- Published domain events → `DomainEventAuditHandler` (when/if published)
- API reads → `AuditApiCallMiddleware`

## Event file paths

**Handlers:**
- `Application/EventHandlers/UserRegisteredEventHandler.cs`
- `Application/EventHandlers/TenantDeletedEventHandler.cs`

**Contracts consumed:**
- `Shared/Ashraak.SharedKernel.Contracts/Auth/Events/UserRegisteredEvent.cs`
- `Shared/Ashraak.SharedKernel.Contracts/Tenant/Events/TenantDeletedEvent.cs`

**Contracts to publish (future):**
- `Shared/Ashraak.SharedKernel.Contracts/Users/Events/*.cs`
