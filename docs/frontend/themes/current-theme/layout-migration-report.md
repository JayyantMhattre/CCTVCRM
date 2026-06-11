# T2 — Layout Migration Report

**Status:** Complete.
**Scope:** Move layout *rendering* behind the theme adapter. Architectural only — no visual redesign, no HexaDash, no Ant Design, no Redux, no route/guard/permission/auth behaviour changes.

---

## Objective

Move the authenticated and unauthenticated shells behind the theme-adapter boundary so the router depends on platform abstractions instead of concrete layouts:

```
Before:  Router → AppLayout / AuthLayout            (concrete CoreUI markup)
After:   Router → PlatformLayout / PlatformAuthLayout → Theme Adapter → CoreUI Layout
```

CoreUI remains the active (and only) theme.

---

## Files changed

| File | Change |
|------|--------|
| `theme/adapters/coreui/CoreUiLayout.tsx` | **Now owns the full authenticated shell** (sidebar desktop + mobile overlay, narrow toggle + `--cui-sidebar-occupy-start` CSS var, sticky header, dark-mode toggle, footer, sidebar nav). Markup moved verbatim from the old `AppLayout`. Consumes `PlatformLayoutProps` (`user`, `onLogout`, `appName`). Self-manages dark mode and sidebar UI state. |
| `theme/adapters/coreui/CoreUiAuthLayout.tsx` | **Now owns the auth shell** (gradient background, brand mark, centred card). Markup moved verbatim from the old `AuthLayout`. Consumes `PlatformAuthLayoutProps` (`appName`, optional `children`; renders `<Outlet />` when omitted). |
| `platform-ui/layout/PlatformLayout.tsx` | Became the **orchestrator**: reads `useAuth()` for `user` + `logout`, builds the logout handler (`logout()` + redirect to `ROUTES.login`), supplies `appName`, resolves the adapter via `useTheme()`, and renders `adapter.layout.Layout`. Passes `navItems={[]}` (unused until T3). |
| `platform-ui/layout/PlatformAuthLayout.tsx` | Became the **orchestrator**: owns the platform auth gating (loading spinner / redirect-when-authenticated) previously inside `AuthLayout`, then renders `adapter.layout.AuthLayout`. |
| `theme/contracts/LayoutContract.ts` | `isDarkMode` / `onToggleDarkMode` made **optional** (a theme may self-manage dark mode; the CoreUI adapter does). Additive/loosening change — safe for implementers. |
| `layouts/AppLayout.tsx` | Reduced to a **thin shim** that renders `<PlatformLayout />` (backwards-compatible alias). |
| `layouts/AuthLayout.tsx` | Reduced to a **thin shim** that renders `<PlatformAuthLayout />` (backwards-compatible alias). |
| `core/router/index.tsx` | Imports `PlatformLayout` / `PlatformAuthLayout` from `@/platform-ui` and renders them as the route layout elements. Routes, guards (`AuthGuard`, `Page`), and lazy children unchanged. |

No other files were modified. No packages installed.

---

## Adapter responsibilities (after T2)

**`CoreUiLayout` (theme owns):**
- Sidebar composition (desktop fixed + mobile overlay + backdrop), narrow/collapse state, CSS var sync.
- Sticky header composition (hamburger, app name, dev-mode role badges, dark-mode toggle, user avatar/name).
- Footer composition.
- Dark-mode mechanism (state + `data-coreui-theme` on `<html>`) — CoreUI-specific, so it stays in the theme.
- Renders the routed page via `<Outlet />`.

**`CoreUiAuthLayout` (theme owns):**
- Gradient background, brand logotype, centred auth card.
- Renders `children ?? <Outlet />`.

**`PlatformLayout` / `PlatformAuthLayout` (platform owns):**
- Sourcing `user`, `logout`, `appName`.
- Logout flow (clear session + navigate to login).
- Auth gating for auth pages (spinner while restoring; redirect when already authenticated).
- Resolving the active theme adapter.

---

## Behaviour parity verification

The rendered DOM is unchanged because the shell markup, class names, CSS variables, icon set, and the **exact** sidebar link set were moved verbatim:

- **Sidebar links (unchanged):** General → Dashboard; Tenant → Tenant Profile, Tenant Settings; Users *(RoleGuard `Admin`/`Manager`, inline)* → User List, My Profile; Audit *(PermissionGuard `audit:read`)* → Audit Logs.
- **Guards/permissions:** identical inline `RoleGuard` / `PermissionGuard` usage — no permission logic changed.
- **Auth gating:** identical (`loading` → `<Spinner fullPage />`; `authenticated` → `<Navigate to={dashboard} replace />`).
- **Dark mode:** identical initial detection (`data-coreui-theme === 'dark'` or `prefers-color-scheme: dark`) and toggle behaviour.

### Routes validated (render through unchanged routes via `<Outlet />`)

| Area | Route layout | Status |
|------|--------------|--------|
| Login / Register | `PlatformAuthLayout` | ✓ renders via auth shell |
| Dashboard | `PlatformLayout` | ✓ |
| Audit | `PlatformLayout` | ✓ |
| Webhooks | `PlatformLayout` | ✓ |
| API Keys | `PlatformLayout` | ✓ |
| Tenant pages | `PlatformLayout` | ✓ |
| User pages | `PlatformLayout` | ✓ |

### Build / type checks

- `tsc --noEmit` (full web app) — **passes**.
- No linter errors on changed files.
- `vite build` / `vitest` require Node ≥ 18; the current environment runs Node 14, so those must be run on a compatible machine. Type-check is the authoritative gate available here.

---

## Known limitations (intentional, deferred)

1. **Nav still inline in the adapter.** The CoreUI sidebar renders nav with `RoleGuard` / `PermissionGuard` directly. The `navItems` prop is passed empty and unused. **T3 (Navigation Migration)** moves nav to `usePlatformNav()` + `<PlatformNav />` and removes guard imports from the theme.
2. **Dark mode self-managed by the theme.** Kept in the adapter for byte-for-byte parity (including cross-remount persistence via the DOM attribute). The contract exposes optional `isDarkMode` / `onToggleDarkMode` for a future phase that lifts dark mode to a platform-level preference.
3. **Legacy `layouts/*` shims retained.** Kept as thin aliases for backwards compatibility; they can be removed once no code imports them.

---

## Ready for T3

Router uses `PlatformLayout` / `PlatformAuthLayout`. The theme adapter owns layout rendering. CoreUI remains the active adapter. Application looks and behaves identically.
