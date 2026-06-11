# Webhook security model

**W2:** HMAC signing implemented on outbound delivery. Replay timestamp signing deferred.

---

## Principles

1. **Tenant isolation** — subscriptions and secrets scoped to `tenantId`; no cross-tenant fan-out.
2. **Authenticity** — subscribers verify Ashraak originated the payload.
3. **Integrity** — tampering detected via HMAC.
4. **Replay resistance** — timestamp + nonce window.
5. **Least privilege** — admin APIs require tenant admin / platform role.

---

## HMAC signing (target)

Headers (illustrative):

| Header | Purpose |
|--------|---------|
| `Webhook-Signature` | `v1=<hex_hmac>` |
| `Webhook-Timestamp` | Unix seconds |
| `Webhook-Id` | Delivery attempt id |
| `Webhook-Event-Type` | `v1.user.created` |
| `X-Correlation-Id` | Platform correlation |

Algorithm: **HMAC-SHA256** over:

```
{timestamp}.{raw_body}
```

Secret: per-subscription **signing secret** (generated on create; shown once).

Subscribers recompute HMAC with stored secret; mismatch → reject.

---

## Secret rotation

1. Support **two active secrets** during rotation window
2. Admin UI: generate new secret → grace period → revoke old
3. Audit log: `secret.rotated` (internal event, not webhook)

Never log plaintext secrets.

---

## Replay protection

W2 signs body only. Subscribers should:

1. Track `X-Webhook-Delivery-Id` for deduplication
2. Optionally enforce `occurredOnUtc` freshness from payload envelope

Timestamp-prefixed signing (`{timestamp}.{body}`) is planned for a future phase.

Platform should reject replay of **admin** replay actions without authorization.

---

## Transport

| Layer | Requirement |
|-------|-------------|
| TLS | HTTPS only for production subscribers |
| URL validation | HTTPS scheme required in prod; block private IP ranges (SSRF mitigation) — W2 |
| mTLS | Future optional ([platform-manifest](./platform-manifest.md)) |
| IP allowlisting | Future per-subscription |

---

## Authorization (admin APIs, future)

| Action | Role |
|--------|------|
| Create/update subscription | Tenant Admin |
| View delivery logs | Tenant Admin / Auditor |
| Replay DLQ | `webhooks:manage` or Tenant Admin |
| Manual delivery retry | `webhooks:manage` or Tenant Admin |
| View DLQ | `webhooks:read` or Tenant Admin |
| Global event registry | Platform Admin |

Mobile: **read-only** logs (W5); no secret or subscription editing.

---

## Data minimization

- Payloads contain only fields required for integration
- Avoid full PII dumps; align with [event-catalog.md](./event-catalog.md)
- Failed delivery bodies truncated in logs

---

## Compliance hooks

- Correlate with [Audit module](../audit/README.md) for admin actions
- Retention policy for webhook logs (TBD in W3 operations)
