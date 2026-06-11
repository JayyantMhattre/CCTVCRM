# Reusable Prompt — Theme Compatibility Report

**When to use:** Lifecycle step 2 / onboarding step 3 ([compatibility](../theme-onboarding-guide.md#step-3--generate-compatibility-report)). Produces `docs/frontend/themes/<theme>/platform-compatibility-report.md`.

**Prereq:** `theme-analysis.md` exists (from [theme-analysis-prompt.md](./theme-analysis-prompt.md)).

---

```
You are producing a platform compatibility report for a purchased UI theme being
onboarded into Ashraak (React 19) via the Theme Adapter Architecture.

THEME: <THEME_NAME>
INPUTS:
- docs/frontend/themes/<THEME_NAME>/theme-analysis.md
- docs/frontend/themes/theme-selection-checklist.md (the acquisition gate)
- src/theme/contracts/* (the 11 contracts the adapter must satisfy)

RULES
- Analysis/documentation ONLY. No code changes, no theme import.
- Classify reuse with: GREEN = visual-only, reuse directly; YELLOW = reuse with an
  adapter wrapper / token mapping; RED = NEVER import (auth, routing, Redux/state,
  services, APIs, demo pages, vendor lock-in).

PRODUCE docs/frontend/themes/<THEME_NAME>/platform-compatibility-report.md covering:

1. Selection checklist result: fill in theme-selection-checklist.md; list any HARD (⛔)
   failures. A hard failure ⇒ recommend REJECT/renegotiate.
2. Stack compatibility matrix: theme stack vs Ashraak stack (React 19, TS, Vite 6,
   Bootstrap/CoreUI 5, TanStack Query, Zustand, react-router, react-hook-form+zod).
   Mark each Compatible / Adaptable / Conflict.
3. Contract coverage: for EACH of the 11 contracts (Layout, Navigation, Card, Table,
   Dialog, Notification, Badge, Avatar, Tabs, Breadcrumb, Chart) state whether the theme
   provides a visual reference and the expected adapter effort (S/M/L).
4. GREEN / YELLOW / RED classification table for every capability and asset
   (layout, nav, components, tokens, icons, charts, auth, routing, state, services, demo).
5. Token extraction plan: which tokens map to --<id>-* custom properties; light + dark.
6. Decoupling assessment: confirm visuals render WITHOUT the theme's Redux/router/auth/
   services. Call out anything welded to app logic (RED) and how the adapter avoids it.
7. Adapter effort estimate: rough size per contract + total; key risks.
8. Recommendation: PROCEED / PROCEED WITH CAVEATS / REJECT, with reasons.

OUTPUT: the markdown file above, with concrete tables.
```

---

**Exit criteria:** every capability classified GREEN/YELLOW/RED; no unresolved RED on a required contract; a clear PROCEED/REJECT recommendation. Then move to [theme-adapter-prompt.md](./theme-adapter-prompt.md).
