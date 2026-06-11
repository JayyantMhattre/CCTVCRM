# Webhook operations

**W3:** Retry engine, DLQ, and recovery APIs operational.

---

## Configuration

| Key | Default | Purpose |
|-----|---------|---------|
| `WebhookDelivery:TimeoutSeconds` | `30` | HTTP timeout per delivery |
| `WebhookDelivery:MaxConcurrentDeliveries` | `4` | Background worker parallelism |
| `WebhookDelivery:UserAgent` | `Ashraak-Webhooks/1.0` | Outbound User-Agent |
| `WebhookRetry:Enabled` | `true` | Automatic retries |
| `WebhookRetry:MaxRetries` | `5` | Total attempts |
| `WebhookRetry:RetryDelaysMinutes` | `[1,5,15,60]` | Backoff schedule |
| `WebhookDLQ:Enabled` | `true` | Dead letter storage |
| `WebhookDLQ:RetentionDays` | `90` | DLQ retention policy |
| `Features:Flags:webhooks.enabled` | `false` | Tenant feature gate |
| `Webhooks:RequireHttpsEndpoints` | `false` (dev) | HTTPS-only subscription URLs |

---

## Monitoring

OpenTelemetry meter `Ashraak.Webhooks.Delivery` exports:

| Metric | Alert threshold (starter) |
|--------|---------------------------|
| `webhook.delivery.successes` / `webhook.delivery.failures` | Success rate < 95% over 15m |
| `webhook.delivery.duration_ms` | p99 > 10s |
| `webhook.retry.scheduled` | Spike without corresponding successes |
| `webhook.dlq.created` | > 0 sustained 1h |
| `webhook.dlq.replayed` | Track recovery activity |

Integrate with [observability.md](./observability.md) and platform [observability.md](../../architecture/observability.md).

---

## Failure investigation

1. Identify `X-Webhook-Delivery-Id` or `X-Correlation-Id` from subscriber report
2. Query **delivery history** — `GET /api/v1/webhooks/deliveries` ([delivery-history.md](./delivery-history.md))
3. Review attempt history: status codes, durations, errors
4. Classify: transient (retry) vs permanent (fix endpoint/schema)
5. Check subscription health: secret rotation, URL, enabled events

---

## Retry analysis

| Symptom | Likely cause |
|---------|--------------|
| All attempts 503 | Subscriber down |
| All attempts 401 | Secret mismatch |
| Single event DLQ | Poison payload |
| Spike after deploy | Schema version mismatch |

Use [retry-strategy.md](./retry-strategy.md) classification table.

---

## Dead letter recovery

1. Fix root cause (URL, secret, subscriber bug)
2. Admin UI: **Replay** single delivery or batch from DLQ
3. Replay creates new delivery attempt; original remains in audit
4. Document change ticket for compliance

---

## Audit queries

Align with platform audit patterns:

- Who created/changed subscription?
- Who triggered manual replay?
- Delivery outcomes per tenant per day

---

## Troubleshooting checklist

- [ ] Subscription enabled for event type?
- [ ] Tenant context correct on outbox message?
- [ ] Feature flag `webhooks.enabled` for tenant?
- [ ] Subscriber URL reachable (no SSRF block)?
- [ ] Clock skew within replay window?
- [ ] Schema version matches subscriber expectation?

---

## Recovery

- Manual retry: `POST /api/v1/webhooks/deliveries/{id}/retry`
- DLQ replay: `POST /api/v1/webhooks/deadletters/{id}/replay`

See [recovery-operations.md](./recovery-operations.md) and [manual-replay.md](./manual-replay.md).
