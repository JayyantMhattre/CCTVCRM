# Ashraak Documentation

Enterprise SaaS template — modular monolith (.NET 10) + React 19 SPA + Flutter mobile (planned).

**Start here** for navigation. All paths are relative to `/docs/`.

---

## Quick links

| I want to… | Go to |
|------------|-------|
| Run the stack locally | [Getting started](./getting-started/local-development.md) |
| Understand architecture | [System overview](./architecture/system-overview.md) |
| Add a new backend module | [Extending — backend module](./extending/add-backend-module.md) |
| Look up an API endpoint | [API overview](./api/overview.md) |
| Configure Docker / env | [Docker setup](./getting-started/docker-setup.md) · [Environment variables](./getting-started/environment-variables.md) |
| Troubleshoot startup | [Startup troubleshooting](./operations/startup-troubleshooting.md) |
| Follow documentation rules | [Documentation governance](./documentation-governance.md) |

---

## Architecture

| Document | Description |
|----------|-------------|
| [System overview](./architecture/system-overview.md) | Platform context and runtime map |
| [Modular monolith](./architecture/modular-monolith.md) | Composition and boundaries |
| [Module map](./architecture/module-map.md) | All modules and dependencies |
| [Layering rules](./architecture/layering-rules.md) | Domain / Application / Infrastructure / Api |
| [Dependency rules](./architecture/dependency-rules.md) | Allowed references between modules |
| [Eventing](./architecture/eventing.md) | MediatR, contract events, integration events |
| [Outbox](./architecture/outbox.md) | Outbox scaffold vs current runtime |
| [Frontend architecture](./architecture/frontend-architecture.md) | React SPA structure |
| [Observability](./architecture/observability.md) | Logs, traces, metrics, health |

---

## Backend modules

| Module | README |
|--------|--------|
| SharedKernel | [modules/shared-kernel/README.md](./modules/shared-kernel/README.md) |
| BuildingBlocks | [modules/building-blocks/README.md](./modules/building-blocks/README.md) |
| Host (Api) | [modules/host/README.md](./modules/host/README.md) |
| Auth | [modules/auth/README.md](./modules/auth/README.md) · [MFA](./modules/auth/mfa/README.md) · [Sessions](./modules/auth/sessions/README.md) |
| Invitations | [modules/invitations/README.md](./modules/invitations/README.md) |
| Tenant | [modules/tenant/README.md](./modules/tenant/README.md) |
| Users | [modules/users/README.md](./modules/users/README.md) |
| Audit | [modules/audit/README.md](./modules/audit/README.md) |
| Notifications | [modules/notifications/README.md](./modules/notifications/README.md) |
| Files | [modules/files/README.md](./modules/files/README.md) |
| Caching | [modules/caching/README.md](./modules/caching/README.md) |

## Platform

| Topic | Link |
|-------|------|
| Outbox processors | [platform/outbox/](./platform/outbox/README.md) |
| Rate limiting | [platform/rate-limits/](./platform/rate-limits/README.md) |
| Correlation ID | [platform/correlation/](./platform/correlation/README.md) |
| Health checks | [platform/health/](./platform/health/README.md) |
| Configuration validation | [platform/configuration/](./platform/configuration/README.md) |
| Feature flags (foundation) | [platform/feature-flags/](./platform/feature-flags/README.md) |
| Package governance (CPM) | [platform/packages/](./platform/packages/README.md) · [ADR-0010](./adr/ADR-0010-build-package-governance.md) |
| Security audit events | [modules/audit/security-events/](./modules/audit/security-events/README.md) |
| **Webhooks** (W4) | [modules/webhooks/](./modules/webhooks/README.md) — operations center implemented |

Each module folder contains: `architecture.md`, `registration.md`, `api.md`, `events.md`, `extending.md`, `operations.md`.

---

## Webhooks (platform capability — W0)

Documentation and governance only — **no implementation** in W0.

