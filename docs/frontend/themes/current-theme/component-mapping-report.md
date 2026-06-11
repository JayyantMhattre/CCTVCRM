# T4 — Component Mapping Report

**Phase:** T4 — Theme Component Inventory & Mapping
**Status:** Analysis and mapping only — **no code migration, no theme integration, no component replacement**
**Theme:** HexaDash v1.3.0 (React / Ant Design variant)
**Platform:** Ashraak `@ashraak/web` — React 19, Vite 6, TypeScript strict, CoreUI 5
**Date:** June 2026

**Prerequisites:** T1 (adapter foundation), T2 (layout migration) and T3 (navigation ownership moved into the platform) are complete. The Theme Adapter Layer (`src/theme/`) and Platform UI layer (`src/platform-ui/`) already exist and are the contracts this report maps against. See [`platform-compatibility-report.md`](./platform-compatibility-report.md) for the 18-area compatibility matrix and the agreed adapter architecture.

---

## 1. Purpose

This report produces a **complete component inventory** of both sides and a **per-component migration mapping** so the team knows exactly what can be migrated safely **before** any HexaDash code is integrated.

It answers three questions per component:

1. What exists in the current Ashraak platform?
2. What is the HexaDash equivalent?
3. Can it migrate, and at what risk?

Companion documents:

| Document | Content |
|----------|---------|
| [`platform-ui-target-architecture.md`](./platform-ui-target-architecture.md) | The target abstraction layer (`Platform*` primitives) |
| [`component-migration-plan.md`](./component-migration-plan.md) | Migration waves 1–3, effort, and readiness gate |

---

## 2. Classification legend

### Migration classification (per component)

| Code | Meaning | Action |
|------|---------|--------|
| **GREEN** | Direct replacement possible — already abstracted behind a contract, or pure structural/SCSS work with no new dependency | Migrate first; low risk |
| **YELLOW** | Adapter required — must pass through a `platform-ui` primitive + theme contract before any HexaDash visual is applied | Migrate behind the adapter only |
| **RED** | Do not migrate — conflicts with platform architecture (Ant Design, Redux, demo auth/routing/state/API) or imports unmaintained/untyped code | Reference only; never import |

### Inventory status (per platform component)

| Status | Meaning |
|--------|---------|
| **Abstracted** | Already exposed via `platform-ui` + a theme contract (theme-swappable today) |
| **Shared** | Lives in `shared/components` — theme-neutral, not yet behind a contract |
| **Inline** | Hand-written CoreUI/Bootstrap markup inside module pages (not a component) |
| **Gap** | No platform component exists; HexaDash has one |

---

## 3. Current platform UI inventory

### 3.1 Platform UI layer — `src/platform-ui/` (theme-agnostic public API for modules)

| Folder | Component / export | Backed by contract? | Notes |
|--------|--------------------|---------------------|-------|
| `layout/` | `PlatformLayout` | `LayoutContract.Layout` | Authenticated shell (sidebar + header + outlet + footer) |
| `layout/` | `PlatformAuthLayout` | `LayoutContract.AuthLayout` | Unauthenticated shell |
| `layout/` | `PlatformHeader`, `PlatformSidebar`, `PlatformFooter` | (structural) | Composition helpers |
| `navigation/` | `usePlatformNav` | `NavigationContract` | Resolves `NAVIGATION_CONFIG` → `NavigationGroup[]` with `visible` flags |
| `navigation/` | `NAVIGATION_CONFIG`, `useFeatureFlags`, `PlatformNavigationProvider`, `PlatformNavRenderer`, `models/*` | `NavigationContract` | Platform owns menu + visibility; theme renders only |
| `tables/` | `PlatformTable<TRow>` | `TableContract` | Generic, type-safe; delegates to adapter |
| `tables/` | `PlatformPagination` | (structural) | Bootstrap `.pagination`; theme-neutral |
| `forms/` | `PlatformFormField` | (structural) | Label + hint + error wrapper; RHF-agnostic, owns no state |
| `dialogs/` | `PlatformDialog` | `DialogContract` | Delegates to adapter |
| `dialogs/` | `PlatformConfirmDialog` | `DialogContract` | Confirm/cancel built on `PlatformDialog` |
| `cards/` | `PlatformCard` | `CardContract` | Delegates to adapter |
| `cards/` | `PlatformPageHeader` | (structural) | Mirrors `shared/PageHeader` |
| `charts/` | `PlatformChart` | (structural — **no chart library shipped**) | Card frame + accessible region only |
| `notifications/` | `PlatformToast` | `NotificationContract` | Renders adapter viewport |
| `notifications/` | `usePlatformNotify` | (façade) | Forwards to `shared/ui/toast` `useToast` |

