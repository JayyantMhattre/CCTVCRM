# Platform Discovery Report

**Project:** CCTVCRM (built on the **Ashraak Enterprise Platform V1** base template)
**Report type:** Project Bootstrap — Platform Discovery
**Platform baseline:** `v1.0.0` (Platform Freeze)
**Date:** June 11, 2026
**Status:** Discovery only — **no code, no modules, no platform changes were made.**

> This document is the output of the mandatory pre-implementation discovery. It catalogs what the platform already provides, classifies each capability for reuse on the CCTV project, and identifies the sanctioned extension points. It does **not** propose or implement any business functionality.

---

## 1. Executive summary

This repository was generated from the **Ashraak** platform — an enterprise, multi-tenant SaaS starter template delivered as a **modular monolith** with three runtimes:

| Runtime | Technology |
|---------|------------|
| Backend | .NET 10, ASP.NET Core Minimal APIs, EF Core 9, OpenIddict, PostgreSQL / Redis / MongoDB |
| Web | React 19, TypeScript, Vite 6, TanStack Query, Zustand, CoreUI 5 |
| Mobile | Flutter (Android/iOS), Riverpod, go_router, OpenAPI-generated SDK |

The **Core Platform is frozen and feature-complete** at `v1.0.0`. Its scope is closed: it is maintained (bug fixes, security, dependency upgrades, docs, new theme adapters) but **not expanded** with product-specific features. All new product value — including everything for the CCTV project — must be built as **business modules on top of the frozen platform**, consuming Core only through published contracts/APIs.

**Key implication for CCTVCRM:** The CCTV solution is a **Business Module** (or set of modules). It must be optional, decoupled, replaceable, and independently deployable. It may freely reuse identity, tenancy, files, notifications, audit, webhooks, API keys, caching, the theme engine, and the mobile/web shells — but it must never modify Core code.

> **Important platform fact:** The web frontend is **React 19 + CoreUI**, and mobile is **Flutter** — *not* Angular and *not* native Kotlin/Swift ([ADR-0012](../adr/ADR-0012-flutter-mobile-platform.md)). Any CCTV frontend work must follow the React/Flutter conventions documented below.

---

## 2. Platform architecture (as discovered)

### 2.1 Backend — modular monolith

`BackEnd/src/Modules/` — each module is a vertical slice (`Domain` / `Application` / `Infrastructure` / `Api`) with its **own PostgreSQL schema**. Modules communicate **only** through `Ashraak.SharedKernel.Contracts` — no cross-module Infrastructure references ([ADR-0001](../adr/ADR-0001-modular-monolith.md), [module-map](../architecture/module-map.md)).

```
React SPA / Flutter ──REST + OAuth2──▶ Ashraak.Api (Host / composition root)
                                        Caching → Auth → Tenant → Users → Audit (observer)
                                                │            │            │
                                          PostgreSQL       Redis        MongoDB
                                        (auth/tenant/      (cache)      (audit)
                                         users/files/
                                         webhooks)
```

| Layer | Project(s) | Notes |
|-------|-----------|-------|
| Host | `Ashraak.Api` | Composition root; middleware (correlation, rate limiting, auth, API-key), health checks, feature flags, env validation |
| SharedKernel | `Ashraak.SharedKernel` | DDD primitives, `Result`, outbox types |
| Contracts | `Ashraak.SharedKernel.Contracts` | `ITenantService`, `IUserService`, `IAuditService`, `INotificationService`, `IFileStorage`, `IWebhookPublisher`, events — **the integration surface for business modules** |
| BuildingBlocks | Application, EventBus, Infrastructure, Data.* | Base classes, behaviors, outbox writer/processor |

**Eventing:** In-process via MediatR. Domain events, contract events (in `SharedKernel.Contracts`), and integration events (`IEventBus` is a stub). RabbitMQ is provisioned in Docker but **not connected** to the API ([eventing](../architecture/eventing.md)). Audit acts as a universal domain-event observer.

### 2.2 Module registration order (Host)

