# Admin UI permissions

---

## JWT permissions

| Permission | UI capability |
|------------|---------------|
| `webhooks:read` | Access operations center, lists, and detail views |
| `webhooks:manage` | Create/edit/disable subscriptions, rotate secrets, retry deliveries, replay dead letters |

Users with `webhooks:manage` implicitly have read access in the UI guard.

---

## Enforcement

| Layer | Mechanism |
|-------|-----------|
| Routes | `WebhooksRouteGuard` — redirects to `/403` without read permission |
| Sidebar | `PermissionGuard permission="webhooks:read"` |
| Actions | `useWebhookPermissions().canManage` hides buttons |

Backend enforces the same permissions on every API call.

---

## Tenant Admin policy

API routes also require `TenantAdmin` authorization policy at the HTTP layer. Ensure admin users hold both the role and webhook permissions.