| Document | Description |
|----------|-------------|
| [Webhooks index](./modules/webhooks/README.md) | Philosophy, navigation |
| [Foundation (W1)](./modules/webhooks/foundation.md) | Subscriptions, catalog, outbox |
| [Delivery engine (W2)](./modules/webhooks/delivery-engine.md) | HTTP dispatch, signing, logs |
| [Payload format](./modules/webhooks/payload-format.md) | Canonical envelope |
| [HMAC signing](./modules/webhooks/hmac-signing.md) | Subscriber verification |
| [Delivery history](./modules/webhooks/delivery-history.md) | Read API |
| [Observability](./modules/webhooks/observability.md) | Logging, health, correlation |
| [Admin UI (W4)](./modules/webhooks/admin-ui/README.md) | Web operations center |
| [Retry engine (W3)](./modules/webhooks/retry-engine.md) | Backoff, scheduler |
| [Dead letter queue](./modules/webhooks/dead-letter-queue.md) | DLQ storage |
| [Recovery operations](./modules/webhooks/recovery-operations.md) | Manual retry/replay |
| [API](./modules/webhooks/api.md) | `/api/v1/webhooks` endpoints |
| [Subscriptions](./modules/webhooks/subscriptions.md) | Domain model |
| [Permissions](./modules/webhooks/permissions.md) | `webhooks:read`, `webhooks:manage` |
| [Platform manifest](./modules/webhooks/platform-manifest.md) | Capability status, owners, dependencies |
| [Architecture](./modules/webhooks/architecture.md) | Registry, delivery, retry, audit layers |
| [Governance](./modules/webhooks/governance.md) | Phase gates W0–W5, PR checklist |
| [Event catalog](./modules/webhooks/event-catalog.md) | `domain.entity.action` naming, versioning |
| [Delivery model](./modules/webhooks/delivery-model.md) | Async outbox → dispatcher flow |
| [Retry strategy](./modules/webhooks/retry-strategy.md) | Backoff, DLQ, poison messages |
| [Security](./modules/webhooks/security.md) | HMAC, secrets, replay protection, tenant isolation |
| [Operations](./modules/webhooks/operations.md) | Monitoring, troubleshooting, DLQ recovery |
| [Extending](./modules/webhooks/extending.md) | How modules publish and integrators subscribe |
| [Roadmap](./modules/webhooks/roadmap.md) | W1–W5 implementation phases |

ADR: [ADR-Webhook-0001](./adr/ADR-Webhook-0001-webhook-platform-architecture.md), [ADR-Webhook-0003](./adr/ADR-Webhook-0003-webhook-delivery-engine.md), [ADR-Webhook-0004](./adr/ADR-Webhook-0004-webhook-retry-and-dlq.md)

---

## Frontend

| Document | Description |
|----------|-------------|
| [Frontend architecture](./frontend/architecture.md) | React 19 + Vite monorepo |
| [Routing and guards](./frontend/routing-and-guards.md) | Routes, AuthGuard, RoleGuard |
| [Auth and session](./frontend/auth.md) | Zustand, JWT, refresh |
| [API layer](./frontend/api-layer.md) | Axios, endpoints, interceptors |
| [Feature modules](./frontend/feature-modules.md) | auth, tenant, users, audit, dashboard |
| [Toasts](./frontend/toasts/README.md) | Global notification system |
| [API error UX](./frontend/errors/README.md) | Interceptor + classification |
| [Audit viewer](./frontend/audit-viewer/README.md) | Admin audit log UI |
| [Notification preferences](./frontend/notifications/README.md) | Email toggle |
| [Tenant settings](./frontend/tenant-settings/README.md) | Workspace settings UI |
| [Correlation support](./frontend/correlation-support/README.md) | X-Correlation-Id in UI |
| [Environment](./frontend/environment.md) | Vite env vars and proxy |

Legacy: [FRONTEND_STARTER.md](../FrontEnd/FRONTEND_STARTER.md) · [COREUI_INTEGRATION.md](../FrontEnd/COREUI_INTEGRATION.md)

---

## Mobile (Flutter)