| Layer | Registers |
|-------|-----------|
| 0 | Shared infra (Caching) — before Auth |
| 1 | Identity (Auth) |
| 2 | Business modules (Tenant, Users, **and future business modules like CCTV**) |
| 3 | Observers (Audit) — last |

### 2.3 Data services

| Service | Used by |
|---------|---------|
| PostgreSQL 17 | Auth, Tenant, Users, Files, Webhooks (per-module schemas) |
| Redis 7 | Caching, sessions, distributed locks, rate limiting |
| MongoDB 7 | Audit (`audit_entries`) |
| Seq | Serilog structured logs |
| RabbitMQ | Provisioned only; **not wired to API** (future) |

---

## 3. Existing platform capabilities (inventory)

Status legend: ✅ Stable · 🟡 Available, opt-in/preview · 📝 Scaffold/stub (see [known-limitations](../releases/known-limitations.md) and [gap analysis](../documentation-audit/documentation-gap-analysis.md)).

### 3.1 Identity

| Capability | Status | Module | Notes |
|------------|--------|--------|-------|
| Authentication (JWT / OAuth2) | ✅ | `Auth` | OpenIddict password grant; `POST /connect/token`; `POST /api/v1/auth/register`. Dev signing keys are **ephemeral** (tokens invalid after restart) |
| RBAC (roles) | ✅ | `Auth` | Roles in JWT claims |
| ABAC (permissions) | ✅ | `Auth` | Permission claims; cached in Redis via `IAuthPermissionChecker` |
| MFA / TOTP | ✅ | `Auth` | [MFA flows](../modules/auth/mfa/flows.md) |
| Sessions | ✅ | `Auth` | List / revoke active sessions |
| Invitations | ✅ | `Auth` | Tokenized invite links |
| Tenancy & isolation | ✅ | `Tenant` | `tenant_id` JWT claim + `TenantResolutionMiddleware` + EF filters; `ITenantService` |
| User profiles & preferences | ✅ | `Users` | Profiles, notification preferences (not credentials); `IUserService` |
| SSO (Google / Microsoft) | 🟡 | `Auth` | Foundations present at `/api/auth/sso/*`; GA hardening deferred |

### 3.2 Platform services

| Capability | Status | Location | Contract / Notes |
|------------|--------|----------|------------------|
| Caching | ✅ | `Caching` | `ICacheService`, `ISessionCacheService`, distributed locks (Redis + memory) |
| Outbox + hosted processors | ✅ | `BuildingBlocks` + Host | Transactional event publication for Auth/Tenant/Users ([ADR-0002](../adr/ADR-0002-outbox-pattern.md), [ADR-0007](../adr/ADR-0007-outbox-hosted-processors.md)) |
| Feature flags | 🟡 | Host | `ConfigFeatureFlagService` — toggle features by config |
| Rate limiting | ✅ | Host middleware | Per-client throttling via Redis |
| Configuration validation | ✅ | Host | Validates env/config at startup |

### 3.3 Files

| Capability | Status | Notes |
|------------|--------|-------|
| Tenant-scoped storage | ✅ | `Files` module; metadata in PostgreSQL `files`, blobs on local / S3 / Azure |
| Contract | ✅ | `IFileStorage` in `SharedKernel.Contracts.Storage` — **never reference `Files.Infrastructure` directly** |
| Default (dev) | — | `Storage:Provider=Local` under `./data/files`; scan hook + tenant isolation ([security](../modules/files/security.md)) |

### 3.4 Notifications

| Capability | Status | Notes |
|------------|--------|-------|
| Event-driven email | ✅ | `Notifications` module; templates + providers (console/SMTP) |
| Contract | ✅ | `INotificationService` in `SharedKernel.Contracts.Notifications` |
| Future channels | — | SMS / in-app inbox are candidate future platform features (gated) |

### 3.5 Audit

| Capability | Status | Notes |
|------------|--------|-------|
| Capture (API + entity + domain events) | ✅ | Middleware + EF interceptor + MediatR `INotificationHandler<IDomainEvent>` → MongoDB (async channel + hosted writer), per-tenant hash chains |
| Contract | ✅ | `IAuditService`, `AuditEntryDto` |
| Read API | 📝 | `GET /api/v1/audit-logs` is a **stub** — does not yet query MongoDB |

