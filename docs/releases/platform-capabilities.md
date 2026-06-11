# Platform Capabilities — V1

Authoritative catalog of Ashraak V1 capabilities, grouped by domain. For each: **Purpose**, **Status**, **Module/Location**, **Dependencies**.

**Status legend:** ✅ Stable (production-shaped) · 🟡 Available, opt-in/preview · 📝 Scaffolded/Documented (see [known-limitations.md](./known-limitations.md) and the [documentation gap analysis](../documentation-audit/documentation-gap-analysis.md) for scaffold-vs-implementation nuances).

---

## 1. Identity

| Capability | Purpose | Status | Module | Dependencies |
|------------|---------|--------|--------|--------------|
| Authentication | Registration, login, JWT/OAuth2 token issuance | ✅ | `Auth` | OpenIddict, EF Core, PostgreSQL |
| Tenancy & isolation | Per-tenant data isolation, provisioning, resolution | ✅ | `Tenant` | EF Core, PostgreSQL |
| RBAC / ABAC | Role- and permission-based authorization | ✅ | `Auth` | JWT claims, guards |
| MFA / TOTP | Multi-factor auth | ✅ | `Auth` | TOTP service ([MFA flows](../modules/auth/mfa/flows.md)) |
| Invitations | Invite users into a tenant with tokenized links | ✅ | `Auth` (Invitation aggregate) | [invitations flows](../modules/invitations/flows.md) |
| Sessions | List/revoke active user sessions | ✅ | `Auth` (UserSession) | [sessions](../modules/auth/sessions/README.md) |
| User profiles & preferences | Profile + notification preferences (not credentials) | ✅ | `Users` | EF Core |
| SSO foundations | External identity providers (Google/Microsoft) | 🟡 | `Auth` | OpenIddict external providers |

## 2. Platform Services

| Capability | Purpose | Status | Module | Dependencies |
|------------|---------|--------|--------|--------------|
| Caching | Distributed + memory cache, sessions, distributed locks | ✅ | `Caching` | Redis ([ADR-0004](../adr/ADR-0004-redis-caching.md)) |
| Outbox | Transactional, reliable event publication | ✅ | `BuildingBlocks` (Outbox) | EF Core ([ADR-0002](../adr/ADR-0002-outbox-pattern.md)) |
| Outbox hosted processors | Background dispatch for Auth/Tenant/Users | ✅ | `Host` + module outbox writers | [ADR-0007](../adr/ADR-0007-outbox-hosted-processors.md) |
| Audit | Capture login/API/entity/domain events | ✅ | `Audit` (observer) | MongoDB ([ADR-0003](../adr/ADR-0003-observer-modules.md)) |
| Notifications | Event-driven email (templates, providers) | ✅ | `Notifications` | SMTP/console providers ([ADR-0006](../adr/ADR-0006-notifications-module.md)) |
| Feature flags | Toggle platform features by config | 🟡 | `Host` (ConfigFeatureFlagService) | [feature-flags](../platform/feature-flags/README.md) |
| Rate limiting | Per-client request throttling | ✅ | `Host` middleware | Redis ([rate-limits](../platform/rate-limits/README.md)) |
| Configuration validation | Validate environment/config at startup | ✅ | `Host` (EnvironmentValidation) | [configuration/validation](../platform/configuration/validation.md) |

## 3. Integration