| Document | Description |
|----------|-------------|
| [Mobile index](./mobile/index.md) | M0 foundation — start here |
| [Platform manifest](./mobile/mobile-platform-manifest.md) | Backend / Web / Mobile traceability |
| [Mobile architecture](./mobile/architecture.md) | `FrontEnd.Mobile/` target structure |
| [Mobile governance](./mobile/mobile-governance.md) | Docs-first rules for Flutter |
| [Module map](./mobile/module-map.md) | Feature ↔ API alignment |
| [State management](./mobile/state-management.md) | Riverpod ([ADR-Mobile-0001](./adr/ADR-Mobile-0001-state-management.md)) |
| [Navigation](./mobile/navigation.md) | GoRouter ([ADR-Mobile-0002](./adr/ADR-Mobile-0002-navigation.md)) |
| [API integration](./mobile/api-integration.md) | Shared REST/OAuth — no mobile APIs |
| [SDK generation](./mobile/sdk-generation.md) | OpenAPI → Dart SDK |
| [Security](./mobile/security.md) | JWT, MFA, tenant, sessions |
| [Offline strategy](./mobile/offline-strategy.md) | Online-first; future sync |
| [Push notifications](./mobile/push-notifications.md) | FCM/APNS abstraction |
| [Testing](./mobile/testing-strategy.md) | Unit, widget, integration |
| [Release process](./mobile/release-process.md) | Flavors, stores, CI |
| [Roadmap](./mobile/roadmap.md) | M0–M4 phases |
| [M1 foundation](./mobile/foundation/README.md) | Flutter platform implementation |
| [M3 modules](./mobile/modules/files/README.md) | Files, sessions, audit, settings |
| [M5 release](./mobile/release/README.md) | Flavors, signing, Fastlane, stores |
| [Coding standards](./mobile/coding-standards.md) | Dart conventions |

Platform ADR: [ADR-0012 Flutter](./adr/ADR-0012-flutter-mobile-platform.md)

**M5:** `FrontEnd.Mobile/` release engineering — flavors, Fastlane, store readiness, release CI.

---

## API reference

| Document | Description |
|----------|-------------|
| [API overview](./api/overview.md) | Versioning, auth, Scalar UI |
| [Auth API](./api/auth.md) | Register, token, SSO |
| [Tenant API](./api/tenant.md) | Provision, current tenant |
| [Users API](./api/users.md) | Profiles and tenant listing |
| [Audit API](./api/audit.md) | Audit log query (stub) |

Interactive: `http://localhost:5000/scalar/v1` (development, Kestrel default).

---

## Getting started

| Document | Description |
|----------|-------------|
| [Backend setup](./getting-started/backend-setup.md) | .NET SDK, build, run API |
| [Frontend setup](./getting-started/frontend-setup.md) | pnpm, Vite dev server |
| [Docker setup](./getting-started/docker-setup.md) | Compose services |
| [Local development](./getting-started/local-development.md) | Full stack workflow |
| [Debugging guide](./getting-started/debugging-guide.md) | Backend and frontend debugging |
| [Environment variables](./getting-started/environment-variables.md) | Complete env reference |

Extended tutorial: [DEVELOPER_GUIDE.md](../DEVELOPER_GUIDE.md) (rename, new module walkthrough).

---

## Extending the template

| Document | Description |
|----------|-------------|
| [Add backend module](./extending/add-backend-module.md) | Four-project module pattern |
| [Add contracts and handlers](./extending/add-contracts-and-handlers.md) | Cross-module events |
| [Add frontend route](./extending/add-frontend-route.md) | Router and guards |
| [Add observer module](./extending/add-observer-module.md) | Audit-style Layer 3 modules |

---

## Operations

| Document | Description |
|----------|-------------|
| [Logging](./operations/logging.md) | Serilog and log levels |
| [Seq usage](./operations/seq-usage.md) | Structured log viewer |
| [Redis troubleshooting](./operations/redis-troubleshooting.md) | Cache and health |
| [Outbox troubleshooting](./operations/outbox-troubleshooting.md) | Scaffold limitations |
| [Observability](./operations/observability.md) | OpenTelemetry and health probes |
| [Startup troubleshooting](./operations/startup-troubleshooting.md) | Common boot failures |
| [Deployment notes](./operations/deployment-notes.md) | Production compose |

