# Webhook foundation (W1)

**Status:** Implemented

W1 delivers subscription management, event catalog registry, outbox publishing, and admin APIs — **no HTTP delivery**.

---

## What is implemented

| Area | Status |
|------|--------|
| `Webhooks` module (Domain, Application, Infrastructure, Api) | Done |
| PostgreSQL `webhooks` schema | Done |
| `webhook_subscriptions` table | Done |
| `webhook_event_definitions` table | Done |
| Subscription CRUD API | Done |
| Secret generation + Data Protection at rest | Done |
| `IWebhookPublisher` → outbox | Done |
| Feature flag `webhooks.enabled` | Done |
| Permissions `webhooks:read`, `webhooks:manage` | Done |
| Outbox processor for `WebhooksDbContext` | Done |
| Audit via domain events + EF interceptor | Done |

---

## What is not implemented (later phases)

- Retry / DLQ (W3)
- Admin UI (W4)
- Mobile visibility (W5)

Delivery engine (W2) is documented in [delivery-engine.md](./delivery-engine.md).

---

## Module layout

```
BackEnd/src/Modules/Webhooks/
├── Ashraak.Webhooks.Domain/
├── Ashraak.Webhooks.Application/
├── Ashraak.Webhooks.Infrastructure/
└── Ashraak.Webhooks.Api/
```

Contracts: `SharedKernel.Contracts/Webhooks/`

---

## Configuration

| Key | Default (dev) | Purpose |
|-----|---------------|---------|
| `ConnectionStrings:Webhooks` | `Search Path=webhooks` | PostgreSQL schema |
| `Features:Flags:webhooks.enabled` | `false` | Tenant gate |
| `Webhooks:RequireHttpsEndpoints` | `false` (dev) | URL validation |

---

## Related

- [subscriptions.md](./subscriptions.md)
- [api.md](./api.md)
- [permissions.md](./permissions.md)
- [roadmap.md](./roadmap.md)
