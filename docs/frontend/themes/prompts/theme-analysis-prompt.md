# Reusable Prompt — Theme Analysis

**When to use:** Lifecycle step 2 ([analysis](../theme-lifecycle.md#2-theme-analysis)) / onboarding step 2. Produces `docs/frontend/themes/<theme>/theme-analysis.md`.

**How to use:** Copy the block below, replace `<THEME_NAME>` / `<PATH_TO_EXTRACTED_THEME>`, and run it against the extracted (reference-only) theme source.

---

```
You are analysing a purchased UI theme for onboarding into the Ashraak React 19 web app
via the Theme Adapter Architecture (see docs/frontend/themes/theme-decision-record.md).

THEME: <THEME_NAME>
SOURCE (reference only, DO NOT import into src/): <PATH_TO_EXTRACTED_THEME>

RULES
- Analysis and documentation ONLY. Do NOT modify app code, install the theme, or import it.
- The platform reuses VISUAL DESIGN only. Flag anything that is application logic.

PRODUCE docs/frontend/themes/<THEME_NAME>/theme-analysis.md covering:

1. Overview: theme name, version, author, licence, intended use.
2. Tech stack: framework + version (React major?), language (TypeScript?), UI kit
   (Bootstrap/Ant/MUI/Tailwind/custom), styling method (SCSS / CSS / CSS-in-JS),
   build tooling assumptions, bundled libraries (Redux, router, charts, icons, forms).
3. Layout / shell: structure of the authenticated shell (sidebar, header, content, footer)
   and any auth/login layout. Note responsive behaviour and collapse logic.
4. Navigation: menu structure, active state, grouping, icon system.
5. Component inventory — for each, note presence + variants:
   cards, tables, forms/inputs, dialogs/modals, notifications/toasts, badges, avatars,
   tabs, breadcrumbs, charts, pagination, loaders, empty/error states.
6. Design tokens: extract colours (incl. semantic variants), typography (families, sizes,
   weights), spacing scale, border radius, shadows, breakpoints. Note light + dark values.
7. Theming mechanism: how light/dark is toggled; CSS variables vs hardcoded values.
8. Application logic baked into the theme (CRITICAL): auth, routing, state/Redux,
   API/services, demo data/pages — list everything that is NOT pure presentation.
9. Dependencies: full list with versions; flag any that conflict with our stack
   (React 19, TanStack Query, Zustand, react-router, react-hook-form + zod, Vite 6).
10. Risks & first impressions: what looks easy (GREEN), adaptable (YELLOW), forbidden (RED).

OUTPUT: the markdown file above. Be concrete and cite file paths from the source.
```

---

**Exit criteria:** `theme-analysis.md` exists, covers all 10 sections, and explicitly separates *visual design* (reusable) from *application logic* (forbidden). Feed it into [theme-compatibility-prompt.md](./theme-compatibility-prompt.md).
