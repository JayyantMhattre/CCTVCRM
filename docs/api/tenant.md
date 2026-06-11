# Tenant API

Implementation: `BackEnd/src/Modules/Tenant/Ashraak.Tenant.Api/Endpoints/TenantEndpoints.cs`

Group: `/api/v1/tenants` — group requires auth except where noted.

---

## POST /api/v1/tenants

Provision a new tenant workspace.

**Auth:** Anonymous (overrides group authorization)

**Request:**

```json
{
  "name": "Acme Corporation",
  "slug": "acme",
  "plan": "Pro",
  "ownerUserId": "00000000-0000-0000-0000-000000000001"
}
```

`plan` enum: `TenantPlan` (see domain).

**Success:** `201 Created`

```json
{ "tenantId": "guid" }
```

**Failure:** `400` ProblemDetails

---

## GET /api/v1/tenants/{tenantId}

**Auth:** Bearer token required

**Scoping:** If request tenant context is set, must match `tenantId` or `403 Forbid`.

**Success:** `200` + `TenantDto` (includes settings)

**Failure:** `404` if not found

---

## GET /api/v1/tenants/current

Returns tenant from `ITenantContext` (JWT / middleware).

**Auth:** Bearer required

**Failure:** `400` if no tenant resolved; `404` if tenant missing

---

## Related

- [modules/tenant/api.md](../modules/tenant/api.md)
