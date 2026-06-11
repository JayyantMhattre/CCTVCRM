# Platform V1 Manifest

Complete inventory of the Ashraak platform at the V1 baseline (the **platform freeze** snapshot). This is the authoritative "what's in the box" reference.

**Related:** [v1.0.0-release-notes.md](./v1.0.0-release-notes.md) · [platform-capabilities.md](./platform-capabilities.md) · [platform-architecture-overview.md](./platform-architecture-overview.md)

---

## 1. Backend modules (9 + building blocks)

`BackEnd/src/Modules/` — each is a Domain/Application/Infrastructure/Api slice with its own PostgreSQL schema.

| Module | Responsibility | Docs |
|--------|----------------|------|
| `Auth` | Identity, JWT/OAuth2, RBAC/ABAC, MFA, invitations, sessions | [docs/modules/auth](../modules/auth/README.md) |
| `Tenant` | Workspace provisioning, settings, isolation | [docs/modules/tenant](../modules/tenant/README.md) |
| `Users` | Profiles & preferences | [docs/modules/users](../modules/users/README.md) |
| `Audit` | Login/API/entity/event capture → MongoDB (observer) | [docs/modules/audit](../modules/audit/README.md) |
| `Notifications` | Event-driven email (providers/templates) | [docs/modules/notifications](../modules/notifications/README.md) |
| `Files` | Tenant-scoped storage (local/S3/Azure) | [docs/modules/files](../modules/files/README.md) |
| `Caching` | Redis + memory cache, sessions, distributed locks | [docs/modules/caching](../modules/caching/README.md) |
| `Webhooks` | Subscriptions, delivery engine, retries, DLQ | [docs/modules/webhooks](../modules/webhooks/README.md) |
| `ApiKeys` | Scoped key issuance, hashing, rotation, revocation, auth | [docs/modules/apikeys](../modules/apikeys/README.md) |

**Building blocks:** `Ashraak.BuildingBlocks.*` — SharedKernel.Contracts, Outbox (writer/processor/hosted service), Infrastructure/Persistence base. Docs: [shared-kernel](../modules/shared-kernel/README.md), [building-blocks](../modules/building-blocks/api.md), [platform/outbox](../platform/outbox/retry-behavior.md).

**Host:** `Ashraak.Api` — composition root, middleware (correlation, rate limiting, auth, API-key), health checks, feature flags, environment validation. Docs: [modules/host](../modules/host/registration.md).

## 2. ADRs (30, incl. template)

`docs/adr/`

**Platform (13):**
ADR-0000-template · ADR-0001-modular-monolith · ADR-0002-outbox-pattern · ADR-0003-observer-modules · ADR-0004-redis-caching · ADR-0005-open-telemetry · ADR-0006-notifications-module · ADR-0007-outbox-hosted-processors · ADR-0008-host-platform-hardening · ADR-0009-auth-tenant-completion · ADR-0010-build-package-governance · ADR-0011-files-storage-module · ADR-0012-flutter-mobile-platform

**Webhooks (4):**
ADR-Webhook-0001-webhook-platform-architecture · ADR-Webhook-0002-webhook-secret-storage · ADR-Webhook-0003-webhook-delivery-engine · ADR-Webhook-0004-webhook-retry-and-dlq

**API Keys (1):**
ADR-ApiKeys-0001-api-keys-platform

**Mobile (12):**
ADR-Mobile-0001-state-management · ADR-Mobile-0002-navigation · ADR-Mobile-0003-environment-configuration · ADR-Mobile-0004-secure-token-storage · ADR-Mobile-0005-openapi-sdk-generation · ADR-Mobile-0006-push-notifications · ADR-Mobile-0007-offline-cache · ADR-Mobile-0008-crash-reporting · ADR-Mobile-0009-analytics · ADR-Mobile-0010-release-strategy · ADR-Mobile-0011-versioning-strategy · ADR-Mobile-0012-signing-strategy

## 3. Documentation sections

`docs/`

