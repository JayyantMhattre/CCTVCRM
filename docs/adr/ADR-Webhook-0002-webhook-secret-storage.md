# ADR-Webhook-0002: Webhook secret storage

## Status

Accepted (W1)

## Date

2026-05-31

## Context

Webhook signing secrets must be stored for future HMAC delivery (W2) while never appearing in logs or API read responses.

## Decision

1. Generate secrets with `RandomNumberGenerator` (32 bytes, Base64).
2. Protect at rest with ASP.NET Core **Data Protection** (`IDataProtector`, purpose `Ashraak.Webhooks.SubscriptionSecret.v1`).
3. Return plaintext secret **only** on create and rotate API responses.
4. Never include secret in `WebhookSubscriptionContract` read DTOs.

## Alternatives

| Alternative | Rejected because |
|-------------|------------------|
| Plaintext in DB | Violates security model |
| One-way hash only | Cannot sign outbound requests in W2 |
| External vault (W1) | Over-engineering for foundation phase |

## Consequences

- Host must run Data Protection (default in Ashraak.Api).
- Secret rotation invalidates old subscriber verification until they update.

## References

- [security.md](../modules/webhooks/security.md)
- [subscriptions.md](../modules/webhooks/subscriptions.md)
