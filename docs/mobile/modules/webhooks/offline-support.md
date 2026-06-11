# Webhook offline support (mobile)

Reuses M4 Hive offline cache (`CachedRepositoryMixin`).

## Cache keys

| Key | Content |
|-----|---------|
| `webhooks:overview` | Dashboard metrics snapshot |
| `webhooks:deliveries` | Delivery list |
| `webhooks:delivery:{id}` | Delivery detail |
| `webhooks:deadletters` | Dead letter list |
| `webhooks:deadletter:{id}` | Dead letter detail |
| `sync:webhooks` | Last sync timestamp |

## Behavior

1. Online-first fetch from webhook APIs
2. On network failure, read-through from Hive cache
3. `OfflineBanner` shown globally when offline
4. `LastSyncFooter` on list/overview pages

## Background sync

`SyncService` refreshes webhook overview when user has `webhooks:read` and connectivity resumes.

## Notifications (local only)

`WebhookAlertProvider` abstraction supports future alerts:

- Delivery failure
- Dead letter
- Retry storm
- Subscription disabled

W5 ships `LocalWebhookAlertProvider` only — no backend push wiring.
