# Feature Modules

Location: `FrontEnd/apps/web/src/modules/`

---

## auth

| File | Purpose |
|------|---------|
| `pages/LoginPage.tsx` | Password login |
| `pages/RegisterPage.tsx` | User registration |
| `pages/SessionsPage.tsx` | Active sessions list + revoke |
| `api.ts` | Token, register, sessions |
| `types.ts` | Request/response types |

---

## dashboard

| File | Purpose |
|------|---------|
| `pages/DashboardPage.tsx` | Quick links; role/permission gated cards |

No dedicated API — navigation only.

---

## tenant

| File | Purpose |
|------|---------|
| `pages/TenantProfilePage.tsx` | `GET /tenants/current` |
| `pages/TenantSettingsPage.tsx` | MFA, locale, session timeout |
| `api.ts` | `getCurrent`, `getSettings`, `updateSettings` |

---

## users

| File | Purpose |
|------|---------|
| `pages/UserListPage.tsx` | Tenant user list |
| `pages/UserProfilePage.tsx` | Single user profile |
| `pages/NotificationPreferencesPage.tsx` | Email notification toggle |
| `api.ts` | list, getById, `updatePreferences` |

Requires Admin or Manager role at route level.

---

## shared/file-upload

| File | Purpose |
|------|---------|
| `FileUpload.tsx` | Reusable uploader with progress + validation |
| `api.ts` | `uploadFile()` multipart helper |

See [file-upload/README.md](./file-upload/README.md).

---

## audit

| File | Purpose |
|------|---------|
| `pages/AuditLogPage.tsx` | Filters, server pagination, date range |
| `api.ts` | `getLogs(filters)` with response normalization |

Route: Admin role. Nav may use `audit:read` permission.

---

## Shared platform (cross-cutting)

| Path | Purpose |
|------|---------|
| `shared/ui/toast/` | Global toast queue + `ToastContainer` |
| `shared/errors/` | API error classification + correlation ID |
| `shared/components/CorrelationIdCopy.tsx` | Support copy button |

## Shared dependencies

All modules use:

- `@/core/api/client`
- `@/core/api/endpoints`
- `@/shared/components/*`
- `@/shared/hooks/useAuth`, `usePermission`, `useApiError`
- `@/shared/ui/toast` for success feedback

---

## Unused API surface

Defined in `endpoints.ts` but no UI:

- SSO (`sso/google`, `sso/microsoft`)
- `tenants.provision`
- `users.listByTenant` (explicit tenant id)

---

## Related

- [extending/add-frontend-route.md](../extending/add-frontend-route.md)
- [FRONTEND_STARTER.md](../../FrontEnd/FRONTEND_STARTER.md)
