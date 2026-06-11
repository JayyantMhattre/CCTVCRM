# Mobile — Webhook visibility (W5)

**Feature:** `FrontEnd.Mobile/lib/features/webhooks/`

Read-only operational visibility for deliveries and dead letters. No administrative actions.

## Scope

| Included | Excluded |
|----------|----------|
| Dashboard metrics | Subscription management |
| Delivery history | Secret rotation |
| Dead letter visibility | Retry / replay |
| Correlation ID copy | Backend changes |

## Routes

| Path | Page |
|------|------|
| `/webhooks` | `WebhookOverviewPage` |
| `/webhooks/deliveries` | `WebhookDeliveriesPage` |
| `/webhooks/deliveries/:id` | `WebhookDeliveryDetailPage` |
| `/webhooks/deadletters` | `WebhookDeadLettersPage` |
| `/webhooks/deadletters/:id` | `WebhookDeadLetterDetailPage` |

## APIs (read-only)

| Endpoint | Purpose |
|----------|---------|
| `GET /api/v1/webhooks/deliveries` | Delivery list |
| `GET /api/v1/webhooks/deliveries/{id}` | Delivery detail |
| `GET /api/v1/webhooks/deadletters` | Dead letter list |
| `GET /api/v1/webhooks/deadletters/{id}` | Dead letter detail |

## State

Riverpod providers:

- `webhookOverviewProvider`
- `webhookDeliveriesProvider`
- `webhookDeadLettersProvider`
- `webhookDeliveryDetailProvider(id)`
- `webhookDeadLetterDetailProvider(id)`

## Related docs

- [dashboard.md](./dashboard.md)
- [deliveries.md](./deliveries.md)
- [deadletters.md](./deadletters.md)
- [permissions.md](./permissions.md)
- [offline-support.md](./offline-support.md)
