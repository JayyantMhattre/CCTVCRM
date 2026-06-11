# Redis Troubleshooting

## Symptoms

| Symptom | Likely cause |
|---------|--------------|
| `/health/ready` fails on redis | Container down, wrong password, wrong host |
| Slow logins | Redis unreachable — permission cache miss every time |
| Session cache miss | Redis flush or key TTL expired |

---

## Verify Redis

```bash
docker exec -it ashraak-redis redis-cli ping
# PONG
```

With password:

```bash
redis-cli -a yourpassword ping
```

---

## Connection string

`ConnectionStrings:Redis` — format:

```
host:port,password=...,ssl=False,abortConnect=False
```

Docker compose sets `ConnectionStrings__Redis` for API container.

---

## Key conventions

See `CacheKeyBuilder` in Caching module:

- Permissions: tenant + user scoped
- Tenant config: `tenant:{id}:config`
- Sessions: `session:{tenantId}:{userId}`

---

## Clear cache (dev)

```bash
docker exec -it ashraak-redis redis-cli FLUSHDB
```

Invalidates permission and session caches — users may need to re-login.

---

## Related

- [modules/caching/operations.md](../modules/caching/operations.md)
- [startup-troubleshooting.md](./startup-troubleshooting.md)
