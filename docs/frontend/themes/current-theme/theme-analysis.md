# HexaDash — Theme Analysis

**Status:** Analysis only — no integration performed.  
**Analyzed path:** `C:\themeforest\hexadash-react\hexadash-react\`  
**Date:** June 2026

---

## 1. Theme name

**HexaDash** (`HexaDash` npm package name), version **1.3.0**, by **SovWare**.  
Distributed as a ThemeForest multi-framework admin dashboard bundle.

---

## 2. Technology stack

| Layer | Technology |
|-------|------------|
| UI framework | React 18.3.1 |
| Build tool | Create React App 5 + CRACO 7 (`craco-less` for Ant Design theming) |
| Language | JavaScript (`.js` / `.jsx`) — no TypeScript |
| Component library | **Ant Design (antd) 5.27.3** |
| CSS-in-JS | styled-components 5.3.3 |
| Global styles | SCSS (`sass`) + compiled `static/css/style.css` |
| Routing | react-router-dom 7.9.1 |
| Server state (demo) | Redux slices + Axios `DataService` |
| Client state | Redux Toolkit + redux-thunk |
| Backend demos | Firebase/Firestore, Auth0 Lock, mock REST |
| i18n | i18next + react-i18next (en, ar, esp) |
| Icons | @iconscout/react-unicons, feather-icons-react |
| Testing | @testing-library/react, Jest (CRA default) |

### Folder structure (React full demo)

```
hexadash-react/
├── boilerplate/          # Stripped starter (same architecture, fewer pages)
├── documentation/        # In-theme documentation SPA
└── hexadash-react/       # Full demo application ← analyzed
    ├── public/
    ├── craco.config.js
    ├── customize-cra-config.js
    └── src/
        ├── App.js                 # Root providers + auth gate
        ├── components/            # Reusable UI (buttons, cards, tables, modals…)
        ├── config/                # Theme tokens, API, Firebase, icons
        ├── container/             # Page-level feature modules (~357 .js files)
        ├── demoData/              # Static JSON fixtures
        ├── hooks/
        ├── i18n/
        ├── layout/                # Shell: sidebar, header, menu, HOC
        ├── redux/                 # ~25 feature slices + theme layout
        ├── routes/                # auth.js + admin/* route modules
        ├── static/                # img/, css/
        └── utility/
