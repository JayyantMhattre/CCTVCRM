# CoreUI Free Bootstrap Admin Template — Integration Guide

> **Source template**: [coreui/coreui-free-bootstrap-admin-template](https://github.com/coreui/coreui-free-bootstrap-admin-template) v5.4.0  
> **Strategy**: CSS-only integration — CoreUI's SCSS is compiled into our Vite build; no CoreUI JS is loaded (React handles all interactivity).

---

## 1 · Why CoreUI instead of plain Bootstrap?

| Feature | plain Bootstrap 5 | `@coreui/coreui` |
|---|---|---|
| Grid + utilities | ✓ | ✓ (superset) |
| Sidebar component | ✗ | ✓ `.sidebar`, `.sidebar-nav` |
| Header / Footer | ✗ | ✓ `.header`, `.footer` |
| Dark mode CSS | partial | ✓ `data-coreui-theme="dark"` |
| CSS custom properties | `--bs-*` | `--cui-*` |
| Admin template classes | ✗ | ✓ `.wrapper`, `.body`, etc. |

`@coreui/coreui` is a strict **superset of Bootstrap 5** — every Bootstrap class and component continues to work unchanged.

---

## 2 · Packages added / changed

```jsonc
// apps/web/package.json
"dependencies": {
  "@coreui/coreui":       "^5.6.1",   // ← CSS library (replaces `bootstrap`)
  "@coreui/icons":        "^3.0.1",   // ← 500+ SVG icon definitions
  "@coreui/icons-react":  "^2.3.0",   // ← <CIcon icon={cilXxx} /> React component
  "simplebar":            "^6.3.3",   // ← custom scrollbar (used by template)
  // react-bootstrap stays — its classes are Bootstrap-compatible with CoreUI
}
"devDependencies": {
  "sass":  "^1.98.0"   // ← Vite compiles our .scss files
}
```

`bootstrap` (plain CSS package) was **removed** — `@coreui/coreui` already includes all Bootstrap 5 CSS internally.

---

## 3 · File changes

```
FrontEnd/apps/web/
├── src/
│   ├── styles/
│   │   ├── coreui.scss         ← NEW: main entry (replaces bootstrap CSS import)
│   │   └── _variables.scss     ← NEW: Sass variable overrides ($primary, fonts…)
│   ├── main.tsx                ← CHANGED: import './styles/coreui.scss'
│   ├── layouts/
│   │   ├── AppLayout.tsx       ← REWRITTEN: CoreUI .sidebar / .header / .wrapper
│   │   └── AuthLayout.tsx      ← REWRITTEN: CoreUI login card layout
│   ├── shared/components/
│   │   ├── Spinner.tsx         ← UPDATED: CoreUI spinner classes + better a11y
│   │   ├── AlertMessage.tsx    ← UPDATED: CIcon for variant icons
│   │   └── PageHeader.tsx      ← UPDATED: text-body / text-body-secondary classes
│   └── modules/dashboard/
│       └── DashboardPage.tsx   ← UPDATED: card-stat class + CIcon icons
```

---

## 4 · CoreUI CSS layout structure

The AppLayout follows the CoreUI template's exact HTML structure:

```html
<body>

  <!-- Fixed sidebar (position: fixed; left: 0) -->
  <div class="sidebar sidebar-dark sidebar-fixed border-end">
    <div class="sidebar-header border-bottom">
      <a class="sidebar-brand">Brand</a>
      <button class="sidebar-toggler"></button>
    </div>
    <div class="sidebar-body">
      <ul class="sidebar-nav">
        <li class="nav-item">
          <a class="nav-link active">
            <svg class="nav-icon">…</svg>
            Dashboard
          </a>
        </li>
      </ul>
    </div>
  </div>

  <!-- Wrapper shifts right via CSS variable set by React state -->
  <div class="wrapper d-flex flex-column min-vh-100">
    <header class="header header-sticky p-0 mb-4">…</header>
    <div class="body flex-grow-1 px-3">
      <div class="container-lg px-4">
        …page content (React Router <Outlet />)…
      </div>
    </div>
    <footer class="footer">…</footer>
  </div>

</body>
```

### How the sidebar shift works

CoreUI uses a CSS custom property on the wrapper:

```css
.wrapper {
  padding-inline-start: var(--cui-sidebar-occupy-start, 0);
  transition: padding 0.15s;
}
```

`AppLayout.tsx` sets `--cui-sidebar-occupy-start` via React `useEffect`:

```ts
// Sidebar expanded (desktop): 16rem
document.documentElement.style.setProperty('--cui-sidebar-occupy-start', '16rem');

// Sidebar narrow (icon-only): 4rem
document.documentElement.style.setProperty('--cui-sidebar-occupy-start', '4rem');
```

---

## 5 · Dark mode

CoreUI v5 supports a full dark mode via the `data-coreui-theme` attribute on the root `<html>` element:

```ts
// Dark
document.documentElement.setAttribute('data-coreui-theme', 'dark');

// Light
document.documentElement.setAttribute('data-coreui-theme', 'light');
```

`AppLayout.tsx` exposes a toggle button in the topbar that switches the attribute.  
`coreui.scss` includes the `@include color-mode(dark) { … }` block with page-level dark overrides.

---

## 6 · Using CoreUI icons

Icons come from `@coreui/icons` (500+ SVG definitions) and are rendered via `@coreui/icons-react`:

```tsx
import { CIcon }              from '@coreui/icons-react';
import { cilSpeedometer }     from '@coreui/icons';

// Inside JSX:
<CIcon icon={cilSpeedometer} className="nav-icon" />

// With size:
<CIcon icon={cilPeople} size="xl" className="text-success" />
```

### Icon naming conventions

| Icon | Import name |
|---|---|
| Dashboard | `cilSpeedometer` |
| Building / Tenant | `cilBuilding` |
| Settings / Gear | `cilSettings` |
| People / Users | `cilPeople` |
| Audit / Document | `cilDescription` |
| Sidebar toggle | `cilMenu`, `cilChevronLeft` |
| Dark mode | `cilMoon`, `cilSun` |
| Logout | `cilAccountLogout` |
| User | `cilUser` |

Browse all icons at [coreui.io/icons](https://coreui.io/icons/).

---

## 7 · SCSS customisation

### Changing brand colours

Edit `src/styles/coreui.scss` → `@use "@coreui/coreui/scss/coreui" as * with (...)`:

```scss
@use "@coreui/coreui/scss/coreui" as * with (
  $primary:  #3b5fc0,   // ← change brand colour here
  $success:  #2eb85c,
  $danger:   #e55353,
  ...
);
```

### Adding app-specific CSS

Add new rules **after** the `@use` block in `coreui.scss`:

```scss
// e.g. custom card variant
.card-invoice {
  border-left: 4px solid var(--cui-primary);
  background-color: var(--cui-primary-bg-subtle);
}
```

### Dark mode overrides

```scss
@include color-mode(dark) {
  .card-invoice {
    background-color: var(--cui-dark-bg-subtle);
  }
}
```

---

## 8 · Adding a new sidebar nav item

1. Import the desired icon from `@coreui/icons`.
2. Add a `<SidebarNavLink>` inside `AppLayout.tsx → <SidebarNavContent>`.
3. Wrap in `<RoleGuard>` or `<PermissionGuard>` if access-controlled.

```tsx
// In AppLayout.tsx, inside <ul class="sidebar-nav">:
import { cilChart } from '@coreui/icons';

<SidebarNavTitle>Reports</SidebarNavTitle>
<SidebarNavLink
  to={ROUTES.reports.list}
  icon={cilChart}
  label="Reports"
/>
```

---

## 9 · CSS variable reference

CoreUI v5 uses `--cui-*` CSS custom properties (vs Bootstrap's `--bs-*`):

| Variable | Purpose |
|---|---|
| `--cui-primary` | Brand primary colour |
| `--cui-body-bg` | Page background |
| `--cui-body-color` | Default text |
| `--cui-body-secondary` | Secondary text |
| `--cui-tertiary-bg` | Light surface (page background) |
| `--cui-sidebar-width` | Desktop sidebar width |
| `--cui-sidebar-occupy-start` | Wrapper left padding |
| `--cui-card-bg` | Card background |
| `--cui-border-color` | Default border |

---

## 10 · Troubleshooting

| Symptom | Fix |
|---|---|
| SCSS compilation fails | Run `npm install` — `sass` devDependency may be missing |
| Icons show as blank squares | Check `@coreui/icons-react` import path and version |
| Sidebar not visible on desktop | Ensure `--cui-sidebar-occupy-start` is set to `16rem` on `<html>` |
| Dark mode not toggling | Ensure `data-coreui-theme` is set on `document.documentElement` |
| `react-bootstrap` components unstyled | They rely on Bootstrap classes which CoreUI includes — check CSS load order |