### 3.2 Theme adapter layer — `src/theme/` (the only place that touches CoreUI)

| Area | Contract | CoreUI adapter implementation |
|------|----------|-------------------------------|
| Layout | `LayoutContract` | `CoreUiLayout`, `CoreUiAuthLayout` |
| Navigation | `NavigationContract` | `CoreUiNav` (+ `navIcons` key→`CIcon` map) |
| Card | `CardContract` | `CoreUiCard` |
| Table | `TableContract` | `CoreUiTable<TRow>` |
| Dialog | `DialogContract` | `CoreUiDialog` |
| Notification | `NotificationContract` | `CoreUiNotificationViewport` |
| Engine | `ThemeProvider` + `ThemeContext` + `registry` + `config` | `ThemeId` union currently `'coreui'` only — **`hexadash` is not yet registered** |

### 3.3 Shared components — `src/shared/` (theme-neutral, not yet behind contracts)

| Component | Category | Status | Notes |
|-----------|----------|--------|-------|
| `PageHeader` | Page header | Shared | Duplicated by `PlatformPageHeader`; consolidate later |
| `EmptyState` | Empty state | Shared | Bootstrap Icons + centred copy |
| `AlertMessage` | Error/feedback | Shared | Uses `@coreui/icons` directly (CoreUI-coupled) |
| `Spinner` | Loader | Shared | `.spinner-border`; full-page + inline |
| `CorrelationIdCopy` | Error state | Shared | Surfaces API correlation id |
| `ui/toast/*` (`toastStore`, `useToast`, `ToastContainer`, `toast.types`) | Notifications | Shared | Platform-owned queue/service |
| `file-upload/FileUpload` | Input + progress | Shared | Contains a Bootstrap `progress-bar` |
| `guards/*` (`AuthGuard`, `RoleGuard`, `PermissionGuard`) | Access control | Shared | **Not themed** — never move to theme layer |
| `pages/NotFoundPage`, `pages/ForbiddenPage` | Error state | Shared | 404 / 403 |

> There is **no `common/` directory** in the web app; shared primitives live under `shared/`. CoreUI is consumed only inside `theme/adapters/coreui/*`, `shared/components/*` (AlertMessage/Spinner), `styles/coreui.scss`, and a few not-yet-migrated module pages.

### 3.4 Module-level UI (inline markup + a few local components)

| Module | UI surface | Category | Status |
|--------|-----------|----------|--------|
| `auth` | `LoginPage`, `RegisterPage`, `SessionsPage` | Forms (RHF+Zod), tables/lists | Inline |
| `dashboard` | `DashboardPage` | Cards/stat tiles (placeholder) | Inline |
| `tenant` | `TenantProfilePage`, `TenantSettingsPage` | Forms, cards | Inline |
| `users` | `UserListPage` (HTML table + initials avatar), `UserProfilePage`, `NotificationPreferencesPage` | Table, avatar, forms | Inline |
| `audit` | `AuditLogPage` | HTML table + filters + **inline pagination** + inline badges | Inline |
| `apikeys` | `ApiKeysListPage`, `ApiKeyDetailPage`, `KeyRevealModal` | Table, dialog (`react-bootstrap/Modal`) | Inline + local component |
| `webhooks` | overview/subscriptions/deliveries/dead-letters pages, `ConfirmDialog`, `SecretRevealModal`, `StatusBadge` | Tables, `list-group` lists, dialogs, inline badges | Inline + local components |

---

## 4. Component classification by category

The categories below match the T4 brief. For each: where it lives today, the HexaDash equivalent, and the classification.

