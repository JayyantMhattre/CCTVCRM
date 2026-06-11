# Rate limiting — Tuning

## Dimensions

Counters are keyed by:

1. **Tenant** — JWT `tenant_id` / `tenantId` or `X-Tenant-ID`; `00000000-0000-0000-0000-000000000000` when absent (e.g. token endpoint)
2. **Route group** — configuration key (e.g. `auth/token`)
3. **Client IP** — `HttpContext.Connection.RemoteIpAddress`

## Guidelines

| Route | Suggested starting point |
|-------|-------------------------|
| `auth/token` | 20/min per IP (brute-force protection) |
| `auth/register` | 10 per 5 min per tenant+IP |

Increase limits behind API gateways that already throttle edge traffic.

## Redis memory

Each key holds a small JSON counter with TTL = remaining window. Keys use prefix `ratelimit:` (no environment prefix — shared across instances by design).

## Disable locally

```json
"RateLimiting": { "Enabled": false }
```
