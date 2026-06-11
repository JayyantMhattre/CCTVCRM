# Ashraak — Frontend Starter

A production-ready, modular React SPA scaffold. Backend-agnostic by design — it works with the Ashraak .NET API today and adapts to any REST/OAuth 2.0 backend tomorrow.

> **Canonical docs:** [docs/frontend/](../docs/frontend/) and [docs/index.md](../docs/index.md).  
> This file is a quick reference; if anything conflicts with `/docs`, **`/docs` wins**.

---

## Technology Stack

| Concern | Library | Version |
|---|---|---|
| Build tool | Vite | 6 |
| UI framework | React | 19 |
| Type safety | TypeScript | 5 (strict) |
| Routing | React Router | v7 |
| Server state | TanStack Query | v5 |
| Client state | Zustand | v5 |
| HTTP client | Axios | v1 |
| Forms & validation | React Hook Form + Zod | v7 + v3 |
| UI shell | **CoreUI 5** (`@coreui/coreui` SCSS) | 5.6+ |
| Icons | Bootstrap Icons (CDN in `index.html`) + CoreUI Icons | — |
| JWT decoding | jwt-decode | v4 |
| Date formatting | date-fns | v4 |
| Monorepo | pnpm workspaces | — |

**Note:** Plain `bootstrap` npm package is **not** used — CoreUI includes Bootstrap 5 CSS. See [COREUI_INTEGRATION.md](./COREUI_INTEGRATION.md).

---

## Folder Structure

```
FrontEnd/
├── pnpm-workspace.yaml          # Monorepo workspace config
├── package.json                 # Root scripts (dev, build, lint, type-check)
│
├── apps/
│   └── web/                     # Main React SPA (@ashraak/web)
│       ├── index.html
│       ├── vite.config.ts
│       ├── tsconfig.json
│       ├── tsconfig.node.json
│       ├── .env.example
│       ├── .env.development
│       └── src/
│           ├── main.tsx                  # Entry point — CoreUI SCSS + React root
│           ├── App.tsx                   # Root component — composes providers + router
│           │
│           ├── core/                     # Framework-level concerns (no business logic)
│           │   ├── api/
│           │   │   ├── client.ts         # Axios instance (shared by all modules)
│           │   │   ├── interceptors.ts   # JWT injection + 401 silent refresh
│           │   │   └── endpoints.ts      # All API URL constants
│           │   ├── auth/
│           │   │   ├── authStore.ts      # Zustand store — auth state + sessionStorage
│           │   │   ├── tokenService.ts   # JWT decode, claim mapping, refresh flow
│           │   │   └── AuthProvider.tsx  # Session restoration on app mount
│           │   ├── router/
│           │   │   ├── index.tsx         # createBrowserRouter — ALL routes (centralized)
│           │   │   └── routeMap.ts       # ROUTES constant — all path strings
│           │   └── providers/
│           │       └── AppProviders.tsx  # QueryClient + AuthProvider composition
│           │
│           ├── shared/                   # Reusable across modules
│           │   ├── types/
│           │   │   ├── auth.types.ts     # AuthUser, DecodedToken, AuthSession
│           │   │   └── api.types.ts      # PaginatedResponse, ProblemDetails, etc.
│           │   ├── hooks/
│           │   │   ├── useAuth.ts        # Reads auth store; exposes hasRole()
│           │   │   ├── usePermission.ts  # hasPermission / hasAllPermissions
│           │   │   ├── useTenant.ts      # Current tenantId from JWT
│           │   │   └── useApiError.ts    # Normalises Axios errors to strings
│           │   ├── guards/
│           │   │   ├── AuthGuard.tsx     # Route: redirect to /login if unauthenticated
│           │   │   ├── RoleGuard.tsx     # Route + inline: role-based access
│           │   │   └── PermissionGuard.tsx # Inline: fine-grained ABAC rendering
│           │   ├── components/
│           │   │   ├── Spinner.tsx       # Full-page and inline loading spinner
│           │   │   ├── AlertMessage.tsx  # Success/error alert banner
│           │   │   ├── PageHeader.tsx    # Page title + subtitle + action slot
│           │   │   └── EmptyState.tsx    # "Nothing here yet" placeholder
│           │   └── pages/
│           │       ├── ForbiddenPage.tsx # /403
│           │       └── NotFoundPage.tsx  # /* (catch-all 404)
│           │
│           ├── layouts/
│           │   ├── AppLayout.tsx         # Sidebar + topbar + <Outlet /> shell
│           │   └── AuthLayout.tsx        # Centred card for login/register
│           │
│           └── modules/                  # Feature modules (self-contained)
│               ├── auth/
│               │   ├── api.ts            # authApi.login(), authApi.register()
│               │   ├── types.ts          # LoginRequest, RegisterRequest, TokenResponse
│               │   └── pages/
│               │       ├── LoginPage.tsx
│               │       └── RegisterPage.tsx
│               ├── dashboard/
│               │   └── pages/
│               │       └── DashboardPage.tsx
│               ├── tenant/
│               │   ├── api.ts            # tenantApi.getCurrent(), tenantApi.getById()
│               │   ├── types.ts          # TenantDto, TenantSettingsDto
│               │   └── pages/
│               │       ├── TenantProfilePage.tsx
│               │       └── TenantSettingsPage.tsx
│               ├── users/
│               │   ├── api.ts            # usersApi.listForCurrentTenant(), .getById()
│               │   ├── types.ts          # UserDto, UserPreferencesDto
│               │   └── pages/
│               │       ├── UserListPage.tsx
│               │       └── UserProfilePage.tsx
│               └── audit/
│                   ├── api.ts            # auditApi.getLogs()
│                   ├── types.ts          # AuditEntryDto, AuditLogFilters
│                   └── pages/
│                       └── AuditLogPage.tsx
│
```

