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

## Project — Aarvii CCTV AMC Management System (D0)

Business-module project built on the frozen platform. Source of truth: [requirements-freeze-v1.md](./project/requirements-freeze-v1.md).

| Document | Description |
|----------|-------------|
| [Requirements Freeze V1](./project/requirements-freeze-v1.md) | **Authoritative frozen requirements** |
| [Product vision](./project/product-vision-document.md) | Vision, objectives, success criteria |
| [Scope freeze](./project/scope-freeze-v1.md) | In/out of scope, approved modules & actors |
| [Actors & personas](./project/actors-and-personas.md) | Public Visitor, Customer, Engineer, Admin |
| [BRD](./project/business-requirements-document.md) | Functional & non-functional requirements |
| [Business rules](./project/business-rules.md) | 46 approved rules by domain |
| [Workflows](./project/workflow-overview.md) | Lead, AMC renewal, tickets, visits, invoices (Mermaid) |
| [High-level design](./project/high-level-design.md) | Context, architecture, stack, security |
| [Application architecture](./project/application-architecture.md) | Website + 3 portals + backend + integrations |
| [Module architecture](./project/module-architecture.md) | 15 approved modules |
| [Mobile architecture](./project/mobile-architecture.md) | Customer & Engineer Flutter apps |
| [Navigation map](./project/navigation-map.md) | Navigation trees per application |
| [Screen inventory](./project/screen-inventory.md) | All V1 screens by module |
| [Project roadmap](./project/project-roadmap.md) | Phases D0–D7 |
| [Platform discovery report](./project-bootstrap/platform-discovery-report.md) | Platform capabilities & reuse assessment |

PDF versions: [docs/project/pdf/](./project/pdf/)

### D0-4 — Entity Model & Database Design (`docs/project/design/`)

| Document | Description |
|----------|-------------|
| [Entity model](./project/design/entity-model.md) | All 32 entities, aggregates, ownership, lifecycle ownership |
| [Database architecture](./project/design/database-architecture.md) | Schema-per-module strategy, transactions, audit & files strategy |
| [Naming standards](./project/design/database-naming-standards.md) | Tables, keys, indexes, audit fields, soft delete |
| [Lifecycle matrix](./project/design/entity-lifecycle-matrix.md) | CRUD/archive/approval/status transitions per entity |
| [ERD overview](./project/design/erd-overview.md) | Complete system ERD (Mermaid) |
| Domain ERDs | [Lead](./project/design/erd-lead-domain.md) · [Customer](./project/design/erd-customer-domain.md) · [AMC](./project/design/erd-amc-domain.md) · [Service](./project/design/erd-service-domain.md) · [Ticket](./project/design/erd-ticket-domain.md) · [Invoice](./project/design/erd-invoice-domain.md) |
| [Future considerations](./project/design/database-future-considerations.md) | Extensibility headroom without current-design changes |

### D0-5 — RBAC, Navigation & Screen Inventory (`docs/project/design/`)

| Document | Description |
|----------|-------------|
| [Platform reuse analysis](./project/design/platform-reuse-analysis.md) | **Key doc** — EXISTS/EXTEND/NEW per requirement + effort |
| [RBAC matrix](./project/design/rbac-matrix.md) | Role × module/screen/action/API/file/audit/notification |
| [Permission catalog](./project/design/permission-catalog.md) | 9 REUSE platform permissions + 30 NEW CCTV permissions |
| [Navigation architecture](./project/design/navigation-architecture.md) | Shell reuse + 4 app navigation trees classified |
| [Screen inventory (design)](./project/design/screen-inventory.md) | 71 screens with role, class, dependencies, mobile |
| [Dashboard design](./project/design/dashboard-design.md) | Admin/Customer/Engineer dashboards, widget classification |
| Portal navigation | [Admin](./project/design/admin-portal-navigation.md) · [Customer](./project/design/customer-portal-navigation.md) · [Engineer](./project/design/engineer-portal-navigation.md) |
| [Mobile screen inventory](./project/design/mobile-screen-inventory.md) | 34 mobile screens; foundation reuse classified |
| [User journeys](./project/design/user-journeys.md) | 6 end-to-end journey diagrams (Mermaid) |

### D0-6 — API Architecture & Module Contracts (`docs/project/design/`)

| Document | Description |
|----------|-------------|
| [API reuse analysis](./project/design/api-reuse-analysis.md) | **Key doc** — REUSE/EXTEND/NEW per API capability + effort |
| [API architecture](./project/design/api-architecture.md) | Versioning, naming, pagination, errors, files/notifications/audit integration |
| [Module contracts](./project/design/module-contracts.md) | Cross-module interfaces, events, dependencies for all 11 modules |
| [Endpoint catalog](./project/design/endpoint-catalog.md) | ~115 CCTV routes + platform REUSE inventory |
| [DTO catalog](./project/design/dto-catalog.md) | Request/response/summary/detail/mobile DTO inventory |
| [Integration design](./project/design/integration-design.md) | Website, portals, mobile, modules, platform services |
| [Event catalog](./project/design/event-catalog.md) | Domain events + webhook names |
| [Notification mapping](./project/design/notification-mapping.md) | Events → email/SMS/push |
| [File management design](./project/design/file-management-design.md) | Platform Files reuse, categories, retention |
| [Audit mapping](./project/design/audit-mapping.md) | Platform Audit reuse, auditable actions |
| [Mobile API consumption](./project/design/mobile-api-consumption.md) | Customer/Engineer apps, offline/sync boundaries |
| [OpenAPI roadmap](./project/design/openapi-roadmap.md) | SDK generation strategy for web + Flutter |

