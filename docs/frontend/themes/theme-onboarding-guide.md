# Theme Onboarding Guide

**Audience:** Engineers onboarding a newly purchased (or newly built) UI theme into the Ashraak web app.
**Promise:** Any theme can be added as a swappable adapter **without touching business modules, auth, routing, permissions, guards, APIs, or backend.**
**Phase:** T8 — Theme Governance.

> This is the canonical, repeatable process. It is the same process used to onboard CoreUI (default) and HexaDash (opt-in). See also: [theme-lifecycle.md](./theme-lifecycle.md), [theme-selection-checklist.md](./theme-selection-checklist.md), [theme-adapter-development-guide.md](./theme-adapter-development-guide.md), [theme-decision-record.md](./theme-decision-record.md).

---

## Mental model in one paragraph

The platform never imports a theme. Business modules render **`platform-ui`** primitives (`PlatformCard`, `PlatformTable`, …). Each primitive delegates to the **active theme adapter** resolved by `ThemeProvider` from `VITE_THEME`. An adapter is a folder under `src/theme/adapters/<id>/` that implements **11 contracts** and is registered in `src/theme/registry.ts`. Onboarding a theme = produce that folder + one registry line. Nothing else changes.

```
business module ──▶ platform-ui primitive ──▶ useTheme().adapter.<contract> ──▶ <theme>Adapter
                                                          ▲
                                            ThemeProvider (VITE_THEME → registry)
```

---

## The 6 steps

### Step 1 — Extract theme

Goal: get the purchased theme source into a **reference-only** location. It is studied, never imported by the app.

1. Unzip the purchased theme into a scratch/reference location (outside `apps/web/src`, e.g. a `theme-source/<name>/` reference folder or a separate analysis branch). It must **not** be importable from `src/` — adapters reuse *visual ideas and tokens*, not the vendor's code.
2. Confirm the licence permits adapting the design into the product.
3. Record source metadata: name, version, UI kit (Bootstrap/Ant/MUI/Tailwind), React version, TypeScript yes/no, styling method (SCSS / CSS-in-JS), bundled libraries (Redux, router, charts, icons).

**Output:** an extracted reference + a one-paragraph source summary.

### Step 2 — Run analysis

Goal: understand what the theme *is* before deciding what to reuse.

Use the reusable prompt: [`prompts/theme-analysis-prompt.md`](./prompts/theme-analysis-prompt.md).

Catalog: layout/shell, navigation, components (cards, tables, forms, dialogs, badges, avatars, tabs, breadcrumbs, charts, notifications), design tokens (colours, typography, spacing, radius, shadows, breakpoints), light/dark handling, dependencies, and any application logic baked into the theme (auth, routing, state).

**Output:** `docs/frontend/themes/<theme>/theme-analysis.md` (mirror the structure used in `current-theme/`).

### Step 3 — Generate compatibility report

Goal: classify every piece of the theme as reusable, adaptable, or forbidden.

Use the reusable prompt: [`prompts/theme-compatibility-prompt.md`](./prompts/theme-compatibility-prompt.md).

Classify each capability:

| Class | Meaning |
|-------|---------|
| 🟢 GREEN | Visual-only; reuse markup/tokens directly in the adapter |
| 🟡 YELLOW | Reusable but needs an adapter wrapper / token mapping |
| 🔴 RED | **Do not import** — auth, routing, Redux/state, services, APIs, demo pages, vendor lock-in |

Run the [theme-selection-checklist.md](./theme-selection-checklist.md) gate here. A RED on a hard requirement (e.g. wrong React major, no TS) means renegotiate or reject.

**Output:** `docs/frontend/themes/<theme>/platform-compatibility-report.md`.

### Step 4 — Create adapter

Goal: implement the 11 contracts using the theme's *visual language* (tokens + markup), with zero vendor application logic.

Follow [theme-adapter-development-guide.md](./theme-adapter-development-guide.md) and the reusable prompt [`prompts/theme-adapter-prompt.md`](./prompts/theme-adapter-prompt.md).

