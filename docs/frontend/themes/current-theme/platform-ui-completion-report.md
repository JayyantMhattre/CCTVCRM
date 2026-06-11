# T5 — Platform UI Completion Report

**Phase:** T5 — Platform UI Completion
**Status:** Complete — abstraction layer only. **No HexaDash, no Ant Design, no visual changes, no module migration, no route/permission/auth changes.**
**Platform:** Ashraak `@ashraak/web` — React 19, Vite 6, TypeScript strict, CoreUI 5
**Date:** June 2026

**Builds on:** T1 (adapter foundation), T2 (layout), T3 (navigation), T4 (component inventory & mapping). T4 identified the missing abstractions; this phase creates them. See [`component-mapping-report.md`](./component-mapping-report.md) and [`platform-ui-target-architecture.md`](./platform-ui-target-architecture.md).

---

## 1. Objective

Complete the Platform UI abstraction layer so that **every component category** required by CoreUI (today), HexaDash (later), and any future theme exists behind `platform-ui`, backed by a theme contract and a working CoreUI adapter. After this phase the platform is **theme-ready** — a `hexadash` adapter can be authored without touching modules.

---

## 2. New abstractions

### 2.1 Platform UI primitives (`src/platform-ui/`)

| Folder | Component(s) | Delegates to | Notes |
|--------|--------------|--------------|-------|
| `badges/` | `PlatformBadge` | `adapter.badge.Badge` | Semantic `variant` + `pill`; no vendor class in modules |
| `avatar/` | `PlatformAvatar` | `adapter.avatar.Avatar` | `name` → initials, optional `src`, `size` |
| `tabs/` | `PlatformTabs`, `PlatformTab` | `adapter.tabs.Tabs` | Declarative `<PlatformTab>` children; platform owns active state (controlled/uncontrolled) |
| `breadcrumbs/` | `PlatformBreadcrumb`, `PlatformBreadcrumbItem` | `adapter.breadcrumb.Breadcrumb` | Declarative children **or** `items` prop; entry without `to` = current page |
| `charts/` | `PlatformChart` (reworked) + `chart.types.ts` | `adapter.chart.Chart` | Now delegates to the adapter; neutral data model |

All primitives follow the established delegation pattern (identical to `PlatformCard`/`PlatformDialog`): read `useTheme()`, resolve the contract component, forward props. Modules import only from `@/platform-ui`.

### 2.2 Declarative descriptors

`PlatformTab` and `PlatformBreadcrumbItem` render nothing themselves — they are typed descriptors whose props are collected by their parent into a plain model handed to the adapter (the React Router `<Route>` pattern). This keeps the JSX ergonomic for modules while the theme receives a flat, presentational model.

### 2.3 Chart foundation (abstraction only)

`ChartContract` defines a **library-agnostic** model — **no ApexCharts, Chart.js, Recharts, or Ant Design is bound**:

| Type | Purpose |
|------|---------|
| `ChartType` | `'line' \| 'bar' \| 'area' \| 'pie' \| 'donut' \| 'scatter' \| 'radar'` |
| `ChartSeries` | `{ name, data: number[], color? }` |
| `ChartData` | `{ categories, series }` |
| `ChartOptions` | `{ height?, showLegend?, showGrid?, stacked?, colors? }` |
| `PlatformChartProps` | `type`, `data`, `options?`, `title?`, `ariaLabel?`, `children?` |

The CoreUI adapter (`CoreUiChart`) renders the themed frame and either a caller-supplied chart element (`children`) or a neutral "No chart backend configured." placeholder. Choosing a backend is a later, deliberate decision (T6+).

---

## 3. Theme contract coverage

`ThemeAdapter` (`src/theme/contracts/index.ts`) now lists **eleven** surfaces. New in T5 are the last five:

| # | Contract | Surface | Status |
|---|----------|---------|--------|
| 1 | `LayoutContract` | `Layout`, `AuthLayout` | T2 |
| 2 | `NavigationContract` | `Nav` | T3 |
| 3 | `CardContract` | `Card` | T1 |
| 4 | `DialogContract` | `Dialog` | T1 |
| 5 | `TableContract` | `Table` | T1 |
| 6 | `NotificationContract` | `Viewport` | T1 |
| 7 | **`BadgeContract`** | `Badge` | **T5** |
| 8 | **`AvatarContract`** | `Avatar` | **T5** |
| 9 | **`TabsContract`** | `Tabs` | **T5** |
| 10 | **`BreadcrumbContract`** | `Breadcrumb` | **T5** |
| 11 | **`ChartContract`** | `Chart` | **T5** |

All contract types are re-exported from `theme/contracts/index.ts` and `theme/index.ts`, so `platform-ui` (and only `platform-ui`) consumes them via `@/theme`.

---

## 4. CoreUI adapter coverage

`coreUiAdapter` (`src/theme/adapters/coreui/index.ts`) now implements **every** contract surface. New CoreUI implementations:

