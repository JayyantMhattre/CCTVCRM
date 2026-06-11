# T4 — Component Migration Plan

**Phase:** T4 — Theme Component Inventory & Mapping (analysis only)
**Status:** Plan only — **no code migration, no theme integration, no component replacement** in this phase.
**Inputs:** [`component-mapping-report.md`](./component-mapping-report.md) (inventory + classification) and [`platform-ui-target-architecture.md`](./platform-ui-target-architecture.md) (target primitives). Sits on top of completed T1–T3.

This document sequences the component-layer work into three **migration waves** ordered by risk, with effort estimates and a readiness gate for HexaDash adapter implementation.

---

## 1. Sequencing principles

1. **Fill abstraction gaps before integrating any theme.** A `hexadash` adapter cannot be added until every category has a `Platform*` primitive and a contract.
2. **GREEN before YELLOW before RED.** Migrate safe, low-risk components first to build confidence and remove inline markup; never migrate RED.
3. **CoreUI parity first.** Each new contract gets a CoreUI adapter at visual parity *before* any HexaDash styling — so migrations cause zero visual change until the theme is deliberately switched.
4. **One module per PR.** Page migrations land module-by-module to keep diffs reviewable (per the risk guidance in the compatibility report).
5. **Data and state never move.** TanStack Query, RHF + Zod, Zustand `authStore`, the toast service, and all guards stay exactly as they are.

---

## 2. Wave 1 — Safe components (GREEN)

**Goal:** remove inline UI markup by introducing low-risk primitives that need only structural + token work. No new runtime dependencies.

| Task | Target primitive | Replaces | Modules touched |
|------|------------------|----------|-----------------|
| Create `PlatformBadge` (`variant` prop) + `BadgeContract` + CoreUI adapter | `PlatformBadge` | `StatusBadge`, `DeliveryStatusBadge`, `EnabledBadge`, `EVENT_BADGE`, inline `<span class="badge">` | webhooks, audit, layout |
| Create `PlatformAvatar` (initials/image) + `AvatarContract` + CoreUI adapter | `PlatformAvatar` | inline `avatar-sm` / `rounded-circle` | layout (header/sidebar), users |
| Standardise pagination | `PlatformPagination` (exists) | inline audit pager | audit |
| Wrap loaders/empty/error | `PlatformSpinner`, `PlatformEmptyState` | `Spinner`, `EmptyState` usage in pages | all list pages |
| Consolidate page headers | `PlatformPageHeader` (exists) | `shared/PageHeader` usages | all pages |

**Exit criteria:** no inline `badge`/avatar markup remains in modules; loaders, empty states, page headers, and pagination all come from `platform-ui`.

**Effort:** **3–5 developer-days.** **Risk: Low.**

---

## 3. Wave 2 — Moderate risk components (YELLOW)

**Goal:** route the remaining everyday surfaces through existing/new contracts so they become theme-swappable. HexaDash contributes tokens + visual reference only; **no Ant Design, no `react-bootstrap` left in modules.**

| Task | Target primitive | Replaces | Modules touched |
|------|------------------|----------|-----------------|
| Migrate inline `<table>` onto `PlatformTable` | `PlatformTable` (exists) | audit table, users table | audit, users |
| Migrate module modals onto `PlatformDialog` | `PlatformDialog`, `PlatformConfirmDialog` (exist) | `KeyRevealModal`, `SecretRevealModal`, `ConfirmDialog` (`react-bootstrap/Modal`) | apikeys, webhooks |
| Add form controls over RHF | `PlatformInput`, `PlatformSelect`, `PlatformTextarea`, `PlatformCheckbox` | native inputs in forms | auth, tenant, users, webhooks |
| Add `PlatformAlert`; isolate `@coreui/icons` | `PlatformAlert` | `AlertMessage` direct usage | all pages |
| Add `PlatformBreadcrumb` (from `routeMap`) + contract | `PlatformBreadcrumb` | header placeholder (gap) | layout |
| Add `PlatformTabs` over Bootstrap `nav-tabs` + contract | `PlatformTabs` | detail pages needing tabs (gap) | webhooks, apikeys |
| Migrate inline `.card` onto `PlatformCard` | `PlatformCard` (exists) | inline `.card` blocks | all pages |
| Add themed toast `Viewport` | `PlatformToast` (exists) | `shared/ui/toast` `ToastContainer` styling | global |

**Exit criteria:** modules import no `@coreui/*`, `antd`, or `react-bootstrap`; tables/dialogs/forms/cards/alerts/tabs/breadcrumbs all flow through `platform-ui`.

**Effort:** **8–12 developer-days** (incremental, one module per PR). **Risk: Medium** — table/dialog/form migrations are the main regression surfaces; mitigate with CoreUI-parity adapters and per-page visual checks.

---

## 4. Wave 3 — High risk components (deferred / dependency-bearing)

**Goal:** the surfaces that need a new dependency or a product decision. Do these last, behind explicit approval.

