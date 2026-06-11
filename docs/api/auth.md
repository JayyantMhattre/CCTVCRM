# Auth API

Implementation: `BackEnd/src/Modules/Auth/Ashraak.Auth.Api/Endpoints/AuthEndpoints.cs`

---

## POST /api/v1/auth/register

Register a user in a tenant.

**Auth:** Anonymous

**Request:**

```json
{
  "tenantId": "00000000-0000-0000-0000-000000000001",
  "email": "user@example.com",
  "password": "SecureP@ssw0rd!",
  "displayName": "Jane Doe"
}
```

**Success:** `201 Created`

```json
{ "userId": "guid" }
```

`Location: /api/v1/users/{userId}`

**Failure:** `400` ProblemDetails (validation, duplicate email)

**Side effects:** Publishes `UserRegisteredEvent` → Users module creates profile (synchronous MediatR).

---

## POST /connect/token

OAuth2 token endpoint (OpenIddict). **Unversioned.**

**Auth:** Anonymous

**Content-Type:** `application/x-www-form-urlencoded`

**Request:**

| Field | Required | Description |
|-------|----------|-------------|
| `grant_type` | Yes | Must be `password` |
| `username` | Yes | Email |
| `password` | Yes | Plain password |
| `tenant_id` | Yes* | Tenant GUID (*or `X-Tenant-ID` header) |

**Success:** OAuth2 token response (access_token, refresh_token, expires_in, …)

**Failure examples:**

| error | When |
|-------|------|
| `invalid_grant` | Bad credentials or inactive/locked user |
| `invalid_tenant` | Missing/invalid tenant id |
| `tenant_inactive` | Tenant not active |
| `unsupported_grant_type` | Not password grant |

**Side effects:**

- Records successful login on `AuthUser` → `UserLoggedInDomainEvent` → Audit
- Caches session in Redis (`ISessionCacheService`, 8h TTL)
- JWT includes roles and permissions

---

## GET /api/auth/sso/google

Starts Google OAuth challenge. Redirects to provider.

## GET /api/auth/sso/microsoft

Starts Microsoft OAuth challenge.

## GET /api/auth/sso/callback

**Status:** Returns authenticated external claims stub — local account linking Phase 2.

---

## Related

- [modules/auth/api.md](../modules/auth/api.md)
- [errors/error-catalog.md](../errors/error-catalog.md)
