# Auth — Architecture

## Project layout

```
BackEnd/src/Modules/Auth/
├── AUTH_MODULE_STATUS.md
├── Ashraak.Auth.Domain/
│   ├── Aggregates/AuthUser/       AuthUser, AuthUserId, domain events
│   ├── Repositories/IAuthUserRepository.cs
│   └── ValueObjects/Permission.cs
├── Ashraak.Auth.Application/
│   ├── Abstractions/IPasswordHasher.cs
│   └── Commands/RegisterUser/     Command, Handler, Validator
├── Ashraak.Auth.Infrastructure/
│   ├── AuthModule.cs              DI composition root
│   ├── Persistence/
│   │   ├── AuthDbContext.cs       Identity + OpenIddict + outbox DbSet
│   │   ├── Configurations/
│   │   ├── Authorization/       RBAC/ABAC tables
│   │   └── Repositories/
│   ├── Security/Argon2PasswordHasher.cs
│   └── Services/
│       ├── AuthPermissionChecker.cs
│       └── DomainEventPublisher.cs
└── Ashraak.Auth.Api/
    ├── Endpoints/AuthEndpoints.cs
    └── Middleware/
        ├── TenantResolutionMiddleware.cs
        └── TenantResolutionExtensions.cs
```

## Domain model

**Aggregate:** `AuthUser` — credentials and auth state (separate from Users module profile)

- Raises domain events on register, login, password change, etc.
- `AuthUserId` is shared GUID with `UserProfile.Id` in Users module

**Authorization data:** RBAC/ABAC tables in `auth` schema via `AuthAuthorizationRepository`

## Persistence

**DbContext:** `AuthDbContext` extends `IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>`

| Feature | Detail |
|---------|--------|
| Provider | PostgreSQL (Npgsql) |
| Schema | `auth` |
| OpenIddict | `builder.UseOpenIddict()` |
| Outbox | `DbSet<OutboxMessage>` — scaffold, no `BaseDbContext` inheritance |
| Interceptors | All DI-registered `IInterceptor` (includes Audit) |

## OpenIddict configuration

File: `AuthModule.cs`

| Setting | Value |
|---------|-------|
| Token endpoint | `/connect/token` |
| Authorization endpoint | `/connect/authorize` |
| Introspection | `/connect/introspect` |
| Logout | `/connect/logout` |
| Flows | Authorization code, password, client credentials, refresh |
| Clients | Anonymous clients accepted |
| Keys | **Ephemeral** encryption + signing |
| ASP.NET Core | Token/authorize/logout passthrough enabled |
| Validation | `UseLocalServer()` + `UseAspNetCore()` |

**Implication:** JWTs signed with in-memory keys — invalidated on every app restart.

## Token issuance (custom handler)

`AuthEndpoints.IssueToken` implements password grant manually (not OpenIddict default handler):

1. Validate tenant active (`ITenantService`)
2. Load user by email + tenant
3. Verify password (`IPasswordHasher` — Argon2)
4. Check active, not locked
5. Record login, save
6. Load roles/permissions (`IAuthPermissionChecker` — Redis cache, 10 min TTL)
7. Cache session (`ISessionCacheService`, 8 hours)
8. Build claims: `sub`, `email`, `tenant_id`, `tenantId`, `role`, `permission`
9. Scopes: `openid`, `offline_access`, `profile`, `roles`
10. `Results.SignIn` with OpenIddict server scheme

## Permission checking

`AuthPermissionChecker`:
- Key: `CacheKeyBuilder.ForPermissions(tenantId, userId)`
- TTL: 10 minutes
- Caches `AuthPermissionSnapshot` (roles + ABAC-filtered permissions)
- On miss: loads from `AuthAuthorizationRepository`

## Tenant resolution middleware

**File:** `TenantResolutionMiddleware.cs`

Runs after `UseAuthentication()` in host pipeline.

| Rule | Behavior |
|------|----------|
| Bypass paths | `/health`, `/connect`, `/api/auth/register`, `/api/auth/sso` |
| Tenant source | JWT `tenant_id`/`tenantId` or `X-Tenant-ID` header |
| Mismatch | 400 if token tenant ≠ header tenant |
| Missing tenant (authenticated) | 401 |
| Inactive tenant | 403 via `ITenantService.IsActiveAsync` |

**Note:** Bypass uses `/api/auth/register` but actual register route is `/api/v1/auth/register`.

## SSO flow

External auth schemes: Google, Microsoft

- Cookie scheme: `Auth.External` (10 min sliding, cookie `ashraak.auth.external`)
- Callbacks: `/api/auth/sso/google/callback`, `/api/auth/sso/microsoft/callback`
- Completion: `/api/auth/sso/callback` returns external claims JSON — **no local account linking yet**

## Security

| Component | Implementation |
|-----------|----------------|
| Password hash | Argon2 via `Argon2PasswordHasher` |
| Identity rules | Min 8 chars, digit, uppercase |
| JWT validation | OpenIddict validation middleware (not JwtBearer) |
