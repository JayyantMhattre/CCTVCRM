# Dead letter visibility (mobile)

**Pages:** `WebhookDeadLettersPage`, `WebhookDeadLetterDetailPage`

## List columns

| Field | API field |
|-------|-----------|
| Event | `eventName` |
| Failure reason | `failureReason` |
| Failure classification | `failureCode` |
| Retry count | `retryCount` |
| Created date | `createdOnUtc` |

## Filters

| Filter | Behavior |
|--------|----------|
| Search | Event, correlation ID, reason, ID |
| Failure type | Match `failureCode` |
| Date range | Client-side on `createdOnUtc` |

## Detail view

- Failure details and correlation ID (copyable)
- Payload (JSON string)
- Retry history summary (count at dead-letter time)

## Actions

None — replay and recovery require the web admin console.
