# T4 — Platform UI Target Architecture

**Phase:** T4 — Theme Component Inventory & Mapping (analysis only)
**Scope:** Defines the **final abstraction layer** (`Platform*` primitives) that every business module will consume, so that the active theme — CoreUI today, HexaDash later, any ThemeForest purchase in future — is fully replaceable.
**Status:** Target design. The gaps identified here were **implemented in T5** — see [`platform-ui-completion-report.md`](./platform-ui-completion-report.md). Sections updated to reflect current state.

**Builds on:** T1 (adapter foundation), T2 (layout), T3 (navigation), and the agreed layering in [`platform-compatibility-report.md`](./platform-compatibility-report.md). Per-component classification is in [`component-mapping-report.md`](./component-mapping-report.md).

> **T5 update:** all five "Gap" primitives below (`PlatformBadge`, `PlatformAvatar`, `PlatformTabs`/`PlatformTab`, `PlatformBreadcrumb`/`PlatformBreadcrumbItem`) and the `ChartContract` foundation now exist, each backed by a theme contract and a CoreUI adapter. The `ThemeAdapter` interface now has 11 surfaces and the CoreUI adapter implements all of them.

---

## 1. Layering (recap)

```
┌──────────────────────────────────────────────────────────────┐
│  BUSINESS MODULES  (src/modules/*)                            │
│  auth · dashboard · tenant · users · audit · apikeys · webhooks│
│  Own: api.ts, types.ts, pages, hooks, Zod schemas, Query keys  │
│  Import UI ONLY from `@/platform-ui`                           │
└───────────────────────────────┬──────────────────────────────┘
                                 │ consumes (stable, typed props)
┌───────────────────────────────▼──────────────────────────────┐
│  PLATFORM UI  (src/platform-ui/*)        ← THIS DOCUMENT       │
│  Theme-agnostic `Platform*` primitives. Renders against        │
│  theme contracts; never against a concrete theme.              │
└───────────────────────────────┬──────────────────────────────┘
                                 │ delegates to active adapter
┌───────────────────────────────▼──────────────────────────────┐
│  THEME ADAPTER LAYER  (src/theme/*)                           │
│  Contracts (interfaces) + ThemeProvider + registry + config.   │
│  Active theme chosen by `VITE_THEME` (default `coreui`).       │
└───────────────────────────────┬──────────────────────────────┘
                                 │ implements
┌───────────────────────────────▼──────────────────────────────┐
│  THEME IMPLEMENTATIONS  (src/theme/adapters/*)                │
│  coreui/   — current CoreUI 5 (default, baseline) ✅ exists     │
│  hexadash/ — HexaDash visual port (CoreUI primitives + tokens) │
│  future/   — next ThemeForest purchase                         │
└──────────────────────────────────────────────────────────────┘
```

**Inviolable boundary:** modules never import `@coreui/*`, `antd`, `react-bootstrap`, or `@/theme/adapters/*`. They import `@/platform-ui` only.

---

## 2. The final abstraction layer

The brief names ten target primitives. The table records each one's contract, current state, and the gap to close. "Exists" means a `platform-ui` component is already present; "Contract" means a `theme/contracts` surface backs it.

| # | Target primitive | Purpose | Contract | Today | Remaining work |
|---|------------------|---------|----------|-------|----------------|
| 1 | **PlatformCard** | Content panel (title, actions, footer, body) | `CardContract` ✅ | Exists (`cards/PlatformCard`) | Migrate inline `.card` usages in modules |
| 2 | **PlatformTable** | Generic, type-safe data table | `TableContract` ✅ | Exists (`tables/PlatformTable<TRow>`) | Migrate inline `<table>` (audit, users); add sort/empty conventions |
| 3 | **PlatformDialog** | Modal / confirm dialog | `DialogContract` ✅ | Exists (`dialogs/PlatformDialog`, `PlatformConfirmDialog`) | Migrate `react-bootstrap/Modal` usages (apikeys, webhooks) |
| 4 | **PlatformForm** | Field chrome over RHF + Zod | _structural_ | Partial (`forms/PlatformFormField`) | Add `PlatformInput`, `PlatformSelect`, `PlatformTextarea`, `PlatformCheckbox` |
| 5 | **PlatformChart** | Chart container (neutral model) | `ChartContract` ✅ **(T5)** | Exists (`charts/PlatformChart` → adapter) | Add a concrete backend (e.g. Recharts) behind the adapter — deferred |
| 6 | **PlatformNotification** | Toast viewport + alert | `NotificationContract` ✅ | Exists (`PlatformToast`, `usePlatformNotify`) | Add `PlatformAlert` (move `AlertMessage` behind it) |
| 7 | **PlatformBadge** | Status / category pill | `BadgeContract` ✅ **(T5)** | Exists (`badges/PlatformBadge`) | Migrate inline badge spans (later phase) |
| 8 | **PlatformAvatar** | User avatar (initials/image) | `AvatarContract` ✅ **(T5)** | Exists (`avatar/PlatformAvatar`) | Migrate inline initials in layout/users (later phase) |
| 9 | **PlatformTabs** | Tabbed content | `TabsContract` ✅ **(T5)** | Exists (`tabs/PlatformTabs`, `PlatformTab`) | Adopt on detail pages (later phase) |
| 10 | **PlatformBreadcrumb** | Page breadcrumb trail | `BreadcrumbContract` ✅ **(T5)** | Exists (`breadcrumbs/PlatformBreadcrumb`, `PlatformBreadcrumbItem`) | Wire into header/pages (later phase) |

