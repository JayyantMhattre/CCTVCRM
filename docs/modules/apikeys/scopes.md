# API key scopes

Scopes use the same `resource:action` format as RBAC permissions.

## Examples

| Scope | Grants |
|-------|--------|
| `users:read` | Read user profiles |
| `users:write` | Create/update users |
| `files:read` | Download files |
| `files:write` | Upload files |
| `webhooks:read` | Read webhook deliveries |
| `webhooks:manage` | Manage webhook subscriptions |

## Admin permissions (JWT only)

| Permission | Purpose |
|------------|---------|
| `apikeys:read` | List keys, view usage |
| `apikeys:manage` | Create, rotate, revoke, change scopes |

## Enforcement

API key scopes are emitted as `permission` claims on the authenticated principal. Downstream handlers use existing `IAuthPermissionChecker` / `HasPermission` patterns.