### D0-7 — Low Level Design (`docs/project/design/lld/`)

| Document | Description |
|----------|-------------|
| [Platform component reuse](./project/design/lld/platform-component-reuse.md) | **Key doc** — REUSE/EXTEND/NEW per UI component |
| [Screen design specification](./project/design/lld/screen-design-specification.md) | All 71 screens: purpose, nav, actions, class |
| [Form catalog](./project/design/lld/form-catalog.md) | Fields, types, validations, permissions |
| [Grid catalog](./project/design/lld/grid-catalog.md) | Columns, sort, filter, search, export |
| [Validation rules](./project/design/lld/validation-rules.md) | Client + server rules by domain |
| [Workflow screen design](./project/design/lld/workflow-screen-design.md) | 6 workflows screen-by-screen |
| [Dashboard specification](./project/design/lld/dashboard-specification.md) | Widget data sources, refresh, permissions |
| [PDF document design](./project/design/lld/pdf-document-design.md) | Contract, visit report, invoice PDF layouts |
| [Report specification](./project/design/lld/report-specification.md) | Admin reports: columns, filters, export |
| [Mobile screen design](./project/design/lld/mobile-screen-design.md) | Customer + Engineer mobile UX |
| [Notification UX design](./project/design/lld/notification-ux-design.md) | Toasts, email/SMS copy, push |
| [File upload design](./project/design/lld/file-upload-design.md) | Categories, limits, two-step pattern |
| [Audit UX design](./project/design/lld/audit-ux-design.md) | Audit viewer reuse + business timelines |
| [Theme usage design](./project/design/lld/theme-usage-design.md) | platform-ui rules, responsive, no theme imports |

### D0-8 — Implementation Roadmap & Delivery Plan (`docs/project/roadmap/`)

| Document | Description |
|----------|-------------|
| [Implementation roadmap](./project/roadmap/implementation-roadmap.md) | **Master roadmap** — phases, dependencies, milestones, critical path, complexity |
| [Platform reuse roadmap](./project/roadmap/platform-reuse-roadmap.md) | EXISTS / REUSE / EXTEND / NEW per requirement |
| [Phase execution playbook](./project/roadmap/phase-execution-playbook.md) | **Gate checklist** — Review Gate 1 (D1-1..D1-7) & Review Gate 2 (full tests) |
| [Backend development phases](./project/roadmap/backend-development-phases.md) | B1–B7 sequencing: entities, APIs, events, audit |
| [Frontend development phases](./project/roadmap/frontend-development-phases.md) | Admin, Customer, Engineer portals — reuse/extend/new |
| [Mobile development phases](./project/roadmap/mobile-development-phases.md) | Customer + Engineer Flutter — screen classification |
| [Database implementation plan](./project/roadmap/database-implementation-plan.md) | Schema rollout, migrations, seed & rollback |
| [Integration roadmap](./project/roadmap/integration-roadmap.md) | Notifications, files, audit, webhooks, PDF, SMS/email |
| [Testing roadmap](./project/roadmap/testing-roadmap.md) | Unit → UAT → regression strategy |
| [Release plan](./project/roadmap/release-plan.md) | Dev/QA/UAT/Prod, versioning, checkpoints, rollback |
| [Sprint plan](./project/roadmap/sprint-plan.md) | 11-sprint sequence (Sprint 0–11) with gates |
| [Risk register](./project/roadmap/risk-register.md) | Business, technical, mobile, infra risks + mitigations |
| [Definition of done](./project/roadmap/definition-of-done.md) | DoD for backend, frontend, mobile, docs, testing, release |

### D1-0 — Architecture Validation & Readiness Review (`docs/project/review/`)

| Document | Description |
|----------|-------------|
| [Architecture validation report](./project/review/architecture-validation-report.md) | Cross-doc consistency: requirements, HLD, ERD, RBAC, API, LLD, roadmap |
| [Gap analysis report](./project/review/gap-analysis-report.md) | Missing items classified Critical/High/Medium/Low |
| [Platform reuse validation](./project/review/platform-reuse-validation.md) | **Key doc** — REUSE/EXTEND/NEW per requirement; duplicate check |
| [Dependency validation](./project/review/dependency-validation.md) | Module, API, DB, frontend, mobile, deployment dependencies |
| [Security readiness review](./project/review/security-readiness-review.md) | Auth, RBAC, role separation, files, audit, OTP, sessions |
| [Data model review](./project/review/data-model-review.md) | Entities, relationships, lifecycle, normalization |
| [Implementation readiness scorecard](./project/review/implementation-readiness-scorecard.md) | Percentage readiness by dimension (~91% overall) |
| [Phase readiness report](./project/review/phase-readiness-report.md) | B1–B7 executable without ambiguity |
| [Risk reassessment](./project/review/risk-reassessment.md) | D0-8 risks reviewed post-validation |
| [Final implementation recommendation](./project/review/final-implementation-recommendation.md) | **GO WITH CONDITIONS** — justification and mandatory conditions |
| [Architecture decision confirmation](./project/review/architecture-decision-confirmation.md) | 30 confirmed ADs, 6 needs-review, 10 future enhancements |

