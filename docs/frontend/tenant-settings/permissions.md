# Tenant settings — Permissions

| Action | Required role |
|--------|----------------|
| View settings form | Any authenticated user |
| Edit + Save | `Admin` or `Manager` |

Non-admins see a warning banner and read-only/disabled fields.

Backend must enforce the same rules on `PATCH /tenants/current/settings` when implemented.

## Notification settings

Per-user email preferences are **not** on this page — use [Notification preferences](../notifications/README.md).

Tenant-level notification routing will align with backend tenant settings when exposed.
