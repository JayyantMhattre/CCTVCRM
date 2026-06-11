# Users — Architecture

## Project layout

```
BackEnd/src/Modules/Users/
├── USERS_MODULE_STATUS.md
├── Ashraak.Users.Domain/
│   ├── Aggregates/UserProfile/    UserProfile, UserId, TenantMembership
│   ├── ValueObjects/UserPreferences.cs
│   ├── Enums/UserStatus.cs
│   ├── Events/                    UserProfileCreatedDomainEvent, UserDeactivatedDomainEvent
│   └── Repositories/IUserProfileRepository.cs
├── Ashraak.Users.Application/
│   ├── Commands/CreateUserProfile/
│   └── EventHandlers/
│       ├── UserRegisteredEventHandler.cs
│       └── TenantDeletedEventHandler.cs
├── Ashraak.Users.Infrastructure/
│   ├── UsersModule.cs
│   ├── Persistence/UsersDbContext.cs
│   ├── Persistence/Configurations/UserProfileConfiguration.cs
│   ├── Persistence/Repositories/UserProfileRepository.cs
│   └── Services/UserService.cs
└── Ashraak.Users.Api/
    └── Endpoints/UserEndpoints.cs
```

## Domain model

**Aggregate:** `UserProfile` — display name, email, preferences, status (no credentials)

**Child entity:** `TenantMembership` — tenant ID, role, joined timestamp

**Value object:** `UserPreferences` — theme, locale, timezone, email notifications

**Enum:** `UserStatus` (Active, Inactive)

### Domain events

| Event | Trigger |
|-------|---------|
| `UserProfileCreatedDomainEvent` | `UserProfile.Create()` |
| `UserDeactivatedDomainEvent` | `UserProfile.Deactivate()` |

## Persistence

**DbContext:** `UsersDbContext`

| Feature | Detail |
|---------|--------|
| Schema | `users` |
| Tables | `profiles`, `tenant_memberships`, `outbox_messages` |
| Global filter | `UserProfile` where `TenantId == _tenantContext.TenantId` |
| Unique index | `(email, tenant_id)` |
| Interceptors | All DI `IInterceptor` (Audit) |

**Important:** Query filter applies to all reads — cross-tenant data hidden at EF level.

## UserService

Implements `IUserService` for cross-module reads:

- `GetUserAsync(userId, tenantId)`
- `GetUsersForTenantAsync(tenantId)`

## Auth integration

Registration flow:

```
Auth RegisterUserCommandHandler
  → UserRegisteredEvent
  → UserRegisteredEventHandler
  → CreateUserProfileCommand
  → UserProfile with same Id as AuthUser
```

File: `Application/EventHandlers/UserRegisteredEventHandler.cs`
