# ADR-ApiKeys-0001: API Keys Platform

**Status:** Accepted  
**Date:** 2026-05-31  
**Context:** Final V1 core capability — machine-to-machine authentication for CRM, LMS, integrations, and future AI agents.

---

## Decision

Implement a dedicated **ApiKeys** bounded context with tenant-scoped keys, scope-based authorization, Argon2id secret hashing, and middleware-based authentication.

---

## Hashing strategy

| Aspect | Decision |
|--------|----------|
| Algorithm | **Argon2id** (memory-hard, side-channel resistant) |
| Storage | `hashed_secret` column only — **never plaintext** |
| Display | Full key shown **once** on create/rotate via `ApiKeyCreatedContract` |
| Lookup | Unique `key_prefix` (`ashk_{env}_{id}`) for O(1) candidate resolution before verify |
| Format | `ashk_{environment}_{prefixId}_{secret}` e.g. `ashk_live_abc12345_...` |

Rationale: Aligns with Auth module `Argon2PasswordHasher` standards; prefix enables indexed lookup without storing reversible secrets.

---

## Authentication strategy

| Aspect | Decision |
|--------|----------|
| Headers | `X-API-Key: {key}` and/or `Authorization: Bearer {key}` (configurable) |
| Middleware | `ApiKeyAuthenticationMiddleware` after JWT `UseAuthentication`, before tenant resolution |
| Management routes | `/api/v1/api-keys` bypass API key auth (JWT required) |
| Claims | `sub`, `tenant_id`, `permission` (from scopes), `api_key_id`, `auth_type=apikey` |
| Validation | `IApiKeyValidator` — prefix lookup + Argon2 verify + active/expiry/revocation checks |

Rationale: Plugs into existing `ICurrentUser`, `ITenantContext`, and permission checks without replacing OpenIddict JWT flow.

---

## Scope strategy

| Aspect | Decision |
|--------|----------|
| Format | `resource:action` (same as RBAC permissions) e.g. `users:read`, `webhooks:manage` |
| Storage | JSON array on `api_keys.scopes` |
| Enforcement | Scopes emitted as JWT-style `permission` claims on API key principal |
| Management | `apikeys:read`, `apikeys:manage` for human admins (JWT) |
| Updates | `UpdateApiKeyScopes` command with `ApiKeyScopesChangedDomainEvent` audit |

Rationale: Reuses `IAuthPermissionChecker` and handler-level authorization patterns; no parallel scope system.

---

## Consequences

- **Positive:** M2M auth without OAuth provider complexity; tenant isolation; audit via domain events; rate limiting per `api_key_id`.
- **Negative:** API keys are bearer secrets — rotation/revocation UX required; scope changes need explicit admin action.
- **Future:** Developer portal, partner onboarding, and OpenIddict client-credentials remain out of V1 scope.
