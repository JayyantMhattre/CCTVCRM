# Auth — API

## Versioned REST

Base: `/api/v1/auth/` (via host versioned group + `MapAuthEndpoints`)

### POST /api/v1/auth/register

| Property | Value |
|----------|-------|
| Name | `Register` |
| Auth | `AllowAnonymous` |
| Handler | MediatR `RegisterUserCommand` |

**Request body (`RegisterRequest`):**

```json
{
  "tenantId": "guid",
  "email": "user@example.com",
  "password": "string",
  "displayName": "string"
}
```

**Responses:**

| Status | Body |
|--------|------|
| 201 | `{ "userId": "guid" }` — Location: `/api/v1/users/{userId}` |
| 400 | ProblemDetails (validation, tenant inactive, duplicate email) |

**Handler flow:** `RegisterUserCommandHandler` — tenant check, Argon2 hash, persist `AuthUser`, publish `UserRegisteredEvent`.

File: `Ashraak.Auth.Api/Endpoints/AuthEndpoints.cs`  
Handler: `Ashraak.Auth.Application/Commands/RegisterUser/RegisterUserCommandHandler.cs`

---

## Unversioned protocol

Base: root `WebApplication` (no `/api/v1` prefix)

### POST /connect/token

| Property | Value |
|----------|-------|
| Name | `IssueToken` |
| Auth | `AllowAnonymous` |
| Content-Type | `application/x-www-form-urlencoded` |

**Form fields:**

| Field | Required | Notes |
|-------|----------|-------|
| `grant_type` | Yes | Must be `password` |
| `username` | Yes | Email address |
| `password` | Yes | Plain password |
| `tenant_id` or `tenantId` | Yes* | Or `X-Tenant-ID` header |

**Success:** OpenIddict sign-in response (access token, refresh token, token_type, expires_in)

**Claims in token:** `sub`, `email`, `tenant_id`, `tenantId`, `role`, `permission`

**Scopes:** `openid`, `offline_access`, `profile`, `roles`

**Error responses (400 JSON):**

```json
{ "error": "invalid_grant", "error_description": "..." }
```

Common errors: invalid grant type, tenant inactive, user not found, invalid password, account locked.

**Side effects:** Session cached 8 hours via `ISessionCacheService`.

---

### GET /api/auth/sso/google

| Property | Value |
|----------|-------|
| Name | `StartGoogleSso` |
| Auth | `AllowAnonymous` |
| Behavior | `Results.Challenge` → Google scheme, redirect to `/api/auth/sso/callback` |

### GET /api/auth/sso/microsoft

| Property | Value |
|----------|-------|
| Name | `StartMicrosoftSso` |
| Auth | `AllowAnonymous` |
| Behavior | `Results.Challenge` → Microsoft scheme |

### GET /api/auth/sso/callback

| Property | Value |
|----------|-------|
| Name | `CompleteSso` |
| Auth | `AllowAnonymous` |
| Behavior | Returns external authentication claims as JSON — **no local account linking** |

---

## OpenIddict endpoints (configured, no minimal API handler)

| Path | Purpose |
|------|---------|
| `/connect/authorize` | Authorization code flow |
| `/connect/introspect` | Token introspection |
| `/connect/logout` | Logout |

Only `/connect/token` has an explicit `MapPost` handler in `AuthEndpoints.cs`.

---

## Frontend integration

React client (`FrontEnd/apps/web/src/modules/auth/api.ts`):

- `POST /connect/token` — login
- `POST /api/v1/auth/register` — registration

SSO paths in frontend `endpoints.ts` may use `/api/v1/auth/sso/*` — backend uses unversioned `/api/auth/sso/*`.

---

## Contract interface (not HTTP)

**`IAuthPermissionChecker`** — used by other modules and token issuance:

- `GetPermissionsAsync(tenantId, userId)` — cached roles + permissions

Implemented in `AuthPermissionChecker.cs`, registered in `AuthModule.cs`.
