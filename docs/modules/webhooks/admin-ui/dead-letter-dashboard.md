# Dead letter dashboard UI

**Routes:** `/webhooks/deadletters`, `/webhooks/deadletters/:id`

---

## List view

Columns:

- Event name
- Failure reason
- Failure code
- Retry count
- Created date
- Correlation ID

Filters: event, subscription, date range, search.

---

## Detail view

- Link to original delivery
- Failure classification (client-side heuristic matching backend classifier)
- Retry count
- Full payload (JSON formatted)
- Correlation ID with copy

---

## Replay (`webhooks:manage`)

1. Confirmation dialog
2. `POST /api/v1/webhooks/deadletters/{id}/replay`
3. Progress via mutation pending state
4. Success toast + redirect to new delivery detail
5. Original dead letter preserved in history