```

**Scale:** ~966 source files under `src/`.

---

## 3. React version

| | HexaDash | Ashraak |
|---|----------|---------|
| React | **18.3.1** | **19.1.0** |
| react-dom | 18.3.1 | 19.1.0 |

**Gap:** One major version. HexaDash has not been validated on React 19. Concurrent features, stricter hydration, and deprecated APIs may require dependency updates before use in Ashraak.

---

## 4. Bootstrap version

**Bootstrap is not used as a UI framework in the React variant.**

| Finding | Detail |
|---------|--------|
| `bootstrap` npm package | Not present in `package.json` |
| Grid system | Ant Design `Row` / `Col` |
| Layout | Ant Design `Layout`, `Header`, `Sider`, `Content` |
| External CSS | Font Awesome **4.7.0** via CDN in `public/index.html` only |

Marketing copy and demo search data reference "Bootstrap Design UI" conceptually, but the implementation is Ant Design–centric.

Ashraak uses **CoreUI 5** (Bootstrap 5–based SCSS). This is a **fundamental UI kit mismatch**.

---

## 5. UI libraries

| Library | Role |
|---------|------|
| **antd 5.27.3** | Primary component system (forms, tables, modals, menus, layout) |
| **styled-components** | Layout shell styling (`layout/Style.js`, auth pages, cards) |
| **@ant-design/colors** | Token generation (devDependency) |
| **craco-less** | Ant Design Less variable overrides at build time |
| Custom SCSS/CSS | `static/css/style.css`, component-level styles |
| **@iconscout/react-unicons** | Sidebar and header icons |
| **@uiw/react-md-editor** | Markdown editor pages |
| **swiper**, **react-big-calendar**, **leaflet** | Feature-specific widgets |

---

## 6. Chart libraries

HexaDash ships **four** charting stacks (demo pages for each):

| Library | Package | Route prefix |
|---------|---------|--------------|
| ApexCharts | `apexcharts` + `react-apexcharts` | `/admin/charts/apexcharts` |
| Chart.js | `chart.js` + `react-chartjs-2` | `/admin/charts/chartjs` |
| Recharts | `recharts` | `/admin/charts/recharts/*` |
| Google Charts | `react-google-charts` | `/admin/charts/google-chart` |

Dashboard demo pages embed charts inline (10 dashboard layout variants: `demo-2` … `demo-10`).

Ashraak currently has **no chart library** in `package.json`. Charts would be net-new dependencies regardless of theme choice.

---

## 7. State management

| Concern | HexaDash approach |
|---------|-------------------|
| Global store | Redux (`createStore`) + `combineReducers` — **not** RTK `configureStore` at root |
| Async | redux-thunk, RTK `createAsyncThunk` in `authSlice` |
| Auth state | `redux/authentication/authSlice.js` — `localStorage` flags |
| Layout/theme | `redux/themeLayout/` — mode, RTL, topMenu |
| Feature demos | 20+ slices (chat, email, kanban, ecommerce, firebase CRUD, etc.) |
| Firebase | `react-redux-firebase` + `redux-firestore` wired in `redux/store.js` |

Ashraak uses **Zustand** for auth and **TanStack Query** for server state. HexaDash's Redux/Firebase architecture should **not** be adopted wholesale.

---

## 8. Routing approach

```
App.js
├── !isLoggedIn → routes/auth.js (public)
└── isLoggedIn  → routes/admin/index.js wrapped in withAdminLayout
                    └── nested <Routes> per feature area
```

| Pattern | Implementation |
|---------|----------------|
| Router | `BrowserRouter` with `basename={process.env.PUBLIC_URL}` |
| Code splitting | `React.lazy` + `Suspense` on route modules |
| Auth gate | Redux `state.auth.login` boolean — not route guards |
| Protected routes | `ProtectedRoute` HOC (checks login flag) |
| Layout injection | HOC: `withAdminLayout(Component)`, `AuthLayout(Component)` |
| 404 | `container/pages/404` |

**Admin route map** (partial): dashboard, pages, gallery, components, charts, tables, widgets, users, ecommerce, email, chat, calendar, kanban, maps, firebase CRUD, axios CRUD, projects, profile, jobs, support tickets, courses, import/export.

Ashraak uses `createBrowserRouter` + `AuthGuard` / `RoleGuard` / `PermissionGuard` with a centralized `routeMap`. HexaDash has no role/permission model.

---

## 9. Authentication pages

| Route | Component | Notes |
|-------|-----------|-------|
| `/` (index) | `SignIn.js` | Email/password demo form |
| `/register` | `Signup.js` | Registration |
| `/forgotPassword` | `ForgotPassword.js` | Password reset UI |
| `/fbSignIn` | `FbSignIn.js` | Firebase auth demo |
| `/fbRegister` | `FbSignup.js` | Firebase registration demo |

**Auth layout:** `AuthLayout` HOC — centered card on `admin-bg-light.png` background, brand logo.

**Auth backends supported (demo):**

- Mock REST (`DataService.post('/login')`) → `localStorage` token
- Firebase (`react-redux-firebase`)
- Auth0 (`auth0-lock` dependency, slice support in `authSlice`)

Ashraak auth pages use **React Hook Form + Zod**, JWT from Ashraak .NET API, MFA/invitation flows. Only **visual layout** of HexaDash auth screens is potentially reusable.

---

## 10. Layout architecture

See [layout-analysis.md](./layout-analysis.md) for full detail.

Summary: **Ant Design Layout** inside a **class-based HOC** (`withAdminLayout`). Fixed header (63px offset), optional fixed sidebar (280px), scrollable content area, footer bar.

Two layout modes controlled by Redux:

- **Side menu** (default) — collapsible `Sider` + vertical `MenueItems`
- **Top menu** — horizontal `TopMenu` in header (desktop ≥ 992px)

---

## 11. Sidebar architecture

- Ant Design `Sider` width **280px**, `theme` prop toggles light/dark
- Custom scrollbar (`@pezhmanparsaee/react-custom-scrollbars`)
- Menu defined in `layout/MenueItems.js` (~1,400+ lines) using Ant `Menu` + `NavLink`
- Collapse triggers: hamburger in header; auto-collapse ≤ 1200px; mobile overlay ≤ 991px
- Active key derived from URL path segments under `/admin`

---

## 12. Header architecture

Fixed `Header` containing:

| Zone | Content |
|------|---------|
| Left | Logo (light/dark SVG swap), sidebar collapse button |
| Center (top-menu mode) | `TopMenu` horizontal navigation |
| Right | `AuthInfo` — search, messages, notifications, language flag, settings panel, user dropdown |
| Mobile | Search icon + ellipsis menu revealing `AuthInfo` |

`AuthInfo` dispatches `logoutUser` and navigates to `/`.

---

## 13. Theme switching support

Controlled by `redux/themeLayout` slice:

| Setting | State key | Values |
|---------|-----------|--------|
| Color mode | `mode` | `lightMode` \| `darkMode` |
| Menu position | `topMenu` | `true` \| `false` |
| Text direction | `rtlData` | `true` \| `false` |

Settings UI in header (`components/utilities/auth-info/settings.js`) dispatches `changeLayoutMode`, `changeMenuMode`, `changeDirectionMode`.

Theme tokens live in:

- `config/theme/themeVariables.js` — color palette, Ant Design token map
- `config/theme/themeConfigure.js` — light/dark semantic backgrounds
- `craco.config.js` — Less `modifyVars` at compile time
- `updateCSSVariables()` — runtime CSS custom property injection

---

## 14. Dark mode support

**Yes — first-class support.**

Mechanism (layered):

1. Redux `mode` → `lightMode` or `darkMode`
2. `updateCSSVariables(isDark)` sets CSS custom properties on `:root`
3. Ant Design `Sider` `theme` prop switches `light` / `dark`
4. styled-components `ThemeProvider` passes `mainContent` to descendants
5. Logo swaps between `logo_dark.svg` and `logo_white.svg`
6. Scrollbar thumb colors adapt to mode

Not using `prefers-color-scheme` or `data-*` attribute convention (unlike CoreUI's `data-coreui-theme`).

---

## 15. Mobile responsiveness

| Breakpoint | Behaviour |
|------------|-----------|
| ≤ 1200px | Sidebar auto-collapsed on mount |
| ≤ 991px | Top menu hidden; sidebar overlay with `.ninjadash-shade` backdrop; mobile header actions |
| Ant `Col` grid | `xs` / `sm` / `md` / `lg` responsive columns throughout forms and pages |

Touch-friendly collapse toggle and full-screen auth layout on small screens.

---

## Theme engine summary

HexaDash does **not** have a pluggable theme engine. Theming is:

1. **Compile-time** — CRACO Less variable overrides
2. **Runtime** — Redux-driven mode switch + CSS variables + styled-components context
3. **Component-level** — Ant Design `ConfigProvider` for RTL direction

Future theme replacement requires abstracting these three layers behind an adapter (see [migration-recommendations.md](./migration-recommendations.md)).

---

## Risk summary

| Risk | Severity |
|------|----------|
| Ant Design vs CoreUI component mismatch | High |
| Redux/Firebase demo state vs Zustand/Query | High |
| CRA vs Vite build pipeline | Medium |
| React 18 vs 19 | Medium |
| JavaScript vs TypeScript | Medium |
| Monolithic menu file maintenance | Medium |
| Large dependency surface (Firebase, maps, ecommerce demos) | Medium |
| Demo auth (localStorage) vs production JWT security model | High |
