# Theme Adapter Architecture

**Status:** T3 — Navigation Migration (implemented). T2 — Layout Migration (implemented). T1 — Theme Adapter Foundation (implemented).  
**Scope:** Architectural foundation + layout rendering + navigation ownership behind the adapter. No HexaDash visuals, no module/component migration, no route/guard/permission/auth behaviour changes.

This document describes the theme system introduced in T1, the layout migration completed in T2, the navigation migration completed in T3, and how the layers fit together.

---

## Goal

Make the UI **theme-replaceable**: a future theme (e.g. HexaDash, or any later purchase) can be plugged in by adding one adapter folder, without changing business modules, routing, guards, authentication, or state management.

---

## Layer model

```
┌──────────────────────────────────────────────────────────────┐
│ BUSINESS MODULES  (src/modules/*)                             │
│ auth, users, audit, apikeys, webhooks, tenant, dashboard      │
│ Import UI only from platform-ui.                              │
└───────────────────────────┬──────────────────────────────────┘
                            │ uses
┌───────────────────────────▼──────────────────────────────────┐
│ PLATFORM UI  (src/platform-ui/)                               │
│ Stable, theme-agnostic component API.                        │
│ Renders against theme contracts via useTheme().              │
└───────────────────────────┬──────────────────────────────────┘
                            │ resolves
┌───────────────────────────▼──────────────────────────────────┐
│ THEME SYSTEM  (src/theme/)                                    │
│ Contracts (interfaces) + ThemeProvider + registry + config.  │
└───────────────────────────┬──────────────────────────────────┘
                            │ implements
┌───────────────────────────▼──────────────────────────────────┐
│ THEME ADAPTERS  (src/theme/adapters/*)                        │
│ coreui/  — default, wraps the existing CoreUI implementation. │
│ <future>/ — added later (e.g. hexadash/).                     │
└──────────────────────────────────────────────────────────────┘
```

---

## What was created (T1)

### `src/theme/`

| File | Responsibility |
|------|----------------|
| `config.ts` | `ThemeId` union, `DEFAULT_THEME_ID`, `KNOWN_THEME_IDS`, `resolveThemeId()` (reads `VITE_THEME`) |
| `contracts/` | The interfaces every theme must implement (see below) |
| `ThemeContext.ts` | React context + `useTheme()` hook |
| `ThemeProvider.tsx` | Resolves the active adapter and provides it via context |
| `registry.ts` | Maps `ThemeId → ThemeAdapter`; `getThemeAdapter()` with default fallback |
| `adapters/coreui/` | The first adapter — wraps the current CoreUI UI |
| `index.ts` | Public theme API barrel |

### Contracts (`src/theme/contracts/`)

| Contract | Surface | Key props |
|----------|---------|-----------|
| `LayoutContract` | `Layout`, `AuthLayout` | `navGroups`, `user`, `onLogout`, `appName`; optional `isDarkMode`, `onToggleDarkMode` |
| `NavigationContract` | `Nav` | `groups: NavigationGroup[]` (resolved render model), `ariaLabel` |
| `CardContract` | `Card` | `title`, `actions`, `footer`, `children` |
| `DialogContract` | `Dialog` | `show`, `onClose`, `title`, `footer`, `size` |
| `TableContract` | `Table` | generic `columns`, `rows`, `rowKey`, `empty` |
| `NotificationContract` | `Viewport` | (reads global toast store) |

The aggregate `ThemeAdapter` interface composes all six contracts plus `id` and `label`.

### `src/platform-ui/`

| Folder | Components / hooks |
|--------|--------------------|
| `layout/` | `PlatformLayout`, `PlatformAuthLayout`, `PlatformHeader`, `PlatformSidebar`, `PlatformFooter` |
| `navigation/` | `models/` (config + visibility), `navigationConfig`, `usePlatformNav`, `useFeatureFlags`, `PlatformNavigationProvider`, `useNavigationModel`, `PlatformNavRenderer` |
| `tables/` | `PlatformTable`, `PlatformPagination` |
| `forms/` | `PlatformFormField` |
| `dialogs/` | `PlatformDialog`, `PlatformConfirmDialog` |
| `cards/` | `PlatformCard`, `PlatformPageHeader` |
| `charts/` | `PlatformChart` |
| `notifications/` | `PlatformToast`, `usePlatformNotify` |
| `index.ts` | Aggregated barrel |

---

## How resolution works

1. `ThemeProvider` (mounted in `core/providers/AppProviders.tsx`) calls `resolveThemeId()`.
2. `resolveThemeId()` reads `import.meta.env.VITE_THEME`; unknown/missing → `DEFAULT_THEME_ID` (`'coreui'`).
3. `getThemeAdapter(id)` returns the registered adapter (or the default).
4. The adapter is exposed via `ThemeContext`.
5. `platform-ui` components call `useTheme()` and render the adapter's component for that surface.

```tsx
// platform-ui/cards/PlatformCard.tsx
export function PlatformCard(props: PlatformCardProps) {
  const { adapter } = useTheme();
  const Card = adapter.card.Card;
  return <Card {...props} />;
}
```

---

## The CoreUI adapter (T2 behaviour)

After T2 the CoreUI adapter **owns the layout markup** directly (it no longer delegates to the legacy files):

