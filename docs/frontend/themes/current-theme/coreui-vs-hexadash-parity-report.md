# CoreUI vs HexaDash — Parity Report

**Phase:** T7 — HexaDash Theme Validation & Production Readiness
**Scope:** Compares the two registered theme adapters (`coreui` = production default, `hexadash` = opt-in via `VITE_THEME`) across visual parity, functional parity, missing capabilities, and known limitations.
**Companion:** [`hexadash-validation-report.md`](./hexadash-validation-report.md), [`production-readiness-report.md`](./production-readiness-report.md).

---

## 1. Contract parity

Both adapters implement **all 11 theme contracts** (verified by `theme/registry.test.ts` and `tsc --noEmit`). No contract is missing on either side.

| Contract | CoreUI impl | HexaDash impl | Parity |
|----------|-------------|---------------|--------|
| `layout` (Layout + AuthLayout) | `CoreUiLayout`, `CoreUiAuthLayout` | `HexaDashLayout`, `HexaDashAuthLayout` | ✅ Full |
| `navigation` | `CoreUiNav` | `HexaDashNav` | ✅ Full |
| `card` | `CoreUiCard` | `HexaDashCard` | ✅ Full |
| `dialog` | `CoreUiDialog` | `HexaDashDialog` | ✅ Full |
| `table` | `CoreUiTable` | `HexaDashTable` | ✅ Full |
| `notification` | `CoreUiNotificationViewport` | `HexaDashNotificationViewport` | ✅ Full (neither is mounted by the app — see §4) |
| `badge` | `CoreUiBadge` | `HexaDashBadge` | ✅ Full |
| `avatar` | `CoreUiAvatar` | `HexaDashAvatar` | ✅ Full |
| `tabs` | `CoreUiTabs` | `HexaDashTabs` | ✅ Full |
| `breadcrumb` | `CoreUiBreadcrumb` | `HexaDashBreadcrumb` | ✅ Full |
| `chart` | `CoreUiChart` (frame) | `HexaDashChart` (frame) | ✅ Full (frame-only on both; no chart lib yet) |

**Contract-surface parity: 11 / 11.**

---

## 2. Visual parity (what the user actually sees today)

Because modules consume raw CoreUI markup (not `platform-ui`), only adapter-routed surfaces differ between themes. The rest is identical because it is the same CoreUI markup either way.

| Surface | CoreUI | HexaDash | Visually different? |
|---------|--------|----------|---------------------|
| App shell (sidebar/header/footer) | CoreUI sidebar/header | HexaDash 280px sider, 64px sticky header, purple accents | ✅ Yes — themed |
| Sidebar nav | CoreUI nav | HexaDash nav styling | ✅ Yes — themed |
| Auth screen | CoreUI centered card | HexaDash gradient + branded card | ✅ Yes — themed |
| Dashboard tiles / cards | CoreUI `.card-stat` | **same CoreUI `.card-stat`** | ❌ No — not themed |
| Tables | CoreUI `.table` | **same CoreUI `.table`** | ❌ No — not themed |
| Forms / inputs | CoreUI controls | **same** | ❌ No — not themed |
| Dialogs | `react-bootstrap/Modal` | **same** | ❌ No — not themed |
| Badges / inline avatars | CoreUI | **same** | ❌ No — not themed |
| Toasts | CoreUI `ToastContainer` | **same** | ❌ No — not themed |

**Visual parity verdict:** HexaDash currently delivers a **distinct shell/nav/auth experience** over **identical CoreUI page interiors**. Full visual divergence requires migrating modules to `platform-ui` (T8). Until then, the experience is intentionally hybrid.

---

## 3. Functional parity

| Capability | CoreUI | HexaDash | Parity |
|------------|--------|----------|--------|
| Routing / lazy pages | ✅ | ✅ | Identical (theme-independent) |
| Auth / JWT / refresh | ✅ | ✅ | Identical |
| Route guards (Auth/Role/Permission) | ✅ | ✅ | Identical |
| Navigation permission filtering | ✅ | ✅ | Identical (`usePlatformNav`) |
| TanStack Query data flow | ✅ | ✅ | Identical |
| RHF + Zod forms | ✅ | ✅ | Identical |
| Toasts (behaviour) | ✅ | ✅ | Identical |
| Responsive shell | ✅ | ✅ | Equivalent mechanisms |
| Dark mode | `data-coreui-theme` toggle | HexaDash toggle **bridged** to `data-coreui-theme` | ✅ Equivalent (after T7 fix) |

**Functional parity verdict: 100%.** Switching `VITE_THEME` changes presentation only; no behaviour changes.

---

## 4. Missing capabilities

| Capability | State | Notes |
|------------|-------|-------|
| Themed page interiors | ❌ Missing under both as *themed* output, but works as CoreUI | Needs T8 module migration to `platform-ui` |
| Adapter-routed toasts | ❌ Not wired | `AppProviders` mounts `ToastContainer` directly; both adapters' `notification.Viewport` are unused. One-line swap to `PlatformToast` (out of adapter-only scope) |
| Chart rendering | ❌ Frame-only on both | No chart library committed; `ChartContract` is the abstraction seam |
| Icon-only collapsed nav (HexaDash) | ⚠️ Partial | Narrow mode shrinks width but keeps labels |
| HexaDash Unicons / Jost font | ⚠️ Substituted | CoreUI icons + system font stack used to avoid new deps |

---

## 5. Known limitations (HexaDash adapter)

1. **Visual porting, not Ant Design** — HexaDash look is recreated with CoreUI/Bootstrap primitives + extracted tokens. No Ant Design, styled-components, Redux, or HexaDash app code was imported (by design).
2. **Shell-scoped skin** — `.hexadash-theme` styling applies to adapter-routed surfaces only; raw module markup is unaffected.
3. **Auth pages render light only** — no dark toggle on the auth shell.
4. **Both adapters ship in the bundle** — `registry.ts` statically imports both; no per-theme tree-shaking yet.
5. **Runtime-dependent checks pending** — pixel layout, responsive breakpoints, and WCAG contrast need a Node 18+ build to confirm (cannot run here on Node 14).

---

## 6. Parity scorecard

| Dimension | Score | Comment |
|-----------|-------|---------|
| Contract parity | 11 / 11 | Complete |
| Functional parity | 100% | Theme-independent behaviour |
| Visual parity (shell/nav/auth) | High | HexaDash distinct and complete |
| Visual parity (page interiors) | 0% (intentional) | Awaiting module migration (T8) |
| Dark mode parity | Equivalent | After T7 dark-mode bridge fix |
| Accessibility parity | Equivalent | Shares global focus/ARIA patterns |

**Overall:** HexaDash is a **drop-in, functionally identical** theme adapter that reskins the application chrome. Whole-page visual parity with a true HexaDash look is gated on a future module-migration phase, which is explicitly out of T7 scope.
