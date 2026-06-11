# Rate limiting — Troubleshooting

## No 429 when expected

- Check `RateLimiting:Enabled` is `true`
- Confirm request path matches `Path` prefix exactly
- Verify HTTP method is listed (or `Methods` is empty)
- Health/openapi/scalar paths are bypassed

## All requests return 429

- Redis clock skew (rare)
- Limit set too low
- Shared NAT IP — many users share one counter

## Redis unavailable

`ICacheService` operations may fail; check application logs and Redis health (`GET /health/ready`).

## Legitimate traffic blocked

- Raise `Limit` for the route group
- Add tenant-specific gateway throttling
- Whitelist internal IPs at reverse proxy (not in middleware today)
