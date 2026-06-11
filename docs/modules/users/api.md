# Users — API

Base path: `/api/v1/users/` (host version group + `MapUserEndpoints`)

File: `BackEnd/src/Modules/Users/Ashraak.Users.Api/Endpoints/UserEndpoints.cs`

All endpoints require authentication.

## PATCH /api/v1/users/{userId}/preferences

| Property | Value |
|----------|-------|
| Auth | Self only (`userId` must match `ICurrentUser.UserId`) |
| Body | `{ "emailNotificationsEnabled": true }` |

Updates notification preferences on the user profile.

## GET /api/v1/users/{userId}

| Property | Value |
|----------|-------|
| Handler | `GetUser` → `IUserService.GetUserAsync` |

**Response:** `UserDto` — id, email, display name, status, preferences summary.

**Note:** EF global tenant filter applies — user must belong to resolved tenant context.

---

## GET /api/v1/users/tenant/{tenantId}

| Property | Value |
|----------|-------|
| Handler | `GetUsersForTenant` |

**Authorization:** 403 if `ITenantContext.TenantId` differs from `{tenantId}`.

**Response:** List of `UserDto` for tenant.

---

## GET /api/v1/users/tenant/current

| Property | Value |
|----------|-------|
| Handler | Uses `ITenantContext.TenantId` |

Equivalent to GET `/tenant/{tenantId}` for current tenant.

---

## Related unversioned endpoint

Auth register returns `201` with `Location: /api/v1/users/{userId}` — profile created asynchronously via event handler.

---

## Contract interface (not HTTP)

**`IUserService`** — `SharedKernel.Contracts/Users/Interfaces/IUserService.cs`

| Method | Purpose |
|--------|---------|
| `GetUserAsync(userId, tenantId)` | Single profile |
| `GetUsersForTenantAsync(tenantId)` | Tenant user list |

Implementation: `UserService.cs`

**`UserDto`:** `SharedKernel.Contracts/Users/Dtos/UserDto.cs`
