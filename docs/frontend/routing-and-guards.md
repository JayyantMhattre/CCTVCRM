# Routing and Guards

**Router:** `FrontEnd/apps/web/src/core/router/index.tsx`  
**Paths:** `FrontEnd/apps/web/src/core/router/routeMap.ts`

Routes are **centralized** (not per-module `routes.tsx`).

---

## Route map

| Path | Guard | Page |
|------|-------|------|
| `/login` | Public | LoginPage |
| `/register` | Public | RegisterPage |
| `/dashboard` | AuthGuard | DashboardPage |
| `/tenant/profile` | AuthGuard | TenantProfilePage |
| `/tenant/settings` | AuthGuard | TenantSettingsPage (stub) |
| `/users` | AuthGuard + RoleGuard Admin/Manager | UserListPage |
| `/users/:userId` | AuthGuard + RoleGuard | UserProfilePage |
| `/audit` | AuthGuard + RoleGuard Admin | AuditLogPage |
| `/403` | — | ForbiddenPage |
| `*` | — | NotFoundPage |

---

## AuthGuard

File: `shared/guards/AuthGuard.tsx`

- Shows spinner while `isLoading`
- Redirects to `/login` with `state.from`
- Renders `<Outlet />` when authenticated

---

## RoleGuard

- **Route mode:** redirect to `/403`
- **Inline mode:** hide children (sidebar links)

---

## PermissionGuard

**Inline only** — checks JWT `permission` claims via `usePermission()`.

Example: Dashboard tile for audit uses `audit:read`; route uses Admin **role** — intentional inconsistency to fix in code phase.

---

## Lazy loading

```tsx
const LoginPage = lazy(() => import('@/modules/auth/pages/LoginPage'));
```

Wrapped in `Suspense` + `Spinner`.

---

## Adding routes

See [extending/add-frontend-route.md](../extending/add-frontend-route.md).

---

## Related

- [auth.md](./auth.md)
