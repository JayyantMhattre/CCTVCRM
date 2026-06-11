# Ashraak — Docker Environment Guide

Production-ready container infrastructure for the Ashraak modular monolith SaaS starter.

> **Canonical ops docs:** [docs/operations/](../docs/operations/) and [docs/getting-started/docker-setup.md](../docs/getting-started/docker-setup.md).  
> This file is the detailed Docker reference; if it conflicts with audited `/docs`, prefer **`/docs`** after verification.

---

## Note on Database: PostgreSQL vs SQL Server

The project uses **PostgreSQL** (via Npgsql) throughout. All module DbContexts, EF Core migrations, health checks, and connection string formats are PostgreSQL-specific. Migrating to SQL Server would require changes in every module's Infrastructure project. This decision is intentional and consistent with previous architectural choices.

---

## Services

| Service | Image | Default Port | Purpose |
|---|---|---|---|
| `ashraak-api` | Built locally | 8080 | .NET 10 modular monolith API host |
| `postgres` | `postgres:17-alpine` | 5432 | Auth, Tenant, Users module data |
| `redis` | `redis:7-alpine` | 6379 | Caching module (permissions, sessions) |
| `mongodb` | `mongo:7` | 27017 | Audit module (tamper-evident log) |
| `rabbitmq` | `rabbitmq:3.13-management-alpine` | 5672 / 15672 | **Infrastructure only** — API not wired (future MassTransit) |
| `seq` | `datalust/seq:latest` | 5341 | Serilog structured log viewer |

---

## File Structure

```
BackEnd/
├── Dockerfile                    # Multi-stage .NET 10 build
├── .dockerignore                 # Excludes bin/, obj/, tests/, secrets from build context
├── docker-compose.yml            # Base: all services + env-var secrets
├── docker-compose.override.yml   # Dev overrides (auto-loaded, simple passwords)
├── docker-compose.prod.yml       # Production hardening (no host ports, resource limits)
├── .env.example                  # Secrets template — commit this
├── .env                          # Actual secrets — GIT-IGNORED, never commit
├── .gitignore
└── scripts/
    └── init-db.sql               # Creates PostgreSQL schemas on first startup
```

---

## Quick Start (Development)

```bash
# 1. Copy the secrets template
cp .env.example .env

# 2. For local dev, copy override passwords from docker-compose.override.yml into .env
#    (.env.example uses CHANGE_ME_* placeholders — override file sets ashraak_dev)

# 3. Start all services (docker-compose.override.yml is auto-applied)
docker compose up -d

# 4. Verify all containers are healthy
docker compose ps

# 5. Database schema
#    PostgreSQL schemas (auth, tenant, users) are created by scripts/init-db.sql on first boot.
#    This repo may not include EF migration folders yet — when added, run:
#    dotnet ef database update --project src/Modules/Auth/Ashraak.Auth.Infrastructure --startup-project src/Host/Ashraak.Api
#    (repeat per module)

# 6. Open the API reference (Docker port 8080)
open http://localhost:8080/scalar/v1

# 7. Open Seq log viewer
open http://localhost:5341

# 8. (Optional) RabbitMQ management — not used by API today
open http://localhost:15672   # user: ashraak / pass: ashraak_dev
```

---

## Production Deployment

### Checklist before deploying

- [ ] All `← REQUIRED` fields in `.env` are filled with strong values
- [ ] `POSTGRES_PASSWORD` is at least 20 characters, randomly generated
- [ ] `MONGO_PASSWORD` is at least 20 characters
- [ ] `REDIS_PASSWORD` is set (empty = no authentication = insecure)
- [ ] `JWT_SIGNING_KEY_BASE64` planned for persistent OpenIddict keys (**not read by code today** — tokens use ephemeral keys until wired)
- [ ] `SEQ_ADMIN_PASSWORD_HASH` is set
- [ ] SSO credentials (`GOOGLE_CLIENT_ID` / `MICROSOFT_CLIENT_ID`) are configured
- [ ] TLS is terminated at the reverse proxy (nginx / Traefik / cloud load balancer)
- [ ] Database host ports are NOT exposed (docker-compose.prod.yml closes them)

### Deploy command

```bash
# Export secrets from your CI/CD secrets store instead of a .env file in prod.
# If using a .env file for an interim prod setup:
docker compose \
  -f docker-compose.yml \
  -f docker-compose.prod.yml \
  --env-file .env.production \
  up -d
```

### Generate a JWT signing key

```bash
# 256-bit random key, base64-encoded — use the output as JWT_SIGNING_KEY_BASE64
openssl rand -base64 32
```

### Generate a Seq admin password hash

```bash
docker run --rm datalust/seq config hash --password="YourStrongPassword"
```

---

## Secrets Strategy

### Development (`.env` file)

```
BackEnd/
└── .env          ← gitignored; contains weak/memorable dev passwords
```

Simple and convenient. Never leave dev passwords in the `.env` file on a shared machine.

### CI/CD (GitHub Actions example)

