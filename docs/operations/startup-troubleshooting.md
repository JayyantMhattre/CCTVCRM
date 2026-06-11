# Startup Troubleshooting

## API won't start

| Error | Fix |
|-------|-----|
| Connection refused postgres | `docker compose up -d postgres`; wait for healthy |
| Mongo connection failed | Start mongodb; check `ConnectionStrings:MongoDB` |
| Redis connection failed | Start redis |
| Port in use | Change Kestrel port or stop conflicting process |
| Fatal during DI | Read stack trace — missing connection string in config |

Check bootstrap logs in console before Serilog reconfiguration.

---

## Database schema missing

Repository has **no EF migrations**. Symptoms:

- relation "auth.users" does not exist

**Fix:**

1. Ensure `init-db.sql` ran (Postgres first boot)
2. Add and apply EF migrations per module, or
3. Run manual DDL matching EF configurations

---

## Health ready fails

```bash
curl http://localhost:5000/health/ready
```

| Check | Name in response |
|-------|------------------|
| PostgreSQL | postgres |
| Redis | redis |
| MongoDB | mongodb |

Disable Audit module → remove Mongo health check registration.

---

## OpenIddict / token errors after restart

**Ephemeral signing keys** — all issued tokens invalid after API restart. Users must log in again.

Planned fix: wire `Auth__SigningKeyBase64` from configuration.

---

## Frontend can't reach API

| Issue | Fix |
|-------|-----|
| CORS | Use Vite dev server (proxy) not raw file open |
| Wrong base URL | Check `VITE_API_BASE_URL` |
| API on 5000, calling 8080 | Match environment to process |

---

## Docker API on 8080 vs local 5000

| Run mode | URL |
|----------|-----|
| `dotnet run` | :5000 |
| `docker compose` API service | :8080 |

---

## Related

- [errors/common-failures.md](../errors/common-failures.md)
- [deployment-notes.md](./deployment-notes.md)
