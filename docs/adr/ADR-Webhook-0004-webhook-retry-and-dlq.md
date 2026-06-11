# ADR-Webhook-0004: Retry engine and dead letter queue

**Status:** Accepted  
**Date:** 2026-05-31  
**Phase:** W3

---

## Context

W2 delivered first-attempt HTTP webhooks with terminal `Failed` status. Enterprise integrators require survival of transient outages, rate limits, and network instability without losing events or audit trail.

---

## Decision

### 1. Database-driven retry scheduling

Failed transient deliveries transition to `Retrying` with `NextRetryOnUtc`. `WebhookRetryHostedService` polls due rows and re-enqueues to the existing delivery worker.

Rationale: survives process restarts; no in-memory-only retry state.

### 2. Fixed backoff schedule (configurable)

Default delays: 1m, 5m, 15m, 60m for attempts 2–5. `MaxRetries = 5` total attempts.

Rationale: predictable operations; matches W3 spec; simpler than jitter for initial release.

### 3. Failure classifier

`WebhookFailureClassifier` maps HTTP codes and transport errors to Transient vs Permanent. Permanent failures skip retry.

Rationale: avoids retry storms on misconfiguration (401/404).

### 4. Dead letter table

Exhausted deliveries create `webhook_dead_letters` row; original delivery marked `DeadLettered`.

Rationale: operational visibility; replay without mutating history.

### 5. Replay creates new delivery

`POST /deadletters/{id}/replay` creates a new `WebhookDelivery` preserving `correlationId`.

Rationale: immutable audit of original failure path; clear lineage.

### 6. Same delivery queue

Retries reuse `WebhookDeliveryQueue` and `WebhookDeliveryHostedService` from W2.

Rationale: minimal new infrastructure; shared concurrency limits.

---

## Consequences

**Positive**

- Enterprise reliability without external message broker
- Full audit lifecycle for retry and DLQ events
- Manual recovery APIs for operators

**Negative**

- Poll-based scheduler adds up to `PollIntervalSeconds` latency before retry
- DLQ retention purge not automated in W3

---

## References

- [retry-engine.md](../modules/webhooks/retry-engine.md)
- [dead-letter-queue.md](../modules/webhooks/dead-letter-queue.md)
- [ADR-Webhook-0003](./ADR-Webhook-0003-webhook-delivery-engine.md)
