# Docker Setup Guide — TP-LOCAL-1

**Project:** Aarvii CCTV AMC Management System  
**Audience:** Developers and QA setting up the local integrated test environment

---

## 1. Overview

The integrated Docker environment runs from the **repository root** using:

| File | Purpose |
|------|---------|
| `docker-compose.yml` | Base stack: API, PostgreSQL, Redis, MongoDB, Seq, Web |
| `docker-compose.override.yml` | Development overrides (auto-loaded) |
| `.env.example` | Environment variable template |

Backend service definitions are **included** from `BackEnd/docker-compose.yml` to avoid duplication.

---

## 2. First-time setup

### 2.1 Install Docker

- **Windows:** Docker Desktop 4.x+ with WSL2 backend recommended
- **macOS:** Docker Desktop
- **Linux:** Docker Engine + Compose plugin

Verify:

```powershell
docker --version
docker compose version
```

### 2.2 Configure environment

```powershell
cd C:\Jayant_Git_Local\CCTVCRM
Copy-Item .env.example .env
```

The `.env.example` defaults work for local development with `docker-compose.override.yml`. No changes required for a standard setup.

**Required variable:** `JWT_SIGNING_KEY_BASE64` — a dev-only key is pre-filled. Generate a new one for non-local use:

```powershell
# PowerShell — 32 random bytes, base64-encoded
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))
```

### 2.3 Build and start

```powershell
docker compose up -d --build
```

First build compiles the .NET API and installs npm packages for the web app. Expect 5–15 minutes on first run.

### 2.4 Check container health

```powershell
docker compose ps
```

All services should show `healthy` (API, postgres, redis, mongodb, web) or `running` (seq, rabbitmq).

```powershell
docker compose logs ashraak-api --tail 50
docker compose logs ashraak-web --tail 20
```

---

## 3. Service reference

### 3.1 ashraak-api

| Property | Value |
|----------|-------|
| Image | Built from `BackEnd/Dockerfile` |
| Build | Multi-stage (.NET SDK 10 → aspnet 10 runtime) |
| Internal port | 8080 |
| Host port | `API_PORT` (default 8080) |
| Health | `GET /health/live`, `GET /health/ready` |
| API docs | `http://localhost:8080/scalar/v1` (Development) |

**Connection strings** are injected via environment variables using ASP.NET Core's `__` convention (e.g. `ConnectionStrings__DefaultConnection`).

### 3.2 postgres

| Property | Value |
|----------|-------|
| Image | `postgres:17-alpine` |
| Volume | `postgres_data` (persistent) |
| Init script | `BackEnd/scripts/init-db.sql` (first boot only) |
| Credentials | `ashraak` / `ashraak_dev` (dev override) |

Connect from host:

```powershell
docker compose exec postgres psql -U ashraak -d ashraak
```

### 3.3 ashraak-web

| Property | Value |
|----------|-------|
| Image | Built from `FrontEnd/Dockerfile` |
| Node version | 20 (Alpine) |
| Dev mode | Vite dev server, port 3000 |
| Prod mode | nginx serves `dist/`, port 80 |

**Networking:** The Vite dev server proxies `/api` and `/connect` to `http://ashraak-api:8080` inside the Docker network. The browser only talks to `http://localhost:3000`.

### 3.4 Supporting services

| Service | Required by API | Notes |
|---------|-----------------|-------|
| redis | Yes | Cache, OTP, sessions |
| mongodb | Yes | Audit module |
| seq | No | Structured logs — `http://localhost:5341` |
| rabbitmq | No | Future event bus — management UI `:15672` |

---

## 4. Database setup

### 4.1 Initialization (automatic)

On **first** PostgreSQL container start (empty volume), `init-db.sql` creates:

- Platform schemas: `auth`, `tenant`, `users`, `files`, `webhooks`, `apikeys`
- CCTV schemas: `cctv_lead`, `cctv_customer`, `cctv_amc`, `cctv_service`, `cctv_ticket`, `cctv_engineer`, `cctv_invoice`
- Bootstrap tables (outbox, invitations, files metadata, etc.)

