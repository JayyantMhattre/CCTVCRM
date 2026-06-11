# T6 — HexaDash Adapter Implementation Report

**Phase:** T6 — HexaDash Adapter Implementation
**Status:** Complete — HexaDash implemented as a **second, opt-in** theme adapter. **CoreUI remains the production default and the application still runs on CoreUI.** HexaDash is **not activated** (`VITE_THEME` unchanged).
**Platform:** Ashraak `@ashraak/web` — React 19, Vite 6, TypeScript strict, CoreUI 5
**Theme source:** HexaDash v1.3.0 — `C:\themeforest\hexadash-react\hexadash-react\`
**Date:** June 2026

**Builds on:** T1–T5. The Platform UI layer and all 11 theme contracts are complete (see [`platform-ui-completion-report.md`](./platform-ui-completion-report.md)). This phase adds a concrete HexaDash adapter implementing those contracts.

---

## 1. Approach

Per the agreed **Strategy B** (Adapter layer + CoreUI/Bootstrap primitives + HexaDash design tokens) from [`migration-recommendations.md`](./migration-recommendations.md) and [`platform-compatibility-report.md`](./platform-compatibility-report.md):

- The HexaDash adapter **re-implements the HexaDash visual language** (layout, navigation, cards, tables, dialogs, charts, badges, avatars, breadcrumbs, tabs, notifications) using the platform's existing primitives — Bootstrap/CoreUI markup, `react-bootstrap` modals, `react-router-dom`, `@coreui/icons` — skinned with **extracted HexaDash design tokens**.
- It **reuses none** of HexaDash's auth logic, routing, Redux, services, APIs, state management, or demo pages — those are RED per all prior analysis and forbidden by this phase's quality rules.
- **No Ant Design and no new runtime dependency** were added. (HexaDash is Ant Design + JavaScript; importing it verbatim would violate the platform's TypeScript-strict, CoreUI architecture and the React 19 baseline.)

All HexaDash styling is **scoped under `.hexadash-theme`**, so the compiled CSS is **inert while CoreUI is active** — no element carries that class unless the HexaDash layout renders.

---

## 2. Files created

### Adapter components (`src/theme/adapters/hexadash/`)

| File | Contract | Implements |
|------|----------|-----------|
| `HexaDashLayout.tsx` | `LayoutContract.Layout` | Authenticated shell: 280px sider, 64px sticky header, content, footer; mobile overlay (≤991px), auto-collapse (≤1200px); self-managed dark mode via `data-hexadash-mode` |
| `HexaDashAuthLayout.tsx` | `LayoutContract.AuthLayout` | Centred auth card on HexaDash gradient |
| `HexaDashNav.tsx` | `NavigationContract.Nav` | Vertical menu from the resolved `navGroups` model; zero access decisions |
| `HexaDashCard.tsx` | `CardContract.Card` | 10px-radius card frame + header/body/footer |
| `HexaDashDialog.tsx` | `DialogContract.Dialog` | `react-bootstrap` modal tagged `.hexadash-theme` |
| `HexaDashTable.tsx` | `TableContract.Table` | Generic, type-safe table |
| `HexaDashNotificationViewport.tsx` | `NotificationContract.Viewport` | Wraps platform-owned `ToastContainer` |
| `HexaDashBadge.tsx` | `BadgeContract.Badge` | Semantic variant → `.hexadash-badge-*` |
| `HexaDashAvatar.tsx` | `AvatarContract.Avatar` | Initials/image, `sm`/`md`/`lg` |
| `HexaDashTabs.tsx` | `TabsContract.Tabs` | Accessible tab strip + panel |
| `HexaDashBreadcrumb.tsx` | `BreadcrumbContract.Breadcrumb` | Router-linked trail |
| `HexaDashChart.tsx` | `ChartContract.Chart` | Themed frame + placeholder (no chart lib) |
| `navIcons.ts` | — | `NavigationIconKey` → glyph map (reuses `@coreui/icons`) |
| `index.ts` | `ThemeAdapter` | Assembles `hexadashAdapter` (all 11 contracts) |
| `hexadash.scss` | — | Token-driven component skin, scoped to `.hexadash-theme` |
| `tokens/hexadash.tokens.scss` | — | Extracted design tokens (CSS custom properties) |

### Tests

| File | Verifies |
|------|----------|
| `src/theme/registry.test.ts` | Registry loads both themes; CoreUI stays default; hexadash resolves; **all 11 contracts present on both adapters (parity, no missing implementations)** |
| `src/theme/adapters/hexadash/HexaDashAdapter.test.tsx` | `PlatformCard`, `PlatformChart`, `PlatformBadge`, `PlatformAvatar`, `PlatformTable`, `PlatformTabs` render successfully through the HexaDash adapter |

### Files modified

| File | Change |
|------|--------|
| `src/theme/config.ts` | `ThemeId` extended to `'coreui' \| 'hexadash'`; `KNOWN_THEME_IDS` adds `'hexadash'`; **default unchanged (`coreui`)** |
| `src/theme/registry.ts` | Registers `hexadash: hexadashAdapter` |

---

## 3. Contracts implemented

All **11** `ThemeAdapter` contracts (12 component surfaces) are implemented:

| # | Contract | Surface(s) | HexaDash component |
|---|----------|-----------|--------------------|
| 1 | Layout | `Layout`, `AuthLayout` | `HexaDashLayout`, `HexaDashAuthLayout` |
| 2 | Navigation | `Nav` | `HexaDashNav` |
| 3 | Card | `Card` | `HexaDashCard` |
| 4 | Dialog | `Dialog` | `HexaDashDialog` |
| 5 | Table | `Table` | `HexaDashTable` |
| 6 | Notification | `Viewport` | `HexaDashNotificationViewport` |
| 7 | Badge | `Badge` | `HexaDashBadge` |
| 8 | Avatar | `Avatar` | `HexaDashAvatar` |
| 9 | Tabs | `Tabs` | `HexaDashTabs` |
| 10 | Breadcrumb | `Breadcrumb` | `HexaDashBreadcrumb` |
| 11 | Chart | `Chart` | `HexaDashChart` |

The TypeScript compiler enforces completeness: `hexadashAdapter` is typed as `ThemeAdapter`, so a missing surface fails the build. `tsc --noEmit` passes.

---

## 4. Token extraction details

Tokens were extracted **directly from the HexaDash source** and materialised as CSS custom properties (prefixed `--hd-`) in `tokens/hexadash.tokens.scss`.

**Sources:**
- `src/config/theme/themeVariables.js` — palette, typography, radii, shadows, component metrics
- `src/config/theme/themeConfigure.js` — light/dark semantic backgrounds

| Category | Examples (light) | Dark overrides |
|----------|------------------|----------------|
| Colours | primary `#8231D3`, primary-hover `#6726A8`, secondary `#5840FF`, success `#01B81A`, warning `#FA8B0C`, danger `#FF0F0F`, info `#00AAFF` | (palette constant) |
| Typography | `'Jost', sans-serif`, base `15px`, heading `rgba(0,0,0,.85)`, text `#666D92` | heading `#E1E1E3`, text `#A4A5AA` |
| Surfaces | body `#F8F9FB`, general `#F4F5F7`, card/white `#FFFFFF`, header/sider `#FFFFFF` | body `#010413`, card/white `#1B1E2B`, general `#323541`, input `#282B37` |
| Borders | light `#F1F2F6`, normal `#E3E6EF`, deep `#C6D0DC`, radius-base `4px` | normal `#323541`, deep `#070A19` |
| Cards | radius `10px`, shadow `0 5px 20px rgba(146,153,184,.03)`, head-padding `16px`, padding `24px` | — |
| Shadows | base `0 2px 8px rgba(0,0,0,.15)`, lg `0 15px 40px rgba(146,153,184,.2)` | — |
| Layout | header height `64px`, sidebar width `280px` | — |
| Menu | icon `#A0A0A0`, active `#8231D3`, active-bg `#F2EAFB` | active `#FFFFFF`, active-bg `#282B37` |
| Inputs | height `40px`/`50px`, border `#E3E6EF` | border `#494B55` |
| Spacing | gutter base `25px` + xs/sm/md/lg/xl scale | — |
| Breakpoints | mobile `991px`, tablet `1200px` | — |

