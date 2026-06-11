# Platform Status — V1

**Status:** Official · **Baseline:** `v1.0.0` (Platform Freeze) · **Phase:** R1

The authoritative status snapshot of the platform at the freeze. Capabilities are grouped by their governance state.

**Related:** [platform-freeze-policy.md](./platform-freeze-policy.md) · [module-classification-policy.md](./module-classification-policy.md) · [V1 capabilities](../releases/platform-capabilities.md) · [V1 roadmap](../releases/platform-roadmap.md) · [known limitations](../releases/known-limitations.md)

---

## 1. Completed capabilities (shipped in V1)

| Capability | Class | Location |
|------------|-------|----------|
| Authentication (JWT/OAuth2, OpenIddict) | Core | `Modules/Auth` |
| Roles & Permissions (RBAC/ABAC) | Core | `Modules/Auth` |
| MFA / TOTP | Core | `Modules/Auth` |
| Sessions | Core | `Modules/Auth` |
| Invitations | Core | `Modules/Auth` |
| Tenants (isolation, settings) | Core | `Modules/Tenant` |
| Users (profiles, preferences) | Core | `Modules/Users` |
| Audit (observer → MongoDB) | Core | `Modules/Audit` |
| Notifications (email providers/templates) | Core | `Modules/Notifications` |
| Files (local/S3/Azure) | Core | `Modules/Files` |
| Caching (Redis, locks, sessions) | Core | `Modules/Caching` |
| Outbox + hosted processors | Core | `BuildingBlocks` + Host |
| Webhooks (delivery, retries, DLQ) | Core | `Modules/Webhooks` |
| API Keys (scopes, rotation, auth) | Core | `Modules/ApiKeys` |
| Feature Flags | Core | Host |
| Rate limiting, correlation, health | Core | Host |
| Observability (Serilog+Seq, OTel) | Core | Host |
| Theme Engine (11 contracts, platform-ui) | Core | `apps/web/src/theme` + `platform-ui` |
| CoreUI theme (default) | Core | `theme/adapters/coreui` |
| HexaDash theme (opt-in) | Core (additive) | `theme/adapters/hexadash` |
| Web SPA (React 19) + admin UIs | Core | `apps/web` |
| Mobile platform (Flutter) | Core | `FrontEnd.Mobile` |
| Documentation Governance | Core | `docs/` + CI |
| Release Engineering (CI + mobile releases) | Core | `.github/workflows` |

## 2. Frozen capabilities

**All Core Platform capabilities listed in §1 are frozen at `v1.0.0`** per the [Platform Freeze Policy](./platform-freeze-policy.md).

- **Frozen = scope closed.** No new product/domain features added to Core.
- **Still maintained:** bug fixes, security fixes, dependency upgrades, documentation, and **new theme adapters** (additive) are allowed.
- **Changes gated by:** documented justification + ADR + governance review + multi-project reuse justification ([extension policy](./platform-extension-policy.md)).

## 3. Deferred capabilities (consciously out of V1)

| Item | Reason | Reference |
|------|--------|-----------|
| Web module migration to `platform-ui` | Module interiors still use raw CoreUI markup; full reskin needs a migration pass | [known-limitations C2/D1](../releases/known-limitations.md) |
| Charting backend | `ChartContract` is library-agnostic; no chart lib committed | [roadmap](../releases/platform-roadmap.md) |
| Adapter-routed toasts | Web mounts `ToastContainer` directly | [validation P2-1](../frontend/themes/current-theme/hexadash-validation-report.md) |
| Per-theme code-splitting | Both adapters ship in the bundle | [validation P2-4](../frontend/themes/current-theme/hexadash-validation-report.md) |
| Promote HexaDash to default | CoreUI remains default until parity accepted | [readiness](../frontend/themes/current-theme/production-readiness-report.md) |
| SSO providers GA | Foundations present; hardening deferred | [capabilities](../releases/platform-capabilities.md) |

## 4. Optional future platform features

Candidates only — each requires the full [extension policy](./platform-extension-policy.md) gate + ADR before any Core entry:

- Additional notification channels (SMS, in-app inbox)
- Background jobs/scheduling beyond outbox processors
- Multi-region / read-replica strategy
- Audit retention/archival policies
- Public API gateway / API versioning surface
- Additional storage providers / signed-URL policies
- More theme adapters via the [onboarding guide](../frontend/themes/theme-onboarding-guide.md)

## 5. Business module roadmap

Product value after V1 is delivered as **business modules** ([business-module-policy.md](./business-module-policy.md)) — optional, decoupled, replaceable, independently deployable. Candidate modules (illustrative, not committed):

| Module | Class | Notes |
|--------|-------|-------|
| CRM | Business | Consumes Auth/Tenants/Notifications/Audit |
| Lead Management | Business | Often paired with CRM |
| Billing / Subscriptions | Business | Consumes Tenants/Users/Webhooks |
| LMS | Business | Consumes Files/Users/Notifications |
| HRMS | Business | Consumes Users/Files/Audit |
| ERP | Business | Large composite; multiple modules |
| Project Management | Business | Consumes Users/Notifications |
| Knowledge Base | Business | Consumes Files/Search |
| Helpdesk / Ticketing | Business | Consumes Notifications/Webhooks |
| AI Solutions | Experimental → Business | Start behind feature flags ([classification](./module-classification-policy.md)) |

Experimental incubations: AI Agents, Workflow Engine, RAG, Automation Framework, Future Integrations.

---

## Platform status summary

| State | Count / Note |
|-------|--------------|
| Completed Core capabilities | See §1 (full V1 surface) |
| Frozen | All Core capabilities at `v1.0.0` |
| Deferred | 6 documented items (§3) |
| Optional future platform features | Candidates only, gated (§4) |
| Business modules in V1 | 0 (platform is the foundation) |

**Core Platform Frozen. Ready for `v1.0.0` Tag. Ready for Business Module Development.**