| # | Category | Platform today | Status | HexaDash equivalent | Classification |
|---|----------|----------------|--------|---------------------|----------------|
| 1 | **Layout** | `PlatformLayout` → `CoreUiLayout` | Abstracted | `withAdminLayout` HOC (Ant `Layout`) | **YELLOW** |
| 2 | **Navigation** | `usePlatformNav` → `CoreUiNav` | Abstracted | `MenueItems.js` / `TopMenu.js` (Ant `Menu`) | **YELLOW** (renderer) / **RED** (menu files) |
| 3 | **Cards** | `PlatformCard` + inline `.card` | Abstracted + Inline | `components/cards` `Cards` frame | **YELLOW** |
| 4 | **Tables** | `PlatformTable` + inline `<table>` (audit, users) | Abstracted + Inline | Ant `Table` / `DataTable.js` (Redux) | **YELLOW** (PlatformTable) / **RED** (DataTable) |
| 5 | **Forms** | `PlatformFormField` + RHF + Zod | Abstracted + Inline | Ant `Form` demos | **YELLOW** (field chrome) / **RED** (Ant Form) |
| 6 | **Inputs** | Native Bootstrap `.form-control` / `.form-select` | Inline | Ant `Input`, `Select`, `Checkbox`, `Slider`, `Tags` | **YELLOW** |
| 7 | **Dropdowns** | None (native `<select>` only) | **Gap** | Ant `Dropdown` / `dropdown` component | **YELLOW** |
| 8 | **Date Pickers** | Native `<input type="date">` (audit filters) | Inline | Ant `DatePicker` (`components/datePicker`) | **RED** (Ant dep) / **YELLOW** (wrap native) |
| 9 | **Dialogs** | `PlatformDialog`/`PlatformConfirmDialog` + module `react-bootstrap/Modal` | Abstracted + Inline | `antd-modals.js` (Ant `Modal`) | **YELLOW** |
| 10 | **Notifications** | `shared/ui/toast` + `PlatformToast` | Abstracted | Ant `notification` + header demo dropdown | **YELLOW** (toast) / **RED** (header demo) |
| 11 | **Charts** | `PlatformChart` frame only — **no lib** | Abstracted (empty) | ApexCharts / Chart.js / Recharts / Google | **YELLOW** (one lib) / **RED** (all four) |
| 12 | **Badges** | Inline `<span class="badge">` (`StatusBadge`, `EVENT_BADGE`) | Inline | Ant `Tag` / `Badge` | **GREEN** |
| 13 | **Tabs** | None | **Gap** | Ant `Tabs` (`components/tabs`) | **YELLOW** (new wrapper) / **RED** (Ant) |
| 14 | **Avatars** | Inline initials (`avatar-sm`) in layout + `UserListPage` | Inline | Ant `Avatar` | **GREEN** |
| 15 | **Lists** | `list-group` inline (webhooks) | Inline | Ant `List` / custom | **GREEN** |
| 16 | **Pagination** | `PlatformPagination` + inline (audit) | Abstracted + Inline | Ant `Table` built-in pagination | **GREEN** |
| 17 | **Breadcrumbs** | None (header placeholder comment only) | **Gap** | `components/page-headers` (Ant `PageHeader` + breadcrumb) | **YELLOW** (new wrapper) |
| 18 | **Loaders** | `Spinner` | Shared | Ant `Spin` / `Skeleton` | **GREEN** |
| 19 | **Empty States** | `EmptyState` | Shared | custom / `Empty` | **GREEN** |
| 20 | **Error States** | `AlertMessage`, `CorrelationIdCopy`, `ForbiddenPage`, `NotFoundPage` | Shared | Ant `Alert` / `Result` | **GREEN** (structure) / **YELLOW** (`AlertMessage` is CoreUI-coupled) |

---

## 5. Detailed per-component mapping

For each item: **Current Component · HexaDash Equivalent · Migration Complexity · Risk · Recommendation**.

### 5.1 GREEN — direct replacement / safe abstraction

| Current Component | HexaDash Equivalent | Complexity | Risk | Recommendation |
|-------------------|---------------------|-----------|------|----------------|
| Inline `<span class="badge">`, `StatusBadge`, `DeliveryStatusBadge`, `EnabledBadge`, `EVENT_BADGE` | Ant `Tag` / `Badge` (visual reference) | Low | Low | Introduce `PlatformBadge` with a `variant` prop; replace inline spans. Pure structural + token work; **do not** import Ant `Tag`. |
| Inline initials avatar (`avatar-sm`, `rounded-circle`) | Ant `Avatar` (visual reference) | Low | Low | Introduce `PlatformAvatar` (initials + optional image). Reuse existing `.avatar-sm` SCSS. |
| `Spinner` | Ant `Spin` / `Skeleton` | Low | Low | Keep `Spinner`; optionally re-style via tokens. No HexaDash code needed. |
| `EmptyState` | Ant `Empty` / custom | Low | Low | Keep; restyle icon/spacing with tokens. |
| `PlatformPagination` + inline audit pager | Ant `Table` pagination | Low | Low | Standardise on `PlatformPagination`; replace the inline audit pager. |
| `list-group` lists (webhooks) | Ant `List` | Low | Low | Optional `PlatformList`; current `list-group` is fine — token styling only. |
| `NotFoundPage`, `ForbiddenPage`, `CorrelationIdCopy` | Ant `Result` | Low | Low | Keep structure; restyle via tokens. |