### 3.6 Webhooks

| Capability | Status | Notes |
|------------|--------|-------|
| Subscriptions | ✅ | `Webhooks` module; tenant-scoped, typed/versioned event catalog |
| Delivery engine | ✅ | Async, HMAC-signed ([delivery-engine](../modules/webhooks/delivery-engine.md)) |
| Retries + DLQ + replay | ✅ | Backoff, dead-letter queue, manual replay ([ADR-Webhook-0004](../adr/ADR-Webhook-0004-webhook-retry-and-dlq.md)) |
| Secret storage | ✅ | Encrypted subscription secrets |
| Admin UI (web) + mobile visibility | ✅ | W4 web operations center + W5 read-only mobile monitoring |
| Contract | ✅ | `IWebhookPublisher`, `IWebhookSubscriptionRepository` — **any module may publish catalog events** |

### 3.7 API Keys

| Capability | Status | Notes |
|------------|--------|-------|
| Issuance & scopes | ✅ | Tenant-scoped, Argon2id-hashed secrets, scope-based M2M auth |
| Auth middleware | ✅ | Authenticates requests via API key |
| Rotation / revocation | ✅ | [rotation](../modules/apikeys/rotation.md) |
| Usage tracking | ✅ | Usage metrics |
| Admin UI (web) + mobile visibility | ✅ | Manage keys (web) + read-only mobile |
| Excluded (future) | — | OAuth provider, developer/partner portal, external IdP for keys |

### 3.8 Theme Engine (Frontend)

| Concept | Status | Notes |
|---------|--------|-------|
| `platform-ui` primitives | ✅ | `PlatformCard`, `PlatformTable`, `PlatformDialog`, etc. — **the only UI API modules use** |
| Theme Adapter Architecture | ✅ | **11 typed contracts** (Layout, Navigation, Card, Table, Dialog, Notification, Badge, Avatar, Tabs, Breadcrumb, Chart) |
| Theme registry / switching | ✅ | `VITE_THEME` → registry; safe default if unset |
| CoreUI theme | ✅ | Production default |
| HexaDash theme | 🟡 | Validated, opt-in (`VITE_THEME=hexadash`) |

**Binding rule:** business modules render `platform-ui` primitives only — **direct import of a vendor theme is forbidden** ([theme decision record](../frontend/themes/theme-decision-record.md)). Adding a new theme is an *additive, allowed* change under the freeze (one adapter folder + one registry line).

### 3.9 Mobile (Flutter)

| Capability | Status | Notes |
|------------|--------|-------|
| App foundation | ✅ | Flutter shell, theming, routing (`go_router`), state (`Riverpod`) |
| Feature parity | ✅ | auth, tenant, users, profile, sessions, audit, apikeys, webhooks, files, notifications, settings |
| Production capabilities | ✅ | secure token storage, offline cache, push notifications, crash reporting, analytics, deep links |
| OpenAPI SDK | 🟡 | Generated `packages/api_client` (generation partially wired) |
| Release engineering | ✅ | fastlane, build flavors (dev/qa/uat/prod), Android/iOS signing, store-readiness CI |

### 3.10 Frontend (Web)

| Capability | Status | Notes |
|------------|--------|-------|
| SPA shell & routing | ✅ | Lazy routes, guards, layouts (`core/`); React Router 7 |
| Auth integration | ✅ | Login/register/session + token refresh (Zustand + TanStack Query) |
| Existing modules | ✅ | `auth`, `dashboard`, `tenant`, `users`, `audit`, `apikeys`, `webhooks` |
| Data fetching / forms | ✅ | TanStack Query; react-hook-form + Zod |
| Toasts, file upload, audit viewer, correlation support | ✅ | `shared/*` + module UIs |

### 3.11 Backend platform infrastructure

