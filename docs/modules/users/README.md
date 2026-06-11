# Users Module

User profile and tenant membership management. Separates business profile data from Auth credentials. Shared user ID with Auth module.

**Source:** `BackEnd/src/Modules/Users/`

## Projects

| Layer | Project |
|-------|---------|
| Domain | `Ashraak.Users.Domain` |
| Application | `Ashraak.Users.Application` |
| Infrastructure | `Ashraak.Users.Infrastructure` |
| Api | `Ashraak.Users.Api` |

## Key facts

- PostgreSQL schema `users`
- **`UserProfile.Id` == `AuthUser.Id`** (shared GUID)
- Global EF query filter by `ITenantContext.TenantId`
- Read-only HTTP API today (no write endpoints)
- Consumes `UserRegisteredEvent` from Auth — **working**
- Consumes `TenantDeletedEvent` — **not triggered** (contract event not published)

## Module documentation

- [Architecture](./architecture.md)
- [Registration](./registration.md)
- [API](./api.md)
- [Events](./events.md)
- [Extending](./extending.md)
- [Operations](./operations.md)

## Dependencies

- [Auth](../auth/README.md) — `UserRegisteredEvent`
- [Tenant](../tenant/README.md) — `TenantDeletedEvent` (intended)
- [Shared Kernel](../shared-kernel/README.md) — `IUserService` contract
