# Users — Extending

## Add write endpoints

1. Create commands in Application (e.g. `UpdateProfile`, `InviteUser`, `DeactivateUser`)
2. Map in `UserEndpoints.cs`:

```csharp
group.MapPut("/{userId:guid}", UpdateProfile)
    .RequireAuthorization();
```

3. Publish contract events after save:

```csharp
await _publisher.Publish(new UserDeactivatedEvent(tenantId, userId), ct);
```

## Fix TenantDeleted integration

Option A — Tenant module publishes contract event after delete.

Option B — Add handler for domain event (requires shared event type or mapping in Tenant module):

```csharp
// Prefer contract event in Tenant handler after SaveChanges
await _publisher.Publish(new TenantDeletedEvent(tenantId), ct);
```

Existing `TenantDeletedEventHandler` will then deactivate all profiles.

## Add validation

```csharp
services.AddValidatorsFromAssembly(typeof(CreateUserProfileCommand).Assembly);
// Register ValidationBehavior in MediatR if desired
```

## Bypass tenant filter for admin queries

For cross-tenant admin reads, use explicit repository methods with `IgnoreQueryFilters()` — do not remove global filter globally.

## Extend IUserService

Add methods to contract in SharedKernel.Contracts, implement in `UserService.cs`, register already handled in `UsersModule.cs`.

## Wire outbox

Inherit `BaseDbContext` in `UsersDbContext` for durable `UserCreatedEvent` / `UserDeactivatedEvent` promotion.

## Caching user profiles

Users.Infrastructure references Caching.Abstractions but does not use cache today. Add `ICacheService.GetOrSetAsync` in `UserService` with keys via `CacheKeyBuilder.ForEntity(tenantId, "users", "profile", userId)`.

## Module boundaries

- Never store passwords in Users module
- User ID must match Auth at creation time (`CreateUserProfileCommand` uses event's UserId)
