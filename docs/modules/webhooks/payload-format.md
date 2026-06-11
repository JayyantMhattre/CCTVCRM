# Webhook payload format

**Status:** Implemented (W2)

All outbound webhook events use a single canonical envelope. The `data` field carries the module-specific payload from `IWebhookPublisher`.

---

## Envelope schema

```json
{
  "eventId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "eventName": "user.created",
  "version": "v1",
  "occurredOnUtc": "2026-05-31T12:00:00Z",
  "tenantId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "correlationId": "00-abc123",
  "data": { }
}
```

| Field | Type | Description |
|-------|------|-------------|
| `eventId` | UUID string | Unique id for this outbound event instance |
| `eventName` | string | Catalog name (`domain.entity.action`) |
| `version` | string | Schema version (e.g. `v1`) |
| `occurredOnUtc` | ISO 8601 UTC | Dispatch time |
| `tenantId` | UUID string | Originating tenant |
| `correlationId` | string \| null | Platform correlation for tracing |
| `data` | object | Module payload (parsed from publisher JSON) |

---

## Builder

`WebhookPayloadBuilder` (`IWebhookPayloadBuilder`) serialises the envelope with **camelCase** property names and compact JSON (no indentation).

If `PayloadJson` from the publisher is not valid JSON, it is wrapped as a JSON string value in `data`.

---

## HTTP delivery

- **Method:** `POST` only
- **Content-Type:** `application/json; charset=utf-8`
- **Body:** Full envelope JSON (same bytes used for HMAC)

---

## Headers (sent with payload)

| Header | Example | Source |
|--------|---------|--------|
| `X-Webhook-Event` | `user.created` | `eventName` |
| `X-Webhook-Version` | `v1` | `version` |
| `X-Webhook-Delivery-Id` | `{delivery-guid}` | `webhook_deliveries.id` |
| `X-Correlation-Id` | `00-abc123` | Platform correlation (when present) |
| `X-Webhook-Signature` | `{hex-hmac}` | HMAC-SHA256 of raw body |

See [hmac-signing.md](./hmac-signing.md).

---

## Versioning rules

- Event names and versions must exist in `webhook_event_definitions`
- Breaking schema changes require a new `version` (e.g. `v2`)
- Subscribers should validate `version` before deserialising `data`

Aligns with [event-catalog.md](./event-catalog.md) and [ADR-Webhook-0001](../../adr/ADR-Webhook-0001-webhook-platform-architecture.md).

---

## Subscriber parsing (recommended)

1. Read headers for routing and verification
2. Verify `X-Webhook-Signature` against raw body
3. Deserialise envelope; check `eventName` + `version`
4. Map `data` to subscriber-specific DTO
