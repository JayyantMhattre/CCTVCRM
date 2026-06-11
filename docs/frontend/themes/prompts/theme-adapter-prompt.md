# Reusable Prompt — Theme Adapter Implementation

**When to use:** Lifecycle step 3 / onboarding step 4 ([create adapter](../theme-onboarding-guide.md#step-4--create-adapter)). Produces a compiling `src/theme/adapters/<id>/` + registry wiring + `docs/frontend/themes/<theme>/<id>-adapter-report.md`.

**Prereq:** `platform-compatibility-report.md` recommends PROCEED.

---

```
You are implementing a NEW theme adapter for Ashraak (React 19) that implements all 11
theme contracts and coexists with the existing adapters. Follow
docs/frontend/themes/theme-adapter-development-guide.md exactly.

THEME: <THEME_NAME>     THEME ID: <id>
INPUTS: src/theme/contracts/*, docs/frontend/themes/<THEME_NAME>/{theme-analysis,platform-compatibility-report}.md
REFERENCE ADAPTERS: src/theme/adapters/coreui/ (default), src/theme/adapters/hexadash/ (opt-in)

HARD RULES
- DO NOT change the default theme (leave DEFAULT_THEME_ID unchanged). DO NOT activate <id>.
- DO NOT modify business modules, auth, routes, guards, permissions, APIs, or backend.
- DO NOT import the vendor theme's Redux, router, auth, services, or demo pages.
  Reuse VISUAL DESIGN only (tokens + markup patterns).
- Scope ALL CSS under .<id>-theme so the adapter is inert unless active.
- TypeScript strict: no `any`; satisfy the ThemeAdapter interface structurally.

TASKS
1. Create src/theme/adapters/<id>/tokens/<id>.tokens.scss: --<id>-* custom properties under
   .<id>-theme (light) and .<id>-theme[data-<id>-mode='dark'] (dark). Colours, typography,
   spacing, radius, shadows, layout dims, breakpoints.
2. Create src/theme/adapters/<id>/<id>.scss: @use the tokens; style .<id>-* classes only
   under .<id>-theme.
3. Create navIcons.ts mapping NavigationIconKey → the theme's glyphs.
4. Implement components (one file each), per the dev guide:
   <Theme>Layout, <Theme>AuthLayout, <Theme>Nav, <Theme>Card, <Theme>Table, <Theme>Dialog,
   <Theme>NotificationViewport, <Theme>Badge, <Theme>Avatar, <Theme>Tabs, <Theme>Breadcrumb,
   <Theme>Chart. Layout imports <id>.scss and applies .<id>-theme at its root.
5. index.ts: export `const <id>Adapter: ThemeAdapter = {...}` wiring all 11 contracts.
6. Register: add <id> to ThemeId union + KNOWN_THEME_IDS in config.ts; add one line to
   REGISTRY in registry.ts. Do NOT touch DEFAULT_THEME_ID.
7. Tests: extend registry test so <id> resolves and exposes the full contract surface;
   add render tests for key platform-ui primitives through <id>.
8. Verify: `npm run type-check` passes; tests pass/compile.
9. Document: docs/frontend/themes/<THEME_NAME>/<id>-adapter-report.md (files created,
   contracts implemented, token extraction, known gaps, quality verification).

CONSTRAINTS REMINDER: adapters render only; the platform owns nav visibility, tab active
state, toast queue, dialog show/close, auth, routing.
```

---

**Exit criteria:** adapter compiles, registered (not default), tests green, report written. Proceed to [theme-validation-prompt.md](./theme-validation-prompt.md).