Supporting primitives already present that round out the layer: `PlatformLayout`, `PlatformAuthLayout`, `PlatformHeader`, `PlatformSidebar`, `PlatformFooter`, `PlatformPagination`, and the navigation set (`usePlatformNav`, `PlatformNavRenderer`, `PlatformNavigationProvider`).

---

## 3. Target folder layout (`src/platform-ui/`)

Items marked ✅ exist today; ➕ are new in this target.

```
src/platform-ui/
├── index.ts                       ✅ barrel
├── layout/                        ✅ PlatformLayout, PlatformAuthLayout, PlatformHeader,
│                                       PlatformSidebar, PlatformFooter
├── navigation/                    ✅ usePlatformNav, NAVIGATION_CONFIG, useFeatureFlags,
│                                       PlatformNavRenderer, PlatformNavigationProvider, models/
├── cards/
│   ├── PlatformCard.tsx           ✅
│   ├── PlatformPageHeader.tsx     ✅
│   └── PlatformStatCard.tsx       ➕ (dashboard tiles)
├── tables/
│   ├── PlatformTable.tsx          ✅
│   └── PlatformPagination.tsx     ✅
├── forms/
│   ├── PlatformFormField.tsx      ✅
│   ├── PlatformInput.tsx          ➕
│   ├── PlatformSelect.tsx         ➕ (also covers Dropdowns)
│   ├── PlatformTextarea.tsx       ➕
│   ├── PlatformCheckbox.tsx       ➕
│   └── PlatformDateField.tsx      ➕ (wraps native <input type="date">)
├── dialogs/
│   ├── PlatformDialog.tsx         ✅
│   └── PlatformConfirmDialog.tsx  ✅
├── charts/
│   ├── PlatformChart.tsx          ✅ (T5: delegates to adapter.chart.Chart)
│   └── chart.types.ts             ✅ (T5: ChartType/Series/Data/Options)
├── notifications/
│   ├── PlatformToast.tsx          ✅
│   ├── usePlatformNotify.ts       ✅
│   └── PlatformAlert.tsx          ➕ (replaces direct AlertMessage)
├── badges/
│   └── PlatformBadge.tsx          ✅ (T5)
├── avatar/
│   └── PlatformAvatar.tsx         ✅ (T5)
├── tabs/
│   ├── PlatformTabs.tsx           ✅ (T5)
│   └── PlatformTab.tsx            ✅ (T5)
├── breadcrumbs/
│   ├── PlatformBreadcrumb.tsx     ✅ (T5)
│   └── PlatformBreadcrumbItem.tsx ✅ (T5)
└── feedback/
    ├── PlatformSpinner.tsx        ➕ (wraps shared/Spinner)
    └── PlatformEmptyState.tsx     ➕ (wraps shared/EmptyState)
```

New theme contracts added in **T5** (`src/theme/contracts/`): `BadgeContract`, `AvatarContract`, `TabsContract`, `BreadcrumbContract`, `ChartContract`, plus the extension of `ThemeAdapter` in `contracts/index.ts`. Each is implemented by `theme/adapters/coreui/*` (parity), and will be implemented by `theme/adapters/hexadash/*` in T6.

---

## 4. Contract design principles

These rules keep the layer theme-replaceable. They are derived from the existing T1–T3 contracts and must hold for every new primitive.