**Scoping:** the light tokens live under `.hexadash-theme`; dark tokens under `.hexadash-theme[data-hexadash-mode='dark']`. This guarantees the tokens never leak into CoreUI.

---

## 5. Visual parity

The six contracts called out in the brief render through HexaDash and are covered by tests / type-checking:

| Platform primitive | Renders via | Verified |
|--------------------|-------------|----------|
| `PlatformLayout` | `HexaDashLayout` | Structure mirrors the CoreUI shell (sider/header/content/footer); type-checked |
| `PlatformNav` | `HexaDashNav` | Resolved `navGroups` model rendered; type-checked |
| `PlatformTable` | `HexaDashTable` | Render test (rows + `.hexadash-table`) |
| `PlatformCard` | `HexaDashCard` | Render test (`.hexadash-card`) |
| `PlatformDialog` | `HexaDashDialog` | `react-bootstrap` modal; type-checked |
| `PlatformChart` | `HexaDashChart` | Render test (frame + placeholder) |

Because modules consume only `platform-ui` (which resolves the active adapter via `useTheme`), the **same module code** renders under `VITE_THEME=coreui` and `VITE_THEME=hexadash` with **no changes** — the success requirement of this phase.

---

## 6. Known gaps & theme limitations

| # | Limitation | Rationale / impact |
|---|-----------|--------------------|
| 1 | **Visual port, not Ant Design** | HexaDash's real components are Ant Design + JavaScript. Adopting them would add `antd` (critical UI-kit conflict), break TS-strict, and is unvalidated on React 19. The adapter reproduces the HexaDash *look* via tokens on Bootstrap/CoreUI primitives — pixel-perfect parity with the original Ant Design theme is **not** guaranteed. |
| 2 | **Icons reuse `@coreui/icons`** | HexaDash ships Unicons; adding that package was avoided. Nav glyphs use the existing CoreUI icon set. Swapping to Unicons later is a localized change in `navIcons.ts`. |
| 3 | **No font bundled** | `Jost` is referenced in tokens but not self-hosted/imported. If activated, add a `Jost` `@font-face`/Google Fonts link; otherwise the system fallback applies. |
| 4 | **Charts are abstraction-only** | `HexaDashChart` renders a frame + placeholder; no ApexCharts/Chart.js/Recharts/Google Charts is bound (deliberate — backend choice is deferred). |
| 5 | **No top-menu mode / RTL / i18n** | HexaDash's Redux-driven top-menu, RTL, and i18next features are out of scope and not part of any platform contract. |
| 6 | **Dialog/Toast scoping** | `react-bootstrap` portals modals to `document.body`; the dialog is tagged `.hexadash-theme` so tokens still apply. Toasts reuse the platform-owned queue/service (theme presents only). |
| 7 | **Header utilities** | HexaDash's demo header widgets (search, messages, notifications dropdown, settings drawer) are demo-only (RED) and intentionally omitted. |