| Capability | Status | Notes |
|------------|--------|-------|
| Modular monolith structure | ✅ | Module isolation via contracts ([ADR-0001](../adr/ADR-0001-modular-monolith.md)) |
| Central package management | ✅ | `Directory.Packages.props`, `global.json` SDK pin, `Directory.Build.props` |
| Structured logging | ✅ | Serilog → Console + Seq |
| Distributed tracing / metrics | ✅ | OpenTelemetry → OTLP ([ADR-0005](../adr/ADR-0005-open-telemetry.md)) |
| Correlation IDs | ✅ | End-to-end request tracing |
| Health probes | ✅ | `/health/live`, `/health/ready` (postgres, redis, mongodb) |
| Docker environment | ✅ | Local infra via Docker Compose |

### 3.12 Documentation Governance & Release Engineering

| Capability | Status | Notes |
|------------|--------|-------|
| Documentation governance | ✅ | "No feature without docs"; mandatory 7-file module doc set; code is source of truth |
| Docs validation CI | ✅ | `docs-validation.yml` (warn by default; `DOC_VALIDATE_MODE=fail` to gate) |
| ADR process | ✅ | 30 ADRs (platform, webhooks, apikeys, mobile) |
| CI (build/test) | ✅ | `ci.yml` backend + frontend |
| Mobile CI + releases | ✅ | `mobile.yml`, `android-release.yml`, `ios-release.yml` |

---

## 4. Governance model (must-follow for CCTV)

The platform enforces a strict three-tier classification ([module-classification-policy](../governance/module-classification-policy.md)):

| Class | Definition | Governance |
|-------|-----------|------------|
| **Core Platform** | Domain-independent, reused by many products | **Frozen** at v1.0.0; change requires justification + ADR + governance review + multi-project reuse evidence |
| **Business Module** | Product/domain-specific value | Optional, decoupled, replaceable, independently deployable |
| **Experimental** | Unproven incubation | Sandbox, behind feature flags |

**Every business module MUST be:**

| Property | Meaning |
|----------|---------|
| Optional | Platform builds & runs without it; removing it never breaks Core |
| Decoupled | Talks to Core only through published contracts/APIs; no edits to Core code |
| Replaceable | Swappable without touching Core |
| Independently deployable | Built, versioned, released on its own cadence |

**Dependency direction:** `Business Module ──▶ Core Platform` — **never the reverse.** Core is never aware of any business module.

**What may NOT enter Core:** domain/product-specific features, CRM/billing/LMS/HRMS/ERP workflows, any single-product capability. These belong in business modules.

---

## 5. CCTV Project Readiness Assessment

**CCTVCRM classification:** **Business Module** (likely a CRM-style domain module, possibly with companion modules). It is the kind of value the platform was explicitly built to host — the [platform-status](../governance/platform-status-v1.md) roadmap even lists **CRM** as a canonical example consuming Auth/Tenants/Notifications/Audit.

### 5.1 Readiness verdict

| Dimension | Verdict | Rationale |
|-----------|---------|-----------|
| Identity & access | ✅ Ready | Auth, RBAC/ABAC, MFA, sessions, invitations all stable and consumable via contracts |
| Multi-tenancy | ✅ Ready | Tenant isolation, resolution middleware, EF filters in place |
| User management | ✅ Ready | Profiles/preferences via `IUserService` |
| File handling (e.g. camera/site documents, snapshots, exports) | ✅ Ready | `IFileStorage` (local/S3/Azure), tenant-scoped |
| Notifications (alerts, reminders) | ✅ Ready (email) | `INotificationService`; SMS/in-app are future platform candidates, or implement within the module |
| Audit / compliance trail | ✅ Ready (capture) | Domain-event capture is automatic; **read API is a stub** — module-specific querying may be needed |
| Integrations / outbound events | ✅ Ready | Webhooks (signed, retried, DLQ) + API Keys for M2M |
| Web UI | ✅ Ready (shell) | React shell, routing, guards, theme engine; build module pages against `platform-ui` |
| Mobile | ✅ Ready (foundation) | Flutter foundation + 11 features; add a `features/cctv*` slice consuming the OpenAPI SDK |
| Eventing/messaging | ⚠️ Partial | In-process MediatR works; RabbitMQ/MassTransit **not wired** — design for outbox + contracts, not a live bus |
| Charts/dashboards | ⚠️ Partial | `ChartContract` exists but no chart library committed (deferred) |