Subsequent restarts skip the init script.

### 4.2 Migrations (manual)

EF Core migrations for CCTV modules are **not** applied automatically:

```powershell
.\scripts\database\apply-migrations.ps1
```

Requires .NET SDK 10 installed on the host.

### 4.3 Seed data (manual)

```powershell
.\scripts\database\run-seed.ps1
```

See [smoke-test-data-guide.md](./smoke-test-data-guide.md).

### 4.4 Database restore

To restore from a `.sql` or `.dump` backup:

```powershell
# Stop API to prevent connections
docker compose stop ashraak-api

# Drop and recreate (destructive)
docker compose exec postgres psql -U ashraak -d postgres -c "DROP DATABASE IF EXISTS ashraak;"
docker compose exec postgres psql -U ashraak -d postgres -c "CREATE DATABASE ashraak;"

# Restore plain SQL
Get-Content .\backup\ashraak-backup.sql | docker compose exec -T postgres psql -U ashraak -d ashraak

# Restart API
docker compose start ashraak-api
```

### 4.5 Reset database (clean slate)

```powershell
docker compose down
docker volume rm cctvcrm_postgres_data
docker compose up -d postgres
# Wait for healthy, then re-run migrations and seed
.\scripts\database\apply-migrations.ps1
.\scripts\database\run-seed.ps1
```

---

## 5. Development workflows

### 5.1 API only (backend compose)

```powershell
cd BackEnd
docker compose up -d
```

Runs infrastructure + API without the web container. Useful when running `pnpm dev` on the host.

### 5.2 Web on host, API in Docker

```powershell
cd BackEnd
docker compose up -d
cd ..\FrontEnd\apps\web
# Set VITE_API_BASE_URL=http://localhost:8080 in .env.development
pnpm dev
```

### 5.3 Full stack in Docker (recommended for TP-LOCAL-1)

```powershell
docker compose up -d
```

### 5.4 Rebuild after code changes

```powershell
# API only
docker compose up -d --build ashraak-api

# Web only (dev mode uses bind-mounts — usually no rebuild needed)
docker compose up -d --build ashraak-web

# Everything
docker compose up -d --build
```

---

## 6. Production web build

```powershell
$env:WEB_BUILD_TARGET = "production"
$env:WEB_CONTAINER_PORT = "80"
docker compose up -d --build ashraak-web
```

Set build-time API URL for browser access:

```powershell
$env:VITE_API_BASE_URL = "http://localhost:8080"
```

---

## 7. Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| API `unhealthy` | Migrations not applied, or dependency down | Check `docker compose logs ashraak-api`; apply migrations |
| API won't start | Missing `JWT_SIGNING_KEY_BASE64` | Ensure `.env` exists with valid base64 key |
| Web blank page | API not reachable from proxy | Verify `ashraak-api` is healthy |
| Port conflict | Local service on 5432/8080/3000 | Change ports in `.env` |
| Postgres init skipped | Volume already exists | `docker volume rm cctvcrm_postgres_data` |
| `curl` healthcheck fails | Old API image without curl | `docker compose build --no-cache ashraak-api` |

### View logs

```powershell
docker compose logs -f ashraak-api
docker compose logs -f ashraak-web
docker compose logs -f postgres
```

### Shell into containers

```powershell
docker compose exec ashraak-api sh
docker compose exec ashraak-web sh
docker compose exec postgres psql -U ashraak -d ashraak
```

---

## 8. Security notes

- `.env` is git-ignored — never commit credentials
- Dev passwords (`ashraak_dev`) are for local use only
- `docker-compose.override.yml` must not be used in production
- For production deployment, see `BackEnd/docker-compose.prod.yml`

---

## 9. Related documentation

- [local-test-environment.md](./local-test-environment.md)
- [smoke-test-data-guide.md](./smoke-test-data-guide.md)
- [BackEnd/DOCKER_ENVIRONMENT.md](../../BackEnd/DOCKER_ENVIRONMENT.md)
- [docs/getting-started/docker-setup.md](../../getting-started/docker-setup.md)
