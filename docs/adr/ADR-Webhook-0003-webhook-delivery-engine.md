# ADR-Webhook-0003: Webhook delivery engine

**Status:** Accepted  
**Date:** 2026-05-31  
**Phase:** W2

---

## Context

W1 established webhook subscriptions, event catalog, and outbox publishing (`WebhookRequestedEvent`). W2 must deliver events to subscriber HTTPS endpoints asynchronously, log outcomes, and remain ready for W3 retries — without performing HTTP inside business transactions.

---

## Decision

### 1. Two-stage async dispatch

1. **Outbox handler** (`WebhookRequestedEventHandler`) → `WebhookDispatcher` creates `WebhookDelivery` rows and enqueues work.
2. **Background worker** (`WebhookDeliveryHostedService`) → `WebhookDeliveryService` performs HTTP POST.

Rationale: separates outbox processing latency from HTTP subscriber latency; enables independent concurrency control.

### 2. Canonical JSON envelope

All events use one envelope shape (`eventId`, `eventName`, `version`, `occurredOnUtc`, `tenantId`, `correlationId`, `data`). Module payloads live in `data` only.

Rationale: integrators implement one parser; versioning is explicit.

### 3. HMAC-SHA256 over raw body

Header `X-Webhook-Signature` = lowercase hex HMAC-SHA256 of the exact POST body bytes, keyed by subscription secret.

Rationale: simple verification; matches industry practice (Stripe/GitHub-style body signing). Timestamp prefix deferred to avoid W2 scope creep.

### 4. Persisted delivery log

Table `webhook_deliveries` stores request payload, response code/body (truncated), status, timing, and correlation. Status enum: `Pending`, `Succeeded`, `Failed` only.

Rationale: operational visibility and audit correlation without DLQ complexity.

### 5. Single attempt (W2)

`attempt_number` is always `1`. Failures are logged; no automatic retry.

Rationale: retry policy, backoff, and DLQ are W3 concerns.

### 6. In-process channel queue

`WebhookDeliveryQueue` (bounded channel) bridges dispatcher and hosted service within the modular monolith.

Rationale: no external broker required for W2; upgrade path to distributed queue remains open.

---

## Consequences

**Positive**

- Clear separation: publish → dispatch → deliver → audit
- Tenant-safe filtering at subscription and delivery layers
- Read-only delivery history API for support
- Domain events integrate with existing `DomainEventAuditHandler`

**Negative**

- In-process queue is lost on process crash before HTTP completes (mitigated by W3 retries reading `Failed` / pending reconciliation)
- No timestamp in HMAC — subscribers must implement own replay deduplication

---

## Alternatives considered

| Alternative | Rejected because |
|-------------|------------------|
| HTTP in outbox handler | Blocks outbox processor; couples DB transaction to network |
| Per-event payload shapes | Higher integrator cost; conflicts with catalog standards |
| Sign `{timestamp}.{body}` in W2 | Adds clock-skew contract before subscriber SDK exists |
| Immediate retry in W2 | Belongs to W3 retry engine with backoff/DLQ |

---

## References

- [delivery-engine.md](../modules/webhooks/delivery-engine.md)
- [payload-format.md](../modules/webhooks/payload-format.md)
- [hmac-signing.md](../modules/webhooks/hmac-signing.md)
- [ADR-Webhook-0001](./ADR-Webhook-0001-webhook-platform-architecture.md)