### 5.2 Constraints & caveats discovered (plan around these)

- **Frontend is React, not Angular.** The user/global C# + Angular standards apply to Angular *where Angular is used*, but **this repo's web is React 19 + CoreUI**. CCTV web work must follow the React conventions and the theme-adapter rules, not introduce Angular.
- **Audit read API is a stub** — if CCTV needs to surface audit data, plan module-level read access.
- **RabbitMQ is not connected** — do not assume a live message broker; use the outbox + contract events.
- **JWT signing keys are ephemeral in dev** — production key persistence must be configured before release.
- **No EF migration assemblies in repo today** — migrations are adopted per-module when needed; follow the DbContext pattern in [add-backend-module](../extending/add-backend-module.md).
- **Module interiors not yet fully migrated to `platform-ui`** — new CCTV pages should be built `platform-ui`-first from the start to stay theme-portable.

---

## 6. Reusable Platform Capabilities (for CCTVCRM)

### 6.1 Directly reusable (consume via contracts/APIs — no Core changes)

| Capability | How CCTV consumes it |
|------------|----------------------|
| **Authentication / JWT / OAuth2** | Protect CCTV endpoints; rely on Host auth middleware |
| **RBAC / ABAC** | Define CCTV permissions; check via `IAuthPermissionChecker` / `[Authorize]` policies |
| **MFA, Sessions, Invitations** | Reuse as-is for CCTV users |
| **Tenancy** | `ITenantService` + automatic EF tenant filtering for CCTV data isolation |
| **Users** | `IUserService` for profile/preference data |
| **Files** | `IFileStorage` for any CCTV documents/exports/snapshots |
| **Notifications** | `INotificationService` for CCTV email alerts |
| **Audit** | Raise CCTV domain events → auto-captured by the Audit observer |
| **Webhooks** | `IWebhookPublisher` to emit CCTV catalog events to external systems |
| **API Keys** | M2M auth for CCTV integrations / devices / partner systems |
| **Caching** | `ICacheService` + distributed locks for CCTV hot paths |
| **Outbox** | Reliable CCTV event publication |
| **Observability** | Logging, tracing, correlation, health — automatic via Host |
| **Theme Engine / `platform-ui`** | Build CCTV web pages against primitives; inherit CoreUI/HexaDash |
| **Mobile foundation** | Add CCTV Flutter feature(s) reusing auth/SDK/secure storage/offline/push |
| **Documentation governance & CI** | CCTV module ships with the mandatory 7-file doc set + tests |

### 6.2 Reusable patterns / scaffolding

- Vertical-slice module structure (`Domain`/`Application`/`Infrastructure`/`Api`) — see [add-backend-module](../extending/add-backend-module.md).
- Per-module PostgreSQL schema + DbContext + audit interceptor pattern.
- Web module structure (`pages`/`components`/`api`/`hooks`/`guards`/`types`) rendering `platform-ui`.
- Mobile feature structure (`pages`/`providers`/`data`/`models`/`widgets`) using Riverpod + go_router + OpenAPI SDK.
- ADR + documentation + feature-flag rollout conventions.

### 6.3 Capabilities likely NOT required by CCTV (available but optional)

These are present in the platform; CCTV can ignore them unless a specific need arises:

- **HexaDash theme** (CoreUI default is sufficient unless a rebrand is desired).
- **SSO providers** (only if CCTV requires external IdP login).
- **Per-theme code splitting / adapter-routed toasts** (deferred platform optimizations, not blockers).
- **Mobile release-engineering internals** (already done; only relevant if CCTV ships a mobile build).
- **Charting backend** (no committed library; only relevant if CCTV needs charts).

> "Not required" means *not a dependency to start CCTV*. None of these should be removed — they remain part of the frozen platform.

---

## 7. Potential extension points (sanctioned)

All of the following are the **correct, governance-approved** ways to deliver CCTV value without touching Core:

