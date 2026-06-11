# Webhooks Platform

**Status:** W5 mobile visibility **implemented** — web admin (W4) + mobile read-only monitoring.

Enterprise webhook capability for Ashraak and all future products (CRM, LMS, HRMS, ERP, AI agents) without per-product redesign.

## Philosophy

Webhooks are a **platform capability**, not a business module.

- Any module may **publish** domain events eligible for webhook dispatch.
- Any external system may **subscribe** to typed, versioned events.
- Delivery is **asynchronous**, **tenant-scoped**, and **auditable**.

## Start here

| I want to… | Go to |
|------------|-------|
| See capability status | [platform-manifest.md](./platform-manifest.md) |
| Understand architecture | [architecture.md](./architecture.md) |
| Follow webhook rules | [governance.md](./governance.md) |
| Name and version events | [event-catalog.md](./event-catalog.md) |
| Understand delivery | [delivery-engine.md](./delivery-engine.md) |
| Payload format | [payload-format.md](./payload-format.md) |
| HMAC verification | [hmac-signing.md](./hmac-signing.md) |
| Delivery history API | [delivery-history.md](./delivery-history.md) |
| Observability | [observability.md](./observability.md) |
| Delivery model (concept) | [delivery-model.md](./delivery-model.md) |
| Retry engine | [retry-engine.md](./retry-engine.md) |
| Dead letter queue | [dead-letter-queue.md](./dead-letter-queue.md) |
| Failure classification | [failure-classification.md](./failure-classification.md) |
| Manual replay | [manual-replay.md](./manual-replay.md) |
| Recovery operations | [recovery-operations.md](./recovery-operations.md) |
| Retry policy (concept) | [retry-strategy.md](./retry-strategy.md) |
| Security model | [security.md](./security.md) |
| Run / troubleshoot (future) | [operations.md](./operations.md) |
| Extend the platform | [extending.md](./extending.md) |
| Delivery phases | [roadmap.md](./roadmap.md) |

## ADR

- [ADR-Webhook-0001](../../adr/ADR-Webhook-0001-webhook-platform-architecture.md) — platform architecture
- [ADR-Webhook-0003](../../adr/ADR-Webhook-0003-webhook-delivery-engine.md) — delivery engine (W2)
- [ADR-Webhook-0004](../../adr/ADR-Webhook-0004-webhook-retry-and-dlq.md) — retry + DLQ (W3)

| Admin UI | [admin-ui/README.md](./admin-ui/README.md) |
| Mobile visibility | [mobile/modules/webhooks/README.md](../../mobile/modules/webhooks/README.md) |

See [roadmap.md](./roadmap.md).
