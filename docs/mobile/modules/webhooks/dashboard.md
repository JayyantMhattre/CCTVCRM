# Webhook dashboard (mobile)

**Page:** `WebhookOverviewPage`  
**Route:** `/webhooks`

## Metrics

Derived client-side from recent delivery and dead-letter API samples:

| Metric | Source |
|--------|--------|
| Total deliveries | Delivery list count |
| Success rate | `Succeeded` / total |
| Failure rate | `Failed` + `DeadLettered` / total |
| Retry count | Sum of `retryCount` |
| Dead letter count | Dead letter list count |

## Sections

- **Recent failures** — up to 5 failed deliveries
- **Recent deliveries** — up to 5 latest deliveries

## Refresh

- Pull-to-refresh on the overview list
- Background sync via `SyncService` when `webhooks:read` is granted

## Navigation

Quick links to delivery history and dead letter dashboards.
