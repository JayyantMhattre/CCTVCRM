# Webhook Operations Center (W4)

**Status:** Implemented — web admin experience for webhook operations.

---

## Overview

The Webhook Operations Center is the tenant administration console for managing subscriptions, investigating deliveries, replaying dead letters, and monitoring webhook health — without direct API calls.

**Location:** `FrontEnd/apps/web/src/modules/webhooks/`

**Routes:**

| Path | Screen |
|------|--------|
| `/webhooks` | Health dashboard |
| `/webhooks/subscriptions` | Subscription list |
| `/webhooks/subscriptions/:id` | Subscription detail |
| `/webhooks/deliveries` | Delivery history |
| `/webhooks/deliveries/:id` | Delivery detail |
| `/webhooks/deadletters` | Dead letter queue |
| `/webhooks/deadletters/:id` | Dead letter detail |

---

## Permissions

| Permission | UI access |
|------------|-----------|
| `webhooks:read` | View all screens |
| `webhooks:manage` | Create, edit, disable, rotate secret, retry, replay |

See [permissions.md](./permissions.md).

---

## Documentation

- [operations-center.md](./operations-center.md) — architecture and navigation
- [subscriptions.md](./subscriptions.md) — subscription management
- [delivery-history.md](./delivery-history.md) — delivery investigation
- [dead-letter-dashboard.md](./dead-letter-dashboard.md) — DLQ and replay

---

## Out of scope (W5)

Mobile webhook visibility remains planned.
