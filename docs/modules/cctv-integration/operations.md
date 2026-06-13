# Operations — CCTV Integration

**Health:** `GET /api/v1/cctv/health` (anonymous)

## Configuration (`appsettings.json`)

```json
"Cctv": {
  "Notifications": {
    "PortalUrl": "http://localhost:5173",
    "DefaultTenantId": "00000000-0000-0000-0000-000000000001"
  }
},
"Sms": {
  "Provider": "console",
  "Http": {
    "Endpoint": "",
    "ApiKeyHeader": "Authorization",
    "ApiKeyValue": "",
    "PhoneNumberField": "to",
    "MessageField": "message"
  }
},
"Features": {
  "Flags": {
    "cctv.integrations.sms.enabled": true,
    "cctv.integrations.pdf.enabled": true
  }
}
```

## SMS provider modes

| `Sms:Provider` | Behaviour |
|----------------|-----------|
| `console` | Log SMS body (default, no vendor lock-in) |
| `http` | POST JSON to `Sms:Http:Endpoint` with configurable field names |

## Email templates

Path: `Host/Ashraak.Api/Templates/cctv/*.txt`  
Keys: see `CctvNotificationTemplateKeys` in Integration.Application.

## PDF generation

- Gated by `cctv.integrations.pdf.enabled`
- Renderer: QuestPDF (community license)
- Storage: platform Files module via `ICctvFileStore`

## Outbox processors

CCTV DbContext outbox workers registered in `CctvIntegrationModule`. Ensure PostgreSQL schemas are migrated before expecting notifications/PDF side effects.

## AMC expiry reminders

`CctvAmcExpiryReminderHostedService` runs daily; sends reminders for active terms expiring in 30 days.
