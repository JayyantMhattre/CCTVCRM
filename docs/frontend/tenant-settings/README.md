# Tenant settings (frontend)

Workspace configuration at `/tenant/settings`.

**Page:** `TenantSettingsPage.tsx`

## Fields

| UI | Setting |
|----|---------|
| Require MFA | `requireMfa` |
| Session timeout | `sessionTimeoutMinutes` |
| Locale | `locale` |
| Timezone | `timezone` |

## API

| Method | Path | Notes |
|--------|------|-------|
| GET | `/api/v1/tenants/current/settings` | Falls back to defaults on 404 |
| PATCH | `/api/v1/tenants/current/settings` | Admin/Manager only (UI) |

## Related

- [routing.md](./routing.md)
- [permissions.md](./permissions.md)
