# Rate limiting

Redis-backed, host-level rate limiting protects abuse-prone routes (auth token, registration) with per-tenant, per-route-group, and per-IP isolation.

**Implementation:** `BackEnd/src/Host/Ashraak.Api/Middleware/RateLimitingMiddleware.cs`

**Storage:** `ICacheService` via `CacheKeyBuilder.ForRateLimit(tenantId, routeGroup, clientIp)`

## Behaviour

| HTTP status | When |
|-------------|------|
| 429 | Limit exceeded |
| (pass) | Under limit or route not configured |

Response includes `Retry-After` (seconds) and JSON problem body.

## Middleware order

Runs after correlation + Serilog request logging, **before** authentication (tenant resolved from `X-Tenant-ID` or JWT when present).

## Related docs

- [configuration.md](./configuration.md)
- [tuning.md](./tuning.md)
- [ops-guide.md](./ops-guide.md)
- [troubleshooting.md](./troubleshooting.md)
- [ADR-0008](../../adr/ADR-0008-host-platform-hardening.md)
