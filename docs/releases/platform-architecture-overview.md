# Platform Architecture Overview — V1

High-level architecture of the Ashraak platform at the V1 baseline. This is the orientation map; deeper detail lives in [docs/architecture/](../architecture/system-overview.md), the [ADR catalog](../adr/ADR-0001-modular-monolith.md), and per-module docs.

**Related:** [v1.0.0-release-notes.md](./v1.0.0-release-notes.md) · [platform-capabilities.md](./platform-capabilities.md) · [platform-v1-manifest.md](./platform-v1-manifest.md)

---

## 1. High-level architecture

Three runtimes share one identity, tenancy, and contract model:

```
        ┌───────────────────────────┐        ┌───────────────────────────┐
        │  Web SPA (React 19)       │        │  Mobile (Flutter)         │
        │  Theme Engine + modules   │        │  Riverpod + go_router     │
        └─────────────┬─────────────┘        └─────────────┬─────────────┘
                      │  REST + OAuth2 (JWT / API Keys)     │
                      └──────────────────┬──────────────────┘
                                         ▼
        ┌─────────────────────────────────────────────────────────────┐
        │  Ashraak.Api (Host) — ASP.NET Core Minimal APIs              │
        │  Middleware: correlation · rate limit · auth · api-key       │
        │  Modules (contract-isolated): Auth · Tenant · Users · Audit  │
        │  Notifications · Files · Caching · Webhooks · ApiKeys        │
        │  BuildingBlocks: SharedKernel.Contracts · Outbox · Infra     │
        └───────────┬───────────────┬───────────────┬─────────────────┘
                    ▼               ▼               ▼
              PostgreSQL          Redis           MongoDB
          (per-module schemas)   (cache,        (audit store)
                                  locks)
        Observability: Serilog→Seq · OpenTelemetry OTLP · health probes
```

**Core principles:** modular monolith ([ADR-0001](../adr/ADR-0001-modular-monolith.md)); modules talk only through `SharedKernel.Contracts`; reliable eventing via the outbox; UI decoupled from themes via the Theme Adapter Architecture.

## 2. Backend

- **Runtime:** .NET 10, ASP.NET Core Minimal APIs, EF Core 9, OpenIddict (OAuth2/JWT).
- **Shape:** a **modular monolith**. Each module is a vertical slice (Domain / Application / Infrastructure / Api) and owns its PostgreSQL schema.
- **Isolation:** no cross-module Infrastructure references; communication is via contracts and domain events.
- **Reliability:** the **Outbox** ([ADR-0002](../adr/ADR-0002-outbox-pattern.md)) persists events transactionally; **hosted processors** ([ADR-0007](../adr/ADR-0007-outbox-hosted-processors.md)) dispatch them in the background.
- **Observer pattern:** Audit observes events without producers depending on it ([ADR-0003](../adr/ADR-0003-observer-modules.md)).
- **Cross-cutting (Host):** correlation IDs, rate limiting, auth + API-key middleware, configuration validation, health checks, feature flags.
- **Data:** PostgreSQL (auth/tenant/users/etc., per-schema), Redis (cache/sessions/locks, [ADR-0004](../adr/ADR-0004-redis-caching.md)), MongoDB (audit).
- **Observability:** Serilog + Seq, OpenTelemetry ([ADR-0005](../adr/ADR-0005-open-telemetry.md)), host hardening ([ADR-0008](../adr/ADR-0008-host-platform-hardening.md)).

Modules: **Auth, Tenant, Users, Audit, Notifications, Files, Caching, Webhooks, ApiKeys** (+ BuildingBlocks).

## 3. Frontend (Web)