### 5.2 YELLOW — adapter required (already partly in place from T1–T3)

| Current Component | HexaDash Equivalent | Complexity | Risk | Recommendation |
|-------------------|---------------------|-----------|------|----------------|
| `PlatformLayout` → `CoreUiLayout` | `withAdminLayout` shell | Medium | Medium | Already behind `LayoutContract`. Add a `hexadash` adapter (CoreUI primitives + HexaDash tokens). Never import the Ant HOC. |
| `usePlatformNav` → `CoreUiNav` | `MenueItems.js` renderer | Medium | Medium | Renderer is swappable; **menu data stays platform-owned**. Add `HexadashNav` rendering the same `NavigationGroup[]`. |
| `PlatformCard` + inline `.card` | `Cards` frame | Medium | Low | Migrate inline `.card` usages in module pages onto `PlatformCard`; add `hexadash` card adapter. |
| `PlatformTable` + inline `<table>` (audit, users) | Ant `Table` | Medium | Medium | Migrate inline tables onto `PlatformTable`. Keep TanStack Query as the data source; reject `DataTable.js`. |
| `PlatformFormField` + native inputs | Ant `Form` field chrome | Medium | Medium | Keep RHF + Zod. Wrap native inputs as `PlatformInput`/`PlatformSelect`; apply HexaDash input tokens only. |
| Native `<select>` (filters) | Ant `Dropdown`/`Select` | Medium | Medium | New `PlatformSelect`/`PlatformDropdown` over native controls; HexaDash styling via tokens. |
| `PlatformDialog`/`PlatformConfirmDialog` + module `react-bootstrap/Modal` | `antd-modals.js` | Medium | Medium | Migrate `KeyRevealModal`, `SecretRevealModal`, `ConfirmDialog` onto `PlatformDialog`. Add `hexadash` dialog adapter. |
| `shared/ui/toast` + `PlatformToast` | Ant `notification` | Medium | Low | Keep the toast queue/service; add a themed `Viewport`. Restyle only. |
| `PlatformChart` (empty) | ApexCharts / Recharts | Medium | Medium | Add **one** library (Recharts preferred for React 19) behind `PlatformChart` when a stats API exists. HexaDash demos are composition references. |
| `AlertMessage` (CoreUI-icon-coupled) | Ant `Alert` | Medium | Low | Re-route through `platform-ui/notifications` (`PlatformAlert`) so the `@coreui/icons` dependency is isolated in the adapter. |
| Breadcrumb (gap) | `components/page-headers` | Medium | Low | New `PlatformBreadcrumb`; feed from `routeMap`. No HexaDash import. |
| Tabs (gap) | Ant `Tabs` | Medium | Medium | New `PlatformTabs` over Bootstrap `nav-tabs`; **do not** import Ant `Tabs`. |
| `<input type="date">` (audit) | Ant `DatePicker` | Medium | Medium | Wrap native date input as `PlatformDateField`; adopting Ant `DatePicker` would pull in Ant — rejected. |

### 5.3 RED — do not migrate

| Theme item | Reason | Recommendation |
|------------|--------|----------------|
| `MenueItems.js` / `TopMenu.js` | 1,400+ lines of demo routes, no permissions | Reference only. Navigation stays in `NAVIGATION_CONFIG` + `usePlatformNav`. |
| `DataTable.js` + `data-filter` Redux slice | DOM-coupled (`document.querySelector`), Redux-bound | Reject. Keep TanStack Query + `PlatformTable`. |
| Ant `Form` demo containers | Replaces RHF + Zod everywhere | Reject. Borrow input tokens only. |
| `authSlice`, Firebase, Auth0, `SignIn.js` | Wrong auth/security model | Reject. Keep JWT + `authStore` + `AuthProvider`. |
| Header notification/message demo dropdowns | Fake demo data | Reject unless a real product feature is specced. |
| All four chart libraries together | Bundle bloat | Reject; pick one. |
| Redux `store.js` + 25+ slices, `react-redux-firebase` | Architecture fork | Reject. Keep Zustand + TanStack Query. |
| `DataService` API wrapper, mock endpoints | No tenant scope / refresh / typed contracts | Reject. Keep `apiClient` + `ENDPOINTS`. |
| `routes/admin/*` (100+ demo routes) | Demo-centric, auth-binary | Reject. Keep `core/router` + guards. |
| Ecommerce, chat, email, calendar, kanban, maps, editor containers | Out of platform scope | Reject. |
| CRA / CRACO build config, i18next wiring | Ashraak uses Vite; i18n deferred | Reject for now. |
| Any HexaDash `.js` component imported verbatim | Untyped JavaScript vs TS strict | Reject. Re-implement typed or use as CSS reference only. |

