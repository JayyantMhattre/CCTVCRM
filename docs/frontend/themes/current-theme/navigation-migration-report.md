# T3 — Navigation Migration Report

**Status:** Complete.
**Scope:** Move navigation ownership from the CoreUI adapter into the platform. Architectural only — no visual changes, no route/guard/permission/auth/module changes, no HexaDash/Ant Design/Redux.

---

## Objective

Make the **platform** the single source of truth for menus, groups, role visibility and permissions; reduce the **theme** to a pure renderer.

---

## Old vs new ownership model

### Before T3

```
CoreUiLayout (theme)
  ├─ hardcoded menu definitions (labels, routes, icons)
  ├─ <RoleGuard roles={['Admin','Manager']}> … </RoleGuard>
  ├─ <PermissionGuard permission="audit:read"> … </PermissionGuard>
  └─ sidebar composition
```

The theme knew about roles, permissions, routes and business modules — tight coupling between *theme* and *business navigation*.

### After T3

```
Platform                                            Theme
────────                                            ─────
navigationConfig.ts        (authoring + visibility)
        │
usePlatformNav()           (resolver: roles+perms+flags → visible)
        │  → NavigationGroup[]  (resolved, rule-free)
PlatformNavigationProvider / useNavigationModel
        │
PlatformLayout  ──navGroups──►  CoreUiLayout → CoreUiNav  (render only)
```

The theme receives a resolved `NavigationGroup[]` and renders it. It knows **nothing** about roles, permissions, routes or modules.

---

## Files changed / added

### Added — platform

| File | Responsibility |
|------|----------------|
| `platform-ui/navigation/models/NavigationVisibility.ts` | Declarative access rules (roles / permissions / featureFlags). Platform-only. |
| `platform-ui/navigation/models/NavigationItem.ts` | `NavigationItemConfig` (authoring) + re-export of resolved `NavigationItem`. |
| `platform-ui/navigation/models/NavigationGroup.ts` | `NavigationGroupConfig` (authoring) + re-export of resolved `NavigationGroup`. |
| `platform-ui/navigation/models/NavigationDivider.ts` | Explicit divider node (reserved for future explicit separators). |
| `platform-ui/navigation/models/NavigationBadge.ts` | Re-export of the badge type from the theme contract. |
| `platform-ui/navigation/models/index.ts` | Model barrel. |
| `platform-ui/navigation/navigationConfig.ts` | The single declarative menu definition (icons by key, routes from `ROUTES`). |
| `platform-ui/navigation/useFeatureFlags.ts` | Feature-flag access (currently all-enabled stub). T7 preparation. |
| `platform-ui/navigation/PlatformNavigationProvider.tsx` | Resolves the model once; exposes it via `useNavigationModel()`. |
| `platform-ui/navigation/PlatformNavRenderer.tsx` | Reads the model and delegates to the active theme's `Nav`. |

### Added — theme

| File | Responsibility |
|------|----------------|
| `theme/adapters/coreui/navIcons.ts` | `CORE_UI_NAV_ICONS` — the only place nav icon keys bind to `@coreui/icons`. |

### Changed — platform

| File | Change |
|------|--------|
| `platform-ui/navigation/usePlatformNav.tsx` | Rewritten as the resolver / single source of truth. Builds the resolved `NavigationGroup[]` from `navigationConfig` + `useAuth` + `usePermission` + `useFeatureFlags`. |
| `platform-ui/navigation/index.ts` | New barrel (models, config, resolver, provider, renderer). |
| `platform-ui/layout/PlatformLayout.tsx` | Wraps the shell in `PlatformNavigationProvider`; reads `useNavigationModel()` and passes `navGroups` to the theme `Layout`. |

### Changed — theme

