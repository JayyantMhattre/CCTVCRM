# Upgrade Guide

How to perform upgrades against the Platform V1 baseline **safely and reproducibly**. Applies to themes, dependencies, the platform itself, modules, and mobile. The guiding rule throughout: **upgrade one major version at a time, verify the build after every phase, and update docs in the same change.**

**Related:** [known-limitations.md](./known-limitations.md) · [platform-roadmap.md](./platform-roadmap.md) · [platform-architecture-overview.md](./platform-architecture-overview.md)

---

## 0. Universal upgrade rules

1. **Analyze the whole project before modifying** — read the relevant config (`Directory.Packages.props`, `global.json`, `angular.json`/`tsconfig.json` equivalents, `pubspec.yaml`) before and after.
2. **One major version at a time.** Never skip majors (frameworks, React, .NET, Flutter, etc.).
3. **Phase + verify.** After each phase, the build must compile and tests must pass before proceeding.
4. **No silent behavior changes.** Preserve business logic; call out breaking changes explicitly.
5. **Docs evolve with code** — every upgrade updates affected docs and, if architectural, adds an [ADR](../adr/ADR-0000-template.md). CI runs documentation validation.
6. **Reversible by default** — prefer additive, flag-guarded, or config-toggled rollouts.

---

## 1. Theme upgrades

Themes are isolated behind the [Theme Adapter Architecture](../frontend/themes/theme-decision-record.md), so upgrades are low-risk.

**Add or upgrade a theme:**
1. Follow the [theme onboarding guide](../frontend/themes/theme-onboarding-guide.md): extract → analyse → compatibility → adapter → validate → activate.
2. Implement/refresh the adapter per the [adapter development guide](../frontend/themes/theme-adapter-development-guide.md) (11 contracts). Keep all CSS scoped under `.<id>-theme`.
3. Validate with the [validation prompt](../frontend/themes/prompts/theme-validation-prompt.md); produce validation + parity + readiness reports.
4. **Do not change `DEFAULT_THEME_ID`** until parity is accepted; activate via `VITE_THEME` first.

**Upgrade the underlying UI kit (e.g., CoreUI/Bootstrap major):**
- Treat as a dependency upgrade (§2) scoped to the affected adapter; re-run theme validation afterward.

**Retire a theme:** follow [theme-lifecycle §5](../frontend/themes/theme-lifecycle.md#5-theme-retirement) — deprecate (keep registered for one release as rollback), then remove the adapter folder + registry/config lines.

---

## 2. Dependency upgrades

Dependencies are centrally governed — see [build/package governance ADR-0010](../adr/ADR-0010-build-package-governance.md) and [dependency-governance](../platform/packages/dependency-governance.md).

**Backend (.NET):**
- Versions live in `BackEnd/Directory.Packages.props` (central package management) and the SDK in `BackEnd/global.json`.
- Upgrade one major at a time; `dotnet restore` + `dotnet build Ashraak.slnx` + run tests after each step.
- For framework majors (.NET, EF Core), review breaking-change notes and update affected modules behind their own commit.

**Web (npm/pnpm):**
- Requires **Node 20+** ([known-limitations C1](./known-limitations.md#1-current-constraints)).
- Upgrade one major at a time (React, Vite, TanStack Query, react-router, etc.). After each: `npm run type-check` (`tsc --noEmit`) → `npm test` → build.
- Never introduce `any` to silence type errors; fix types properly.

**Mobile (Flutter/Dart):**
- Versions in `pubspec.yaml`; follow [Mobile versioning ADR-0011](../adr/ADR-Mobile-0011-versioning-strategy.md).
- `flutter pub get` → analyze → test → build after each bump.

**Verification gate:** the relevant CI pipeline (`ci.yml`, `mobile.yml`) must pass before merge.

---

## 3. Platform upgrades

For changes to cross-cutting platform infrastructure (Host, BuildingBlocks, observability, caching, outbox):

1. **ADR first** if the change is architectural (new pattern, new infra dependency).
2. Change `BuildingBlocks`/`Host` in isolation; keep module contracts (`SharedKernel.Contracts`) backward-compatible. Contract changes ripple to all modules — version and migrate deliberately.
3. Database changes go through EF Core migrations per module (per-schema); never hand-edit applied migrations.
4. Re-run health checks, observability wiring, and rate-limit/config validation.
5. Update [platform docs](../platform/platform-manifest.md) and the [architecture overview](./platform-architecture-overview.md).

---

## 4. Module upgrades

Modules are isolated; upgrade them independently.

**Backend module:**
- Follow [extending/add-backend-module](../extending/add-backend-module.md) conventions when modifying structure.
- Keep cross-module communication via contracts only ([ADR-0001](../adr/ADR-0001-modular-monolith.md)); no cross-module Infrastructure references.
- Add EF migrations for schema changes; update the module's docs under `docs/modules/<module>/`.

**Web module:**
- Follow [extending/add-frontend-route](../extending/add-frontend-route.md).
- Prefer migrating UI to `platform-ui` primitives (this also advances [D1](./known-limitations.md#3-known-technical-debt) toward full theme support). Don't reintroduce raw theme-specific markup.
- Keep auth/routing/guards/data hooks unchanged unless the module's contract changes.

**Verification:** type-check + tests per module; update module README + API docs.

---

## 5. Mobile upgrades

- State management (Riverpod) and navigation (go_router) patterns are fixed by [Mobile-0001](../adr/ADR-Mobile-0001-state-management.md)/[Mobile-0002](../adr/ADR-Mobile-0002-navigation.md) — keep new features consistent.
- API client is **generated** from OpenAPI ([Mobile-0005](../adr/ADR-Mobile-0005-openapi-sdk-generation.md)) — regenerate after backend contract changes rather than hand-editing.
- Releases follow [Mobile-0010 release strategy](../adr/ADR-Mobile-0010-release-strategy.md), signing ([Mobile-0012](../adr/ADR-Mobile-0012-signing-strategy.md)), and versioning ([Mobile-0011](../adr/ADR-Mobile-0011-versioning-strategy.md)); pipelines: `mobile.yml`, `android-release.yml`, `ios-release.yml`.
- Use the documented [rollback strategy](../mobile/release/rollback-strategy.md) if a release regresses.

---

## 6. Documentation requirements (for every upgrade)

Per [documentation governance](../documentation-governance.md):

1. Update canonical docs under `docs/` for any changed behavior (module folder + API/ops as needed).
2. Add/update an [ADR](../adr/) for significant architectural decisions.
3. Run `./scripts/validate-docs.ps1` (or `.sh`) before opening the PR.
4. Complete the [PR documentation checklist](../documentation-pr-checklist.md).
5. For releases, append release notes (mirror [v1.0.0-release-notes.md](./v1.0.0-release-notes.md)) and update the [manifest](./platform-v1-manifest.md) if inventory changed.

CI **Documentation Validation** (`docs-validation.yml`) enforces this (warn by default; `DOC_VALIDATE_MODE=fail` for a hard gate).

---

## 7. Upgrade checklist (copy per upgrade)

- [ ] Read current config before changing (packages/SDK/tsconfig/pubspec).
- [ ] One major version at a time; breaking changes noted.
- [ ] Build compiles after each phase; tests pass.
- [ ] Type-check clean (`tsc --noEmit` for web); no new `any`.
- [ ] Business logic unchanged unless intended; contracts versioned if touched.
- [ ] Docs updated in the same PR; ADR added if architectural.
- [ ] `validate-docs` + relevant CI pipeline green.
- [ ] Rollback path identified.