| Task | Target primitive | Constraint | Decision needed |
|------|------------------|-----------|-----------------|
| Add **one** chart library (Recharts preferred for React 19) behind `PlatformChart` + `ChartContract` | `PlatformChart` (frame exists) | Reject all-four HexaDash libs; lazy-load | Is there a stats API + dashboard requirement? |
| `PlatformDateField` wrapping native `<input type="date">` | `PlatformDateField` | Do **not** adopt Ant `DatePicker` (pulls in Ant) | Are richer date features needed? |
| `PlatformSelect`/`PlatformDropdown` rich variants | `PlatformSelect` | Native-first; no Ant `Select` | Is multi-select/search required? |
| `PlatformStatCard` + dashboard grid | `PlatformStatCard` | HexaDash dashboards are wireframe reference only | Final dashboard spec |
| Auth page **visual** refresh | `PlatformAuthLayout` (exists) | Logic (JWT, tenant, MFA, RHF/Zod) stays RED | Adopt HexaDash auth background/card? |

**Exit criteria:** charting (if approved) lazy-loads a single library; date/select/dashboard primitives exist; auth visuals optionally restyled with **no logic change**.

**Effort:** **5–8 developer-days** (scope-dependent; charting dominates). **Risk: Medium-High** — driven by the chart-library bundle decision and React 19 compatibility.

---

## 5. RED — explicitly excluded from all waves

Never scheduled, never imported (reference only): `MenueItems.js`/`TopMenu.js`, `DataTable.js` + `data-filter` Redux, Ant `Form` demos, `authSlice`/Firebase/Auth0, header notification/message demos, all four chart libraries together, the Redux store + 25+ slices, `DataService` + mock endpoints, the 100+ `routes/admin/*` demo routes, ecommerce/chat/email/calendar/kanban/maps/editor containers, CRA/CRACO config, and any HexaDash `.js` component imported verbatim.

The theme-specific business pages, auth-page logic, routing, state management, and APIs analysed in [`component-mapping-report.md`](./component-mapping-report.md) §6 are all **RED — do not adopt**.

---

## 6. Effort summary

| Wave | Components | Effort | Risk |
|------|-----------|--------|------|
| Wave 1 — Safe (GREEN) | Badge, Avatar, Pagination, Loader, Empty/Error, PageHeader | 3–5 days | Low |
| Wave 2 — Moderate (YELLOW) | Table, Dialog, Forms/Inputs, Card, Alert, Tabs, Breadcrumb, Toast | 8–12 days | Medium |
| Wave 3 — High risk (deferred) | Chart+lib, DateField, Select, StatCard, Auth visual | 5–8 days | Medium-High |
| **Component layer total** | | **16–25 days** (~3–5 weeks) | |

Excludes the already-completed T1–T3 shell/navigation work. A subsequent `hexadash` adapter + token extraction is a separate effort (estimated 5–8 days in the compatibility report's T2/T3 figures) that can only begin once the readiness gate below is green.

---

## 7. Readiness gate for HexaDash adapter implementation

The platform is **ready to implement a `hexadash` adapter** when **all** of the following hold:

- [ ] Wave 1 complete — no inline badge/avatar markup; GREEN primitives in `platform-ui`.
- [ ] Wave 2 complete — modules import no `@coreui/*`, `antd`, or `react-bootstrap`.
- [ ] Every category in the mapping report maps to a `Platform*` primitive (no `Gap` rows).
- [ ] New contracts (`Badge`, `Avatar`, `Tabs`, `Breadcrumb`, `Chart`) added to `ThemeAdapter` and implemented by the CoreUI adapter at parity.
- [ ] `theme/tokens/hexadash.tokens.scss` extracted from HexaDash `themeVariables.js` (colors, radius, spacing, `Jost`).
- [ ] Chart-library decision made (Wave 3) **or** explicitly deferred.
- [ ] ESLint import-boundary rules from the target architecture are in place.

**Current status:** T1–T3 contracts exist and are proven by the CoreUI adapter, but **Waves 1–3 are not started** and five contracts are still gaps. The platform is therefore **architecturally ready** (the layering and contract pattern work) but **not yet feature-ready** to drop in HexaDash. Closing Wave 1 + Wave 2 + the new contracts is the prerequisite.

---

## 8. Final recommendation

1. **Migrate first:** Wave 1 GREEN components (badges, avatars, pagination, loaders, empty/error, page headers) — fastest, safest, removes the most inline markup.
2. **Never migrate:** all RED items in §5 — HexaDash routing, state, auth logic, API layer, demo pages, `MenueItems.js`, `DataTable.js`, Ant Form, and the multi-charting stack.
3. **Estimated effort:** ~16–25 developer-days for the component layer across three waves, then a separate ~5–8 days to author the `hexadash` adapter + tokens.
4. **Readiness for HexaDash adapter:** the contract architecture is ready; complete Waves 1–2 and the five new contracts (§7) before integrating any HexaDash visual. Until then, HexaDash remains a **reference only**.

> No code was migrated, no theme was integrated, and no component was replaced in T4. This plan defines order, effort, and the gate; execution is a later phase.
