# Subscriptions UI

**Route:** `/webhooks/subscriptions`

---

## List view

Displays:

- Name
- Endpoint URL
- Enabled status
- Created / updated timestamps

---

## Actions (`webhooks:manage`)

| Action | API |
|--------|-----|
| Create | `POST /api/v1/webhooks/subscriptions` |
| Edit | `PUT /api/v1/webhooks/subscriptions/{id}` |
| Disable | `POST /api/v1/webhooks/subscriptions/{id}/disable` |
| Rotate secret | `POST /api/v1/webhooks/subscriptions/{id}/rotate-secret` |
| View details | Navigation to detail page |

---

## Secret rotation

1. Confirmation dialog warns about subscriber update
2. On success, secret reveal modal shows value **once**
3. Copy button + success toast
4. Audit recorded server-side via domain events

---

## Detail page

**Route:** `/webhooks/subscriptions/:id`

Shows metadata, secret guidance, recent deliveries, and recent failures for the subscription.
