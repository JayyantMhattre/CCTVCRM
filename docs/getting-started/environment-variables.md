# Environment Variables

Source: `BackEnd/.env.example`, `docker-compose.yml`, `appsettings.json`.

**Rule:** If a variable is listed in `.env.example` but not read in code, it is marked **(not wired)**.

---

## API host

| Variable | Example | Read by | Purpose |
|----------|---------|---------|---------|
| `ASPNETCORE_ENVIRONMENT` | Development | Host | Logging, OpenAPI |
| `API_PORT` | 8080 | Docker | Container port mapping |
| `ConnectionStrings__DefaultConnection` | Postgres | Health, fallback | DB |
| `ConnectionStrings__Auth` | Postgres + `Search Path=auth` | AuthModule | Auth schema |
| `ConnectionStrings__Tenant` | Postgres + `Search Path=tenant` | TenantModule | Tenant schema |
| `ConnectionStrings__Users` | Postgres + `Search Path=users` | UsersModule | Users schema |
| `ConnectionStrings__Redis` | localhost:6379 | CachingModule | Cache |
| `ConnectionStrings__MongoDB` | mongodb://... | AuditModule | Audit DB |
| `Seq__Url` | http://seq:5341 | Program.cs Serilog | Log sink |
| `OpenTelemetry__Endpoint` | http://... | **(not wired)** | Intended OTLP |
| `Auth__SigningKeyBase64` | base64 | **(not wired)** | OpenIddict uses ephemeral keys |
| `GOOGLE_CLIENT_ID` / `SECRET` | — | AuthModule | SSO |
| `MICROSOFT_CLIENT_ID` / `SECRET` | — | AuthModule | SSO |

## PostgreSQL

| Variable | Default |
|----------|---------|
| `POSTGRES_HOST` | postgres |
| `POSTGRES_PORT` | 5432 |
| `POSTGRES_DB` | ashraak |
| `POSTGRES_USER` | ashraak |
| `POSTGRES_PASSWORD` | required |

## Redis

| Variable | Default |
|----------|---------|
| `REDIS_HOST` | redis |
| `REDIS_PORT` | 6379 |
| `REDIS_PASSWORD` | optional |

## MongoDB

| Variable | Default |
|----------|---------|
| `MONGO_HOST` | mongodb |
| `MONGO_PORT` | 27017 |
| `MONGO_DB` | ashraak_audit |
| `MONGO_USER` / `MONGO_PASSWORD` | required in prod |

## RabbitMQ

| Variable | Note |
|----------|------|
| `RABBITMQ_*` | Container only — **API not connected** |

## Frontend (Vite)

| Variable | File | Purpose |
|----------|------|---------|
| `VITE_API_BASE_URL` | `.env.development` | API origin |
| `VITE_API_VERSION` | `.env.development` | `v1` |
| `VITE_APP_NAME` | `.env.development` | UI title |

---

## Related

- [DOCKER_ENVIRONMENT.md](../../BackEnd/DOCKER_ENVIRONMENT.md)
- [operations/deployment-notes.md](../operations/deployment-notes.md)
