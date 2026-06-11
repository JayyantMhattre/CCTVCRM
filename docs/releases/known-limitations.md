# Known Limitations — Platform V1

Honest inventory of current constraints, tradeoffs, technical debt, and intentionally deferred items as of the V1 baseline. None of these block the platform freeze; they are documented so downstream teams plan with full information.

**Related:** [platform-roadmap.md](./platform-roadmap.md) · [upgrade-guide.md](./upgrade-guide.md) · [documentation gap analysis](../documentation-audit/documentation-gap-analysis.md)

**Severity legend:** 🔴 Plan around it · 🟠 Notable · 🟡 Minor.

---

## 1. Current constraints

| # | Constraint | Severity | Notes |
|---|-----------|----------|-------|
| C1 | **Node.js 18+ required** for the web toolchain | 🔴 | Vite 6 and Vitest 3 require Node ≥ 18. Some dev environments still run Node 14, where `vite build` / `vitest run` fail to start (a `SyntaxError` on modern syntax). Use Node 20+ (per the root README prerequisites). This is environmental, not a code defect. |
| C2 | **Web module interiors not yet themed** | 🟠 | Business modules render raw CoreUI/Bootstrap markup (not `platform-ui` primitives), and `coreui.scss` is always loaded. Under `VITE_THEME=hexadash`, the shell/nav/auth are HexaDash-skinned but page interiors stay CoreUI-styled. See the [HexaDash validation report](../frontend/themes/current-theme/hexadash-validation-report.md) (P1-1). |
| C3 | **No charting backend** | 🟡 | `ChartContract` is library-agnostic and renders a frame/placeholder; no chart library is bundled. |
| C4 | **Toasts not routed through the theme adapter** | 🟡 | The web app mounts `ToastContainer` directly in `AppProviders`; the adapters' notification viewport is unused (P2-1). |
| C5 | **Both theme adapters ship in the bundle** | 🟡 | `registry.ts` statically imports all adapters; no per-theme code-splitting yet (P2-4). |
| C6 | **Documentation scaffold vs implementation gaps** | 🟠 | Some docs describe intended behavior ahead of full implementation; tracked in the [documentation gap analysis](../documentation-audit/documentation-gap-analysis.md) and [outdated docs report](../documentation-audit/outdated-docs-report.md). |

## 2. Known tradeoffs (by design)

| Tradeoff | Rationale |
|----------|-----------|
| **Theme indirection** — modules call `platform-ui`, not components directly | Decouples business code from any theme; enables unlimited future themes ([decision record](../frontend/themes/theme-decision-record.md)). |
| **Adapters are hand-written (visual porting)** — not drop-in vendor integration | Prevents vendor lock-in (no vendor Redux/router/auth); bounded by 11 contracts. |
| **Modular monolith, not microservices** | Operational simplicity + module isolation via contracts ([ADR-0001](../adr/ADR-0001-modular-monolith.md)); can be decomposed later. |
| **Outbox-based eventing, not a message broker** | Reliability without infra overhead ([ADR-0002](../adr/ADR-0002-outbox-pattern.md)); a broker can be layered on later. |
| **Per-module PostgreSQL schemas in one database** | Isolation with single-DB ops; splitting DBs is possible later. |
| **Audit in MongoDB (separate store)** | Write-optimized capture decoupled from transactional data ([ADR-0003](../adr/ADR-0003-observer-modules.md)). |

## 3. Known technical debt

| # | Item | Severity | Resolution path |
|---|------|----------|-----------------|
| D1 | Module migration to `platform-ui` pending (closes C2) | 🟠 | A dedicated migration phase; the architecture makes it incremental. |
| D2 | Narrow/collapsed sidebar in HexaDash keeps labels (no true icon-only mode) | 🟡 | Adapter enhancement ([validation P2-2](../frontend/themes/current-theme/hexadash-validation-report.md)). |
| D3 | Some inline UI (badges/dialogs) duplicated across modules instead of using primitives | 🟡 | Folds into D1. |
| D4 | Runtime-only checks (pixel layout, WCAG contrast) for HexaDash pending a Node 18+ run | 🟡 | Run validation on a compatible environment; no code change expected. |

## 4. Theme observations

### HexaDash limitations
- Themes the **shell, navigation, and auth shell** only; module page interiors remain CoreUI-styled until module migration (C2/D1).
- Auth pages render in light mode only (no dark toggle on the auth shell — by design).
- Icons reuse `@coreui/icons` and the system font stack rather than HexaDash's original Unicons/Jost, to avoid new runtime dependencies (visual-porting strategy).
- Contrast/responsive specifics need runtime confirmation on Node 18+ (D4).
- Readiness verdict: **READY WITH MINOR ISSUES** as an opt-in theme ([production readiness report](../frontend/themes/current-theme/production-readiness-report.md)).

### CoreUI limitations
- Remains the **production default**; it is the assumed styling for all module markup, so module interiors are coupled to CoreUI/Bootstrap classes today (the root cause of C2).
- This coupling is acceptable for V1 but is what the `platform-ui` migration (D1) will unwind.

## 5. Node 18 requirement (detail)

The web app (`FrontEnd/apps/web`) builds and tests with **Vite 6** and **Vitest 3**, both of which require **Node.js ≥ 18** (Node **20+** recommended, matching the README prerequisites). On Node 14/16:
- `vite dev` / `vite build` and `vitest run` will not start.
- `tsc --noEmit` (type-check) still works and is the primary static gate when a full build can't run.

**Action for downstream teams:** standardize on Node 20+ in local and CI environments.

## 6. Intentionally deferred

Documented in full in [platform-roadmap.md](./platform-roadmap.md#2-deferred-intentionally-out-of-v1):
- Module migration to `platform-ui` (and full HexaDash reskin).
- Charting backend; adapter-routed toasts; per-theme code-splitting.
- Promoting HexaDash to default; SSO provider GA.
- All business/experimental modules (billing, reporting, AI, realtime, external bus) — product-specific, built on top of the platform.

---

> These limitations are stable and expected for the V1 baseline. They do not prevent **Platform Freeze**; they define the starting backlog for downstream products.
