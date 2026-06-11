# Auth Module Status

> **Canonical:** [docs/modules/auth/](../../../docs/modules/auth/README.md) — keep in sync with `/docs`.

This document validates the current Auth module against the requested specification and records incremental changes made in this phase.

## Baseline Specification Coverage

### 1) Projects

All required projects already existed and were kept intact:

- `Ashraak.Auth.Domain`
- `Ashraak.Auth.Application`
- `Ashraak.Auth.Infrastructure`
- `Ashraak.Auth.Api`

### 2) Integrations

- **OpenIddict server**: present before; now extended with password flow pass-through token endpoint and claim enrichment.
- **Multi-tenant identity**: present before (`AuthUser.TenantId` + tenant-scoped uniqueness); now enforced in token issuance and middleware.
- **RBAC + ABAC**: added concrete infrastructure persistence + runtime checker.
- **SSO (Google + Microsoft)**: provider registration + challenge endpoints added.

### 3) Tenant-Aware Authentication and JWT Claims

Token issuance now resolves tenant context and emits:

- `tenant_id`
- `tenantId`
- role claims (`role`)
- permission claims (`permission`)

### 4) Added Components

- **Permission caching (Redis + Memory)**: implemented via `IAuthPermissionChecker` + shared `ICacheService` (`CacheKeyBuilder.ForPermissions(...)`).
- **Tenant resolution middleware**: added and wired into host pipeline.

### 5) Modular Boundaries

No direct dependency on other module implementation assemblies was introduced. Auth relies on cross-module contracts (`ITenantService`) and shared abstractions only.

## Folder Structure (Auth)

```text
src/Modules/Auth/
  Ashraak.Auth.Domain/
  Ashraak.Auth.Application/
  Ashraak.Auth.Infrastructure/
    AuthModule.cs
    Persistence/
      AuthDbContext.cs
      Authorization/
        AuthRoleAssignment.cs
        AuthPermissionGrant.cs
        AuthPermissionSnapshot.cs
      Configurations/
        AuthRoleAssignmentConfiguration.cs
        AuthPermissionGrantConfiguration.cs
      Repositories/
        AuthAuthorizationRepository.cs
    Services/
      AuthPermissionChecker.cs
  Ashraak.Auth.Api/
    Endpoints/
      AuthEndpoints.cs
    Middleware/
      TenantResolutionMiddleware.cs
      TenantResolutionExtensions.cs
```

## Key Classes

- `AuthModule` (`Infrastructure`)
  - Registers OpenIddict server/validation, external SSO providers, permission checker, and persistence services.
- `AuthDbContext` (`Infrastructure`)
  - Auth schema EF context for identities, OpenIddict entities, RBAC/ABAC tables, and outbox.
- `AuthAuthorizationRepository` (`Infrastructure`)
  - Loads role assignments and grants; ensures baseline role/permission bootstrap.
- `AuthPermissionChecker` (`Infrastructure`)
  - Implements `IAuthPermissionChecker`; evaluates RBAC + simple ABAC conditions and caches snapshots.
- `TenantResolutionMiddleware` (`Api`)
  - Resolves tenant from claims/headers and blocks mismatches/inactive tenants.
- `AuthEndpoints` (`Api`)
  - Supports registration, tenant-aware token issuance, and SSO challenge/callback endpoints.

## DbContext Design

`AuthDbContext` in schema `auth` now includes:

- `AuthUsers` (`users`)
- `RoleAssignments` (`role_assignments`)
- `PermissionGrants` (`permission_grants`)
- OpenIddict model mappings (`builder.UseOpenIddict()`)
- `OutboxMessages`

### Authorization Tables

- `role_assignments`
  - `id`, `tenant_id`, `user_id`, `role_name`, `created_on_utc`
  - unique index: `(tenant_id, user_id, role_name)`

- `permission_grants`
  - `id`, `tenant_id`, `role_name?`, `user_id?`, `permission`, `condition_expression?`, `created_on_utc`
  - indexes support role-based and user-based lookups

## Middleware Pipeline Integration

Host pipeline now includes:

1. `UseAuthentication()`
2. `UseTenantResolution()`
3. `UseAuthorization()`
4. `UseAuditApiCallLogging()` (Audit module)
5. `UseOutputCache()`

Tenant middleware bypasses health and auth bootstrap paths, validates tenant consistency, and blocks inactive tenant access early.

## API Integration Points

- `POST /api/v1/auth/register`
  - Registers tenant-scoped identity.
- `POST /connect/token`
  - Password grant endpoint (OpenIddict pass-through), validates tenant and emits tenant/roles/permissions claims.
- `GET /api/auth/sso/google`
- `GET /api/auth/sso/microsoft`
- `GET /api/auth/sso/callback`

## Notes

- This phase is incremental and preserves prior architectural decisions.
- External SSO callback currently returns authenticated external claims; mapping external identities to local tenant users remains a separate follow-up task.