1. Create `src/theme/adapters/<id>/`.
2. Extract tokens into `tokens/<id>.tokens.scss` as CSS custom properties **scoped under a `.<id>-theme` wrapper** (light + dark).
3. Create `<id>.scss` skin consuming those tokens, also scoped under `.<id>-theme`.
4. Implement the 11 contract components (`<Theme>Layout`, `<Theme>AuthLayout`, `<Theme>Nav`, `<Theme>Card`, `<Theme>Table`, `<Theme>Dialog`, `<Theme>Notification…`, `<Theme>Badge`, `<Theme>Avatar`, `<Theme>Tabs`, `<Theme>Breadcrumb`, `<Theme>Chart`).
5. Assemble `index.ts` exporting `const <id>Adapter: ThemeAdapter = { id, label, layout, navigation, … }`.
6. Register it:
   - add the id to the `ThemeId` union and `KNOWN_THEME_IDS` in `src/theme/config.ts`;
   - add one line to `REGISTRY` in `src/theme/registry.ts`.
7. **Keep the default unchanged** — do not change `DEFAULT_THEME_ID`.

**Output:** a compiling adapter folder + one registry line.

### Step 5 — Validate

Goal: prove the new theme renders the whole platform and breaks nothing.

Activate temporarily: set `VITE_THEME=<id>` (env or `.env.local`) — never commit a default change. Use the reusable prompt [`prompts/theme-validation-prompt.md`](./prompts/theme-validation-prompt.md).

Quality gates:

- `npm run type-check` (`tsc --noEmit`) passes.
- `npm test` — registry + adapter contract tests (all 11 contracts resolve on every adapter).
- Per-module walkthrough: layout, navigation, forms, tables, dialogs, notifications, cards, charts, tabs, badges, breadcrumbs, pagination.
- Responsive (desktop/tablet/mobile, sidebar/header/nav collapse), dark mode, accessibility (keyboard, focus, tab order, contrast, ARIA).

**Output:** `docs/frontend/themes/<theme>/<id>-validation-report.md` with P0–P3 findings, a parity report vs the default, and a production-readiness decision (READY / READY WITH MINOR ISSUES / NOT READY). Apply **adapter-only** fixes.

### Step 6 — Activate

Goal: make the validated theme live in a controlled way.

Options, least-to-most committal:

1. **Per-environment opt-in:** set `VITE_THEME=<id>` for a preview/staging environment. Default stays as-is.
2. **Promote to default:** once parity is accepted, change `DEFAULT_THEME_ID` in `config.ts` (one line) — a reversible flip.
3. **Retire old theme:** see [theme-lifecycle.md](./theme-lifecycle.md#5-theme-retirement). Keep the previous adapter registered for at least one release as a rollback path.

**Output:** activation note in the readiness report + (if promoted) the `DEFAULT_THEME_ID` change.

---

## Definition of done

- [ ] 11/11 contracts implemented and registered; `tsc` + tests green.
- [ ] Tokens scoped under `.<id>-theme`; no global leakage; default theme visually unchanged.
- [ ] No imports of vendor auth/routing/state/services anywhere in `src/`.
- [ ] No changes to modules, auth, routes, guards, permissions, APIs, or backend.
- [ ] Validation, parity, and readiness reports produced.
- [ ] Default theme flip (if any) is a single line in `config.ts`.

---

## Hard rules (apply to every onboarding)

1. **Adapters render; the platform decides.** No access/role/permission/routing logic in an adapter.
2. **No vendor app code.** Reuse design (tokens, markup patterns), never the theme's Redux/router/auth/services/demo pages.
3. **Scope all CSS** under `.<id>-theme` so inactive themes are inert.
4. **Default is sacred.** Onboarding never alters the current default until an explicit promotion step.
5. **Type-safety is the contract.** If `tsc` passes against `ThemeAdapter`, the theme is structurally complete.