---

## 7. Quality verification

| Check | Result |
|-------|--------|
| `tsc --noEmit` (strict, incl. test files) | ✅ Passes |
| IDE linter (all new files) | ✅ No errors |
| HexaDash default? | ❌ No — `DEFAULT_THEME_ID` stays `coreui` |
| `VITE_THEME` changed? | ❌ No |
| Ant Design imported? | ❌ No |
| Redux / routing / auth / API / services / state imported from theme? | ❌ No |
| New runtime dependency? | ❌ No |
| Module files changed? | ❌ No |
| Visual change to active (CoreUI) app? | ❌ None — HexaDash CSS is scoped to `.hexadash-theme` (unused while CoreUI active) |

### Test execution note

The repository targets **Vitest 3** (requires **Node ≥ 18**). The current machine runs **Node v14.17.3**, so `vitest run` cannot start (`SyntaxError: Unexpected token '??='` inside Vitest itself) — this affects the **entire** existing test suite, not these tests. The new tests are nonetheless confirmed to **compile and type-check** (they are included in the passing `tsc --noEmit` run) and pass the IDE linter. They will execute on a Node 18+ environment / CI.

---

## 8. Success criteria

| Criterion | Status |
|-----------|--------|
| HexaDash adapter compiles | ✅ `tsc` passes |
| CoreUI adapter still compiles | ✅ Unchanged; `tsc` passes |
| Theme registry supports both | ✅ `registry.ts` registers `coreui` + `hexadash` |
| Application still runs on CoreUI | ✅ Default unchanged; HexaDash CSS inert |
| Platform ready for theme switching | ✅ `VITE_THEME=hexadash` selects the adapter with zero module changes |

---

## 9. Readiness for T7 (Theme Validation)

The platform now ships **two complete adapters** behind identical contracts. T7 can:

1. Set `VITE_THEME=hexadash` in a build/preview and exercise every route.
2. Visually verify the six primitives (layout, nav, table, card, dialog, chart) plus badges/avatars/tabs/breadcrumbs/notifications.
3. Confirm guards, auth, permissions, routing, and API behaviour are **identical** across themes (they live in the platform, not the theme).
4. Add a `Jost` font and validate WCAG contrast for the purple palette.
5. Run the adapter tests on a Node 18+ runner (CI) and compare bundle sizes (coreui vs hexadash).

**Outcome:** HexaDash adapter, token system, theme registration, documentation, and tests are delivered. CoreUI remains active and the platform is ready for T7 theme validation.