| Capability | Purpose | Status | Module | Dependencies |
|------------|---------|--------|--------|--------------|
| Webhooks — subscriptions | Register outbound event subscriptions | ✅ | `Webhooks` | EF Core ([Webhook-0001](../adr/ADR-Webhook-0001-webhook-platform-architecture.md)) |
| Webhooks — delivery engine | Signed, async event delivery | ✅ | `Webhooks` | HMAC signing ([delivery-engine](../modules/webhooks/delivery-engine.md)) |
| Webhooks — retries & DLQ | Backoff retries, dead-letter queue, replay | ✅ | `Webhooks` | [Webhook-0004](../adr/ADR-Webhook-0004-webhook-retry-and-dlq.md) |
| Webhooks — secret storage | Encrypted subscription secrets | ✅ | `Webhooks` | [Webhook-0002](../adr/ADR-Webhook-0002-webhook-secret-storage.md) |
| API Keys — issuance & scopes | Create scoped programmatic keys | ✅ | `ApiKeys` | [ADR-ApiKeys-0001](../adr/ADR-ApiKeys-0001-api-keys-platform.md) |
| API Keys — auth middleware | Authenticate requests via API key | ✅ | `ApiKeys` | hashing, middleware |
| API Keys — rotation/revocation | Rotate and revoke keys | ✅ | `ApiKeys` | [rotation](../modules/apikeys/rotation.md) |
| Files — storage | Tenant-scoped upload/download/URL | ✅ | `Files` | local/S3/Azure ([ADR-0011](../adr/ADR-0011-files-storage-module.md)) |

## 4. Frontend (Web)

| Capability | Purpose | Status | Location | Dependencies |
|------------|---------|--------|----------|--------------|
| SPA shell & routing | Lazy-loaded routes, guards, layouts | ✅ | `apps/web/src/core` | react-router |
| Auth integration | Login/register/session, token refresh | ✅ | `modules/auth` | Zustand, TanStack Query |
| Theme Engine (platform-ui) | Theme-agnostic UI primitives | ✅ | `apps/web/src/platform-ui` | — |
| Theme Adapter Architecture | 11 typed theme contracts + adapters | ✅ | `apps/web/src/theme` | [theme docs](../frontend/themes/theme-decision-record.md) |
| CoreUI theme | Production default theme | ✅ | `theme/adapters/coreui` | CoreUI 5 / Bootstrap 5 |
| HexaDash theme | Alternate theme (validated, opt-in) | 🟡 | `theme/adapters/hexadash` | scoped SCSS tokens ([readiness](../frontend/themes/current-theme/production-readiness-report.md)) |
| Data fetching | Server state, caching, retries | ✅ | modules | TanStack Query |
| Forms & validation | Typed forms with schema validation | ✅ | modules | react-hook-form + Zod |
| Toasts/notifications | App-wide toast system | ✅ | `shared/ui/toast` | [toasts](../frontend/toasts/usage.md) |
| Webhooks admin UI | Operations center for webhooks | ✅ | `modules/webhooks` | [admin-ui](../modules/webhooks/admin-ui/operations-center.md) |
| API Keys admin UI | Manage API keys | ✅ | `modules/apikeys` | [admin-ui](../modules/apikeys/admin-ui/README.md) |
| Audit viewer | Browse audit logs | ✅ | `modules/audit` | [audit-viewer](../frontend/audit-viewer/README.md) |
| File upload | Upload component | ✅ | `shared/file-upload` | [file-upload](../frontend/file-upload/README.md) |
| Correlation support | Surface correlation IDs for support | ✅ | `core` | [correlation-support](../frontend/correlation-support/support-workflow.md) |

## 5. Mobile

