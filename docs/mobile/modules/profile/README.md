# Mobile — Profile enhancements (M3)

**Feature:** `FrontEnd.Mobile/lib/features/profile/`

Extends profile foundation with operational shortcuts.

## APIs

| Data | Method | Path |
|------|--------|------|
| User profile | `GET` | `/api/v1/users/{userId}` |
| Tenant info | `GET` | `/api/v1/tenants/current` |
| Roles | JWT claims | `sub`, `role`, `tenant_id` |

## M3 enhancements

- Avatar display (`avatarUrl` + local upload preview via Files)
- Tenant name, slug, plan
- Role list from JWT
- Shortcuts to notification preferences and sessions

## Avatar upload note

Profile PATCH for `avatarUrl` is not exposed on backend yet. Upload stores file via Files API and shows local session preview; full persistence awaits profile update API.

## Route

`/profile`