1. **Props are theme-neutral.** No `className` that encodes a vendor (`bg-success` belongs in the adapter, not the module). Expose semantic props (`variant: 'success' | 'danger' | …`) instead.
2. **Platform decides, theme renders.** Visibility, permissions, data, and business rules are resolved in the platform (as `usePlatformNav` does); the adapter receives only resolved primitives.
3. **State stays out of the theme.** Form state (RHF + Zod), server state (TanStack Query), auth state (Zustand), and the toast queue remain platform-owned. Themes render presentation only.
4. **Generics preserved.** `PlatformTable<TRow>` and any future generic primitive call the adapter component directly to keep the row-type generic intact.
5. **Optional self-management.** As with dark mode in `LayoutContract`, a contract may let a theme manage a concern internally; the prop is then optional.
6. **One service, many viewports.** Cross-cutting services (notifications) live once in `shared/`; only the viewport is themed (`NotificationContract`).

### Example: new `BadgeContract` (target shape)

```typescript
// theme/contracts/BadgeContract.ts
import type { ComponentType, ReactNode } from 'react';

export type PlatformBadgeVariant =
  | 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' | 'dark';

export interface PlatformBadgeProps {
  variant?: PlatformBadgeVariant;
  children: ReactNode;
  className?: string;
}

export type PlatformBadgeComponent = ComponentType<PlatformBadgeProps>;

export interface BadgeContract {
  Badge: PlatformBadgeComponent;
}
```

The CoreUI adapter maps `variant` → `badge bg-{variant}` (matching today's `StatusBadge`); a future HexaDash adapter maps the same `variant` to its own tokens. Modules write `<PlatformBadge variant="success">Enabled</PlatformBadge>` and never see a CSS class.

---

## 5. Import rules (target; to be enforced by lint)

| From → To | Allowed |
|-----------|---------|
| `modules/*` → `@/platform-ui` | ✅ Yes |
| `modules/*` → `@/theme/*` | ❌ No |
| `modules/*` → `@coreui/*`, `antd`, `react-bootstrap` | ❌ No (after migration) |
| `modules/*` → `@/shared/*` (guards, hooks, types) | ✅ Yes (non-themed) |
| `platform-ui/*` → `@/theme` (contracts, `useTheme`) | ✅ Yes |
| `platform-ui/*` → `@/theme/adapters/*` | ❌ No (use the registry via `useTheme`) |
| `theme/adapters/*` → `@coreui/*`, `antd`, vendor CSS | ✅ Yes (isolated here only) |
| `core/*` → `platform-ui` | ❌ No |

Planned enforcement: ESLint `no-restricted-imports` rules mirroring this table.

---

## 6. Theme replacement procedure (target outcome)

To skin the app with HexaDash (or any future theme):

1. Add `theme/adapters/hexadash/` implementing every contract in `ThemeAdapter` (Layout, Navigation, Card, Table, Dialog, Notification, **Badge, Avatar, Tabs, Breadcrumb, Chart**).
2. Add `theme/tokens/hexadash.tokens.scss` (colors, radius, spacing, `Jost` font) — extracted from HexaDash `themeVariables.js`.
3. Extend the `ThemeId` union to `'coreui' | 'hexadash'` and add to `KNOWN_THEME_IDS` (`theme/config.ts`).
4. Register the adapter in `theme/registry.ts` (one line).
5. Set `VITE_THEME=hexadash`.

**Zero changes** to `modules/`, `core/`, `shared/guards/`, or any `Platform*` prop. That is the success test for this architecture.

---

## 7. Definition of done for the abstraction layer

The Platform UI layer is "complete" when:

1. Every category in [`component-mapping-report.md`](./component-mapping-report.md) §4 maps to a `Platform*` primitive (no `Gap` rows remain). — ✅ **Done in T5**
2. A grep of `src/modules/` for `@coreui`, `antd`, `react-bootstrap`, and raw `className="card"`/`"table"`/`"badge"` returns **zero** (excluding tests). — ⏳ Pending module migration (later phase)
3. Each contract has a CoreUI adapter at parity (no visual regression). — ✅ **Done in T5** (11/11 surfaces; `tsc` passes)
4. The `ThemeAdapter` interface in `theme/contracts/index.ts` lists all surfaces, and `registry.ts` resolves them for `coreui`. — ✅ **Done in T5**
5. Adding the `hexadash` adapter requires only the five steps in §6. — ✅ **Ready for T6**

> **T4** defined this target (analysis only). **T5** implemented the missing primitives and contracts (see [`platform-ui-completion-report.md`](./platform-ui-completion-report.md)). Module migration (criterion 2) remains a later phase and was intentionally not done.