- `CoreUiLayout` contains the full authenticated shell (fixed/overlay sidebar, narrow toggle + CSS var, sticky header, dark-mode toggle, footer). It consumes `PlatformLayoutProps` for `user`, `onLogout`, `appName`. Dark mode is self-managed inside the adapter (state + `data-coreui-theme`), matching legacy behaviour exactly (including cross-remount persistence).
- `CoreUiAuthLayout` contains the auth shell (gradient background, brand, centred card). It consumes `PlatformAuthLayoutProps` (`appName`, optional `children`; renders `<Outlet />` when `children` is omitted). The auth *gating* (loading spinner / redirect-when-authenticated) is now owned by the `PlatformAuthLayout` orchestrator, not the theme.
- `CoreUiNav`, `CoreUiCard`, `CoreUiDialog`, `CoreUiTable` mirror existing CoreUI/Bootstrap markup used across the app.
- `CoreUiNotificationViewport` wraps the existing `ToastContainer`.

> **T3 update:** the CoreUI sidebar no longer contains any menu definitions or `RoleGuard` / `PermissionGuard` usage. It renders a fully-resolved navigation model (`navGroups`) supplied by the platform, via `CoreUiNav` + the `CORE_UI_NAV_ICONS` key→glyph map. The adapter now contains **zero** business-navigation logic.

---

## Layout orchestration (T2)

```
Router → PlatformLayout      → adapter.layout.Layout     (CoreUiLayout)
Router → PlatformAuthLayout  → adapter.layout.AuthLayout (CoreUiAuthLayout)
```

- `PlatformLayout` (`platform-ui/layout/`) is a **thin orchestrator**: it reads `useAuth()` for `user` + `logout`, builds the logout handler (`logout()` + redirect to `ROUTES.login`), supplies `appName`, and renders the adapter's `Layout`. It contains no markup.
- `PlatformAuthLayout` orchestrates the **platform auth gating** (`status === 'loading'` → `<Spinner fullPage />`; `status === 'authenticated'` → `<Navigate to={ROUTES.dashboard} replace />`) and otherwise renders the adapter's `AuthLayout`.
- The legacy `layouts/AppLayout.tsx` / `layouts/AuthLayout.tsx` are now **thin compatibility shims** that simply render `PlatformLayout` / `PlatformAuthLayout`. New code should use `@/platform-ui` directly.

---

## Why behaviour is unchanged in T2

- The router still defines the **same routes, guards, and structure** — only the layout *element* changed (`<AppLayout/>` → `<PlatformLayout/>`, `<AuthLayout/>` → `<PlatformAuthLayout/>`).
- The shell markup, class names, CSS variables, icons, and nav links were moved verbatim into `CoreUiLayout` / `CoreUiAuthLayout`.
- Guards, auth store, API client, permissions, and modules were not touched.
- Dark-mode logic was kept in the adapter to preserve identical behaviour.

Net effect: the rendered DOM is the same; only the file that produces it moved behind the adapter boundary.

---

## Navigation ownership (T3)

Navigation is owned entirely by the platform; the theme only renders it.

```
Platform                                   Theme
────────                                   ─────
navigationConfig.ts   (authoring model)
   │  NavigationGroupConfig + NavigationVisibility (roles/permissions/flags)
   ▼
usePlatformNav()      (resolver — single source of truth)
   │  evaluates roles + permissions + feature flags
   ▼
PlatformNavigationProvider → useNavigationModel()
   │  resolved NavigationGroup[]  (visible booleans only, no rules)
   ▼
PlatformLayout  ──navGroups prop──►  CoreUiLayout → CoreUiNav   (renders only)
```

- **Authoring** (`platform-ui/navigation/models`, `navigationConfig.ts`): declares groups/items and their `visibility` rules. Theme-agnostic — icons are referenced by key.
- **Resolution** (`usePlatformNav`): the single source of truth. Applies role (`hasRole`, any-of), permission (`hasPermission`, all-of) and feature-flag (`isEnabled`, all-of) rules — **matching the previous guards exactly** — and emits a render model with `visible` booleans.
- **Distribution** (`PlatformNavigationProvider` / `useNavigationModel`): resolves once, shared by any surface.
- **Rendering** (theme `Nav`, e.g. `CoreUiNav`): receives `NavigationGroup[]`, renders content for `visible` groups/items, and draws a separator between adjacent groups. It sees **no** roles, permissions, routes-knowledge or rules.

The full group list (including not-visible groups) is passed to the theme so separator placement is byte-identical to the pre-T3 sidebar.

### Feature flags (prepared, no behaviour change)

`NavigationVisibility.featureFlags` + `useFeatureFlags()` are wired through the resolver. There is no front-end flag backend yet, so `isEnabled` returns `true` for every flag — navigation is unchanged. When a real flag source arrives, only `useFeatureFlags` changes.

---

## Configuration

| Variable | Values | Effect |
|----------|--------|--------|
| `VITE_THEME` | `coreui` (default) | Selects the active theme adapter |

Example (`.env.development`):

```
VITE_THEME=coreui
```

---

## Next phase

**T4 — Component Migration:** move shared UI primitives (tables, forms, dialogs, cards, charts) used by modules behind `platform-ui` + theme contracts, so module code no longer imports CoreUI/Bootstrap markup directly.

See [current-theme/layout-migration-report.md](./current-theme/layout-migration-report.md) for the T2 change log, [current-theme/navigation-migration-report.md](./current-theme/navigation-migration-report.md) for the T3 change log, [theme-replacement-guide.md](./theme-replacement-guide.md) for how to add a new theme, and [theme-governance.md](./theme-governance.md) for the layering rules.
