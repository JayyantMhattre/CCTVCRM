# Users Module Status

> **Canonical:** [docs/modules/users/](../../../docs/modules/users/README.md)

This document validates the Users module against the current modular SaaS specification and records incremental updates made in this phase.

## Coverage Check

### Profile Data (NOT Auth)

`UserProfile` remains strictly profile/business data:

- display name
- avatar URL
- status
- tenant memberships
- preferences (added this phase)

No password, token, MFA secret, or credential state is stored here.

### Preferences

Added value object `UserPreferences` with:

- theme
- locale
- timezone
- email notifications enabled

Mapped as owned columns in `users.profiles` (`pref_*` columns).

### Mapping with Auth User

Mapping strategy is explicit and preserved:

- `UserProfile.Id` equals `AuthUser.Id` (shared GUID identity).
- Auth module now publishes `UserRegisteredEvent`.
- Users module handles it with `UserRegisteredEventHandler` and creates profile via `CreateUserProfileCommand`.

This preserves ownership boundaries:

- Auth owns authentication/identity credential lifecycle.
- Users owns profile/business lifecycle.

## Entities

- `UserProfile` (aggregate root)
- `TenantMembership` (child entity)
- `UserPreferences` (value object, added)

## Interfaces

- Domain: `IUserProfileRepository`
- Cross-module contract: `IUserService`

## Module Interaction with Auth

Interaction is contract/event based (no direct data access):

1. Auth registers new identity.
2. Auth publishes `UserRegisteredEvent` (contracts assembly).
3. Users handles event and creates `UserProfile`.

Cross-module reads remain through `IUserService` contract.

## DB Design

Schema: `users`

Primary table: `users.profiles`

- identity and tenant columns
- email/display/avatar/status columns
- preference columns: `pref_theme`, `pref_locale`, `pref_timezone`, `pref_email_notifications_enabled`
- timestamps
- unique index on `(email, tenant_id)`

Membership table:

- `users.tenant_memberships` (one-to-many from profile)

Outbox (scaffold):

- `OutboxMessages` DbSet prepared; **UserRegisteredEvent** is handled via **synchronous** `IPublisher.Publish` from Auth today. See [docs/architecture/eventing.md](../../../docs/architecture/eventing.md).

## Tenant Resolver / Isolation Integration

Users API now validates resolver scope for tenant list endpoints:

- `GET /api/users/tenant/{tenantId}` forbids mismatched tenant access.
- `GET /api/users/tenant/current` uses resolved tenant context directly.

In infrastructure, `UsersDbContext` still enforces tenant query filter through `ITenantContext`.