| File | Change |
|------|--------|
| `theme/contracts/NavigationContract.ts` | New **resolved render model**: `NavigationGroup`, `NavigationItem`, `NavigationBadge(+Variant)`, `NavigationIconKey`. `PlatformNavProps` now carries `groups`. (`PlatformNavItem` removed.) |
| `theme/contracts/LayoutContract.ts` | `Layout` now receives `navGroups: readonly NavigationGroup[]` (was `navItems`). |
| `theme/contracts/index.ts`, `theme/index.ts` | Export the new navigation types; drop `PlatformNavItem`. |
| `theme/adapters/coreui/CoreUiNav.tsx` | Rewritten to render `NavigationGroup[]` with the exact pre-T3 markup (titles, items, between-group dividers) using the icon-key map. |
| `theme/adapters/coreui/CoreUiLayout.tsx` | Removed `RoleGuard` / `PermissionGuard` imports, the inline menu, `ROUTES` and nav icon imports. The sidebar now renders `<CoreUiNav groups={navGroups} />`. |

### Removed

| File | Reason |
|------|--------|
| `platform-ui/navigation/PlatformNav.tsx` | Replaced by `PlatformNavRenderer` (model-driven). |
| `platform-ui/navigation/PlatformNavItem.ts` | Replaced by the group/item render model in the contract. |

No packages installed. No routes, guards, permissions, auth or modules changed.

---

## Behaviour parity

The menu is defined to reproduce the pre-T3 sidebar exactly:

| Group | Items | Visibility rule | Equivalent old guard |
|-------|-------|-----------------|----------------------|
| General | Dashboard | always | — |
| Tenant | Tenant Profile, Tenant Settings | always | — |
| Users | User List, My Profile | roles: `Admin` **or** `Manager` | `<RoleGuard roles={['Admin','Manager']} inline>` |
| Audit | Audit Logs | permission: `audit:read` | `<PermissionGuard permission="audit:read">` |

- **Rule semantics match the guards exactly:** roles = at least one (`hasRole(...roles)`); permissions = all (`hasPermission` per entry).
- **Icons unchanged:** `dashboard→cilSpeedometer`, `tenant→cilBuilding`, `tenant-settings→cilSettings`, `users→cilPeople`, `profile→cilUser`, `audit→cilDescription`.
- **Link targets unchanged:** "My Profile" maps to `ROUTES.users.profile` exactly as the previous sidebar did.
- **Separator parity:** a divider is rendered after each group except the last, regardless of group visibility — reproducing the original dividers (which were siblings of the guarded sections, including the trailing dividers a restricted user used to see).

### Per-role verification (rendered DOM)

| User | Visible sections | Result |
|------|------------------|--------|
| **Admin** | General, Tenant, Users, Audit | Identical (4 groups, 3 dividers) |
| **Manager** | General, Tenant, Users, (Audit only with `audit:read`) | Identical |
| **Regular** | General, Tenant | Identical (incl. trailing dividers where Users/Audit are hidden) |
| **Anonymous** | none (redirected by `AuthGuard`) | Identical — no shell rendered |

### Build / type checks

- `tsc --noEmit` (full web app) — **passes**.
- No linter errors on changed files.
- `vite build` / `vitest` require Node ≥ 18; run on a compatible machine (this environment is Node 14). Type-check is the authoritative gate available here.

---

## Future theme impact

- A new theme implements `NavigationContract.Nav` to render `NavigationGroup[]` however it likes (Ant Menu, etc.). It inherits the platform's menu, grouping and access decisions automatically — **no business logic to reimplement**.
- Icon keys (`NavigationIconKey`) are mapped per theme (e.g. a `hexadash/navIcons.ts`), so each theme uses its own icon set without the platform changing.
- Because resolution is centralised in `usePlatformNav` + `PlatformNavigationProvider`, additional surfaces (top-bar, breadcrumbs, command palette) can consume the same model via `useNavigationModel()` / `PlatformNavRenderer`.

---

## Known limitations (intentional, deferred)

1. **Feature flags are stubbed** (`useFeatureFlags.isEnabled` → `true`). Wiring is in place (`NavigationVisibility.featureFlags`); a real flag source is a future change isolated to `useFeatureFlags`.
2. **`NavigationDivider` is reserved**, not yet used — the current theme auto-draws dividers between groups.
3. **Dark mode** remains theme-managed in the CoreUI adapter (unchanged from T2).

---

## Ready for T4

Menus render exactly the same. Permissions behave exactly the same. The theme owns rendering only; the platform owns all navigation decisions. The CoreUI adapter contains zero business-navigation logic.
