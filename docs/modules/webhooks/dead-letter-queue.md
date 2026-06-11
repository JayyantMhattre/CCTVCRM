# Webhook dead letter queue (W3)

**Status:** Implemented

Terminal storage for webhook deliveries that exhausted all retry attempts.

---

## Table

`webhooks.webhook_dead_letters`

| Column | Purpose |
|--------|---------|
| `id` | Dead letter id |
| `delivery_id` | Original delivery |
| `subscription_id` | Target subscription |
| `tenant_id` | Tenant scope |
| `event_name` | Catalog event |
| `payload` | Full envelope JSON |
| `failure_reason` | Last error message |
| `failure_code` | Last HTTP status |
| `retry_count` | Retries performed |
| `correlation_id` | Platform trace id |
| `created_on_utc` | DLQ entry time |

---

## When entries are created

`DeadLetterService.MoveToDeadLetterAsync` runs when:

- Retry engine is enabled
- Failure is transient (or unknown)
- `AttemptNumber >= MaxRetries` on failure

Domain event: `WebhookDeadLetterCreated` → audit pipeline.

---

## Configuration

Section: `WebhookDLQ`

| Key | Default | Purpose |
|-----|---------|---------|
| `Enabled` | `true` | Store DLQ rows |
| `RetentionDays` | `90` | Retention policy (operational) |

If DLQ disabled, exhausted deliveries are marked `Failed` instead.

---

## API (read)

| Method | Route | Permission |
|--------|-------|------------|
| GET | `/api/v1/webhooks/deadletters` | `webhooks:read` |
| GET | `/api/v1/webhooks/deadletters/{id}` | `webhooks:read` |

Query filters: `subscriptionId`, `eventName`, `fromUtc`, `toUtc`, `limit`.

---

## Security

- Tenant-scoped query filter on all reads
- Secrets never stored in DLQ payload (envelope only)
- Replay requires `webhooks:manage` or Admin role

See [manual-replay.md](./manual-replay.md).
