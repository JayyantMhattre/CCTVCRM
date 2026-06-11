# Manual replay and recovery (W3)

**Status:** Implemented — API only (no Admin UI until W4).

---

## Manual delivery retry

Force an immediate retry of a failed or scheduled delivery.

```http
POST /api/v1/webhooks/deliveries/{id}/retry
Authorization: Bearer {token}
```

**Permission:** `webhooks:manage` or Tenant Admin.

**Eligible statuses:** `Failed`, `Retrying`

**Behavior:**

1. Validates tenant ownership
2. Increments attempt number
3. Sets status `Pending`
4. Enqueues to delivery worker

---

## Dead letter replay

Create a **new** delivery from a DLQ entry, preserving correlation.

```http
POST /api/v1/webhooks/deadletters/{id}/replay
Authorization: Bearer {token}
```

**Permission:** `webhooks:manage` or Tenant Admin.

**Behavior:**

1. Loads dead letter (tenant-scoped)
2. Creates new `WebhookDelivery` with same payload, subscription, correlation
3. Raises `WebhookDeadLetterReplayed` audit event
4. Enqueues new delivery id
5. Returns **201 Created** with new delivery contract

**Original** delivery and dead letter records are preserved for audit.

---

## Replay rules

| Rule | Enforcement |
|------|-------------|
| Preserve correlation | `correlationId` copied from dead letter |
| New delivery id | Fresh `WebhookDeliveryId` per replay |
| Audit trail | Original + replay events in audit log |
| Tenant isolation | Tenant id validated on every operation |
| No secret exposure | Replay uses stored envelope only |

---

## Operational workflow

1. Investigate failure via `GET /deliveries/{id}` or `GET /deadletters/{id}`
2. Fix subscriber endpoint, secret, or payload issue
3. Replay dead letter or manually retry delivery
4. Confirm success via delivery history API

See [recovery-operations.md](./recovery-operations.md).
