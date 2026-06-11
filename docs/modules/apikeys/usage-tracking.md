# API key usage tracking

## Per-key metrics (persisted)

| Field | Description |
|-------|-------------|
| `last_used_on_utc` | Last authenticated request |
| `request_count` | Total requests |
| `success_count` | HTTP &lt; 400 |
| `failure_count` | HTTP ≥ 400 |
| `last_correlation_id` | Latest `X-Correlation-Id` |

Updated by `ApiKeyUsageMiddleware` after each API-key-authenticated request.

## Observability (OpenTelemetry)

Meter: `Ashraak.ApiKeys`

- `apikeys.requests`
- `apikeys.failures`
- `apikeys.created`
- `apikeys.revoked`

## Admin visibility

Web Operations Center and mobile read-only views show usage summary on list and detail pages.