| File | Renders | Parity reference |
|------|---------|------------------|
| `CoreUiBadge.tsx` | `badge bg-{variant}` (+`text-dark` for light/warning/info, optional `rounded-pill`) | Existing `StatusBadge`, audit `EVENT_BADGE` |
| `CoreUiAvatar.tsx` | Circular initials (or `<img>`), `sm`/`md`/`lg` | Existing header/sidebar `.avatar-sm` |
| `CoreUiTabs.tsx` | Accessible `.nav-tabs` strip + `.tab-content` (ARIA `tablist`/`tab`/`tabpanel`) | New (no prior tabs in platform) |
| `CoreUiBreadcrumb.tsx` | `.breadcrumb` with router `Link`s; active = `aria-current="page"` | New (header had a placeholder only) |
| `CoreUiChart.tsx` | Card-style `figure` frame + placeholder/children | Mirrors the prior `PlatformChart` frame |

Each adapter is built on **CoreUI/Bootstrap 5 markup or `react-bootstrap`/`react-router-dom`** only — the same primitives already used elsewhere in the codebase. No new runtime dependency was added.

---

## 5. Files changed

### Added (theme contracts)
- `theme/contracts/BadgeContract.ts`
- `theme/contracts/AvatarContract.ts`
- `theme/contracts/TabsContract.ts`
- `theme/contracts/BreadcrumbContract.ts`
- `theme/contracts/ChartContract.ts`

### Added (CoreUI adapters)
- `theme/adapters/coreui/CoreUiBadge.tsx`
- `theme/adapters/coreui/CoreUiAvatar.tsx`
- `theme/adapters/coreui/CoreUiTabs.tsx`
- `theme/adapters/coreui/CoreUiBreadcrumb.tsx`
- `theme/adapters/coreui/CoreUiChart.tsx`

### Added (platform-ui primitives)
- `platform-ui/badges/PlatformBadge.tsx` + `index.ts`
- `platform-ui/avatar/PlatformAvatar.tsx` + `index.ts`
- `platform-ui/tabs/PlatformTabs.tsx`, `PlatformTab.tsx` + `index.ts`
- `platform-ui/breadcrumbs/PlatformBreadcrumb.tsx`, `PlatformBreadcrumbItem.tsx` + `index.ts`
- `platform-ui/charts/chart.types.ts`

### Modified
- `theme/contracts/index.ts` — `ThemeAdapter` extended with `badge`, `avatar`, `tabs`, `breadcrumb`, `chart`; type re-exports added
- `theme/index.ts` — new contract/type re-exports
- `theme/adapters/coreui/index.ts` — five surfaces registered
- `platform-ui/charts/PlatformChart.tsx` — reworked to delegate to `adapter.chart.Chart`
- `platform-ui/charts/index.ts` — exports chart types
- `platform-ui/index.ts` — unified exports for the four new folders

---

## 6. Quality verification

| Check | Result |
|-------|--------|
| `tsc --noEmit` (strict) | ✅ Passes |
| IDE linter (new/edited files) | ✅ No errors |
| HexaDash imported? | ❌ No |
| Ant Design imported? | ❌ No |
| New runtime dependency added? | ❌ No (CoreUI/Bootstrap/`react-bootstrap`/`react-router-dom` only) |
| Visual changes? | ❌ None — primitives are not yet consumed by modules; `AppLayout`/pages unchanged |
| Module migration? | ❌ None |
| Route / permission / auth changes? | ❌ None |

> The new primitives are **available but unused** — no module was migrated onto them (that is a later phase). Existing UI renders exactly as before.

---

## 7. Success criteria

| Criterion | Status |
|-----------|--------|
| All component categories identified in T4 exist in Platform UI | ✅ Badge, Avatar, Tabs, Breadcrumb, Chart added; no `Gap` rows remain |
| Theme contracts complete | ✅ 11 contracts in `ThemeAdapter` |
| CoreUI adapter implements all contracts | ✅ `coreUiAdapter` satisfies every surface; `tsc` proves it |
| Platform becomes theme-ready | ✅ A new adapter need only implement the 11 surfaces + tokens |
| Ready for HexaDash adapter implementation | ✅ See §8 |

---

## 8. Readiness for T6 (HexaDash adapter)

The platform is now **architecturally and feature complete** at the abstraction layer. To add HexaDash in T6:

1. Create `theme/adapters/hexadash/` implementing all 11 `ThemeAdapter` surfaces (CoreUI primitives styled with HexaDash tokens — **no Ant Design**).
2. Add `theme/tokens/hexadash.tokens.scss` (colors, radius, spacing, `Jost`) extracted from HexaDash `themeVariables.js`.
3. Extend `ThemeId` to `'coreui' | 'hexadash'` (`theme/config.ts`) and add to `KNOWN_THEME_IDS`.
4. Register the adapter in `theme/registry.ts` (one line).
5. Set `VITE_THEME=hexadash`.

**Still deliberately deferred (not blockers):**
- Choosing a concrete chart backend (the `ChartContract` is ready; the library is a separate decision).
- Migrating module pages onto the new primitives (replacing inline badges/avatars/tables) — a later phase; out of scope here.
- ESLint import-boundary rules enforcing `modules → platform-ui` only.

**Outcome:** Platform UI is complete, theme contracts are complete, the CoreUI adapter implements every contract, and the platform is ready for the HexaDash adapter in T6.
