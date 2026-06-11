# Theme Selection Checklist

**Use this BEFORE purchasing or committing to any theme.** It is the acquisition gate in the [theme lifecycle](./theme-lifecycle.md#1-theme-acquisition). A theme that fails a **hard requirement** should be rejected or renegotiated — adapting around it costs more than buying a compatible theme.

**Legend:** ⛔ Hard requirement (fail = reject) · ⚠️ Strong preference (fail = added adapter cost) · ℹ️ Nice to have.

---

## 1. Platform compatibility (technical)

| ✓ | Check | Severity | Why it matters |
|---|-------|----------|----------------|
| ☐ | Built for **React** (not Vue/Angular/HTML-only) | ⛔ | The app is React 19; non-React themes can't supply components |
| ☐ | Compatible with our **React major (19)** | ⛔ | Mismatched React majors break hooks/runtime; we never skip React majors |
| ☐ | Ships **TypeScript** types (or is trivially typeable) | ⛔ | `ThemeAdapter` is type-checked; `any` is forbidden by repo rules |
| ☐ | Styling is **SCSS/CSS** (Bootstrap-family) or token-extractable | ⚠️ | Easiest to scope under `.<id>-theme`; CSS-in-JS is adaptable but heavier |
| ☐ | **Bootstrap 5 / CoreUI 5** compatible (or Bootstrap-neutral) | ⚠️ | Modules use Bootstrap utility classes today; a Bootstrap-family theme minimizes interior-content friction |
| ☐ | **Vite-buildable** (no webpack-only tooling assumptions) | ⚠️ | The web app builds with Vite 6 |
| ☐ | Design **tokens are extractable** (colours, spacing, typography, radius, shadows, breakpoints) | ⚠️ | Tokens are the unit of reuse for the adapter |
| ☐ | Provides **light AND dark** palettes (or derivable) | ⚠️ | Dark mode is a validated requirement |

## 2. Decoupling (no application-logic coupling) — all ⛔/⚠️

The adapter reuses **visual design only**. Anything below that is welded into the theme is a 🔴 RED zone we must be able to ignore.

| ✓ | Check | Severity | Why it matters |
|---|-------|----------|----------------|
| ☐ | **No Redux coupling** — visuals don't require the theme's store to render | ⛔ | We use Zustand + TanStack Query; importing the theme's Redux is forbidden |
| ☐ | **No routing coupling** — components don't hard-depend on the theme's router | ⛔ | We own routing (`react-router` + guards); themes must not dictate routes |
| ☐ | **No auth coupling** — login/guards/session are not baked into the shell | ⛔ | Auth/permissions are platform-owned and untouchable |
| ☐ | **No API/service coupling** — components don't call the theme's data services | ⛔ | Data flow is ours (TanStack Query + our API client) |
| ☐ | Layout/components usable **without demo data wiring** | ⚠️ | Demo pages/state are 🔴 RED and must be discardable |
| ☐ | Icons are **swappable** (not a hard vendor lock) | ⚠️ | We map a stable `NavigationIconKey` to glyphs per adapter |
| ☐ | Charts (if any) are **optional / replaceable** | ℹ️ | `ChartContract` is library-agnostic; no chart lib is mandated |

## 3. Surface coverage (can it satisfy the 11 contracts?)

The theme should provide a visual reference for each contract. Missing references are buildable but add design work.

| ✓ | Contract | Theme should offer |
|---|----------|--------------------|
| ☐ | `LayoutContract` | App shell (sidebar + header + content + footer) **and** an auth/centered layout |
| ☐ | `NavigationContract` | Sidebar/top nav with active + grouped sections |
| ☐ | `CardContract` | Content card with header/body/footer |
| ☐ | `TableContract` | Data table styling (header, rows, hover, responsive) |
| ☐ | `DialogContract` | Modal/dialog (sizes, centered, header/footer) |
| ☐ | `NotificationContract` | Toast/snackbar visual style |
| ☐ | `BadgeContract` | Status/category pills with semantic colours |
| ☐ | `AvatarContract` | Avatar (image + initials fallback, sizes) |
| ☐ | `TabsContract` | Tab strip + active panel |
| ☐ | `BreadcrumbContract` | Breadcrumb trail with current-page state |
| ☐ | `ChartContract` | Chart frame styling (actual rendering optional) |

## 4. Quality, licence & maintenance

| ✓ | Check | Severity |
|---|-------|----------|
| ☐ | Licence permits adapting the design into our commercial product | ⛔ |
| ☐ | Accessible patterns (semantic HTML, focus states, ARIA) present or addable | ⚠️ |
| ☐ | Responsive across desktop / tablet / mobile | ⚠️ |
| ☐ | Actively maintained / documented | ⚠️ |
| ☐ | Reasonable asset weight (fonts/icons/CSS) for our bundle budget | ⚠️ |
| ☐ | No mandatory paid runtime services to render | ⛔ |

---

## Decision rule

- **Any ⛔ unchecked → REJECT** (or renegotiate the requirement away).
- **Several ⚠️ unchecked → proceed only with a sized adapter-cost estimate** in the compatibility report.
- **All ⛔ + most ⚠️ checked → APPROVE for analysis** → continue to [theme-onboarding-guide.md](./theme-onboarding-guide.md).

> Record the completed checklist in the theme's `platform-compatibility-report.md`. The checklist result is part of the acquisition exit criteria in the [lifecycle](./theme-lifecycle.md).
