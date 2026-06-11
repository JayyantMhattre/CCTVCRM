# Notifications — Operations

## Configuration

```json
"Notifications": {
  "Provider": "console",
  "TemplatesPath": "Templates"
}
```

SMTP (when `Provider` = `smtp`):

```json
"Notifications": {
  "Provider": "smtp",
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": 587,
    "UseSsl": true,
    "FromAddress": "noreply@example.com",
    "Username": "",
    "Password": ""
  }
}
```

## Verify

1. Register user → welcome email logged to console/Seq
2. Provision tenant → welcome log (owner email lookup Phase 2)
3. `GET /api/v1/notifications/health` as Admin

## Troubleshooting

| Issue | Check |
|-------|-------|
| Template not found | `Templates/*.txt` copied to output directory |
| No email on event | Outbox processor running; bridge handlers registered |
| SMTP failure | Credentials, firewall, `Notifications:Smtp` |
