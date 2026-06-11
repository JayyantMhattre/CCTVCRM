# HexaDash — Navigation Analysis

**Sources:** `src/routes/`, `src/layout/MenueItems.js`, `src/layout/TopMenu.js`, `src/App.js`

---

## Routing architecture

### Top-level split (authenticated vs public)

`App.js` gates the entire router on Redux `state.auth.login`:

```
isLoggedIn === false  →  <Route path="/*" element={<Auth />} />
isLoggedIn === true   →  <Route path="/admin/*" element={<ProtectedRoute Component={Admin} />} />
                        <Route path="*" element={<NotFound />} />
```

There is **no** nested public route for logged-in users (e.g. no `/login` while authenticated redirect).

Default redirect when logged in at `/`:

```javascript
<Route path="/" element={<Navigate to="/admin" />} />
```

`basename` is set to `process.env.PUBLIC_URL` (`/hexadash-react` in package.json `homepage`).

---

## Public routes — `routes/auth.js`

| Path | Component | Lazy |
|------|-----------|------|
| `/` (index) | `SignIn` | Yes |
| `/forgotPassword` | `ForgotPassword` | Yes |
| `/register` | `SignUp` | Yes |
| `/fbSignIn` | `FbSignIn` | Yes |
| `/fbRegister` | `FbSignup` | Yes |
| `*` | Redirect to `/` | — |

Wrapped by `AuthLayout` HOC (background + logo + centered card).

---

## Admin routes — `routes/admin/index.js`

All admin routes are prefixed `/admin` and wrapped by `withAdminLayout(Admin)`.

### Route modules

| Module file | Path prefix | Features |
|-------------|-------------|----------|
| `dashboard.js` | `/admin`, `/admin/demo-*` | 10 dashboard layouts |
| `pages.js` | `/admin/pages/*` | FAQ, gallery, pricing, 404, etc. |
| `components.js` | `/admin/components/*` | 50+ UI demos |
| `charts.js` | `/admin/charts/*` | Chart.js, Apex, Recharts, Google |
| `table.js` | `/admin/tables/*` | Basic table, data table |
| `users.js` | `/admin/users/*` | User list, add, grid, group |
| `ecommerce.js` | `/admin/ecommerce/*` | Products, cart, orders, sellers |
| `widgets.js` | `/admin/widgets/*` | Stat widgets |
| `features.js` | `/admin/features/*` | Forms, WYSIWYG, etc. |
| `projects.js` | `/admin/project/*` | Project management |
| `maps.js` | `/admin/maps/*` | Google, Leaflet, Vector |
| `icons.js` | `/admin/icons/*` | Feather, Unicons, Ant icons |
| `firebase.js` | `/admin/firebase/*`, `/admin/firestore/*` | Firestore CRUD |
| `axios.js` | `/admin/axios/*` | REST CRUD demo |
| `gallery.js` | `/admin/gallery/*` | Image gallery |

### Inline routes (in `admin/index.js`)

| Path | Feature |
|------|---------|
| `/admin/all-articles` | Knowledge base articles |
| `/admin/knowledgeBase/*` | Knowledge base |
| `/admin/app/task/*` | Task manager |
| `/admin/app/support/tickets/*` | Support tickets |
| `/admin/app/course/*` | LMS courses |
| `/admin/importExport/*` | Import/export |
| `/admin/app/to-do` | Todo list |
| `/admin/app/note/*` | Notes |
| `/admin/contact/*` | Contact list/grid/add |
| `/admin/app/calendar/*` | Calendar |
| `/admin/profile/myProfile/*` | User profile |
| `/admin/main/chat/*` | Chat app |
| `/admin/email/*` | Email inbox |
| `/admin/editor` | Markdown editor |
| `/admin/app/jobs/*` | Job board |

**Total route surface:** 100+ demo pages.

---

## Menu system

### Side menu — `MenueItems.js`

| Aspect | Detail |
|--------|--------|
| Component | Ant Design `Menu` with `items` tree |
| Icons | Unicons per item |
| i18n | All labels via `t('key')` |
| Active state | `selectedKeys` / `openKeys` synced to URL |
| Path base | Hard-coded `/admin` |
| Submenus | Nested `children` arrays |
| Layout toggles | Menu items dispatch Redux actions for dark/RTL/topMenu |