- **Stack:** React 19 + TypeScript, Vite 6, TanStack Query (server state), Zustand (client/auth state), react-router (routing + guards), react-hook-form + Zod (forms/validation).
- **Composition:** `AppProviders` (ThemeProvider → QueryClient → AuthProvider) wraps a lazy-loaded route tree behind `AuthGuard`/`RoleGuard`/permission guards.
- **Theme Engine:** modules render `platform-ui` primitives; the active theme adapter is resolved from `VITE_THEME`. See [§5](#5-theme-engine).
- **Modules:** auth, dashboard, tenant, users, audit, apikeys, webhooks — plus shared components, guards, hooks, and the toast system.

## 4. Mobile

- **Stack:** Flutter (Android/iOS) — [ADR-0012](../adr/ADR-0012-flutter-mobile-platform.md). Riverpod for state ([Mobile-0001](../adr/ADR-Mobile-0001-state-management.md)); go_router for navigation ([Mobile-0002](../adr/ADR-Mobile-0002-navigation.md)).
- **Foundation:** environment config ([Mobile-0003](../adr/ADR-Mobile-0003-environment-configuration.md)), secure token storage ([Mobile-0004](../adr/ADR-Mobile-0004-secure-token-storage.md)), offline cache ([Mobile-0007](../adr/ADR-Mobile-0007-offline-cache.md)), crash reporting ([Mobile-0008](../adr/ADR-Mobile-0008-crash-reporting.md)), analytics ([Mobile-0009](../adr/ADR-Mobile-0009-analytics.md)), push ([Mobile-0006](../adr/ADR-Mobile-0006-push-notifications.md)).
- **API access:** a generated OpenAPI SDK ([Mobile-0005](../adr/ADR-Mobile-0005-openapi-sdk-generation.md)) in `packages/api_client`.
- **Features (mirror the platform):** auth, tenant, users, profile, sessions, audit, apikeys, webhooks, files, notifications, settings.
- **Release:** strategy/signing/versioning ([Mobile-0010..0012](../adr/ADR-Mobile-0010-release-strategy.md)) with fastlane pipelines.

## 5. Theme Engine

The web UI is themed exclusively through the **Theme Adapter Architecture** ([decision record](../frontend/themes/theme-decision-record.md)):

```
business module ──▶ platform-ui primitive ──▶ useTheme().adapter.<contract> ──▶ <theme>Adapter
                                                          ▲
                                            ThemeProvider (VITE_THEME → registry)
```

- **`platform-ui/`** — stable, theme-agnostic primitives (the only UI API modules use).
- **`theme/contracts/`** — **11 typed contracts** (Layout, Navigation, Card, Table, Dialog, Notification, Badge, Avatar, Tabs, Breadcrumb, Chart) unified by `ThemeAdapter`.
- **`theme/adapters/<id>/`** — concrete implementations reusing a theme's **visual design only**: `coreui` (default) and `hexadash` (opt-in).
- **`registry.ts` + `config.ts`** — map `ThemeId` → adapter; safe default if `VITE_THEME` is unset/invalid.

Adapters render; the platform decides (access, routing, nav visibility, state). Direct theme integration is forbidden.

## 6. Module system

- **Backend module slice:** `Domain` (aggregates, events) → `Application` (commands/queries/handlers) → `Infrastructure` (EF, repositories, outbox writer) → `Api` (endpoints). Wired into the Host via a module registration extension.
- **Contracts:** inter-module messaging flows through `SharedKernel.Contracts`; modules never reference each other's Infrastructure.
- **Web module slice:** `pages` / `components` / `api` / `hooks` / `guards` / `types`, mounted via the central router with guards.
- **Mobile feature slice:** `pages` / `providers` / `data` / `models` / `widgets` per feature.
- **Extending:** add modules via the [extension guides](../extending/add-backend-module.md); every module ships docs.

## 7. Governance system

- **Documentation governance** — code and docs evolve together; PR checklist + `validate-docs` + CI gate ([documentation-governance](../documentation-governance.md)).
- **ADR process** — significant decisions recorded in `docs/adr/` (30 ADRs incl. template) using [ADR-0000-template](../adr/ADR-0000-template.md).
- **Build & package governance** — central package management, one source of versions ([ADR-0010](../adr/ADR-0010-build-package-governance.md)).
- **Theme governance** — selection checklist, onboarding guide, lifecycle, adapter dev guide, reusable prompts ([theme docs](../frontend/themes/theme-decision-record.md)).
- **Release engineering** — CI for backend/frontend, docs validation, and mobile build/release pipelines.

---

> This overview is the V1 orientation map. For the full inventory see [platform-v1-manifest.md](./platform-v1-manifest.md); for deeper architecture see [docs/architecture/](../architecture/system-overview.md).
