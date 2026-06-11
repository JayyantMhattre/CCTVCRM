# Webhook permissions

ABAC permissions follow platform convention `resource:action`.

---

## Permissions

| Permission | Code | Grants |
|------------|------|--------|
| Read | `webhooks:read` | List and view subscriptions (no secret) |
| Manage | `webhooks:manage` | Create, update, disable, rotate secret |

Defined in `Permission.Defaults` (Auth) and enforced in command/query handlers via `IAuthPermissionChecker`.

---

## Default access

| Role / grant | Read | Manage |
|--------------|------|--------|
| **Admin** | Yes | Yes |
| `webhooks:manage` | Yes | Yes |
| `webhooks:read` | Yes | No |
| Member (baseline) | No | No |

API routes also require `TenantAdmin` policy (`Admin` or `Manager` role) at the HTTP layer.

---

## Granting access

Assign `webhooks:manage` to tenant admins via Auth permission grants (same pattern as `user:invite`).

---

## Feature flag

Even with permissions, `Features:Flags:webhooks.enabled` (or tenant override) must be `true`.