```yaml
# .github/workflows/deploy.yml
- name: Deploy
  env:
    POSTGRES_PASSWORD: ${{ secrets.POSTGRES_PASSWORD }}
    MONGO_PASSWORD:    ${{ secrets.MONGO_PASSWORD }}
    JWT_SIGNING_KEY_BASE64: ${{ secrets.JWT_SIGNING_KEY_BASE64 }}
  run: |
    docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

Store all `← REQUIRED` variables as GitHub/GitLab secrets or in your cloud provider's secrets manager.

### Production (Docker Swarm / Kubernetes)

**Docker Swarm** — use `docker secret create` and mount as `/run/secrets/<name>`.
Modify `docker-compose.yml` to read `POSTGRES_PASSWORD` from `cat /run/secrets/postgres_password`.

**Kubernetes** — use `kubectl create secret generic ashraak-secrets` and inject as env vars or volume mounts via `secretKeyRef`.

---

## Environment Variable Reference

All variables used by `docker-compose.yml`. Defaults shown where applicable.

### API

| Variable | Default | Description |
|---|---|---|
| `API_PORT` | `8080` | Host port for the API |
| `API_VERSION` | `latest` | Image tag |
| `ASPNETCORE_ENVIRONMENT` | `Production` | ASP.NET Core env |

### PostgreSQL

| Variable | Default | Description |
|---|---|---|
| `POSTGRES_USER` | `ashraak` | DB username |
| `POSTGRES_PASSWORD` | *(none)* | **Required** — DB password |
| `POSTGRES_DB` | `ashraak` | DB name |
| `POSTGRES_HOST` | `postgres` | Service hostname (Docker network) |
| `POSTGRES_PORT` | `5432` | Host port |

### Redis

| Variable | Default | Description |
|---|---|---|
| `REDIS_HOST` | `redis` | Service hostname |
| `REDIS_PORT` | `6379` | Host port |
| `REDIS_PASSWORD` | *(empty)* | **Required in prod** |

### MongoDB

| Variable | Default | Description |
|---|---|---|
| `MONGO_HOST` | `mongodb` | Service hostname |
| `MONGO_PORT` | `27017` | Host port |
| `MONGO_USER` | `ashraak` | Root username |
| `MONGO_PASSWORD` | *(none)* | **Required** |
| `MONGO_DB` | `ashraak_audit` | Audit database name |

### RabbitMQ

| Variable | Default | Description |
|---|---|---|
| `RABBITMQ_USER` | `ashraak` | Username |
| `RABBITMQ_PASSWORD` | *(none)* | **Required** |
| `RABBITMQ_PORT` | `5672` | AMQP port |
| `RABBITMQ_MGMT_PORT` | `15672` | Management UI port |

### Observability

| Variable | Default | Description |
|---|---|---|
| `SEQ_HOST` | `seq` | Seq service hostname |
| `SEQ_PORT` | `5341` | Seq UI host port |
| `SEQ_ADMIN_PASSWORD_HASH` | *(empty)* | **Required in prod** |

### Auth / SSO

| Variable | Default | Description |
|---|---|---|
| `JWT_SIGNING_KEY_BASE64` | *(none)* | Documented for prod — **not consumed by AuthModule yet** (ephemeral signing keys) |
| `GOOGLE_CLIENT_ID` | *(empty)* | Google OAuth client ID |
| `GOOGLE_CLIENT_SECRET` | *(empty)* | Google OAuth secret |
| `MICROSOFT_CLIENT_ID` | *(empty)* | Microsoft Entra client ID |
| `MICROSOFT_CLIENT_SECRET` | *(empty)* | Microsoft Entra secret |

---

## Configuration Priority (ASP.NET Core)

The app merges configuration from these sources (highest priority first):

```
1. Environment variables          (set by Docker / CI / cloud)
   ConnectionStrings__Auth=...   maps to  ConnectionStrings:Auth  JSON key
   Auth__SigningKeyBase64=...     maps to  Auth:SigningKeyBase64

2. appsettings.Production.json   (minimal — only logging, no secrets)

3. appsettings.Development.json  (loaded only when ASPNETCORE_ENVIRONMENT=Development)

4. appsettings.json              (base defaults — dev connection strings)
```

This means you can always override any JSON value in production by setting the equivalent environment variable (using `__` as the section separator).

---

## Dockerfile — Build Stages

```
mcr.microsoft.com/dotnet/sdk:10.0          (build stage — ~900 MB)
        │
        │  dotnet restore  (cached unless .csproj files change)
        │  dotnet build
        │  dotnet publish
        ▼
mcr.microsoft.com/dotnet/aspnet:10.0       (final stage — ~250 MB)
        │
        │  COPY --from=publish /app/publish .
        │  USER appuser  (non-root)
        ▼
    ashraak/api:latest
```

The multi-stage build ensures the final image contains only the runtime, not the SDK or source code.

---

## Health Checks

All services define Docker health checks. The API container starts only after PostgreSQL, Redis, and MongoDB report healthy.

| Endpoint | Purpose |
|---|---|
| `GET /health/live` | Liveness — is the process running? |
| `GET /health/ready` | Readiness — are all dependencies reachable? |

---

## Volumes (Persistent Data)

| Volume | Service | Contents |
|---|---|---|
| `postgres_data` | postgres | All module relational data |
| `redis_data` | redis | Cache (AOF persistence) |
| `mongo_data` | mongodb | Audit log entries |
| `rabbitmq_data` | rabbitmq | Message queue data |
| `seq_data` | seq | Structured log storage |

Back up these volumes before upgrades or when migrating to a managed database service.
