# Webhook recovery operations (W3)

**Status:** Implemented

Operational recovery APIs and background processes for enterprise-reliable webhook delivery.

---

## Recovery capabilities

| Capability | Mechanism |
|------------|-----------|
| Automatic retry | `WebhookRetryHostedService` + backoff |
| Dead letter storage | `webhook_dead_letters` table |
| Manual retry | `POST /deliveries/{id}/retry` |
| DLQ replay | `POST /deadletters/{id}/replay` |
| DLQ inspection | `GET /deadletters` |

---

## Investigation checklist

1. Find delivery or dead letter by `correlationId`, subscription, or event name
2. Check `lastFailureReason` / `failureReason` and HTTP `failureCode`
3. Classify per [failure-classification.md](./failure-classification.md)
4. Fix root cause (endpoint, secret, schema, network)
5. Replay or manual retry
6. Verify `Succeeded` in delivery history

---

## Health checks

`/health/ready` verifies:

- `WebhookDeliveryHostedService` (HTTP worker)
- `WebhookRetryHostedService` (retry scanner)
- Delivery and retry DI registrations

Degraded if either hosted service is missing.

---

## Metrics (OpenTelemetry)

Meter: `Ashraak.Webhooks.Delivery`

| Metric | Purpose |
|--------|---------|
| `webhook.delivery.attempts` | Total HTTP attempts |
| `webhook.delivery.successes` | Successful deliveries |
| `webhook.delivery.failures` | Failed attempts (before retry routing) |
| `webhook.retry.scheduled` | Retries scheduled |
| `webhook.dlq.created` | Dead letters created |
| `webhook.dlq.replayed` | Manual replays |
| `webhook.delivery.duration_ms` | Delivery latency histogram |

Derive success rate, failure rate, and DLQ depth from these counters in your observability backend.

---

## What is not in W3

- Admin UI for replay/DLQ (W4)
- Mobile visibility (W5)
- Automated DLQ retention purge job (future)

See [roadmap.md](./roadmap.md).
