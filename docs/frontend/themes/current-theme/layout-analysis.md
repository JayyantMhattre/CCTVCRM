# HexaDash — Layout Analysis

**Source:** `hexadash-react/hexadash-react/src/layout/`  
**Related:** `container/profile/authentication/Index.js` (auth shell)

---

## Overview

HexaDash uses a **Higher-Order Component (HOC)** pattern to inject the admin shell around route content. There is no layout route object (unlike Ashraak's React Router `AppLayout` outlet pattern).

```
┌─────────────────────────────────────────────────────────────┐
│  Header (fixed, full width, z-index above sidebar)          │
│  [Logo] [Collapse]     [TopMenu?]     [Search|Bell|User]    │
├──────────┬──────────────────────────────────────────────────┤
│          │                                                  │
│  Sider   │  Content (scrollable main area)                  │
│  280px   │  ┌────────────────────────────────────────────┐  │
│  fixed   │  │  <WrappedComponent />  (page outlet)       │  │
│          │  └────────────────────────────────────────────┘  │
│  Menu    │  Footer (copyright + links)                      │
│          │                                                  │
└──────────┴──────────────────────────────────────────────────┘
```

Mobile (≤ 991px): sidebar becomes overlay; semi-transparent `.ninjadash-shade` backdrop closes it on click.

---

## Admin layout — `withAdminLayout.js`

**File:** `src/layout/withAdminLayout.js`  
**Pattern:** `ThemeLayout(WrappedComponent)` → connected class component

### Structural components

| Element | Ant Design / custom | Details |
|---------|---------------------|---------|
| Outer wrapper | `LayoutContainer` (styled) | Full-page container |
| Header | `Layout.Header` | `position: fixed`, `top: 0`, full width |
| Sidebar | `Layout.Sider` | `width={280}`, `position: fixed`, `height: 100vh`, `margin-top: 63px` |
| Content | `Layout.Content` | Inside `atbd-main-layout` class |
| Footer | `FooterStyle` (styled `Row`) | Copyright + nav links |

### State (local component state)

| State | Purpose |
|-------|---------|
| `collapsed` | Sidebar expanded/collapsed |
| `hide` | Mobile auth-info panel visibility |

### State (Redux-connected props)

| Prop | Redux path | Effect |
|------|------------|--------|
| `layoutMode` | `ChangeLayoutMode.mode` | Light/dark sider theme, logo variant |
| `rtl` | `ChangeLayoutMode.rtlData` | Flips sidebar position left/right |
| `topMenu` | `ChangeLayoutMode.topMenu` | Hides sider, shows horizontal menu |

### Responsive behaviour

```javascript
// Auto-collapse on mount
collapsed: window.innerWidth <= 1200 && true

// Mobile toggle only below 990px
toggleCollapsedMobile() { if (window.innerWidth <= 990) ... }

// Top menu only when topMenu && width > 991
```

### Scrollbar

Custom `Scrollbars` component inside `Sider` with mode-aware thumb colors.

---

## Auth layout — `authentication/Index.js`

**Pattern:** `AuthLayout(WraperContent)` HOC

```
┌────────────────────────────────────────┐
│  Full-viewport background image        │
│  (admin-bg-light.png)                  │
│                                        │
│     ┌──────────────────────────┐       │
│     │  Logo (logo_dark.svg)    │       │
│     │  ┌────────────────────┐  │       │
│     │  │  Auth form card    │  │       │
│     │  │  (Login/Register…) │  │       │
│     │  └────────────────────┘  │       │
│     └──────────────────────────┘       │
└────────────────────────────────────────┘
```

- Wrapped in `Suspense` with Ant `Spin` fallback
- No header/sidebar — isolated public shell
- Styled via `AuthenticationWrap` in `overview/style.js`

Ashraak equivalent: `layouts/AuthLayout.tsx` (CoreUI-centered card, minimal chrome).

---

## Header architecture

**Component tree:**

```
Header
└── .ninjadash-header-content (flex)
    ├── __left
    │   └── .navbar-brand
    │       ├── Link → /admin (logo)
    │       └── Button (collapse toggle)
    ├── __right
    │   ├── .ninjadash-navbar-menu → TopMenu (conditional)
    │   └── .ninjadash-nav-actions → AuthInfo
    └── __mobile
        └── Search + ellipsis → SmallScreenAuthInfo
```

### Header zones

| Zone | Desktop | Mobile |
|------|---------|--------|
| Brand | Logo swaps by `layoutMode` | Same |
| Navigation | `TopMenu` when `topMenu && width > 991` | Hidden |
| Utilities | `AuthInfo` (search, messages, notifications, i18n, settings, user) | Collapsed into ellipsis drawer |
| Collapse | Sidebar toggle button | Same + overlay shade |

### `AuthInfo` (`components/utilities/auth-info/info.js`)

| Feature | Implementation |
|---------|----------------|
| Search | `Search` component with popover |
| Messages | `Message` dropdown |
| Notifications | `Notification` dropdown |
| Language | Flag selector → `i18n.changeLanguage` |
| Settings | `Settings` panel (layout mode, RTL, menu mode) |
| User menu | Avatar dropdown — profile links, sign out |
| Sign out | `dispatch(logoutUser())` → navigate `/` |

All user data is **hard-coded demo content** (e.g. "Abdullah Bin Talha").

---

## Sidebar architecture

**File:** `layout/MenueItems.js` (exported as default, imported as `MenueItems`)

| Property | Value |
|----------|-------|
| Width | 280px (Ant `Sider`) |
| Menu component | Ant Design `Menu` |
| Icons | `@iconscout/react-unicons` |
| Labels | `react-i18next` `t()` keys |
| Link component | `react-router-dom` `NavLink` |
| Open/selected keys | Derived from URL path under `/admin` |

### Menu structure (top-level groups)

- Dashboard (10 demo variants)
- Layout (light/dark, RTL, top menu toggles — duplicates settings)
- Changelog
- Application (chat, email, calendar, contact, e-commerce, etc.)
- Pages (profile, settings, pricing, breadcrumbs, knowledge base, etc.)
- Components (50+ UI element demos)
- Charts
- Maps
- Icons
- Tables
- Forms
- Users
- Widgets
- Firebase / Axios CRUD demos

**Maintenance note:** Single 1,400+ line file with inline `getItem()` tree. Not modular per feature.

### Sidebar vs Ashraak `AppLayout`

| Aspect | HexaDash | Ashraak (CoreUI) |
|--------|----------|------------------|
| Width | 280px fixed | 16rem (`--cui-sidebar-occupy-start`) |
| Narrow mode | Collapse to icons | `sidebar-narrow` (4rem) via toggler |
| Nav definition | Monolithic `MenueItems.js` | Inline `<SidebarNavLink>` per route in `AppLayout.tsx` |
| Access control | None | `RoleGuard`, `PermissionGuard` |
| Scroll | Custom scrollbars | Native / SimpleBar potential |
| CSS classes | `ninjadash-*` BEM-like | CoreUI `sidebar`, `sidebar-nav` |

---

## Content area

Pages render inside `Layout.Content` without an additional container wrapper at the layout level. Individual pages use:

- `container/styled.js` — `Main` wrapper
- `components/cards/frame/cards-frame.js` — `Cards` title wrapper
- `components/page-headers/` — breadcrumb headers

Footer is **always visible** at bottom of content (not sticky to viewport).

---

## Styling approach

| Layer | Files | Purpose |
|-------|-------|---------|
| Global CSS | `static/css/style.css` | Base HexaDash / ninjadash classes |
| styled-components | `layout/Style.js` | Layout shell, footer, top menu |
| Ant Design Less | `craco.config.js` | Token overrides at build |
| Runtime variables | `config/theme/themeVariables.js` | `updateCSSVariables()` |
| Semantic tokens | `config/theme/themeConfigure.js` | Light/dark background sets |

Primary brand color: **`#8231D3`** (purple).  
Font family: **Jost** (referenced in theme tokens).

---

## Layout mode switching flow

```
User opens Settings panel (AuthInfo)
    → dispatch(changeLayoutMode('darkMode'))
        → Redux ChangeLayoutMode.mode updated
            → withAdminLayout re-renders (logo, sider theme)
            → App.js useEffect → updateCSSVariables(true)
            → styled-components ThemeProvider receives mainContent
```

---

## Comparison with Ashraak AppLayout

Ashraak (`FrontEnd/apps/web/src/layouts/AppLayout.tsx`):

- CoreUI HTML structure: `.sidebar` outside `.wrapper`
- Dark mode via `document.documentElement.setAttribute('data-coreui-theme', 'dark')`
- Permission-aware nav links
- User footer card in sidebar
- Breadcrumb/title in header sticky bar

**Visual similarity:** Both use fixed sidebar + sticky header + content outlet.  
**Implementation similarity:** Low — different component libraries, state mechanisms, and nav models.

---

## Layout reuse assessment

| Asset | Reuse potential |
|-------|-----------------|
| Visual design (spacing, colors, card style) | Medium — extract as design tokens |
| `withAdminLayout` HOC | Low — tightly coupled to Ant Design + Redux |
| `MenueItems.js` structure | Low — must be rewritten for Ashraak routes/permissions |
| Auth layout background/card | Medium — visual reference only |
| Responsive breakpoint logic | Medium — patterns portable to CoreUI shell |
| Footer component | High — simple markup, easy to replicate |
