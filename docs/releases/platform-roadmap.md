# Platform Roadmap

Tracks what shipped in V1, what was intentionally deferred, and candidate future work. This is a **planning reference**, not a commitment — items are added/reprioritized through the [ADR process](../adr/ADR-0000-template.md).

**Related:** [platform-capabilities.md](./platform-capabilities.md) · [known-limitations.md](./known-limitations.md) · [upgrade-guide.md](./upgrade-guide.md)

---

## 1. Completed in V1

### Backend / Platform
- ✅ Modular monolith with contract-only inter-module communication ([ADR-0001](../adr/ADR-0001-modular-monolith.md))
- ✅ Auth: registration, login, JWT/OAuth2 (OpenIddict), RBAC/ABAC, MFA/TOTP, invitations, sessions
- ✅ Tenant: provisioning, settings, isolation
- ✅ Users: profiles + preferences
- ✅ Audit observer module → MongoDB ([ADR-0003](../adr/ADR-0003-observer-modules.md))
- ✅ Notifications: event-driven email, providers/templates ([ADR-0006](../adr/ADR-0006-notifications-module.md))
- ✅ Files: tenant-scoped storage (local/S3/Azure) ([ADR-0011](../adr/ADR-0011-files-storage-module.md))
- ✅ Caching: Redis + memory, sessions, distributed locks ([ADR-0004](../adr/ADR-0004-redis-caching.md))
- ✅ Outbox + hosted processors ([ADR-0002](../adr/ADR-0002-outbox-pattern.md), [ADR-0007](../adr/ADR-0007-outbox-hosted-processors.md))
- ✅ Webhooks platform: subscriptions, delivery engine, retries, DLQ, secret storage ([Webhook-0001..0004](../adr/ADR-Webhook-0001-webhook-platform-architecture.md))
- ✅ API Keys platform: issuance, scopes, hashing, rotation, revocation, auth middleware ([ADR-ApiKeys-0001](../adr/ADR-ApiKeys-0001-api-keys-platform.md))
- ✅ Observability: Serilog+Seq, OpenTelemetry, correlation, health, rate limits ([ADR-0005](../adr/ADR-0005-open-telemetry.md), [ADR-0008](../adr/ADR-0008-host-platform-hardening.md))

### Frontend (Web)
- ✅ React 19 SPA: routing, guards, lazy modules
- ✅ **Theme Engine** — Theme Adapter Architecture with 11 contracts
- ✅ CoreUI adapter (production default) + HexaDash adapter (validated, opt-in)
- ✅ Admin UIs: Webhooks operations center, API Keys, Audit viewer, Users, Tenant settings, Sessions, Notification preferences

### Mobile
- ✅ Flutter foundation ([ADR-0012](../adr/ADR-0012-flutter-mobile-platform.md)) with Riverpod + go_router
- ✅ Feature parity for auth, tenant, users, profile, sessions, audit, API keys, webhooks, files, notifications, settings
- ✅ Secure storage, offline cache, push, crash reporting, analytics, OpenAPI SDK, deep links

### Engineering & Governance
- ✅ Central package management + build governance ([ADR-0010](../adr/ADR-0010-build-package-governance.md))
- ✅ CI: backend/frontend, docs validation, mobile build + store releases
- ✅ Documentation governance + ADR catalog (30 ADRs incl. template)
- ✅ Theme governance: onboarding, lifecycle, selection checklist, adapter dev guide, reusable prompts

---

## 2. Deferred (intentionally out of V1)

These were consciously scoped out; the architecture supports them as additive work.

| Item | Why deferred | Tracking |
|------|--------------|----------|
| **Module migration to `platform-ui`** | Web module interiors still use raw CoreUI markup; full theme reskin needs a migration pass | [HexaDash validation P1-1](../frontend/themes/current-theme/hexadash-validation-report.md) |
| **Charting backend** | `ChartContract` is library-agnostic; no chart lib committed yet | [ChartContract](../frontend/themes/theme-adapter-development-guide.md#11-chartcontract) |
| **Adapter-routed toasts** | Web mounts `ToastContainer` directly; not via theme adapter | [validation P2-1](../frontend/themes/current-theme/hexadash-validation-report.md) |
| **Per-theme code-splitting** | Both theme adapters ship in the bundle | [validation P2-4](../frontend/themes/current-theme/hexadash-validation-report.md) |
| **Promote HexaDash to default** | CoreUI remains default until parity accepted | [readiness](../frontend/themes/current-theme/production-readiness-report.md) |
| **Full SSO providers GA** | Foundations present; provider hardening deferred | — |

---

## 3. Future (candidate platform features)

> Optional, not committed. Each requires its own ADR before implementation.

### Optional future platform features
- Additional notification channels (SMS/push-to-web, in-app inbox)
- Background jobs / scheduling abstraction beyond outbox processors
- Multi-region / read-replica data strategy
- Fine-grained audit retention/archival policies
- API gateway / public API versioning surface
- Additional storage providers and signed-URL policies
- Theme marketplace: more adapters onboarded via the [onboarding guide](../frontend/themes/theme-onboarding-guide.md)

### Business modules (product-specific, build on the platform)
These are **examples** a downstream product would add using the [module extension guides](../extending/add-backend-module.md):
- Billing / subscriptions / plans
- Reporting & dashboards (with a charting backend)
- Org/team hierarchy beyond tenant
- Workflow/approvals
- Customer-facing portal modules

### Experimental modules (incubation)
- AI-assisted features (search, summarization) behind feature flags
- Real-time collaboration (WebSocket/SignalR surface)
- Event streaming / external bus integration (Kafka/RabbitMQ) layered on the outbox

---

## 4. Principles for roadmap execution

1. **Additive, not disruptive** — new capabilities arrive as modules/adapters without rewriting existing ones.
2. **ADR-first** — significant changes get an ADR before code.
3. **Docs with code** — every roadmap item ships with documentation ([governance](../documentation-governance.md)).
4. **One major version at a time** for dependency/framework upgrades ([upgrade-guide.md](./upgrade-guide.md)).
