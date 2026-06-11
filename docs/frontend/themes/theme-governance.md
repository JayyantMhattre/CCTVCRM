# Theme Governance

Rules that keep the UI theme-replaceable. These constraints apply from T1 onward.

---

## Layering rules

```
modules  →  platform-ui  →  theme (contracts)  →  theme/adapters/*
```

| From → To | Allowed | Notes |
|-----------|---------|-------|
| `modules/*` → `platform-ui` | ✅ | The only UI dependency modules should have |
| `modules/*` → `theme/*` | ❌ | Never import contracts or adapters in modules |
| `modules/*` → `@coreui/*`, `react-bootstrap`, `antd` | ❌ (target state) | Use `platform-ui` primitives instead |
| `platform-ui/*` → `theme` | ✅ | Renders against contracts via `useTheme()` |
| `platform-ui/*` → `theme/adapters/*` | ❌ | Must go through the registry, not a concrete adapter |
| `theme/adapters/<x>/*` → `@coreui/*`, `react-bootstrap`, vendor libs | ✅ | Vendor coupling is isolated here, on purpose |
| `theme/*` → `modules/*` | ❌ | Themes must not know about business features |
| `core/*` → `platform-ui` | ❌ | Core stays UI-agnostic |

> T1 reality: modules still import some CoreUI/`react-bootstrap` directly and the router still uses the legacy layouts. That is expected — those migrations happen in T4/T5. The rules above describe the **target state** the migration drives toward.

---

## What belongs in each layer

| Layer | Owns | Must NOT own |
|-------|------|--------------|
| **Modules** | Business rules, API calls, query keys, Zod schemas, page composition | CSS classes, vendor UI imports, theme decisions |
| **platform-ui** | Stable typed component API, composition, a11y defaults, permission-aware nav building | Theme colours, vendor-specific markup |
| **theme (contracts)** | Interfaces, provider, registry, config | Any rendering, business logic |
| **theme adapters** | Concrete vendor markup + styling | Routing, auth, permissions, API |

---

## Permission & security boundaries

- **Permission and role decisions stay in the platform.** `usePlatformNav` computes `visible` flags from `useAuth`/`usePermission`. Adapters render only what they are told.
- **Auth, session, routing, and guards are out of theme scope.** Themes never import `authStore`, guards, `apiClient`, or `routeMap` for decision-making (they receive resolved data/handlers as props).
- A theme cannot widen access: it has no way to bypass `AuthGuard`, `RoleGuard`, `PermissionGuard`, or module route guards.

---

## Contract change policy

The `ThemeAdapter` contracts are a shared API between the platform and every theme. Treat them like a public interface:

1. **Additive changes** (new optional props, new surfaces) are safe — add them with sensible defaults so existing adapters keep compiling.
2. **Breaking changes** (renamed/removed props, required new props) must update **every** registered adapter in the same change.
3. New surfaces should be justified by real module needs, not speculative theming.
4. Keep props **data-shaped and theme-neutral** (no vendor-specific types in contracts).

---

## Adding UI to a module (target workflow)

1. Need a card/table/dialog/etc.? Import it from `@/platform-ui`.
2. Missing a primitive? Add it to `platform-ui` (and a contract + adapter implementation if it is themable).
3. Never reach for `@coreui/*` or `react-bootstrap` directly in a module.

---

## Definition of done for theme work

- `tsc` type-check passes under the strict project config.
- No module imports a concrete theme adapter.
- Permission-gated navigation still derives from `routeMap` + guards.
- Auth/session flows untouched.
- Switching `VITE_THEME` changes only presentation.

---

## Related

- [theme-adapter-architecture.md](./theme-adapter-architecture.md)
- [theme-replacement-guide.md](./theme-replacement-guide.md)
- [current-theme/platform-compatibility-report.md](./current-theme/platform-compatibility-report.md)
