# Reusable Prompt — Theme Validation & Production Readiness

**When to use:** Lifecycle step 4 / onboarding step 5 ([validate](../theme-onboarding-guide.md#step-5--validate)). Produces validation, parity, and readiness reports + adapter-only fixes.

**Prereq:** adapter registered and compiling.

---

```
You are validating a registered theme adapter as a production-ready option for Ashraak.
Temporarily activate it with VITE_THEME=<id> and validate the ENTIRE platform.

THEME ID: <id>     DEFAULT (unchanged): <default-id>

VALIDATION RULES
- ONLY adapter-level fixes allowed (adapter components, token mappings, layout/component
  rendering). DO NOT change Auth, Permissions, Routes, Guards, APIs, Business logic,
  Modules, Database, or Backend.
- Do NOT commit a default change; activation is temporary (env only).

MODULES TO VALIDATE (every one):
Dashboard, Auth, Users, Roles, Permissions, Tenants, Audit, Notifications, Files,
Webhooks, API Keys, Sessions, Invitations, Settings, Profile.
(For any listed area without a front-end page, mark N/A with a reason.)

FOR EVERY PAGE VERIFY:
Layout, Navigation, Forms, Tables, Dialogs, Notifications, Cards, Charts, Tabs, Badges,
Breadcrumbs, Pagination, Responsive behaviour, Dark mode.

RESPONSIVE: desktop, tablet, mobile; sidebar/header behaviour; nav collapse; tables; dialogs.
DARK MODE: colours, contrast, borders, cards, tables, forms, notifications.
ACCESSIBILITY: keyboard nav, focus states, tab order, contrast, ARIA.

METHOD NOTE: if a browser build can't run in this environment, validate by static tracing
of the render path (what resolves through the adapter vs raw markup) + `tsc --noEmit` +
unit tests, and mark runtime-only checks as "needs runtime confirmation".

PRODUCE:
1. docs/frontend/themes/<THEME>/<id>-validation-report.md
   - how theming reaches the screen (what goes through the adapter vs not)
   - per-module matrix + per-aspect checks + responsive + dark mode + accessibility
   - gap analysis classified P0 (blocking) / P1 (high) / P2 (medium) / P3 (low)
   - list of adapter-only fixes applied
2. docs/frontend/themes/<THEME>/coreui-vs-<id>-parity-report.md
   - contract parity (11/11), visual parity, functional parity, missing capabilities,
     known limitations, parity scorecard
3. docs/frontend/themes/<THEME>/production-readiness-report.md
   - performance review (bundle, CSS, render, adapter overhead)
   - constraint-compliance checklist
   - DECISION: READY / READY WITH MINOR ISSUES / NOT READY + Go/No-Go recommendation

APPLY adapter-only fixes for clear defects; re-run type-check after each.
```

---

**Exit criteria:** three reports produced; a documented READY / READY WITH MINOR ISSUES / NOT READY decision; only adapter-level changes made. Promotion to default (if any) follows [theme-onboarding-guide.md](../theme-onboarding-guide.md#step-6--activate).
