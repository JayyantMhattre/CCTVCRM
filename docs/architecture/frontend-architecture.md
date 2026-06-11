# Frontend Architecture

**Stack:** React 19, TypeScript 5 (strict), Vite 6, React Router 7 — **not Angular**.

Package: `@ashraak/web` at `FrontEnd/apps/web/`.

---

## High-level structure

```
src/
├── core/           # API client, auth, router, providers
├── layouts/        # AppLayout (authenticated), AuthLayout (public)
├── modules/        # Feature folders (auth, tenant, users, audit, dashboard)
├── shared/         # Guards, hooks, components, error pages
└── styles/         # CoreUI SCSS
```

---

## State management

| Concern | Library | Location |
|---------|---------|----------|
| Server data | TanStack Query v5 | Feature pages (`useQuery`) |
| Auth session | Zustand v5 | `core/auth/authStore.ts` |
| Forms | React Hook Form + Zod | Auth pages |

Session persistence: `sessionStorage` key `ashraak_session` (tokens + user claims).

---

## API communication

- Axios instance: `core/api/client.ts`
- Base URL: `VITE_API_BASE_URL` (default proxied to `http://localhost:5000`)
- JWT: request interceptor attaches `Authorization: Bearer`
- 401: silent refresh via `tokenService.refresh()`, then retry once

---

## Routing

Centralized in `core/router/index.tsx` (not per-module `routes.tsx`).

Guards:

- `AuthGuard` — route-level authentication
- `RoleGuard` — route or inline role check
- `PermissionGuard` — inline permission check only

---

## UI kit

**CoreUI 5** (SCSS) — see [COREUI_INTEGRATION.md](../../FrontEnd/COREUI_INTEGRATION.md).

Legacy quick reference: [FRONTEND_STARTER.md](../../FrontEnd/FRONTEND_STARTER.md) (aligned with CoreUI).

---

## Feature module pattern

Each feature folder contains:

- `api.ts` — typed API calls using `ENDPOINTS`
- `types.ts` — DTOs matching backend contracts
- `pages/*.tsx` — route targets (lazy-loaded)

---

## Known inconsistencies (documented, not fixed in doc phase)

| Issue | Detail |
|-------|--------|
| Audit route guard | Router uses `RoleGuard(['Admin'])`; nav uses `audit:read` permission |
| Profile nav link | Sidebar may link to `/users/:userId` without substituting id |
| Tenant settings | Placeholder page — Phase 2 |
| SSO UI | Endpoints defined; no login buttons wired |

---

## Related

- [Frontend docs index](../frontend/architecture.md)
- [Routing and guards](../frontend/routing-and-guards.md)
- [ADR N/A — React SPA chosen in template design]
