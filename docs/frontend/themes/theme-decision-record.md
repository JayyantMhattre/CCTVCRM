# Theme Decision Record — Theme Adapter Architecture

**Status:** Accepted · **Date:** June 2026 · **Phase:** T8 — Theme Governance
**Context:** Ashraak web app (`FrontEnd/apps/web`, React 19 + TypeScript + Vite).
**Decision:** Themes are integrated **exclusively** through a Theme Adapter Architecture. Direct integration of a purchased theme into business code is **forbidden**.

This is the permanent record of *why* the theme system is built the way it is. It governs all future theme work alongside [theme-onboarding-guide.md](./theme-onboarding-guide.md), [theme-lifecycle.md](./theme-lifecycle.md), and [theme-adapter-development-guide.md](./theme-adapter-development-guide.md).

---

## 1. Context & problem

The product needs to change visual themes over time (rebrands, purchased premium themes, client white-labelling) **without** rewriting business features each time. Purchased admin themes (CoreUI, HexaDash, etc.) typically ship far more than visuals: their own routing, Redux store, auth screens with baked-in logic, API/service stubs, and demo pages. Naively "switching themes" usually means re-integrating all of that — an expensive, regression-prone rewrite every time.

We required an approach where:
- swapping themes does not touch auth, routing, permissions, guards, APIs, data flow, or module logic;
- multiple themes can coexist and be selected by configuration;
- adding a future theme is bounded, repeatable work;
- type-safety guarantees a theme is "complete".

## 2. Options considered

| Option | Summary | Verdict |
|--------|---------|---------|
| **A. Direct integration** | Import the purchased theme's components/pages/store directly into modules | ❌ Rejected — couples business logic to a vendor; every theme change is a rewrite; vendor lock-in |
| **B. Fork-per-theme** | Maintain a branch/build per theme | ❌ Rejected — combinatorial maintenance; drift; merge hell |
| **C. CSS-only reskin** | Swap stylesheets, keep one component set | ⚠️ Partial — works for colours but can't restructure layout/nav/markup; no clean multi-theme story |
| **D. Theme Adapter Architecture** | A stable `platform-ui` API + typed contracts + per-theme adapters selected at runtime | ✅ **Chosen** |

## 3. Decision — the architecture

```
business module ──▶ platform-ui primitive ──▶ useTheme().adapter.<contract> ──▶ <theme>Adapter
                                                          ▲
                                            ThemeProvider (VITE_THEME → registry)
```

Layers:
- **`platform-ui/`** — theme-agnostic primitives (`PlatformCard`, `PlatformTable`, `PlatformDialog`, `PlatformBadge`, `PlatformAvatar`, `PlatformTabs`, `PlatformBreadcrumb`, `PlatformChart`, `PlatformLayout`, …). Modules import **only** these. This is the stable public API.
- **`theme/contracts/`** — TypeScript interfaces defining **11 surfaces** a theme must implement (Layout, Navigation, Card, Table, Dialog, Notification, Badge, Avatar, Tabs, Breadcrumb, Chart), unified by the `ThemeAdapter` interface.
- **`theme/adapters/<id>/`** — concrete implementations (visual design only). Today: `coreui` (default), `hexadash` (opt-in).
- **`theme/registry.ts` + `config.ts`** — map a `ThemeId` to an adapter; resolve the active id from `VITE_THEME` with a safe default.
- **`ThemeProvider` / `useTheme()`** — supply the active adapter via React context.

**The platform decides; the adapter renders.** All access control, routing, nav visibility, tab/active state, toast queue, and dialog state are resolved by the platform and passed to adapters as plain presentational models.

## 4. Why direct integration is forbidden

1. **Coupling = rewrites.** Importing vendor components/pages welds business code to one theme; the next theme change re-touches every module.
2. **Vendor lock-in.** Vendor routing/Redux/auth/services leak their assumptions into our app and constrain our stack (we use react-router, Zustand, TanStack Query, react-hook-form+zod — not the vendor's).
3. **Security & correctness.** Themes ship demo auth/guards. Letting a theme make access decisions is a security risk; access control must stay in the platform.
4. **Untestable boundaries.** Without contracts, "does this theme support X?" is answered by manual clicking, not the compiler.
5. **Unbounded scope.** Direct integration has no clear "done"; adapters have a typed, finite contract surface.

**Therefore:** adapters reuse a theme's **visual design only** (tokens + markup patterns). They never import the vendor's Redux, router, auth, services, or demo pages.

## 5. Benefits

- **Theme swap ≈ config change.** Selecting/promoting a theme is `VITE_THEME` / a one-line `DEFAULT_THEME_ID` change.
- **Zero module churn.** Business modules are unaffected by theme work (validated in T7: 100% functional parity across themes).
- **Bounded onboarding.** A new theme = one adapter folder + one registry line; effort is predictable.
- **Type-safe completeness.** If `tsc` passes against `ThemeAdapter`, all 11 surfaces exist. Registry tests prove every adapter resolves.
- **Coexistence & rollback.** Multiple themes ship together; reverting is instant via env or a one-line default flip.
- **Isolation.** Each theme's CSS is scoped under `.<id>-theme`; inactive themes are inert — no global bleed.
- **Unlimited future themes.** The capability is permanent, not a one-off migration.

## 6. Tradeoffs (accepted)

| Tradeoff | Mitigation |
|----------|------------|
| Indirection: modules call `platform-ui`, not components directly | Small, well-documented surface; better long-term flexibility |
| Adapters must be hand-written per theme (visual porting, not drop-in) | This is the point — it's what prevents vendor lock-in; effort is bounded by 11 contracts |
| Contracts must evolve as new UI needs appear | Adding a contract is a typed, reviewable change applied once across adapters |
| All registered adapters ship in the bundle (no per-theme tree-shaking yet) | Adapters are small (no vendor app code); per-theme code-splitting is an available optimization |
| Full visual reskin of legacy module interiors requires migrating modules to `platform-ui` | Tracked as a separate migration phase; the architecture makes it incremental and safe |

## 7. Consequences & rules (binding)

1. Business modules import from `platform-ui` only — never from `theme/adapters/*` or a vendor theme.
2. Adapters contain **no** business/auth/routing/state logic and make **no** access decisions.
3. Adding/removing a theme touches only `theme/adapters/*`, `registry.ts`, and `config.ts`.
4. The default theme always resolves, even when `VITE_THEME` is missing/invalid.
5. Every theme that ships has analysis, compatibility, validation, and readiness docs.
6. All theme CSS is scoped per theme.

## 8. Status of implementation

- Contracts: **11/11** defined (`theme/contracts/`).
- Adapters: `coreui` (default, in production) and `hexadash` (implemented, validated, opt-in).
- Governance: this record + onboarding guide + lifecycle + selection checklist + adapter dev guide + reusable prompts (T8).

**Decision stands:** the Theme Adapter Architecture is the permanent, sole mechanism for theming Ashraak. Direct theme integration is prohibited.
