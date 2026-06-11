# Theme Lifecycle

**Purpose:** Define the full lifecycle of a theme in Ashraak — from acquisition to retirement — so the theme system remains a permanent, governed platform capability rather than a one-off migration.
**Phase:** T8 — Theme Governance.

**Related:** [theme-onboarding-guide.md](./theme-onboarding-guide.md) · [theme-selection-checklist.md](./theme-selection-checklist.md) · [theme-adapter-development-guide.md](./theme-adapter-development-guide.md) · [theme-decision-record.md](./theme-decision-record.md)

---

## Lifecycle at a glance

```
 ┌────────────┐   ┌────────────┐   ┌──────────────────┐   ┌────────────┐   ┌────────────┐
 │  1. Acquire│──▶│ 2. Analyse │──▶│ 3. Implement     │──▶│ 4. Validate│──▶│ 5. Retire  │
 │  (purchase)│   │ (report)   │   │   (adapter)      │   │ (activate) │   │ (deprecate)│
 └────────────┘   └────────────┘   └──────────────────┘   └────────────┘   └────────────┘
        │                                                         │
        └──────────────── gate: selection checklist ─────────────┘ (reject early if it fails)
```

A theme is always in exactly one **state**:

| State | Meaning | `registry.ts` | `DEFAULT_THEME_ID` |
|-------|---------|---------------|--------------------|
| `candidate` | Acquired + under analysis; no adapter yet | not registered | — |
| `implemented` | Adapter compiles, registered, not default | registered | unchanged |
| `active-default` | Promoted to the platform default | registered | this theme |
| `deprecated` | Superseded but kept for rollback | registered | not this theme |
| `retired` | Removed from the codebase | unregistered | — |

---

## 1. Theme acquisition

**Goal:** obtain a theme that is *technically compatible by construction*, not just visually appealing.

- Run the [theme-selection-checklist.md](./theme-selection-checklist.md) **before** any purchase. Hard-fails (wrong React major, no TypeScript, Redux/router/auth coupling that can't be stripped) are grounds to reject.
- Confirm licence allows adapting the design into a commercial product and redistributing the compiled result.
- Capture source metadata (version, UI kit, dependencies) — this is the input to analysis.

**Exit criteria:** licence cleared, checklist passed, source extracted to a reference-only location (never imported by `src/`).

## 2. Theme analysis

**Goal:** produce an objective map of the theme's anatomy and risks.

- Use [`prompts/theme-analysis-prompt.md`](./prompts/theme-analysis-prompt.md) and [`prompts/theme-compatibility-prompt.md`](./prompts/theme-compatibility-prompt.md).
- Deliver `theme-analysis.md` + `platform-compatibility-report.md` under `docs/frontend/themes/<theme>/`.
- Every capability is classified 🟢 GREEN / 🟡 YELLOW / 🔴 RED. RED = never imported (auth, routing, Redux, services, APIs, demo pages).

**Exit criteria:** compatibility report signed off; no unresolved RED on a required capability.

## 3. Theme implementation

**Goal:** realize the theme as an adapter implementing all 11 contracts.

- Follow [theme-adapter-development-guide.md](./theme-adapter-development-guide.md) and [`prompts/theme-adapter-prompt.md`](./prompts/theme-adapter-prompt.md).
- Extract tokens (`tokens/<id>.tokens.scss`), build the scoped skin (`<id>.scss`), implement components, assemble `index.ts`, register the id in `config.ts` + `registry.ts`.
- **Coexistence is mandatory:** the new adapter must build and run alongside existing adapters; the default must not change here.

**Exit criteria:** `tsc --noEmit` passes; registry + adapter contract tests pass; default unchanged.

## 4. Theme validation

**Goal:** prove production readiness without changing business behaviour.

- Activate temporarily with `VITE_THEME=<id>`; run [`prompts/theme-validation-prompt.md`](./prompts/theme-validation-prompt.md).
- Validate every module + every aspect (layout, nav, forms, tables, dialogs, notifications, cards, charts, tabs, badges, breadcrumbs, pagination, responsive, dark mode, accessibility).
- Produce `<id>-validation-report.md` (P0–P3), `coreui-vs-<id>-parity-report.md`, and `production-readiness-report.md` with a decision: **READY / READY WITH MINOR ISSUES / NOT READY**.
- Only **adapter-level** fixes are allowed during validation.

**Exit criteria:** a documented readiness decision. `implemented → active-default` only after an explicit promotion (a one-line `DEFAULT_THEME_ID` change) per [theme-onboarding-guide.md](./theme-onboarding-guide.md#step-6--activate).

## 5. Theme retirement

**Goal:** remove a superseded theme cleanly, with a safe rollback window.

Retirement is the reverse of onboarding and is also adapter-only:

1. **Deprecate (keep registered):** mark the theme `deprecated` in its docs. Ensure it is **not** the default. Keep it registered for **at least one release** so `VITE_THEME=<oldId>` is an instant rollback if the new default regresses.
2. **Announce:** note the deprecation + removal target in the theme's report and the changelog.
3. **Remove (after the rollback window):**
   - delete `src/theme/adapters/<oldId>/`;
   - remove its line from `REGISTRY` in `registry.ts`;
   - remove the id from the `ThemeId` union and `KNOWN_THEME_IDS` in `config.ts`;
   - `resolveThemeId()` automatically falls back to `DEFAULT_THEME_ID` for any now-unknown `VITE_THEME` value, so stale env values fail safe.
4. **Verify:** `tsc` + tests green; default theme unaffected; archive the theme's docs under `docs/frontend/themes/<theme>/` for history.

**Exit criteria:** adapter removed, registry/config cleaned, build green, no orphaned `VITE_THEME` references, docs archived.

**Never:** retire the only registered theme, or retire the active default without first promoting a replacement.

---

## Governance invariants (true in every state)

1. Exactly one theme is the default; it always resolves even if `VITE_THEME` is invalid.
2. Business modules never change across the lifecycle — only `platform-ui`, contracts, and adapters do.
3. Adding/removing a theme touches only `theme/adapters/*`, `registry.ts`, and `config.ts`.
4. Every theme that ever ships has an analysis report, compatibility report, validation report, and readiness decision.
5. CSS is always scoped per theme; inactive themes are inert.