**URL → menu key mapping** includes special cases for Firebase and Axios nested routes (lines 84–111).

### Top menu — `TopMenu.js`

| Aspect | Detail |
|--------|--------|
| Visibility | `topMenu === true` AND `window.innerWidth > 991` |
| Structure | Horizontal `NavLink` list with mega-menu dropdowns |
| Active class | DOM manipulation via `useLayoutEffect` |
| Content | Mirrors side menu groups (dashboard, apps, pages, components…) |

Switching menu mode:

```javascript
dispatch(changeMenuMode(true|false))  // redux/themeLayout/actionCreator.js
```

---

## Navigation state management

| State | Store | Used by |
|-------|-------|---------|
| Login flag | `auth.login` | App.js route gate |
| Layout mode | `ChangeLayoutMode.mode` | Sidebar theme, CSS vars |
| Top menu | `ChangeLayoutMode.topMenu` | Menu position |
| RTL | `ChangeLayoutMode.rtlData` | `ConfigProvider direction` |
| i18n locale | i18next | Menu labels, AuthInfo flag |

No navigation history stack, breadcrumbs state, or persisted sidebar collapse in Redux (collapse is local component state only).

---

## Guards and access control

| Mechanism | HexaDash | Ashraak |
|-----------|----------|---------|
| Auth guard | Binary `isLoggedIn` flag | `AuthGuard` + JWT validation |
| Role guard | **None** | `RoleGuard(['Admin'])` |
| Permission guard | **None** | `PermissionGuard` per nav item |
| Route-level lazy loading | Yes | Yes |
| 403 page | No | `ForbiddenPage` at `/403` |

`ProtectedRoute` (`components/utilities/protectedRoute.js`) only verifies login state — no role checks.

---

## Comparison with Ashraak routing

Ashraak (`core/router/index.tsx`):

```
/login, /register          → AuthLayout (public)
/                          → AuthGuard → AppLayout
  /dashboard
  /tenant/*
  /users/*                 → RoleGuard
  /audit                   → RoleGuard
  /api-keys/*              → ApiKeysRouteGuard
  /webhooks/*              → WebhooksRouteGuard
/403, *
```

| Concern | HexaDash | Ashraak |
|---------|----------|---------|
| Router API | `BrowserRouter` + `<Routes>` | `createBrowserRouter` |
| Layout | HOC wrap per route tree | Layout route with `<Outlet>` |
| Module routes | Flat admin route files | Feature module lazy pages |
| Route map | Implicit in menu + route files | Explicit `routeMap.ts` |
| API integration | Demo/mock | Typed `ENDPOINTS` + Axios |

---

## Menu → route alignment gaps

1. **Menu is the source of truth for demo navigation** — not a separate config shared with routes.
2. Adding a route requires editing both `routes/admin/*.js` AND `MenueItems.js` (and `TopMenu.js` if top-menu mode).
3. Changelog version item reads from `demoData/changelog.json` — not app version.
4. Several menu links point to component demo pages Ashraak will never need (maps, ecommerce, firebase).

---

## i18n navigation

Configured in `src/i18n/config.js` with locales:

- `en` — English
- `ar` — Arabic (RTL demo)
- `esp` — Spanish

Menu labels and AuthInfo use `useTranslation()`. Ashraak has **no i18n** layer currently.

---

## Recommendations for Ashraak nav integration (analysis only)

1. **Do not import `MenueItems.js`** — generate Ashraak nav from `routeMap.ts` + permission metadata.
2. **Preserve Ashraak guard chain** — theme skin must not replace `AuthGuard` / `RoleGuard`.
3. **Map HexaDash visual patterns** to CoreUI `SidebarNav` structure, not Ant `Menu`.
4. If adopting HexaDash colors/spacing, define a `NavItem` adapter interface:

```typescript
interface ThemeNavItem {
  label: string;
  path: string;
  icon: ReactNode;
  permission?: string;
  roles?: string[];
  children?: ThemeNavItem[];
}
```

5. Top-menu mode is optional for Ashraak — side-menu aligns better with current `AppLayout`.
