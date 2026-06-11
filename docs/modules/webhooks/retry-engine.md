# Webhook retry engine (W3)

**Status:** Implemented

Transforms webhook delivery from best-effort (W2) into enterprise-reliable delivery with exponential backoff retries.

---

## Retry schedule (default)

| Attempt | Delay before attempt |
|---------|---------------------|
| 1 | Immediate |
| 2 | 1 minute |
| 3 | 5 minutes |
| 4 | 15 minutes |
| 5 | 1 hour |

Configurable via `WebhookRetry:RetryDelaysMinutes` (array index = delay after attempt N fails).

---

## Components

| Component | Responsibility |
|-----------|----------------|
| `WebhookFailureClassifier` | Classifies failures as Transient, Permanent, or Unknown |
| `WebhookRetryBackoffCalculator` | Computes delay before next attempt |
| `WebhookDeliveryOutcomeHandler` | Decides retry vs terminal fail vs DLQ |
| `WebhookRetryHostedService` | Polls `Retrying` deliveries when `NextRetryOnUtc` is due |

---

## Delivery states

| Status | Meaning |
|--------|---------|
| `Pending` | Queued or executing |
| `Succeeded` | HTTP 2xx |
| `Failed` | Permanent failure (no more retries) |
| `Retrying` | Transient failure; waiting for `NextRetryOnUtc` |
| `DeadLettered` | Max retries exhausted; record in DLQ |

---

## Flow

```
HTTP failure → Classify
  → Permanent → Failed (terminal)
  → Transient + attempts remain → Retrying (NextRetryOnUtc set)
  → Transient + exhausted → DeadLettered + webhook_dead_letters row

WebhookRetryHostedService (poll)
  → Retrying + due → Pending (AttemptNumber++) → delivery queue → HTTP
```

---

## Configuration

Section: `WebhookRetry`

| Key | Default | Purpose |
|-----|---------|---------|
| `Enabled` | `true` | Master switch |
| `MaxRetries` | `5` | Total attempts including first |
| `RetryDelaysMinutes` | `[1,5,15,60]` | Backoff schedule |
| `InitialDelayMinutes` | `1` | Fallback if array empty |
| `BackoffMultiplier` | `5` | Fallback exponential |
| `PollIntervalSeconds` | `30` | Retry scanner interval |

---

## Audit events

- `WebhookRetryScheduled`
- `WebhookRetryAttempted`
- `WebhookRetrySucceeded` (success after attempt > 1)
- `WebhookRetryFailed`

See [failure-classification.md](./failure-classification.md) and [ADR-Webhook-0004](../../adr/ADR-Webhook-0004-webhook-retry-and-dlq.md).