| Section | Contents |
|---------|----------|
| `adr/` | Architecture Decision Records (30) |
| `api/` | API reference (users, files, …) |
| `architecture/` | System overview, modular monolith, observability |
| `documentation-audit/` | Scaffold-vs-implementation gap analysis, outdated-docs report |
| `errors/` | Common failures, error catalog, problem-details |
| `extending/` | Add backend/frontend module, contracts, observers |
| `frontend/` | Web docs incl. **themes/** (engine, governance, current-theme reports), toasts, errors, file-upload, audit-viewer, notifications, correlation-support, tenant-settings |
| `getting-started/` | Local development, environment variables, debugging |
| `mobile/` | Mobile foundation, modules, release, manifest |
| `modules/` | Per-module docs (auth, tenant, users, audit, notifications, files, caching, webhooks, apikeys, invitations, building-blocks, shared-kernel, host) |
| `operations/` | Logging, Seq, Redis, deployment, startup troubleshooting |
| `platform/` | Correlation, health, feature-flags, rate-limits, outbox, packages, configuration, platform-manifest |
| `releases/` | **This V1 release documentation set** |

Governance docs (root of `docs/`): [index.md](../index.md), [documentation-governance.md](../documentation-governance.md), [documentation-pr-checklist.md](../documentation-pr-checklist.md), [developer-workflow.md](../developer-workflow.md).

## 4. CI pipelines (5)

`.github/workflows/`

| Pipeline | Purpose |
|----------|---------|
| `ci.yml` | Backend + frontend build/test |
| `docs-validation.yml` | Documentation governance enforcement |
| `mobile.yml` | Flutter build/test |
| `android-release.yml` | Android store release |
| `ios-release.yml` | iOS store release |

## 5. Themes (2 adapters, 11 contracts)

`FrontEnd/apps/web/src/theme/`

| Theme | Status | Location |
|-------|--------|----------|
| CoreUI | ✅ Production default | `theme/adapters/coreui` |
| HexaDash | 🟡 Validated, opt-in (`VITE_THEME=hexadash`) | `theme/adapters/hexadash` |

**Contracts (11):** Layout · Navigation · Card · Table · Dialog · Notification · Badge · Avatar · Tabs · Breadcrumb · Chart. Governance: [theme docs](../frontend/themes/theme-decision-record.md).

## 6. Frontend (Web)

`FrontEnd/apps/web/`

- **Stack:** React 19, TypeScript, Vite 6, TanStack Query, Zustand, react-router, react-hook-form + Zod, CoreUI 5. **Node 20+** required.
- **Modules (7):** `auth`, `dashboard`, `tenant`, `users`, `audit`, `apikeys`, `webhooks`.
- **Core:** `core/` (router, providers, api client, auth), `shared/` (components, guards, hooks, ui/toast, file-upload), `platform-ui/` (theme-agnostic primitives), `theme/` (contracts, adapters, registry, provider).
- **Env:** `VITE_API_BASE_URL`, `VITE_API_VERSION`, `VITE_APP_NAME`, `VITE_THEME`.

## 7. Backend

`BackEnd/`

- **Stack:** .NET 10, ASP.NET Core Minimal APIs, EF Core 9, OpenIddict.
- **Solution:** `Ashraak.slnx`; central package management (`Directory.Packages.props`), SDK pin (`global.json`), shared build props (`Directory.Build.props`).
- **Data:** PostgreSQL (per-module schemas), Redis, MongoDB (audit).
- **Observability:** Serilog + Seq, OpenTelemetry OTLP, correlation IDs, health probes, Redis rate limits.
- **Local infra:** Docker Compose ([DOCKER_ENVIRONMENT](../../BackEnd/DOCKER_ENVIRONMENT.md)).

## 8. Mobile

`FrontEnd.Mobile/`

- **Stack:** Flutter (Android/iOS), Riverpod, go_router; OpenAPI-generated `packages/api_client`.
- **Features (11):** `auth`, `tenant`, `users`, `profile`, `sessions`, `audit`, `apikeys`, `webhooks`, `files`, `notifications`, `settings`.
- **Capabilities:** secure token storage, offline cache, push notifications, crash reporting, analytics, deep links.
- **Release:** fastlane via `mobile.yml` / `android-release.yml` / `ios-release.yml`; strategy/signing/versioning per Mobile ADRs.
- Manifest detail: [mobile-platform-manifest](../mobile/mobile-platform-manifest.md).

---

## V1 inventory summary

| Area | Count |
|------|-------|
| Backend modules | 9 (+ BuildingBlocks + Host) |
| Web modules | 7 |
| Mobile features | 11 |
| Theme adapters | 2 (CoreUI default, HexaDash opt-in) |
| Theme contracts | 11 |
| ADRs | 30 (incl. template) |
| CI pipelines | 5 |
| Runtimes | 3 (Backend, Web, Mobile) |

**Platform V1 Release Documentation Complete. Ready for Platform Freeze.**
