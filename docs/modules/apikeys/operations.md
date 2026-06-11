# API Keys operations

## Health

- `/health/ready` includes `apikeys` platform check
- `GET /api/v1/api-keys/health` — module liveness

## Configuration (`appsettings.json`)

```json
"ApiKeys": {
  "Environment": "prod",
  "DefaultExpiryDays": 0,
  "AllowHeaderAuthentication": true,
  "AllowBearerAuthentication": true,
  "PerKeyRateLimit": 1000,
  "PerKeyRateLimitWindowSeconds": 60
}
```

## Database

Schema `apikeys` created in `BackEnd/scripts/init-db.sql`. Apply EF migrations when introduced.

## Troubleshooting

| Symptom | Check |
|---------|-------|
| 401 with API key | Key revoked, expired, or wrong environment prefix |
| 403 on management | User lacks `apikeys:read` / `apikeys:manage` |
| Rate limited | Per-key or route policy in `RateLimiting` section |