Deep reference: [DOCKER_ENVIRONMENT.md](../BackEnd/DOCKER_ENVIRONMENT.md)

---

## Architecture decision records (ADR)

| ADR | Topic |
|-----|-------|
| [Template](./adr/ADR-0000-template.md) | ADR format |
| [ADR-0001](./adr/ADR-0001-modular-monolith.md) | Modular monolith |
| [ADR-0002](./adr/ADR-0002-outbox-pattern.md) | Outbox pattern |
| [ADR-0003](./adr/ADR-0003-observer-modules.md) | Observer modules (Audit) |
| [ADR-0004](./adr/ADR-0004-redis-caching.md) | Redis caching |
| [ADR-0005](./adr/ADR-0005-open-telemetry.md) | OpenTelemetry |
| [ADR-0006](./adr/ADR-0006-notifications-module.md) | Notifications module |
| [ADR-0007](./adr/ADR-0007-outbox-hosted-processors.md) | Outbox hosted processors |
| [ADR-0012](./adr/ADR-0012-flutter-mobile-platform.md) | Flutter mobile platform |
| [ADR-Mobile-0001](./adr/ADR-Mobile-0001-state-management.md) | Riverpod |
| [ADR-Mobile-0002](./adr/ADR-Mobile-0002-navigation.md) | GoRouter |
| [ADR-Mobile-0003](./adr/ADR-Mobile-0003-environment-configuration.md) | Mobile environments |
| [ADR-Mobile-0004](./adr/ADR-Mobile-0004-secure-token-storage.md) | Secure token storage |
| [ADR-Mobile-0005](./adr/ADR-Mobile-0005-openapi-sdk-generation.md) | OpenAPI SDK (Dart) |
| [ADR-Mobile-0006](./adr/ADR-Mobile-0006-push-notifications.md) | Push notifications |
| [ADR-Mobile-0007](./adr/ADR-Mobile-0007-offline-cache.md) | Offline cache |
| [ADR-Mobile-0008](./adr/ADR-Mobile-0008-crash-reporting.md) | Crash reporting |
| [ADR-Mobile-0009](./adr/ADR-Mobile-0009-analytics.md) | Analytics |
| [ADR-Mobile-0010](./adr/ADR-Mobile-0010-release-strategy.md) | Mobile release |
| [ADR-Mobile-0011](./adr/ADR-Mobile-0011-versioning-strategy.md) | Mobile versioning |
| [ADR-Mobile-0012](./adr/ADR-Mobile-0012-signing-strategy.md) | Mobile signing |
| [ADR-Webhook-0001](./adr/ADR-Webhook-0001-webhook-platform-architecture.md) | Webhook platform architecture |
| [ADR-Webhook-0002](./adr/ADR-Webhook-0002-webhook-secret-storage.md) | Secret storage (Data Protection) |

---

## Errors

| Document | Description |
|----------|-------------|
| [Error catalog](./errors/error-catalog.md) | API and auth error codes |
| [Problem details](./errors/problem-details.md) | RFC 7807 responses |
| [Common failures](./errors/common-failures.md) | Startup and infra |

---

## Documentation audit (May 2026)

| Report | Description |
|--------|-------------|
| [Gap analysis](./documentation-audit/documentation-gap-analysis.md) | Coverage matrix |
| [Missing docs](./documentation-audit/missing-docs-report.md) | What was absent |
| [Outdated docs](./documentation-audit/outdated-docs-report.md) | Legacy drift |
| [Standardization plan](./documentation-audit/documentation-standardization-plan.md) | Target structure |

---

## Governance

| Doc | Purpose |
|-----|---------|
| [Documentation governance](./documentation-governance.md) | Mandatory rules |
| [Developer workflow](./developer-workflow.md) | Code → docs → PR loop |
| [PR checklist](./documentation-pr-checklist.md) | Copy into pull requests |
| [Doc validation config](./doc-validation.json) | CI/local path mappings |

**Automation:** `scripts/validate-docs.ps1` · GitHub Actions `docs-validation.yml` (warn by default)
