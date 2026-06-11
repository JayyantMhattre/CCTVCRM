# Users API

Implementation: `BackEnd/src/Modules/Users/Ashraak.Users.Api/Endpoints/UserEndpoints.cs`

Group: `/api/v1/users` — all endpoints require authentication.

---

## GET /api/v1/users/{userId}

**Success:** `200` + `UserDto`

**Failure:** `404` if profile not found

---

## GET /api/v1/users/tenant/{tenantId}

List profiles for tenant.

**Scoping:** If caller tenant context is set, must match `tenantId` or `403`.

**Success:** `200` + array of `UserDto` (may be empty)

---

## GET /api/v1/users/tenant/current

List profiles for tenant from `ITenantContext`.

**Failure:** `400` if no tenant context

---

## UserDto shape

See `Ashraak.SharedKernel.Contracts/Users/Dtos/UserDto.cs`:

- User id, tenant id, email, display name, status
- `Preferences` — theme, locale, timezone, email notifications flag

---

## Related

- [modules/users/api.md](../modules/users/api.md)
