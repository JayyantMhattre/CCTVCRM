# Webhook API

Base path: `/api/v1/webhooks`

**Auth:** Bearer JWT + tenant context. **Policy:** `TenantAdmin` (Admin/Manager). **Feature flag:** `webhooks.enabled`.

---

## Endpoints

| Method | Route | Permission | Description |
|--------|-------|------------|-------------|
| GET | `/health` | Anonymous | Module health |
| GET | `/subscriptions` | `webhooks:read` or Admin | List subscriptions |
| GET | `/subscriptions/{id}` | `webhooks:read` or Admin | Get subscription |
| POST | `/subscriptions` | `webhooks:manage` or Admin | Create (returns secret once) |
| PUT | `/subscriptions/{id}` | `webhooks:manage` or Admin | Update |
| POST | `/subscriptions/{id}/rotate-secret` | `webhooks:manage` or Admin | Rotate secret |
| POST | `/subscriptions/{id}/disable` | `webhooks:manage` or Admin | Disable |
| GET | `/deliveries` | `webhooks:read` or Admin | Delivery history (filters) |
| GET | `/deliveries/{id}` | `webhooks:read` or Admin | Delivery detail |
| POST | `/deliveries/{id}/retry` | `webhooks:manage` or Admin | Manual retry |
| GET | `/deadletters` | `webhooks:read` or Admin | DLQ list |
| GET | `/deadletters/{id}` | `webhooks:read` or Admin | DLQ detail |
| POST | `/deadletters/{id}/replay` | `webhooks:manage` or Admin | Replay dead letter |

See [delivery-history.md](./delivery-history.md), [manual-replay.md](./manual-replay.md).

---

## Create subscription

```http
POST /api/v1/webhooks/subscriptions
Content-Type: application/json

{
  "name": "CRM Integration",
  "endpointUrl": "https://integrations.example.com/webhooks/ashraak",
  "subscribedEventNames": ["user.created", "user.updated"]
}
```

**201 Created** — body includes `secret` (shown once).

---

## Update subscription

```http
PUT /api/v1/webhooks/subscriptions/{id}
Content-Type: application/json

{
  "name": "CRM Integration",
  "endpointUrl": "https://integrations.example.com/webhooks/ashraak",
  "enabled": true
}
```

---

## Rotate secret

```http
POST /api/v1/webhooks/subscriptions/{id}/rotate-secret
```

**200 OK** — new `secret` in response (once).

---

## Disable

```http
POST /api/v1/webhooks/subscriptions/{id}/disable
```

---

## Errors

| Code | Meaning |
|------|---------|
| `Webhooks.Disabled` | Feature flag off |
| `Webhooks.ManageForbidden` | Missing manage permission |
| `Webhooks.ReadForbidden` | Missing read permission |
| `Webhooks.NameExists` | Duplicate name in tenant |
| `Webhooks.EndpointHttps` | HTTPS required |
| `Webhooks.NotFound` | Wrong id or tenant |

OpenAPI: Scalar UI (`/scalar/v1`) when host runs in Development.
