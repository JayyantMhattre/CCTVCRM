# Webhook delivery history

**Status:** Implemented (W2) — read-only APIs. No retry or replay endpoints until W3.

---

## Purpose

Operators and integrators inspect first-attempt delivery outcomes: status, HTTP response, timing, and correlation for support and auditing.

---

## API

Base: `/api/v1/webhooks`

| Method | Route | Permission | Description |
|--------|-------|------------|-------------|
| GET | `/deliveries` | `webhooks:read` or Admin | Filtered delivery history |
| GET | `/deliveries/{id}` | `webhooks:read` or Admin | Single delivery detail |

**Feature flag:** `webhooks.enabled` must be on for the tenant.

---

## List deliveries

```http
GET /api/v1/webhooks/deliveries?subscriptionId={guid}&eventName=user.created&status=Failed&fromUtc=2026-05-01T00:00:00Z&toUtc=2026-05-31T23:59:59Z&limit=100
```

| Query | Type | Description |
|-------|------|-------------|
| `subscriptionId` | GUID | Filter by subscription |
| `eventName` | string | Catalog event name |
| `status` | string | `Pending`, `Succeeded`, `Failed`, `Retrying`, `DeadLettered` |
| `fromUtc` | ISO 8601 | `started_on_utc` lower bound |
| `toUtc` | ISO 8601 | `started_on_utc` upper bound |
| `limit` | int | Max rows (default 100) |

Results are ordered by `startedOnUtc` descending. Tenant scope is implicit from JWT tenant context.

---

## Delivery detail

```http
GET /api/v1/webhooks/deliveries/{id}
```

**200 OK** — `WebhookDeliveryContract`:

| Field | Description |
|-------|-------------|
| `id` | Delivery id |
| `subscriptionId` | Target subscription |
| `eventName` / `eventVersion` | Event identity |
| `correlationId` | Platform trace id |
| `payload` | Request envelope JSON |
| `attemptNumber` | Current attempt (increments on retry) |
| `retryCount` | Number of retries scheduled |
| `lastFailureReason` / `lastFailureCode` | Last error details |
| `nextRetryOnUtc` | Scheduled retry time when `Retrying` |
| `status` | `Pending`, `Succeeded`, `Failed`, `Retrying`, `DeadLettered` |
| `responseCode` | HTTP status when available |
| `responseBody` | Truncated response text |
| `startedOnUtc` / `completedOnUtc` | Timing |

---

## Status values (W2)

| Status | Meaning |
|--------|---------|
| `Pending` | Created; queued or in flight |
| `Succeeded` | HTTP 2xx received |
| `Failed` | Non-success HTTP, transport error, or validation failure |

For DLQ entries see `GET /api/v1/webhooks/deadletters` ([dead-letter-queue.md](./dead-letter-queue.md)).

---

## Audit alignment

On terminal outcomes, domain events feed the platform audit pipeline:

- `WebhookDeliverySucceeded` → audit action **WebhookDeliverySucceeded**
- `WebhookDeliveryFailed` → audit action **WebhookDeliveryFailed**

Use `correlationId` to link API history, Serilog entries, and audit records.

---

## Troubleshooting

1. `GET /deliveries` filtered by subscription, event, status, date range
2. Check `responseCode` and `responseBody` for subscriber errors
3. Confirm subscription enabled and subscribed to event type
4. Verify subscriber HMAC verification ([hmac-signing.md](./hmac-signing.md))

See [operations.md](./operations.md).
