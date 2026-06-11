# Platform Compatibility & Integration Strategy

**Theme:** HexaDash v1.3.0 (React / Ant Design variant)  
**Platform:** Ashraak `@ashraak/web` — React 19, Vite 6, TypeScript, CoreUI 5  
**Status:** Analysis and planning only — no code changes  
**Date:** June 2026

**Prerequisites:** This report builds on all documents in `docs/frontend/themes/current-theme/` and the current platform architecture documented in `docs/frontend/`.

---

## Executive summary

HexaDash and Ashraak share the same **conceptual admin-shell pattern** (fixed sidebar, sticky header, auth card, dashboard tiles) but differ fundamentally in **UI kit, state management, auth model, and routing guards**. A full theme merge is incompatible with platform architecture.

The safest, most maintainable path is **partial adoption via a Theme Adapter Layer and a Platform UI abstraction**:

```
Business Modules  →  Platform UI  →  Theme Adapter  →  Theme Implementation
```

**Final recommendation:** **Use theme partially** — adopt layout visual language, design tokens, and dashboard wireframes; reject theme routing, state, API, auth, tables, forms, and Ant Design components as-is. See [Final recommendation](#final-recommendation) for section-level decisions.

---

## Platform baseline (Ashraak)

| Concern | Current implementation |
|---------|------------------------|
| Package | `@ashraak/web` at `FrontEnd/apps/web/` |
| Entry | `main.tsx` → `App.tsx` → `AppProviders` → `AppRouter` |
| Layout | `layouts/AppLayout.tsx`, `layouts/AuthLayout.tsx` |
| Router | `createBrowserRouter` in `core/router/index.tsx` |
| Route map | `core/router/routeMap.ts` |
| Guards | `AuthGuard`, `RoleGuard`, `PermissionGuard`, module guards (`ApiKeysRouteGuard`, `WebhooksRouteGuard`) |
| Auth | Zustand `authStore` + `AuthProvider`; JWT in `sessionStorage` |
| Server state | TanStack Query v5 per module |
| API | Shared `apiClient` (Axios) + interceptors |
| UI kit | CoreUI 5 SCSS (`styles/coreui.scss`, `_variables.scss`) |
| Forms | React Hook Form + Zod |
| Tables | Native HTML `<table>` + CoreUI classes + TanStack Query |
| Modals | `react-bootstrap/Modal` (ConfirmDialog, KeyRevealModal) |
| Notifications | Custom toast system (`shared/ui/toast/`) |
| Charts | None installed |
| Modules | `auth`, `dashboard`, `tenant`, `users`, `audit`, `apikeys`, `webhooks` |

---

## Theme baseline (HexaDash)

| Concern | Theme implementation |
|---------|---------------------|
| Package | `HexaDash` 1.3.0 at `C:\themeforest\hexadash-react\hexadash-react\` |
| Entry | CRA `index.js` → `App.js` → Redux `Provider` |
| Layout | HOC `withAdminLayout`, `AuthLayout` |
| Router | `BrowserRouter` + binary `isLoggedIn` gate |
| Guards | `ProtectedRoute` (login flag only) |
| Auth | Redux `authSlice`; `localStorage` + demo REST / Firebase / Auth0 |
| Server state | Redux slices (25+) + mock `DataService` |
| API | `config/dataService/dataService.js` |
| UI kit | Ant Design 5 + styled-components + custom SCSS |
| Forms | Ant Design `Form` |
| Tables | Ant Design `Table` + Redux `data-filter` slice |
| Modals | Ant Design `Modal` wrapper |
| Notifications | Ant `notification`, header dropdowns (demo data) |
| Charts | ApexCharts, Chart.js, Recharts, Google Charts |
| Demo pages | 100+ routes (ecommerce, chat, firebase CRUD, etc.) |

---

## Compatibility matrix (18 areas)

Legend:

- **GREEN** — Can be adopted directly (assets, patterns, or already aligned)
- **YELLOW** — Requires Theme Adapter / Platform UI wrapper before use
- **RED** — Must not be adopted; conflicts with platform architecture

---

### 1. Layout system

| | Platform | Theme |
|---|----------|-------|
| Pattern | CoreUI `.sidebar` + `.wrapper` + `<Outlet>` | Ant `Layout` HOC `withAdminLayout` |
| Injection | React Router layout route | HOC wraps admin route tree |
| Footer | Minimal / inline in layout | Fixed footer row in HOC |
| State | Local React state (`useState`) | Redux `ChangeLayoutMode` + local collapse |

**Classification: YELLOW**

**Why:** Structural intent matches (sidebar + header + content), but implementation is tied to Ant Design `Layout`, Redux theme slice, and class-based HOC. Cannot drop in without replacing CoreUI shell. Visual tokens and breakpoint behaviour (991px, 1200px) can be ported behind `PlatformLayout`.

---

### 2. Sidebar

| | Platform | Theme |
|---|----------|-------|
| Width | 16rem (`--cui-sidebar-occupy-start`) | 280px Ant `Sider` |
| Modes | Full + narrow (icon-only) | Collapsed icon mode |
| Nav source | Inline in `AppLayout` + guards | `MenueItems.js` (~1,400 lines) |
| Scroll | Native | Custom scrollbars package |
| Permissions | `RoleGuard` / `PermissionGuard` inline | None |

**Classification: YELLOW**

**Why:** Sidebar **visual design** and collapse behaviour are adoptable as SCSS + adapter. `MenueItems.js` is RED content (demo routes, no permissions). Platform must own nav item generation from `routeMap` + JWT claims; theme only renders items.

---

### 3. Header

| | Platform | Theme |
|---|----------|-------|
| Content | App name, dark mode, user avatar | Logo, collapse, search, messages, notifications, i18n, settings, user |
| Dark mode | `data-coreui-theme` on `<html>` | Redux + CSS variables |
| User data | Real JWT claims via `useAuth()` | Hard-coded demo user |
| Top menu mode | Not supported | Optional horizontal `TopMenu` |

**Classification: YELLOW**

**Why:** Header **layout zones** (brand left, utilities right) are a useful reference. HexaDash `AuthInfo` bundles demo widgets (messages, notifications, billing links) that Ashraak does not have. Adopt structure via `PlatformHeader`; implement utilities from platform hooks, not theme components.

---

### 4. Navigation

| | Platform | Theme |
|---|----------|-------|
| Definition | `AppLayout` + `routeMap.ts` | `MenueItems.js` + `TopMenu.js` |
| i18n | None | i18next |
| Active state | `NavLink` `active` class | Ant `Menu` selectedKeys from URL |
| Access control | Per-link guards | None |

**Classification: RED** (theme menu files) / **YELLOW** (nav rendering abstraction)

**Why:** Theme navigation is a static demo catalog. **Never import** `MenueItems.js` or `TopMenu.js`. Platform navigation logic stays in `core/router` + permission hooks. Only the **visual nav renderer** belongs in the theme adapter (`PlatformNav` → theme-specific markup).

---

### 5. Routing

| | Platform | Theme |
|---|----------|-------|
| API | `createBrowserRouter`, `RouteObject[]` | `BrowserRouter`, nested `<Routes>` |
| Auth gate | `AuthGuard` on layout branch | `isLoggedIn` ternary in `App.js` |
| Public routes | `AuthLayout` + `/login`, `/register` | `AuthLayout` HOC + `/`, `/register`, `/forgotPassword` |
| Module routes | ~25 production routes | 100+ demo routes |
| 403 | `ForbiddenPage` | Not present |
| Lazy loading | `React.lazy` + `Suspense` | Same pattern |

**Classification: RED**

**Why:** Theme routing is auth-binary and demo-centric. Ashraak's centralized router with role/permission/module guards must not change. Theme provides **zero** reusable route definitions for Ashraak features.

---

### 6. Authentication pages

| | Platform | Theme |
|---|----------|-------|
| Pages | Login, Register (+ Sessions when authed) | SignIn, Signup, ForgotPassword, Firebase variants |
| Forms | RHF + Zod | Ant Design Form |
| API | `POST /connect/token` OAuth | Mock `/login`, Firebase, Auth0 |
| Layout | `AuthLayout` CoreUI card | `AuthLayout` HOC + background image |
| Tenant | `tenantId` required on login | Not modeled |

**Classification: YELLOW** (visual) / **RED** (logic)

**Why:** Login/register **business logic** (JWT, tenant UUID, MFA-ready) must stay. Auth page **background, card proportions, typography** from HexaDash can be applied via `PlatformAuthLayout` + SCSS tokens. Do not adopt `SignIn.js` or `authSlice`.

---

### 7. Dashboard pages

| | Platform | Theme |
|---|----------|-------|
| Content | Permission-aware stat cards, static tiles | 10 demo dashboards with charts/widgets |
| Data | Placeholder; ready for Query hooks | Static `demoData` / Redux |
| Components | CoreUI `.card-stat`, `PageHeader` | `Cards` frame, ApexCharts widgets |

**Classification: YELLOW**

**Why:** HexaDash dashboard demos are **layout wireframes**, not data layers. Use as visual reference for `PlatformDashboardGrid` / `PlatformStatCard`. Wire to TanStack Query when backend stats exist. Do not import demo containers.

---

### 8. Tables

| | Platform | Theme |
|---|----------|-------|
| Implementation | HTML `<table>` + CoreUI classes | Ant Design `Table` |
| Data | TanStack Query | Redux `data-filter` + DOM queries |
| Features | Pagination (audit), filters (audit) | Built-in Ant filters, row selection |
| Typing | TypeScript DTOs | PropTypes / untyped |

**Classification: RED** (theme tables) / **YELLOW** (future `PlatformTable`)

**Why:** `DataTable.js` is Redux-coupled and uses `document.querySelector`. Conflicts with Query-driven pages (`UserListPage`, `AuditLogPage`). Introduce `PlatformTable` abstraction if Ant-style tables are desired later; current CoreUI tables work and need only token styling.

---

### 9. Forms

| | Platform | Theme |
|---|----------|-------|
| Library | React Hook Form + Zod | Ant Design Form |
| Validation | Schema-first, typed | Ant rules / `initialValue` demos |
| Usage | Login, Register, module forms | 50+ UI demo forms |

**Classification: RED** (theme forms) / **YELLOW** (field styling)

**Why:** Form **state and validation** are platform standards. Changing to Ant Form would rewrite every module page. Apply HexaDash input **visual tokens** (height, radius, border color) to CoreUI/Bootstrap form classes via SCSS only.

---

### 10. Modals

| | Platform | Theme |
|---|----------|-------|
| Library | `react-bootstrap/Modal` | Ant Design `Modal` + styled wrapper |
| Usage | `ConfirmDialog`, `KeyRevealModal`, `SecretRevealModal` | Demo modals page |
| API | Typed props (`show`, `onConfirm`, etc.) | `visible`, Redux-driven demos |

**Classification: YELLOW**

**Why:** Modal **behaviour** is already centralized in a few module components. Wrap behind `PlatformDialog` so theme swap changes markup/classes only. Do not import `antd-modals.js` directly into modules.

---

### 11. Notifications

| | Platform | Theme |
|---|----------|-------|
| Toast | Custom `useToast` + `ToastContainer` (CoreUI) | Ant `notification` API |
| In-app | None (no header notification center) | Header `Notification` dropdown (demo) |
| API errors | Toast via interceptors / `useApiError` | N/A |

**Classification: YELLOW** (toast styling) / **RED** (header notification demo)

**Why:** Platform toast system is purpose-built (correlation ID display, variants). Restyle via adapter to match HexaDash colors. Header notification/message center is demo-only and out of scope unless product requests it.

---

### 12. Charts

| | Platform | Theme |
|---|----------|-------|
| Libraries | None | ApexCharts, Chart.js, Recharts, Google Charts |
| Usage | N/A | Extensive demo routes |

**Classification: GREEN** (add one library when needed) / **RED** (import all four)

**Why:** Charts are net-new for Ashraak. When dashboard stats APIs exist, add **one** chart library (Recharts recommended for React 19 compatibility) behind `PlatformChart`. HexaDash demos serve as **composition reference** only.

---

### 13. Theme engine

| | Platform | Theme |
|---|----------|-------|
| Mechanism | CoreUI SCSS + `data-coreui-theme` | CRACO Less + Redux mode + CSS vars + styled-components |
| Tokens | `_variables.scss` (`$primary`, etc.) | `themeVariables.js`, `themeConfigure.js` |
| Runtime switch | `AppLayout` local state → `<html>` attribute | Redux `changeLayoutMode` dispatch |
| Pluggable | No — hardcoded CoreUI | No — hardcoded Ant/HexaDash |

**Classification: YELLOW**

**Why:** Neither side has a replaceable theme engine today. This integration **introduces** one: token files + `ThemeProvider` + adapter registry. HexaDash token **values** are GREEN for extraction; HexaDash **engine code** is RED.

---

### 14. Responsive design

| | Platform | Theme |
|---|----------|-------|
| Sidebar mobile | Overlay + backdrop (`d-lg-none`) | Overlay + `.ninjadash-shade` ≤ 991px |
| Breakpoints | CoreUI/Bootstrap `lg` (992px) | 991px, 1200px |
| Grid | Bootstrap/CoreUI utilities | Ant `Row`/`Col` |

**Classification: GREEN**

**Why:** Breakpoints are nearly identical. Responsive **patterns** (mobile drawer, auto-collapse) are directly applicable to CoreUI `AppLayout` via SCSS/media queries without new dependencies.

---

### 15. State management

| | Platform | Theme |
|---|----------|-------|
| Client auth | Zustand `authStore` | Redux `authSlice` |
| Server data | TanStack Query | Redux slices per feature |
| Layout prefs | Local state (dark mode) | Redux `ChangeLayoutMode` |
| Firebase | None | `react-redux-firebase` wired in store |

**Classification: RED**

**Why:** Theme Redux store is demo scaffolding. Adopting any slice would fork platform architecture. Layout preferences (dark mode, sidebar narrow) may move to Zustand `uiStore` or `localStorage` in platform — not Redux.

---

### 16. API layer

| | Platform | Theme |
|---|----------|-------|
| Client | `apiClient` singleton | `DataService` Axios wrapper |
| Auth header | JWT interceptor | Token in `localStorage` |
| Refresh | Silent refresh on 401 | None |
| Endpoints | Typed `endpoints.ts` | Mock `/login`, `/register` |
| Errors | `useApiError`, correlation ID | Basic error strings |

**Classification: RED**

**Why:** Completely different contracts and security model. Theme API layer has no Ashraak endpoint mappings, tenant scope, or refresh flow.

---

### 17. Permission system

| | Platform | Theme |
|---|----------|-------|
| Roles | JWT `role` claim → `RoleGuard` | None |
| Permissions | JWT `permission` → `PermissionGuard`, module hooks | None |
| Route guards | `RoleGuard`, `ApiKeysRouteGuard`, `WebhooksRouteGuard` | `ProtectedRoute` (login only) |
| Nav hiding | Inline guards on sidebar links | All links visible |

**Classification: RED**

**Why:** Theme has no RBAC/ABAC. Permission system is non-negotiable platform infrastructure. Theme adapter nav must accept `visible: boolean` or `requiredPermission` per item from platform — never from theme.

---

### 18. Session management

| | Platform | Theme |
|---|----------|-------|
| Storage | `sessionStorage` (`ashraak_session`) | `localStorage` (`hexadash_login`, `access_token`) |
| Token type | JWT access + refresh | Opaque demo token / Firebase |
| Restoration | `AuthProvider` on mount | `initializeAuth()` Redux dispatch |
| Logout | `clearSession()` + redirect | `logoutUser` thunk |
| Security | Access token in memory preference documented | Demo-grade persistence |

**Classification: RED**

**Why:** Theme session model is incompatible with Ashraak security requirements. Auth pages may look like HexaDash; session handling must remain 100% platform-owned.

---

## Classification summary

| Area | Status | Notes |
|------|--------|-------|
| 1. Layout system | YELLOW | Adapter + token port |
| 2. Sidebar | YELLOW | Visual only; reject MenueItems |
| 3. Header | YELLOW | Structure reference; reject AuthInfo demo |
| 4. Navigation | RED / YELLOW | Logic RED; renderer YELLOW |
| 5. Routing | RED | Keep platform router |
| 6. Authentication pages | YELLOW / RED | Visual YELLOW; logic RED |
| 7. Dashboard pages | YELLOW | Wireframe reference |
| 8. Tables | RED / YELLOW | Theme RED; PlatformTable YELLOW |
| 9. Forms | RED / YELLOW | RHF+Zod RED line; styling YELLOW |
| 10. Modals | YELLOW | PlatformDialog wrapper |
| 11. Notifications | YELLOW / RED | Toast style YELLOW; header demo RED |
| 12. Charts | GREEN / RED | One lib GREEN; all four RED |
| 13. Theme engine | YELLOW | Build new adapter engine |
| 14. Responsive design | GREEN | Patterns align |
| 15. State management | RED | Keep Zustand + Query |
| 16. API layer | RED | Keep apiClient |
| 17. Permission system | RED | Keep guards |
| 18. Session management | RED | Keep JWT flow |

| Count | Areas |
|-------|-------|
| GREEN | 2 (responsive, charts-once) |
| YELLOW | 11 |
| RED | 7 (plus partial RED on 4 YELLOW rows) |

---

## Theme Adapter architecture

### Layer model

```
┌─────────────────────────────────────────────────────────────┐
│  BUSINESS MODULES (modules/*)                               │
│  auth, users, audit, webhooks, apikeys, tenant, dashboard   │
│  Own: api.ts, types.ts, pages, hooks, domain logic          │
│  Must NOT import: antd, theme adapters, CoreUI directly     │
└──────────────────────────┬──────────────────────────────────┘
                           │ consumes
┌──────────────────────────▼──────────────────────────────────┐
│  PLATFORM UI (src/platform-ui/)                             │
│  Stable, typed, theme-agnostic component API                │
│  PlatformLayout, PlatformNav, PlatformTable, PlatformForm   │
└──────────────────────────┬──────────────────────────────────┘
                           │ delegates to
┌──────────────────────────▼──────────────────────────────────┐
│  THEME ADAPTER LAYER (src/theme/)                           │
│  Contracts (interfaces) + ThemeProvider + adapter registry  │
│  Selects implementation via VITE_THEME or runtime config    │
└──────────────────────────┬──────────────────────────────────┘
                           │ implements
┌──────────────────────────▼──────────────────────────────────┐
│  THEME IMPLEMENTATIONS (src/theme/adapters/*)                │
│  coreui/   — current CoreUI 5 (default, baseline)           │
│  hexadash/ — HexaDash visual port (SCSS tokens + layout)    │
│  future/   — next ThemeForest purchase                      │
└─────────────────────────────────────────────────────────────┘
```

### Folder structure (planned)

```
FrontEnd/apps/web/src/
├── core/                    # UNCHANGED — api, auth, router
├── modules/                 # UNCHANGED — migrates to platform-ui imports over time
├── shared/                  # UNCHANGED — guards, hooks (not theme-aware)
│
├── platform-ui/             # NEW — public UI API for modules
│   ├── index.ts             # barrel exports
│   ├── layout/
│   │   ├── PlatformLayout.tsx
│   │   ├── PlatformAuthLayout.tsx
│   │   ├── PlatformHeader.tsx
│   │   ├── PlatformSidebar.tsx
│   │   └── PlatformFooter.tsx
│   ├── navigation/
│   │   ├── PlatformNav.tsx
│   │   ├── PlatformNavItem.ts
│   │   └── usePlatformNav.ts    # builds items from routeMap + permissions
│   ├── tables/
│   │   ├── PlatformTable.tsx
│   │   ├── PlatformTableColumn.ts
│   │   └── PlatformPagination.tsx
│   ├── forms/
│   │   ├── PlatformFormField.tsx  # RHF-compatible wrapper
│   │   ├── PlatformInput.tsx
│   │   └── PlatformSelect.tsx
│   ├── dialogs/
│   │   ├── PlatformDialog.tsx
│   │   └── PlatformConfirmDialog.tsx
│   ├── cards/
│   │   ├── PlatformCard.tsx
│   │   ├── PlatformStatCard.tsx
│   │   └── PlatformPageHeader.tsx
│   ├── charts/
│   │   ├── PlatformChart.tsx
│   │   └── chart.types.ts
│   └── notifications/
│       ├── PlatformToast.tsx      # facade over useToast
│       └── PlatformAlert.tsx
│
├── theme/                   # NEW — adapter layer (not imported by modules)
│   ├── index.ts
│   ├── ThemeProvider.tsx
│   ├── ThemeContext.ts
│   ├── config.ts              # activeTheme: 'coreui' | 'hexadash'
│   ├── registry.ts            # maps theme id → adapter set
│   ├── contracts/
│   │   ├── LayoutContract.ts
│   │   ├── NavContract.ts
│   │   ├── TableContract.ts
│   │   ├── DialogContract.ts
│   │   ├── CardContract.ts
│   │   └── ChartContract.ts
│   ├── tokens/
│   │   ├── coreui.tokens.scss
│   │   └── hexadash.tokens.scss
│   └── adapters/
│       ├── coreui/
│       │   ├── CoreUILayout.tsx
│       │   ├── CoreUINav.tsx
│       │   ├── CoreUITable.tsx
│       │   └── ...
│       └── hexadash/
│           ├── HexadashLayout.tsx
│           ├── HexadashNav.tsx
│           ├── hexadash.scss
│           └── ...
│
└── layouts/                 # DEPRECATED over time → thin wrappers calling PlatformLayout
    ├── AppLayout.tsx          # becomes orchestrator only
    └── AuthLayout.tsx
```

### Responsibilities

| Layer | Owns | Must not own |
|-------|------|--------------|
| **Modules** | Business rules, API calls, Query keys, Zod schemas | CSS classes, antd, CoreUI markup |
| **Platform UI** | Stable props interfaces, composition, a11y defaults | Theme colors, vendor-specific DOM |
| **Theme Adapter** | Mapping platform props → theme markup/CSS | Business logic, API, permissions |
| **Theme impl** | Visual rendering, SCSS, icons | Route definitions, auth state |

### Key abstractions

#### `PlatformNavItem`

```typescript
interface PlatformNavItem {
  id: string;
  label: string;
  path: string;
  icon?: ReactNode;
  children?: PlatformNavItem[];
  /** Pre-computed by usePlatformNav — adapter does not check JWT */
  visible: boolean;
  badge?: string | number;
}
```

#### `PlatformLayoutProps`

```typescript
interface PlatformLayoutProps {
  navItems: PlatformNavItem[];
  user: AuthUser | null;
  isDarkMode: boolean;
  onToggleDarkMode: () => void;
  onLogout: () => void;
  children: React.ReactNode;
}
```

#### `ThemeAdapterRegistry`

```typescript
interface ThemeAdapterSet {
  id: 'coreui' | 'hexadash' | string;
  Layout: ComponentType<PlatformLayoutProps>;
  AuthLayout: ComponentType<{ children: ReactNode }>;
  Nav: ComponentType<{ items: PlatformNavItem[] }>;
  Card: ComponentType<PlatformCardProps>;
  Table: ComponentType<PlatformTableProps>;
  Dialog: ComponentType<PlatformDialogProps>;
  // …
}
```

### Migration strategy (adapter-first)

1. **Introduce contracts** without changing visuals (coreui adapter wraps existing markup).
2. **Point `AppLayout` at `PlatformLayout`** — behaviour unchanged.
3. **Add hexadash adapter** behind `VITE_THEME=hexadash` flag.
4. **Migrate modules** from direct CoreUI/`PageHeader` imports to `platform-ui` exports one module at a time.
5. **Never import** `theme/adapters/hexadash` from modules — only `platform-ui`.

### Theme replacement procedure

To swap HexaDash for a future theme:

1. Add `theme/adapters/newtheme/` implementing `ThemeAdapterSet`.
2. Add `theme/tokens/newtheme.tokens.scss`.
3. Register in `registry.ts`.
4. Set `VITE_THEME=newtheme`.
5. Zero changes to `modules/`, `core/`, or `shared/guards/`.

---

## Platform UI layer (`src/platform-ui/`)

### Recommended abstractions

| Folder | Components | Purpose |
|--------|------------|---------|
| `layout/` | `PlatformLayout`, `PlatformAuthLayout`, `PlatformHeader`, `PlatformSidebar`, `PlatformFooter` | Application shell; replaces direct `AppLayout`/`AuthLayout` usage in router |
| `navigation/` | `PlatformNav`, `usePlatformNav` | Renders nav from `routeMap` + guards; hides permission logic from theme |
| `tables/` | `PlatformTable`, `PlatformPagination` | Unified table API for list pages (users, audit, api keys, webhooks) |
| `forms/` | `PlatformFormField`, `PlatformInput`, `PlatformSelect` | RHF-compatible wrappers; consistent field chrome |
| `dialogs/` | `PlatformDialog`, `PlatformConfirmDialog` | Replaces direct `react-bootstrap/Modal` in modules |
| `cards/` | `PlatformCard`, `PlatformStatCard`, `PlatformPageHeader` | Replaces `PageHeader` + raw `.card` markup |
| `charts/` | `PlatformChart`, `PlatformChartSeries` | Lazy-loaded chart backend (Recharts default) |
| `notifications/` | `PlatformToast`, `PlatformAlert` | Facade over `useToast` and `AlertMessage` |

### Import rules (enforced in lint phase)

| From → To | Allowed |
|-----------|---------|
| `modules/*` → `platform-ui` | Yes |
| `modules/*` → `theme/*` | **No** |
| `modules/*` → `@coreui/*` | **No** (after migration) |
| `platform-ui` → `theme/*` | Yes |
| `theme/adapters/*` → `antd` / `@coreui/*` | Yes (isolated) |
| `core/*` → `platform-ui` | No |

### Existing shared components → platform-ui mapping

| Current | Future |
|---------|--------|
| `shared/components/PageHeader.tsx` | `platform-ui/cards/PlatformPageHeader.tsx` |
| `shared/components/AlertMessage.tsx` | `platform-ui/notifications/PlatformAlert.tsx` |
| `shared/components/EmptyState.tsx` | `platform-ui/cards/PlatformEmptyState.tsx` |
| `shared/components/Spinner.tsx` | Stays in `shared/` (non-themed) |
| `shared/ui/toast/*` | Wrapped by `PlatformToast` |
| `modules/webhooks/components/ConfirmDialog.tsx` | `platform-ui/dialogs/PlatformConfirmDialog.tsx` |

---

## Phased migration plan

### Phase T1 — Theme Adapter Foundation

**Goal:** Establish layer boundaries without visual change.

| Task | Scope |
|------|-------|
| Create `theme/contracts/` interfaces | New files only |
| Create `theme/adapters/coreui/` wrapping existing CoreUI markup | Parity with current UI |
| Create `ThemeProvider` + `registry.ts` | `VITE_THEME=coreui` default |
| Extract `hexadash.tokens.scss` from theme analysis | Tokens only |
| Add `platform-ui/index.ts` barrel (empty stubs) | API skeleton |
| Document import rules | ESLint `no-restricted-imports` (future) |

**Exit criteria:** App runs identically; `VITE_THEME=coreui` is the only active theme; no module imports changed yet.

**Effort:** 3–5 days

---

### Phase T2 — Layout Migration

**Goal:** Router uses Platform Layout; optional HexaDash visual adapter.

| Task | Scope |
|------|-------|
| Implement `PlatformLayout` + `PlatformAuthLayout` | `platform-ui/layout/` |
| Refactor `layouts/AppLayout.tsx` to thin orchestrator | Calls `usePlatformNav` + `PlatformLayout` |
| Refactor `layouts/AuthLayout.tsx` | Same |
| Implement `theme/adapters/hexadash/HexadashLayout.tsx` | CoreUI structure + HexaDash SCSS |
| Apply token overrides to `_variables.scss` | Purple palette option |
| Dark mode bridge | Unified `ThemeModeService` → CoreUI attribute or CSS vars |

**Exit criteria:** `VITE_THEME=hexadash` renders HexaDash-styled shell; guards and routes unchanged.

**Effort:** 5–8 days

---

### Phase T3 — Navigation Migration

**Goal:** Nav driven by platform; theme only renders.

| Task | Scope |
|------|-------|
| Implement `usePlatformNav` | Reads `routeMap`, applies Role/Permission guards |
| Implement `PlatformNav` + CoreUI/Hexadash nav adapters | |
| Remove inline nav JSX from `AppLayout` | |
| Align breakpoints (991/992px) | SCSS only |

**Exit criteria:** Adding a route = update `routeMap` + router only; nav updates automatically; permission hiding preserved.

**Effort:** 3–5 days

---

### Phase T4 — Component Migration

**Goal:** Modules use platform-ui primitives.

| Task | Priority order |
|------|----------------|
| `PlatformPageHeader`, `PlatformCard`, `PlatformAlert` | High — used everywhere |
| `PlatformConfirmDialog` | High — webhooks, apikeys |
| `PlatformTable` + `PlatformPagination` | Medium — users, audit |
| `PlatformFormField` wrappers | Medium — auth pages last (already RHF) |
| `PlatformToast` facade | Low — styling only |
| `PlatformChart` + add Recharts | Low — when stats API exists |

**Exit criteria:** No module imports `react-bootstrap` or raw `.card`/`.table` directly; all via `platform-ui`.

**Effort:** 8–12 days (incremental per module)

---

### Phase T5 — Module Migration

**Goal:** Every feature module fully decoupled from theme.

| Module | T4 dependencies |
|--------|-----------------|
| `dashboard` | StatCard, PageHeader |
| `users` | Table, PageHeader, Card |
| `audit` | Table, Pagination, filters |
| `apikeys` | Table, Dialog, PageHeader |
| `webhooks` | Table, Dialog, Badge |
| `tenant` | Card, Form fields |
| `auth` | AuthLayout, Form fields, Alert |

**Exit criteria:** Grep for `@coreui`, `react-bootstrap`, `className="card` in `modules/` returns zero (except tests).

**Effort:** 10–15 days

---

### Phase T6 — Theme Replacement Validation

**Goal:** Prove replaceability.

| Task | Method |
|------|--------|
| Create `theme/adapters/mock/` with intentionally different colors | 1-day spike |
| Switch `VITE_THEME=mock` | Visual regression only |
| Run E2E / manual guard tests | Auth, 403, permission-hidden nav |
| Bundle size comparison | coreui vs hexadash vs mock |
| Document swap procedure | Update README |

**Exit criteria:** Theme swap requires zero module file changes; only adapter + tokens.

**Effort:** 2–4 days

---

### Total estimated effort

| Phase | Effort | Risk |
|-------|--------|------|
| T1 Foundation | 3–5 days | Low |
| T2 Layout | 5–8 days | Medium |
| T3 Navigation | 3–5 days | Medium |
| T4 Components | 8–12 days | Medium |
| T5 Modules | 10–15 days | Medium |
| T6 Validation | 2–4 days | Low |
| **Total** | **31–49 days** (~6–10 weeks) | |

Assumes one developer familiar with the codebase. Parallel work on T4/T5 per module reduces calendar time.

---

## Risk analysis

### High risk

| Area | Risk | Mitigation |
|------|------|------------|
| Dual UI kits (Ant + CoreUI) | Style conflicts, 2× bundle size | **Never** add `antd` unless product mandates full migration |
| Theme routing import | Accidental guard bypass | RED classification; lint ban on `theme/routes` |
| Redux/Firebase from theme | Architecture fork | RED; no theme store imports |
| Auth/session replacement | Security regression | Keep `authStore` + `AuthProvider` immutable |
| `MenueItems.js` adoption | 100+ dead links, no permissions | Build `usePlatformNav` instead |
| Full codebase merge | 6–12 month rewrite | Explicitly rejected |

**Effort if triggered:** 3–6 months recovery

---

### Medium risk

| Area | Risk | Mitigation |
|------|------|------------|
| Layout refactor | Sidebar/header regressions on mobile | Visual regression checklist per breakpoint |
| Dark mode dual mechanisms | Inconsistent tokens | Single `ThemeModeService` |
| PlatformTable scope creep | Rebuilding Ant Design Table | Start with current HTML table wrapper |
| React 19 + future antd | If Strategy C chosen later | Spike before commitment |
| Module migration churn | Large PRs | One module per PR |
| HexaDash visual fidelity | Product expects pixel-perfect | Set expectations: CoreUI primitives + tokens |

**Effort if triggered:** 2–4 weeks per incident

---

### Low risk

| Area | Risk | Mitigation |
|------|------|------------|
| Token / color port | WCAG contrast failure | Token review against a11y |
| Font (Jost) addition | FOUT / layout shift | `font-display: swap` |
| Chart library addition | Bundle size | Lazy load `PlatformChart` |
| Responsive breakpoints | 1px off at 991/992 | Align to Bootstrap `lg` |
| Static assets (logo, bg) | Brand mismatch | Design approval |

**Effort if triggered:** 1–3 days per item

---

## Final recommendation

### Decision: **Use theme partially (Option 2)**

| Option | Verdict | Rationale |
|--------|---------|-----------|
| 1. Use theme fully | **Reject** | Requires Ant Design, Redux, CRA patterns; destroys guards, API, session model |
| 2. Use theme partially | **Accept** | Tokens + layout + dashboard wireframes via adapter |
| 3. Use only layout | **Subset of 2** | Minimum viable; may undershoot product visual expectations |
| 4. Reject specific sections | **Accept** | See table below |

### Section-level decisions

| Theme section | Decision | Reason |
|---------------|----------|--------|
| **Layout shell** (`withAdminLayout`) | **Partial** — visual port via adapter | Structure aligns; implementation must use CoreUI + tokens |
| **Sidebar** (`MenueItems.js`) | **Reject** | Demo menu, no permissions; platform builds nav |
| **Header** (`AuthInfo` demo widgets) | **Reject** demo widgets; **partial** layout | Messages/billing/notifications are fake |
| **Top menu mode** | **Reject** (initially) | Ashraak has no product requirement; adds complexity |
| **Routing** (`routes/admin/*`) | **Reject** | 100+ demo routes |
| **Auth pages** (visual) | **Partial** | Background, card layout only; keep RHF + JWT |
| **Auth** (`authSlice`, Firebase, Auth0) | **Reject** | Wrong security model |
| **Dashboard demos** | **Partial** | Layout reference for stat cards/charts |
| **Tables** (`DataTable.js`) | **Reject** | Redux-coupled; keep Query + PlatformTable |
| **Forms** (Ant Form demos) | **Reject** | Keep RHF + Zod |
| **Modals** (wrapper pattern) | **Partial** | `PlatformDialog` abstraction |
| **Notifications** (header) | **Reject** | Demo only |
| **Toast styling** | **Partial** | Restyle existing toast |
| **Charts** (one library) | **Partial** | Add when API ready; demos as reference |
| **Theme tokens** (`themeVariables.js`) | **Adopt** | Map to SCSS/CSS variables |
| **Redux store** | **Reject** | All slices |
| **API** (`DataService`) | **Reject** | Use `apiClient` |
| **i18n** | **Defer** | Not in Ashraak roadmap |
| **Ecommerce, chat, email, firebase CRUD** | **Reject** | Out of platform scope |
| **Static assets** (logo, auth bg) | **Adopt** | Branding decision |
| **Responsive patterns** | **Adopt** | Breakpoints align |
| **CRA / CRACO config** | **Reject** | Ashraak uses Vite |

### Recommended strategy: **B — Adapter layer + CoreUI primitives + HexaDash tokens**

This delivers HexaDash visual identity without:

- Changing routes or guards
- Replacing Zustand / TanStack Query
- Importing Ant Design
- Touching business modules until Platform UI is ready

### Success criteria

1. `VITE_THEME` switches shell appearance without module changes.
2. New ThemeForest purchase replaces only `theme/adapters/<name>/` + tokens.
3. Permission-aware navigation remains generated from `routeMap.ts`.
4. JWT session flow unchanged and security-reviewed.
5. Bundle size increase &lt; 50 KB gzipped (tokens + SCSS; no antd).

---

## Related documentation

| Document | Path |
|----------|------|
| Theme index | [README.md](./README.md) |
| Theme analysis | [theme-analysis.md](./theme-analysis.md) |
| Layout analysis | [layout-analysis.md](./layout-analysis.md) |
| Navigation analysis | [navigation-analysis.md](./navigation-analysis.md) |
| Component analysis | [component-analysis.md](./component-analysis.md) |
| Dependency analysis | [dependency-analysis.md](./dependency-analysis.md) |
| Migration recommendations | [migration-recommendations.md](./migration-recommendations.md) |
| Platform architecture | [../../architecture.md](../../architecture.md) |
| Routing and guards | [../../routing-and-guards.md](../../routing-and-guards.md) |
| CoreUI integration | [../../../../FrontEnd/COREUI_INTEGRATION.md](../../../../FrontEnd/COREUI_INTEGRATION.md) |

---

## Next document (future phase)

When implementation begins, create:

`docs/frontend/themes/current-theme/implementation-checklist.md`

— tracking T1–T6 tasks, PR links, and visual regression status. Not created in this analysis phase.
