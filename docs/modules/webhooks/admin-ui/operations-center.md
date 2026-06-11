# Operations center

The `/webhooks` overview page is the entry point for webhook operations.

---

## Health metrics

Metrics are **derived client-side** from recent delivery and dead letter API samples (no new backend metrics endpoints):

| Metric | Source |
|--------|--------|
| Total deliveries | Delivery history sample |
| Success / failure rate | Status counts |
| Retry count | Sum of `retryCount` |
| Dead letter count | DLQ list sample |
| Average delivery time | `completedOnUtc - startedOnUtc` |

Refresh occurs on page load and via TanStack Query stale window (30s).

---

## Quick links

Cards link to subscriptions, delivery history, and dead letter queue.

---

## Navigation

Sidebar section **Webhooks** (visible with `webhooks:read`):

- Operations Center
- Subscriptions
- Deliveries
- Dead Letters

Dashboard also includes a Webhooks quick-access card when permitted.