`packages/ui` (`@ashraak/ui`) is **planned Phase 2** — the `FrontEnd/packages/` folder is not in the repo yet.

---

## Authentication Flow

```
User submits login form
        │
        ▼
authApi.login() → POST /connect/token (OAuth 2.0 password grant)
        │
        ▼
TokenResponse { access_token, refresh_token, expires_in }
        │
        ▼
tokenService.decodeToken()   → DecodedToken (raw JWT claims)
tokenService.mapTokenToUser() → AuthUser { userId, email, tenantId, roles, permissions }
        │
        ▼
authStore.setSession()       → Zustand + sessionStorage key `ashraak_session`
        │
        ▼
navigate(intendedPath)       → AuthGuard renders <Outlet /> → AppLayout
```

### Token Refresh Flow (transparent to the user)

```
Any API request returns 401
        │
        ▼
interceptors.ts → _retry flag not set
        │
        ▼
tokenService.refresh() → POST /connect/token (refresh_token grant)
        │
    ┌───┴────────────────────────────┐
    │ Success                        │ Failure
    ▼                                ▼
authStore.setSession(newTokens)   authStore.clearSession()
Retry original request            window.location = /login
```

---

## Guard System

### Route-level guards (router wrapping)

```tsx
// Redirect to /login if unauthenticated:
{ element: <AuthGuard />, children: [...protectedRoutes] }

// Redirect to /403 if user lacks all of the listed roles:
{ element: <RoleGuard roles={['Admin', 'Manager']} />, children: [...adminRoutes] }
```

### Inline guards (component hiding)

```tsx
// Role-based — renders children or null:
<RoleGuard roles={['Admin']} inline>
  <DeleteButton />
</RoleGuard>

// Permission-based — renders children or optional fallback:
<PermissionGuard permission="audit:read">
  <Link to="/audit">Audit Logs</Link>
</PermissionGuard>

<PermissionGuard permission="users:manage" fallback={<ReadOnlyBadge />}>
  <EditUserButton />
</PermissionGuard>
```

---

## API Integration Pattern

Every module owns its own `api.ts` file that:

1. Imports `apiClient` from `core/api/client.ts`.
2. Imports URL constants from `core/api/endpoints.ts`.
3. Exports a typed object (e.g. `auditApi`, `usersApi`) with one function per API call.
4. Never creates its own Axios instance or hardcodes URLs.

```ts
// modules/audit/api.ts
import { apiClient } from '@/core/api/client';
import { ENDPOINTS } from '@/core/api/endpoints';

export const auditApi = {
  getLogs: async (filters?: AuditLogFilters): Promise<AuditEntryDto[]> => {
    const res = await apiClient.get(ENDPOINTS.audit.logs, { params: filters });
    return res.data;
  },
};
```

TanStack Query wraps these calls in page components:

```ts
const { data, isLoading, error } = useQuery({
  queryKey: ['audit', 'logs'],
  queryFn:  () => auditApi.getLogs(),
});
```

---

## Adding a New Module

1. **Create** `src/modules/<name>/` with `api.ts`, `types.ts`, `pages/`.
2. **Register routes** in `src/core/router/index.tsx` (lazy import + route object).
3. **Add paths** to `src/core/router/routeMap.ts`.
4. **Add nav link** to `src/layouts/AppLayout.tsx` (wrap in `<RoleGuard>` / `<PermissionGuard>` if needed).
5. **Add endpoints** to `src/core/api/endpoints.ts`.

---

## Environment Variables

All variables are prefixed `VITE_` and injected at build time via `import.meta.env`.

| Variable | Description | Default |
|---|---|---|
| `VITE_API_BASE_URL` | Backend REST API base URL | `http://localhost:5000` |
| `VITE_API_VERSION` | API version segment | `v1` |
| `VITE_APP_NAME` | Display name in browser tab | `Ashraak` |

Copy `.env.example` → `.env.development` for local development.

---

## Running Locally

```bash
# From the repo root (FrontEnd/)
pnpm install

# Start dev server at http://localhost:3000
pnpm dev

# Type-check without emitting
pnpm type-check

# Lint
pnpm lint

# Production build
pnpm build
```

The Vite dev server proxies `/api/*` and `/connect/*` to `VITE_API_BASE_URL`,
so no CORS configuration is needed during development.

---

## Security Notes

| Concern | Mitigation |
|---|---|
| Session storage | Full session (access + refresh tokens + user) in **`sessionStorage`** key `ashraak_session` — tab-scoped, cleared when tab closes |
| Zustand | In-memory mirror of session for React reactivity |
| Not used | `localStorage` for tokens (by default) |
| XSS | Avoid `dangerouslySetInnerHTML`; render user content as text |
| API auth | `Authorization: Bearer` header from interceptor (`core/api/interceptors.ts`) |
| `withCredentials` | `true` on Axios — ready for future httpOnly cookie SSO |
| Expired sessions | 401 → silent refresh once → on failure `clearSession()` + redirect `/login` |

Details: [docs/frontend/auth.md](../docs/frontend/auth.md)

---

## Known template gaps (documented)

| Item | Status |
|------|--------|
| Audit route guard | Route uses Admin **role**; some nav uses `audit:read` **permission** |
| Sidebar “My Profile” | Must use `userId` in path — see `AppLayout.tsx` |
| Tenant settings page | Phase 2 placeholder |
| SSO buttons | API routes exist; UI not wired |
| `pnpm lint` | Script exists; ESLint config may be minimal |

---

## Documentation workflow

Code and docs evolve together — see [docs/developer-workflow.md](../docs/developer-workflow.md).