---

## 6. Special analysis — theme-specific surfaces (all RED)

Per the brief, every theme-specific application surface is marked **RED — do not adopt**. These are the parts of HexaDash that look like an app but are demo scaffolding.

| Theme-specific surface | Examples in HexaDash | Classification | Reason |
|------------------------|----------------------|----------------|--------|
| **Business pages** | ecommerce, CRM, project, invoice, profile, chat, email, calendar, kanban | **RED** | No relation to Ashraak domains (tenant/users/audit/apikeys/webhooks); Redux + demo data. |
| **Auth pages (logic)** | `SignIn.js`, `Signup.js`, `ForgotPassword.js`, Firebase/Auth0 variants | **RED** | Wrong session model; Ashraak uses JWT + tenant + RHF/Zod. Only background/card **visuals** may be referenced (YELLOW visual). |
| **Routing** | `routes/admin/*`, `ProtectedRoute` (login-boolean) | **RED** | Bypasses Role/Permission/module guards. Keep `core/router`. |
| **State management** | Redux Toolkit store, 25+ slices, redux-thunk, `react-redux-firebase` | **RED** | Conflicts with Zustand + TanStack Query. |
| **APIs** | `config/dataService/dataService.js`, mock `/login` `/register` | **RED** | No typed endpoints, tenant scope, or silent refresh. Keep `apiClient`. |

**Rule:** these surfaces may be opened as **visual references** during implementation, but **no file from them is ever imported** into Ashraak.

---

## 7. Mapping summary

| Classification | Count | Categories |
|----------------|-------|-----------|
| **GREEN** (migrate first) | 7 | Badges, Avatars, Loaders, Empty states, Pagination, Lists, Error pages |
| **YELLOW** (adapter required) | 13 | Layout, Navigation renderer, Cards, Tables, Forms, Inputs, Dropdowns, Dialogs, Toast, Charts, AlertMessage, Breadcrumbs, Tabs, Date fields |
| **RED** (never migrate) | 12+ | Menu files, DataTable, Ant Form, auth logic, header demos, 4× charts, Redux, DataService, demo routes, demo business pages, CRA config, verbatim JS |

### Gaps (platform has no component; needs a new `Platform*` primitive)

| Gap | Target primitive | Wave |
|-----|------------------|------|
| Badge | `PlatformBadge` | 1 |
| Avatar | `PlatformAvatar` | 1 |
| Breadcrumb | `PlatformBreadcrumb` | 2 |
| Tabs | `PlatformTabs` | 2 |
| Dropdown / Select | `PlatformSelect` / `PlatformDropdown` | 2 |
| Chart backend | `PlatformChart` + one library | 3 |
| Date field | `PlatformDateField` | 3 |

---

## 8. Final recommendation (this phase)

1. **Migrate first (GREEN):** badges, avatars, pagination, loaders, empty/error states — they are pure structural/token work and several are already abstracted. Introduce `PlatformBadge` and `PlatformAvatar` to remove inline markup.
2. **Migrate behind the adapter (YELLOW):** layout, navigation rendering, cards, tables, dialogs, forms/inputs, toast — all already have (or trivially get) a contract from T1–T3. HexaDash contributes **tokens + visual reference only**.
3. **Never migrate (RED):** HexaDash routing, state, auth logic, API layer, demo business/auth pages, `MenueItems.js`, `DataTable.js`, Ant Form, and all-four chart libraries.
4. **Effort:** see [`component-migration-plan.md`](./component-migration-plan.md) — roughly **16–25 developer-days** across three waves for the component layer (excludes the already-completed T1–T3 shell/nav work).
5. **Readiness for HexaDash adapter implementation:** the contracts (`LayoutContract`, `NavigationContract`, `CardContract`, `TableContract`, `DialogContract`, `NotificationContract`) are in place and the CoreUI adapter proves them. The platform is **ready** to add a `hexadash` adapter once: (a) the GREEN/YELLOW `Platform*` primitives and gaps are filled, and (b) HexaDash design tokens are extracted to `theme/tokens/hexadash.tokens.scss`.

> **No code was migrated, no theme was integrated, and no component was replaced in this phase.** This document and its two companions define *what* to migrate and *in what order*; implementation is a later phase.
