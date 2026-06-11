# Mobile — Tenant settings

**Feature:** `FrontEnd.Mobile/lib/features/settings/`

## APIs

| Action | Method | Path | Auth |
|--------|--------|------|------|
| Read | `GET` | `/api/v1/tenants/current/settings` | Bearer + tenant |
| Update | `PATCH` | `/api/v1/tenants/current/settings` | `TenantAdmin` (Admin/Manager) |

## Fields (M3)

- `requireMfa`
- `locale`
- `timezone`
- `sessionTimeoutMinutes`
- `passwordMinLength`

## Permissions

- **Admin / Manager** — edit and save
- **Other roles** — read-only form with warning banner

Roles from JWT (`currentUserProvider.canManageTenant`).

## Provider

`SettingsProvider` — load, draft edits, save.

## Route

`/settings`
