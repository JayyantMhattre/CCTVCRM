# ADR-Webhook-0001: Webhook platform architecture

## Status

Accepted (W0)

## Date

2026-05-31

## Deciders

Platform team

---

## Context

Ashraak must expose **integration-friendly outbound events** for CRM, LMS, HRMS, ERP, AI agents, and future products. Requirements:

- Reusable across products (not CRM-specific)
- Tenant-isolated subscriptions
- Reliable delivery without blocking business transactions
- Auditable and operable at enterprise scale

Internal eventing today: MediatR + outbox scaffold ([ADR-0002](./ADR-0002-outbox-pattern.md), [eventing.md](../architecture/eventing.md)).

---

## Decision

1. Webhooks are a **platform capability**, not a vertical business module.
2. Delivery is **asynchronous** only, driven by **outbox** after database commit.
3. Event names follow **`domain.entity.action`** with explicit **`vN`** schema versioning.
4. Subscriptions are **tenant-scoped** with per-endpoint **HMAC secrets**.
5. Failed deliveries use **exponential backoff** and terminal **dead letter queue**.
6. **W0 is documentation-only**; implementation follows [roadmap.md](../modules/webhooks/roadmap.md).

---

## Rationale

### Why asynchronous delivery

External HTTP latency and availability must not affect API response times or transaction boundaries. Outbox ensures we never acknowledge subscriber success before our own data is committed.

### Why outbox integration

Reuses existing platform direction (ADR-0002, outbox processors) instead of inventing a parallel message bus in W1. At-least-once semantics match webhook industry norms.

### Why tenant isolation

Multi-tenant SaaS requires subscription secrets, endpoints, and delivery logs partitioned by `tenantId`. Cross-tenant leakage is a critical security failure.

### Why platform capability vs CRM module

CRM/LMS/ERP are consumers. Coupling webhook design to one product forces redesign for the next product. Generic registry + catalog supports all.

---

## Alternatives considered

| Alternative | Rejected because |
|-------------|------------------|
| **Synchronous HTTP in command handlers** | Blocks requests; no safe retry; couples availability |
| **RabbitMQ-only (no outbox)** | Duplicates reliability story; outbox already platform standard |
| **Per-product webhook modules** | Not reusable; manifest and ops fragmentation |
| **Polling API for integrators** | Higher load; worse UX for real-time integrations |
| **Single global webhook secret** | Violates tenant isolation and rotation per customer |

---

## Consequences

### Positive

- Clear extension path for all modules via event catalog
- Aligns with observability (correlation IDs) and audit patterns
- Vendor-neutral subscriber contract (HTTPS + HMAC)

### Negative / tradeoffs

- At-least-once requires subscriber idempotency
- Outbox + worker adds operational surface before first delivery
- Schema versioning burden (`v1` vs `v2`)

---

## References

- [modules/webhooks/README.md](../modules/webhooks/README.md)
- [architecture.md](../modules/webhooks/architecture.md)
- [delivery-model.md](../modules/webhooks/delivery-model.md)
- [security.md](../modules/webhooks/security.md)
- [platform-manifest.md](../modules/webhooks/platform-manifest.md)
