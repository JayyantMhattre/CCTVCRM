# Tenant — API

Base path: `/api/v1/tenants/` (host version group + `MapTenantEndpoints`)

File: `BackEnd/src/Modules/Tenant/Ashraak.Tenant.Api/Endpoints/TenantEndpoints.cs`

## POST /api/v1/tenants

| Property | Value |
|----------|-------|
| Auth | `AllowAnonymous` |
| Handler | `ProvisionTenant` → `ProvisionTenantCommand` |

**Request (`ProvisionTenantRequest`):**

```json
{
  "name": "Acme Corp",
  "slug": "acme",
  "plan": "Free",
  "ownerUserId": "guid-or-null"
}
```

**Responses:**

| Status | Body |
|--------|------|
| 201 | Tenant created — tenant ID in response |
| 400 | ProblemDetails (validation, slug conflict) |

---

## GET /api/v1/tenants/{tenantId}

| Property | Value |
|----------|-------|
| Auth | Required |
| Handler | `GetTenant` → `GetTenantQuery` |

**Authorization:** Returns 403 if `ITenantContext.TenantId` is set and differs from `{tenantId}`.

**Response:** Tenant DTO (name, slug, plan, status, settings summary).

---

## GET /api/v1/tenants/current

| Property | Value |
|----------|-------|
| Auth | Required |
| Handler | `GetCurrentTenant` |

Uses resolved `ITenantContext.TenantId` — same as GET by ID for current tenant.

**Fails** if no tenant in context (401/403 from middleware or handler).

---

## GET /api/v1/tenants/current/settings

| Property | Value |
|----------|-------|
| Auth | Required |
| Handler | `GetTenantSettings` → `GetTenantSettingsQuery` |

Returns `TenantSettingsDto`: `requireMfa`, `locale`, `timezone`, `sessionTimeoutMinutes`, notification flags.

---

## PATCH /api/v1/tenants/current/settings

| Property | Value |
|----------|-------|
| Auth | `TenantAdmin` policy |
| Handler | `UpdateTenantSettings` → `UpdateTenantSettingsCommand` |

Changes are audited via tenant domain events.

---

## Contract interface (not HTTP)

**`ITenantService`** — `SharedKernel.Contracts/Tenant/Interfaces/ITenantService.cs`

| Method | Used by |
|--------|---------|
| `GetTenantAsync(tenantId)` | Auth, internal |
| `IsActiveAsync(tenantId)` | Auth token/register, tenant middleware |
| `GetFeatureFlagAsync(tenantId, flag)` | Feature gating (future) |
| `GetSeatLimitAsync(tenantId)` | Seat enforcement (future) |

Implementation: `TenantService.cs`
