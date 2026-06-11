# Webhook HMAC signing

**Status:** Implemented (W2)

Subscribers verify that payloads originated from Ashraak and were not tampered with in transit.

---

## Algorithm

| Item | Value |
|------|-------|
| Algorithm | HMAC-SHA256 |
| Encoding | UTF-8 (secret and body) |
| Output | Lowercase hexadecimal string |
| Header | `X-Webhook-Signature` |

Implementation: `WebhookSignatureService` (`IWebhookSignatureService`).

---

## What is signed

The HMAC is computed over the **exact raw JSON body** sent in the HTTP POST — the full envelope from [payload-format.md](./payload-format.md).

```
signature = HMAC_SHA256(subscription_secret, raw_body_bytes)
```

No timestamp prefix in W2 (replay window via subscriber deduplication is recommended; timestamp signing is a future enhancement).

---

## Secret source

- Per-subscription signing secret (generated on create / rotate)
- Stored encrypted at rest via Data Protection ([ADR-Webhook-0002](../../adr/ADR-Webhook-0002-webhook-secret-storage.md))
- Unprotected only in memory during delivery — **never logged**

---

## Subscriber verification (example)

```csharp
using System.Security.Cryptography;
using System.Text;

static bool Verify(string secret, string rawBody, string signatureHeader)
{
    var key = Encoding.UTF8.GetBytes(secret);
    var data = Encoding.UTF8.GetBytes(rawBody);
    var expected = Convert.ToHexString(HMACSHA256.HashData(key, data)).ToLowerInvariant();
    return CryptographicOperations.FixedTimeEquals(
        Encoding.UTF8.GetBytes(expected),
        Encoding.UTF8.GetBytes(signatureHeader.Trim()));
}
```

1. Read raw request body (before JSON parsing)
2. Compute HMAC with stored secret
3. Compare to `X-Webhook-Signature` using constant-time comparison
4. Reject on mismatch (`401` recommended)

---

## Security rules

| Rule | Enforcement |
|------|-------------|
| No secret in logs | Delivery service logs ids and status only |
| Tenant-scoped secret | Secret belongs to one subscription / tenant |
| HTTPS in production | `Webhooks:RequireHttpsEndpoints` (see [security.md](./security.md)) |
| Payload integrity | Signature covers full body |

---

## Rotation

When a secret is rotated via `POST /subscriptions/{id}/rotate-secret`, new deliveries use the current secret. Subscribers should update their stored secret promptly. Dual-secret grace window is planned for a future phase.

---

## ADR

[ADR-Webhook-0003](../../adr/ADR-Webhook-0003-webhook-delivery-engine.md) — payload envelope and signing decisions.
