# HexaDash Theme — Analysis Index

**Analysis phase only.** No application code was modified. No theme integration was performed.

## Theme location

| Item | Value |
|------|-------|
| **Root path** | `C:\themeforest` |
| **Primary variant (React)** | `C:\themeforest\hexadash-react\hexadash-react\` |
| **Boilerplate variant** | `C:\themeforest\hexadash-react\boilerplate\` |
| **Theme name** | **HexaDash** (package name `HexaDash`) |
| **Version** | 1.3.0 |
| **Vendor** | SovWare (ThemeForest) |

## Bundle contents

The purchase includes multiple framework ports of the same design system:

| Folder | Framework |
|--------|-----------|
| `hexadash-react` | React (CRA + Ant Design) — **recommended reference for Ashraak** |
| `hexadash-react-tailwind` | React + Tailwind CSS |
| `hexadash-angular` | Angular |
| `hexadash-nextjs` | Next.js |
| `hexadash-vue` / `hexadash-typescript-vue` | Vue |
| `hexadash-svelte` | Svelte |
| `hexadash-html` | Static HTML |
| `hexadash-laravel` / `hexadash-django` / `hexadash-nodejs` | Backend-integrated variants |
| `Hexadash-tailwind-css` | Tailwind CSS assets |

Ashraak's frontend (`FrontEnd/apps/web`) is **React 19 + Vite + TypeScript + CoreUI 5**. The React Ant Design variant is the closest match for visual and structural analysis.

## Documentation files

| File | Purpose |
|------|---------|
| [theme-analysis.md](./theme-analysis.md) | Executive summary, technology stack, and high-level assessment |
| [layout-analysis.md](./layout-analysis.md) | Shell architecture: header, sidebar, content, footer |
| [navigation-analysis.md](./navigation-analysis.md) | Routing, menu system, guards, and URL structure |
| [component-analysis.md](./component-analysis.md) | Tables, forms, modals, charts, dashboards, auth UI |
| [dependency-analysis.md](./dependency-analysis.md) | Full dependency inventory and version comparison |
| [migration-recommendations.md](./migration-recommendations.md) | Reuse/wrap/avoid guidance and Theme Adapter Layer strategy |
| [platform-compatibility-report.md](./platform-compatibility-report.md) | Platform vs theme comparison, adapter design, phased plan T1–T6 |
| [layout-migration-report.md](./layout-migration-report.md) | T2 change log: layout rendering moved behind the theme adapter |
| [navigation-migration-report.md](./navigation-migration-report.md) | T3 change log: navigation ownership moved into the platform |
| [component-mapping-report.md](./component-mapping-report.md) | T4: component inventory + GREEN/YELLOW/RED mapping (analysis only) |
| [platform-ui-target-architecture.md](./platform-ui-target-architecture.md) | T4 design / T5 status: the final `Platform*` abstraction layer |
| [component-migration-plan.md](./component-migration-plan.md) | T4: migration waves 1–3, effort, and HexaDash readiness gate |
| [platform-ui-completion-report.md](./platform-ui-completion-report.md) | T5 change log: missing primitives + contracts + CoreUI adapters added |
| [hexadash-adapter-report.md](./hexadash-adapter-report.md) | T6 change log: HexaDash implemented as a second, opt-in theme adapter |
| [hexadash-validation-report.md](./hexadash-validation-report.md) | T7: full-platform validation under `VITE_THEME=hexadash` + P0–P3 gaps |
| [coreui-vs-hexadash-parity-report.md](./coreui-vs-hexadash-parity-report.md) | T7: visual/functional parity, missing capabilities, known limitations |
| [production-readiness-report.md](./production-readiness-report.md) | T7: performance review + Go/No-Go production readiness decision |

## Quick reference — report checklist

| # | Topic | Summary |
|---|-------|---------|
| 1 | Theme name | HexaDash 1.3.0 |
| 2 | Technology stack | React 18, CRA/CRACO, Ant Design 5, Redux, styled-components, SCSS |
| 3 | React version | **18.3.1** (Ashraak uses 19.1.0) |
| 4 | Bootstrap version | **Not bundled** — Ant Design grid/layout; Font Awesome 4.7 CDN only |
| 5 | UI libraries | Ant Design 5, styled-components, custom SCSS |
| 6 | Chart libraries | ApexCharts, Chart.js, Recharts, Google Charts |
| 7 | State management | Redux Toolkit + redux-thunk; Firebase/Firestore slices |
| 8 | Routing | React Router DOM v7, lazy routes, HOC layout wrappers |
| 9 | Authentication pages | Login, Register, Forgot Password, Firebase, Auth0-ready |
| 10 | Layout architecture | Ant `Layout` HOC (`withAdminLayout`), fixed header + collapsible sider |
| 11 | Sidebar architecture | 280px Ant `Sider`, custom scrollbar, 1400+ line menu config |
| 12 | Header architecture | Fixed top bar, logo, collapse toggle, search, notifications, user menu |
| 13 | Theme switching | Redux `ChangeLayoutMode` — light/dark, RTL, top-menu vs side-menu |
| 14 | Dark mode | CSS variables + Ant Design theme tokens + `data`-driven styled-components |
| 15 | Mobile responsiveness | Breakpoints at 991px and 1200px; overlay shade; mobile auth drawer |

## Ashraak platform context

Current Ashraak frontend (for migration comparison):

- **Package:** `@ashraak/web` at `FrontEnd/apps/web/`
- **Stack:** React 19, Vite 6, TypeScript 5, React Router 7, TanStack Query 5, Zustand 5, CoreUI 5
- **Auth:** JWT via Ashraak .NET API (`sessionStorage`, silent refresh)
- **Layout:** `AppLayout` (CoreUI sidebar + header pattern)

See [migration-recommendations.md](./migration-recommendations.md) for how these two stacks relate.

## Next steps (out of scope for this phase)

1. Decide adoption scope: visual skin only vs. full Ant Design migration.
2. Prototype Theme Adapter Layer interfaces against `AppLayout`.
3. Extract design tokens (colors, spacing, typography) into a token map.
4. Spike one page (e.g. Dashboard) behind the adapter without touching feature logic.
