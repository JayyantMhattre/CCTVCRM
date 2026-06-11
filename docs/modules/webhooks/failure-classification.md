# Webhook failure classification (W3)

**Status:** Implemented

`WebhookFailureClassifier` determines whether a failed delivery should be retried or fail fast.

---

## Failure types

| Type | Retry? | Examples |
|------|--------|----------|
| **Transient** | Yes | 429, 500, 502, 503, 504, timeout, connection errors |
| **Permanent** | No | 400, 401, 403, 404, 410, 422, disabled subscription, tenant mismatch |
| **Unknown** | Yes (if attempts remain) | Unclassified HTTP codes |

---

## HTTP status mapping

### Transient (retryable)

- `408` Request Timeout
- `429` Too Many Requests
- `500` Internal Server Error
- `502` Bad Gateway
- `503` Service Unavailable
- `504` Gateway Timeout

### Permanent (not retryable)

- `400` Bad Request
- `401` Unauthorized
- `403` Forbidden
- `404` Not Found
- `410` Gone
- `422` Unprocessable Entity

---

## Transport errors

Messages containing `timeout`, `connection`, `network`, or `HttpRequestException` / `TaskCanceledException` are classified **Transient**.

---

## Platform errors (permanent)

| Message | Rationale |
|---------|-----------|
| Subscription not found / tenant mismatch | Data integrity — no point retrying |
| Subscription is disabled | Operator action required |
| Invalid URL / Invalid Payload | Configuration fix required |

---

## Outcome routing

```
Classify failure
  → Permanent → Status: Failed
  → Transient + CanRetry → Status: Retrying
  → Transient + exhausted → DLQ + Status: DeadLettered
```

`WebhookDeliveryOutcomeHandler` orchestrates routing. Never retries inside the HTTP request pipeline.

---

## Subscriber guidance

- Return **2xx** quickly for success
- Return **4xx** for permanent client/schema errors
- Return **5xx** only for transient server issues
- Use **410** when endpoint is permanently retired
