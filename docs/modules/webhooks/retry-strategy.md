# Webhook retry strategy

**W0:** Policy documentation only — no retry engine implemented.

---

## Goals

- Maximize successful delivery without harming platform stability
- Classify failures (transient vs permanent)
- Prevent poison messages from infinite loops
- Preserve audit trail for every attempt

---

## Retry policy (default target)

| Parameter | Default | Notes |
|-----------|---------|-------|
| Max attempts | 8 | Including initial delivery |
| Backoff | Exponential with jitter | `delay = min(cap, base * 2^attempt) + jitter` |
| Base delay | 60 seconds | Configurable per environment |
| Cap | 24 hours | Between attempts |
| Jitter | ±20% | Avoid thundering herd |

Example schedule (illustrative): 1m → 2m → 4m → 8m → 16m → 32m → 64m → DLQ.

---

## Failure classification

| HTTP / error | Retry? | Rationale |
|--------------|--------|-----------|
| Timeout / connection reset | Yes | Transient network |
| 408, 429, 500, 502, 503, 504 | Yes | Subscriber or gateway temporary |
| 401, 403 (after signature verify) | No | Auth misconfiguration |
| 404 | No | Wrong endpoint — operator fix |
| 400, 422 | No | Payload rejected — fix schema |
| 410 | No | Gone — disable subscription |

---

## Dead letter policy

When max attempts exhausted:

1. Move delivery record to **Dead Letter Queue** (DLQ)
2. Mark subscription health `degraded` (alert optional)
3. Retain full attempt history in **Webhook Audit**
4. Allow **manual replay** from admin UI (W4) with new correlation id

---

## Poison message handling

| Scenario | Handling |
|----------|----------|
| Malformed outbox payload | Quarantine + alert; no retry |
| Subscriber always 500 | DLQ after max attempts |
| Signing secret invalid | Fail fast; no retry; notify tenant admin |
| Event type retired | Skip dispatch; log `skipped_deprecated` |

---

## Subscriber guidance (document for integrators)

- Respond **2xx** quickly; process async if needed
- Implement **idempotency** on `id` field
- Return **410** when endpoint permanently retired

---

## Operations

See [operations.md](./operations.md) for investigation and replay procedures (future).
