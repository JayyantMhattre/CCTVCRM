# Webhook observability

**Status:** Implemented (W3) — structured logging, health checks, and OpenTelemetry metrics.

---

## Correlation

Every delivery preserves the platform `correlationId` from publish through HTTP headers, delivery logs, Serilog, and audit.

Trace chain:

```
Business event → Outbox → Dispatcher → Delivery → Audit
                      ↘ correlationId preserved at each step
```

Header: `X-Correlation-Id` on outbound HTTP requests when correlation is present.

---

## Structured logging (Serilog)

`WebhookDeliveryService` emits:

| Level | When | Fields |
|-------|------|--------|
| Information | HTTP 2xx | `DeliveryId`, `StatusCode`, `DurationMs`, `CorrelationId` |
| Warning | HTTP non-success or transport error | Same + exception on transport failures |

`WebhookDispatcher` logs when no matching subscriptions exist (Information).

`WebhookRequestedEventHandler` logs dispatch start with `EventName`, `TenantId`, `CorrelationId`.

**Never logged:** subscription secrets, raw signing keys.

---

## OpenTelemetry

Meter: `Ashraak.Webhooks.Delivery` (registered in host OTel pipeline)

| Instrument | Type | Description |
|------------|------|-------------|
| `webhook.delivery.attempts` | Counter | HTTP delivery attempts |
| `webhook.delivery.successes` | Counter | Successful deliveries |
| `webhook.delivery.failures` | Counter | Failed attempts |
| `webhook.retry.scheduled` | Counter | Retries scheduled |
| `webhook.dlq.created` | Counter | Dead letters created |
| `webhook.dlq.replayed` | Counter | DLQ replays |
| `webhook.delivery.duration_ms` | Histogram | Delivery latency |

Serilog enrichers and correlation middleware complement metrics for trace-level investigation.

---

## Health checks

| Endpoint | Check |
|----------|-------|
| `/health/ready` | `WebhooksHealthCheck` — dispatcher, delivery service, delivery + retry hosted workers |

Degraded if `WebhookDeliveryHostedService` or `WebhookRetryHostedService` is not registered.

---

## Operational queries

| Question | Where |
|----------|-------|
| Did delivery succeed? | `GET /api/v1/webhooks/deliveries/{id}` |
| Recent failures for tenant? | `GET /deliveries?status=Failed` |
| Audit trail? | Audit module — `WebhookDeliverySucceeded` / `WebhookDeliveryFailed` |
| Support ticket trace? | `X-Correlation-Id` + delivery id |

---

## Retry and DLQ lifecycle

| Event | Log / metric |
|-------|--------------|
| Retry scheduled | `WebhookRetryScheduled` audit + `webhook.retry.scheduled` |
| Retry attempted | `WebhookRetryAttempted` audit |
| DLQ created | `WebhookDeadLetterCreated` audit + `webhook.dlq.created` |
| DLQ replayed | `WebhookDeadLetterReplayed` audit + `webhook.dlq.replayed` |

See [retry-engine.md](./retry-engine.md) and [operations.md](./operations.md).
