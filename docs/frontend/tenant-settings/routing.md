# Tenant settings — Routing

| Path | Component | Guard |
|------|-----------|-------|
| `/tenant/settings` | `TenantSettingsPage` | `AuthGuard` only |

All authenticated users can open the page. Edit controls are disabled unless the user has **Admin** or **Manager** role (`useAuth().hasRole`).

Sidebar: **Tenant Settings** under Tenant section (no extra role guard on nav).