### Sprint 0 — Foundation bootstrap (D1)

| Document | Description |
|----------|-------------|
| [Sprint 0 completion report](./project/sprint-0-completion-report.md) | **Sprint 0 deliverables** — **APPROVED ✅** |
| [Sprint 1 / B1 completion report](./project/sprint-1-b1-completion-report.md) | **Lead Management (B1)** — **COMPLETE ✅** |
| [Sprint 1 / D1-2 completion report](./project/sprint-1-d1-2-completion-report.md) | **Customer Management (D1-2)** — **COMPLETE ✅** |
| [Sprint 1 / D1-3 completion report](./project/sprint-1-d1-3-completion-report.md) | **Site Management (D1-3)** — **COMPLETE ✅** |
| [Sprint 1 / D1-4 completion report](./project/sprint-1-d1-4-completion-report.md) | **AMC Management (D1-4)** — **COMPLETE ✅** |
| [Sprint 1 / D1-5 completion report](./project/sprint-1-d1-5-completion-report.md) | **Scheduling & Visits (D1-5)** — **COMPLETE ✅** |
| [Sprint 1 / D1-6 completion report](./project/sprint-1-d1-6-completion-report.md) | **Ticket Management (D1-6)** — **COMPLETE ✅** |
| [Sprint 1 / D1-7 completion report](./project/sprint-1-d1-7-completion-report.md) | **Invoice Management (D1-7)** — **COMPLETE ✅** |
| [Review Gate 2 completion report](./project/review-gate-2-completion-report.md) | **Full test execution (D1-1..D1-7)** — **PASSED ✅** |
| [Sprint 1 / D1-8 completion report](./project/sprint-1-d1-8-completion-report.md) | **Engineer Management (D1-8)** — **COMPLETE ✅** |
| [D1-9 completion report](./project/d1-9-engineer-portal-completion-report.md) | **Engineer Portal (D1-9)** — **COMPLETE ✅** |
| [D1-10 completion report](./project/d1-10-mobile-completion-report.md) | **Mobile Applications (D1-10)** — **COMPLETE ✅** |
| [D1-11 completion report](./project/d1-11-reporting-completion-report.md) | **Reporting (D1-11)** — **COMPLETE ✅** |
| [D1-12 completion report](./project/d1-12-hardening-completion-report.md) | **Hardening & Release Readiness (D1-12)** — **COMPLETE ✅** |
| [D1-13a completion report](./project/d1-13a-public-website-completion-report.md) | **Public Website (D1-13 Wave 1)** — **COMPLETE ✅** |
| [D1-13b completion report](./project/d1-13b-customer-portal-completion-report.md) | **Customer Portal Web (D1-13 Wave 1)** — **COMPLETE ✅** |
| [D1-13c completion report](./project/d1-13c-notifications-completion-report.md) | **CCTV Notifications (D1-13 Wave 2)** — **COMPLETE ✅** |
| [D1-13d completion report](./project/d1-13d-pdf-generation-completion-report.md) | **Production PDF (D1-13 Wave 2)** — **COMPLETE ✅** |
| [Release candidate review](./project/review/release-candidate-review.md) | **RC review** — Conditional GO, await architectural sign-off |
| [Project completeness review](./project/review/project-completeness-review.md) | **V1 traceability** — post D1-13 Wave 2; ~65% strict / ~80% weighted |
| [CCTV module map](./project/cctv-module-map.md) | **Authoritative CCTV module inventory** — deps, routes, flags, phases |
| [CCTV module naming freeze](./project/cctv-module-naming-freeze.md) | **Frozen naming** — projects, schemas, APIs, docs |
| ADR-CCTV-0001 | [SMS provider strategy](./project/adr/ADR-CCTV-0001-sms-provider-strategy.md) |
| ADR-CCTV-0002 | [PDF generation strategy](./project/adr/ADR-CCTV-0002-pdf-generation-strategy.md) |
| ADR-CCTV-0003 | [Module naming freeze](./project/adr/ADR-CCTV-0003-module-naming-freeze.md) |
| Module docs | [cctv-lead](./modules/cctv-lead/README.md) · [cctv-customer](./modules/cctv-customer/README.md) · [cctv-amc](./modules/cctv-amc/README.md) · [cctv-service](./modules/cctv-service/README.md) · [cctv-ticket](./modules/cctv-ticket/README.md) · [cctv-engineer](./modules/cctv-engineer/README.md) · [cctv-invoice](./modules/cctv-invoice/README.md) · [cctv-reporting](./modules/cctv-reporting/README.md) · [cctv-integration](./modules/cctv-integration/README.md) |

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