| Extension point | Mechanism | Guide / Rule |
|-----------------|-----------|--------------|
| **New backend business module(s)** | Add `Modules/Cctv*` vertical slices; register at Host **Layer 2**; own DB schema | [add-backend-module](../extending/add-backend-module.md), [business-module-policy](../governance/business-module-policy.md) |
| **New contract events** | Add CCTV events to `SharedKernel.Contracts`; publish/consume via MediatR + outbox | [add-contracts-and-handlers](../extending/add-contracts-and-handlers.md), [eventing](../architecture/eventing.md) |
| **Webhook catalog events** | Register CCTV event types before publishers ship | [event-catalog](../modules/webhooks/event-catalog.md) |
| **New web module** | `apps/web/src/modules/cctv*` rendering `platform-ui`; mount via central router; reuse Core guards | [add-frontend-route](../extending/add-frontend-route.md) |
| **New mobile feature** | `FrontEnd.Mobile/lib/features/cctv*` using Riverpod + go_router + OpenAPI SDK | [business-module-policy](../governance/business-module-policy.md) §4 |
| **New theme adapter** (optional) | Additive; one adapter folder + one registry line | [theme-onboarding-guide](../frontend/themes/theme-onboarding-guide.md) |
| **Observer module** (if CCTV needs cross-cutting capture) | Register at **Layer 3**, like Audit | [add-observer-module](../extending/add-observer-module.md) |
| **Feature-flag-gated rollout** | Gate CCTV behind `ConfigFeatureFlagService` | [feature-flags](../platform/feature-flags/README.md) |

**Required artifacts for each CCTV module** ([business-module-policy](../governance/business-module-policy.md) §5): README declaring classification = Business Module + owner + Core dependencies; full `docs/modules/<module>/` set; automated tests; independent SemVer; recommended feature flag.

**Anti-patterns to avoid** (rejected in review): importing/editing Core internals instead of using contracts; making Core depend on CCTV; importing a UI theme directly; sharing a DB schema with Core/another module; coupling two business modules.

---

## 8. Recommended next steps (planning, not implementation)

> No code in this phase. These are the discovery-driven recommendations for the subsequent planning phase.

1. **Confirm CCTV scope & module decomposition** — decide whether CCTVCRM is one module or several (e.g. CRM core + device/site management) and which Core contracts each consumes.
2. **Confirm classification** — register CCTV as Business Module(s); plan independent versioning and a feature flag.
3. **Map CCTV needs to reusable capabilities** — use §6.1 as the consumption checklist; identify any genuine gaps.
4. **Identify any Core gaps early** — e.g. Audit read API stub, charting library, SMS channel — and decide module-level vs. (rare) extension-policy path.
5. **Plan the documentation set** — the mandatory 7-file module docs + ADRs are part of definition-of-done from the first PR.
6. **Plan web/mobile slices** — `platform-ui`-first web pages; Flutter feature slice if mobile is in scope.

---

## 9. Source documents reviewed

- `README.md`, `docs/index.md` (entry points)
- `docs/releases/platform-v1-manifest.md`, `docs/releases/platform-capabilities.md`
- `docs/architecture/module-map.md`, `system-overview.md`, `eventing.md`
- `docs/governance/` — platform-status-v1, platform-freeze-policy, module-classification-policy, business-module-policy
- `docs/modules/` — auth, tenant, users, audit, notifications, files, caching, webhooks, apikeys (READMEs + supporting docs)
- `docs/frontend/architecture.md`, `docs/frontend/themes/theme-decision-record.md`
- `docs/mobile/mobile-platform-manifest.md`
- `docs/documentation-governance.md`, `docs/extending/add-backend-module.md`
- `docs/adr/` (30 ADRs — platform, webhooks, apikeys, mobile)

---

**Conclusion:** The Ashraak platform is a mature, frozen, feature-complete foundation. CCTVCRM is well-positioned to be delivered as one or more **Business Modules** that reuse the platform's identity, tenancy, files, notifications, audit, webhooks, API keys, caching, theme engine, and mobile/web shells — strictly through published contracts, with **no changes to the frozen Core**.
