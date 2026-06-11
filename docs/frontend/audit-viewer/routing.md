# Audit viewer — Routing

| Path | Component | Guard |
|------|-----------|-------|
| `/audit` | `AuditLogPage` | `AuthGuard` + `RoleGuard(['Admin'])` |

Registered in `core/router/index.tsx` under the authenticated `AppLayout`.

Sidebar link is wrapped in `PermissionGuard permission="audit:read"` so users without the permission do not see the nav item (route remains Admin-role gated).
