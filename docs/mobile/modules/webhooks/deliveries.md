# Delivery history (mobile)

**Pages:** `WebhookDeliveriesPage`, `WebhookDeliveryDetailPage`

## List columns

| Field | API field |
|-------|-----------|
| Event name | `eventName` |
| Status | `status` |
| Timestamp | `startedOnUtc` |
| Duration | `completedOnUtc` − `startedOnUtc` |
| Response code | `responseCode` |
| Correlation ID | `correlationId` |

## Filters (client-side on cached list)

| Filter | Behavior |
|--------|----------|
| Event type | Match `eventName` |
| Status | Match `status` |
| Search | Event, correlation ID, delivery ID |
| Date range | Planned — server query params supported in repository |

## Detail view

Read-only sections:

- Summary (subscription, attempts, retries, failure info)
- Timeline (started, completed, next retry)
- Response summary (`responseBody`)
- Correlation banner with copy

**Note:** Delivery contract does not include request payload; payload is available on dead letters.

## Actions

None — retry and replay remain in the web Operations Center.
