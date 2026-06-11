# Caching — Operations

## Prerequisites

Redis is **mandatory** — app fails at startup if `ConnectionStrings:Redis` is missing.

## Configuration

| Key | Local example | Docker |
|-----|---------------|--------|
| `ConnectionStrings:Redis` | `localhost:6379` | `redis:6379,password=...,abortConnect=false,...` |
| `ASPNETCORE_ENVIRONMENT` | `Development` | `Production` |

Env var: `ConnectionStrings__Redis`

Production secrets via environment — not in `appsettings.Production.json`.

## Docker Redis

From `BackEnd/docker-compose.yml`:

- Image: `redis:7-alpine`
- AOF persistence, optional password
- `maxmemory 256mb`, policy `allkeys-lru`

## Health check

```powershell
curl http://localhost:5000/health/ready
```

Redis failure causes ready check to fail.

## Verify connectivity

```powershell
redis-cli -h localhost ping
# PONG

redis-cli KEYS "development:*"
```

Keys prefixed with lowercased environment name.

## Active cache keys (by module)

| Pattern | Owner | TTL |
|---------|-------|-----|
| `{env}:{tenant}:auth:perms:{userId}` | Auth | 10 min |
| `{env}:{tenant}:session:{userId}` | Auth | 8 hours |
| `{env}:{tenant}:tenant:config:{tenantId}` | Tenant | 5 min |
| `{env}:{tenant}:tenant:features` | Tenant | 5 min |

## Manual invalidation

```powershell
# Single permission cache
redis-cli DEL "development:{tenant-id}:auth:perms:{user-id}"

# All keys for tenant module prefix
redis-cli KEYS "development:{tenant-id}:tenant:*"
```

Or use `ICacheInvalidationService` from a diagnostic admin endpoint (not built yet).

## Troubleshooting

| Issue | Cause | Action |
|-------|-------|--------|
| Startup exception | Missing Redis config | Set `ConnectionStrings:Redis` |
| Stale permissions | 10 min TTL | Wait or DELETE key |
| Stale tenant config | 5 min TTL | Wait or DELETE key |
| Session not found | Expired (8h) or restart cleared L1 only | Re-login; L2 should persist |
| Ready check fails | Redis down | Start redis container |
| Connection timeout | Network/firewall | Check docker network, `abortConnect=false` |
| Memory pressure | Large cached objects | Review Redis `maxmemory` + LRU |

## L1 vs L2 on multi-instance

L1 is per-process — after deploy, each instance rebuilds L1 from L2. Expect brief L2 load spike.

Single instance dev: L1 serves most reads after first hit.

## Host output cache

Separate Redis usage for HTTP output cache (30s). Keys managed by ASP.NET — do not confuse with `ICacheService` keys.

## Performance notes

- `RemoveByPrefixAsync` uses `KEYS`/`SCAN` — expensive on large keyspaces; use sparingly
- JSON serialization — large DTOs increase memory and network
- Singleton services — thread-safe; `IMemoryCache` is process-local

## Related

- [Auth operations](../auth/operations.md) — session and permission TTLs
- [Tenant operations](../tenant/operations.md) — config cache
- [Host operations](../host/operations.md) — output cache, health checks

## Build

```powershell
dotnet build BackEnd/src/Modules/Caching/Ashraak.Caching.Redis/Ashraak.Caching.Redis.csproj
```

No standalone deployable — ships with `Ashraak.Api`.
