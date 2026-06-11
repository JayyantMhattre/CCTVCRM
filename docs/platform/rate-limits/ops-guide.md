# Rate limiting — Operations

## Verify

1. Enable Redis and start API.
2. Exceed limit on `POST /connect/token` — expect `429` and `Retry-After` header.
3. Search Seq: `Rate limit exceeded` (includes route key, tenant, IP).

## Load balancers

Preserve client IP via `X-Forwarded-For` — ensure forwarded headers middleware is enabled in production if the API sits behind a proxy (future enhancement).

## Monitoring

- Spike in 429 responses on auth routes
- Redis latency affecting limit checks (falls back to cache miss = new window)

## Rollout

1. Deploy with high limits.
2. Observe 429 rate in logs.
3. Tighten `Limit` / `WindowSeconds` per environment.
