# Delivery history UI

**Routes:** `/webhooks/deliveries`, `/webhooks/deliveries/:id`

---

## List filters

- Event type
- Status (`Pending`, `Succeeded`, `Failed`, `Retrying`, `DeadLettered`)
- Subscription ID
- Date range (from / to)
- Search (event name or correlation ID)

---

## List columns

- Event name
- Status badge
- HTTP response code
- Duration (computed)
- Started timestamp
- Correlation ID

---

## Detail view

Displays:

- Status, response code, duration, attempt/retry counts
- Correlation ID with copy button
- Standard outbound headers (from delivery metadata)
- Response body (formatted JSON when possible)
- Failure details when present

### Manual retry (`webhooks:manage`)

`POST /api/v1/webhooks/deliveries/{id}/retry` with confirmation dialog.