| Capability | Purpose | Status | Location | Dependencies |
|------------|---------|--------|----------|--------------|
| App foundation | Flutter shell, theming, routing | ✅ | `FrontEnd.Mobile/lib` | go_router ([Mobile-0002](../adr/ADR-Mobile-0002-navigation.md)) |
| State management | App/feature state | ✅ | `lib` | Riverpod ([Mobile-0001](../adr/ADR-Mobile-0001-state-management.md)) |
| Auth | Login/session on mobile | ✅ | `features/auth` | secure storage ([Mobile-0004](../adr/ADR-Mobile-0004-secure-token-storage.md)) |
| Tenant/Users/Profile/Settings | Core account features | ✅ | `features/*` | API SDK |
| Sessions | Manage sessions | ✅ | `features/sessions` | [sessions](../mobile/modules/sessions/README.md) |
| Audit | View audit logs | ✅ | `features/audit` | [audit](../mobile/modules/audit/README.md) |
| API Keys | Manage keys on mobile | ✅ | `features/apikeys` | [apikeys](../mobile/modules/apikeys/README.md) |
| Webhooks | Operations + alerts | ✅ | `features/webhooks` | [webhooks](../mobile/modules/webhooks/README.md) |
| Files | Upload/preview | ✅ | `features/files` | [files](../mobile/modules/files/README.md) |
| Notifications | Preferences + push | ✅ | `features/notifications` | push ([Mobile-0006](../adr/ADR-Mobile-0006-push-notifications.md)) |
| Offline cache | Local data availability | ✅ | `lib/core` | [Mobile-0007](../adr/ADR-Mobile-0007-offline-cache.md) |
| Crash reporting | Capture crashes | ✅ | `lib/core` | [Mobile-0008](../adr/ADR-Mobile-0008-crash-reporting.md) |
| Analytics | Usage analytics | ✅ | `lib/core` | [Mobile-0009](../adr/ADR-Mobile-0009-analytics.md) |
| OpenAPI SDK | Generated API client | ✅ | `packages/api_client` | [Mobile-0005](../adr/ADR-Mobile-0005-openapi-sdk-generation.md) |
| Deep links | Link routing | ✅ | `lib` | [deep-links](../mobile/deep-links/README.md) |

## 6. Operations

| Capability | Purpose | Status | Location | Dependencies |
|------------|---------|--------|----------|--------------|
| Structured logging | App logs with correlation | ✅ | `Host` | Serilog + Seq ([logging](../operations/logging.md)) |
| Distributed tracing | OTLP traces/metrics | ✅ | `Host` | OpenTelemetry ([ADR-0005](../adr/ADR-0005-open-telemetry.md)) |
| Correlation IDs | Trace a request end-to-end | ✅ | `Host` middleware | [correlation](../platform/correlation/support-guide.md) |
| Health probes | Liveness/readiness + dependency checks | ✅ | `Host/Health` | [health](../platform/health/README.md) |
| Rate limit ops | Throttle abusive clients | ✅ | `Host` | Redis |
| Docker environment | Local infra (DBs, Redis, Seq) | ✅ | `BackEnd` | Docker ([DOCKER_ENVIRONMENT](../../BackEnd/DOCKER_ENVIRONMENT.md)) |
| Startup troubleshooting | Diagnose boot issues | ✅ | docs | [startup-troubleshooting](../operations/startup-troubleshooting.md) |

## 7. Engineering

| Capability | Purpose | Status | Location | Dependencies |
|------------|---------|--------|----------|--------------|
| Modular monolith structure | Module isolation via contracts | ✅ | `BackEnd/src` | [ADR-0001](../adr/ADR-0001-modular-monolith.md) |
| SharedKernel.Contracts | Cross-module messaging | ✅ | `BuildingBlocks` | [shared-kernel](../modules/shared-kernel/README.md) |
| Central package management | Single source of versions | ✅ | `Directory.Packages.props` | [ADR-0010](../adr/ADR-0010-build-package-governance.md) |
| CI (build/test) | Backend + frontend pipeline | ✅ | `.github/workflows/ci.yml` | GitHub Actions |
| Docs validation CI | Enforce documentation governance | ✅ | `.github/workflows/docs-validation.yml` | validate-docs script |
| Mobile CI + releases | Build + Android/iOS store releases | ✅ | `mobile.yml`, `android-release.yml`, `ios-release.yml` | fastlane ([release](../mobile/release/README.md)) |
| ADR process | Record architectural decisions | ✅ | `docs/adr` | [ADR template](../adr/ADR-0000-template.md) |
| Documentation governance | Code + docs evolve together | ✅ | docs | [documentation-governance](../documentation-governance.md) |
| Module extension guides | Add backend/frontend modules | ✅ | docs | [extending](../extending/add-backend-module.md) |

---

> Capability statuses reflect the V1 baseline. For deferred/future capabilities see [platform-roadmap.md](./platform-roadmap.md); for constraints see [known-limitations.md](./known-limitations.md).
