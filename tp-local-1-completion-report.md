# TP-LOCAL-1 Completion Report

**Ticket:** TP-LOCAL-1 — Local Integrated Test Environment  
**Project:** Aarvii CCTV AMC Management System (Ashraak)  
**Date:** 2026-06-13  
**Phase:** Environment setup only (code freeze respected)  
**Status:** **COMPLETE** — artifacts delivered; runtime validation pending local Docker install

---

## 1. Executive summary

A reproducible Docker-based local test environment has been created at the repository root, integrating:

| Component | Technology | Container |
|-----------|------------|-----------|
| Database | PostgreSQL 17 | `postgres` |
| Backend API | .NET 10 (`Ashraak.Api`) | `ashraak-api` |
| Web application | React 19 + Vite 6, Node 20 | `ashraak-web` |
| Cache | Redis 7 | `redis` |
| Audit store | MongoDB 7 | `mongodb` |
| Logging | Seq | `seq` |

**No business logic was modified.** Infrastructure and documentation only.

**Smoke tests were NOT executed** per ticket scope.

---

## 2. Deliverables checklist

| Deliverable | Path | Status |
|-------------|------|--------|
| Root `docker-compose.yml` | `/docker-compose.yml` | ✅ Created |
| Root `docker-compose.override.yml` | `/docker-compose.override.yml` | ✅ Created |
| Root `.env.example` | `/.env.example` | ✅ Created |
| API Dockerfile (curl fix) | `/BackEnd/Dockerfile` | ✅ Updated |
| Web Dockerfile | `/FrontEnd/Dockerfile` | ✅ Created |
| Web nginx config | `/FrontEnd/nginx.conf` | ✅ Created |
| Web `.dockerignore` | `/FrontEnd/.dockerignore` | ✅ Created |
| Smoke seed SQL | `/scripts/test-data/seed-smoke-chain.sql` | ✅ Created |
| Migration script (PS) | `/scripts/database/apply-migrations.ps1` | ✅ Created |
| Migration script (sh) | `/scripts/database/apply-migrations.sh` | ✅ Created |
| Seed runner (PS) | `/scripts/database/run-seed.ps1` | ✅ Created |
| Local test environment doc | `/docs/project/testing/local-test-environment.md` | ✅ Created |
| Docker setup guide | `/docs/project/testing/docker-setup-guide.md` | ✅ Created |
| Smoke test data guide | `/docs/project/testing/smoke-test-data-guide.md` | ✅ Created |
| Completion report | `/tp-local-1-completion-report.md` | ✅ This file |

---

## 3. Docker architecture

```
┌─────────────────────────────────────────────────────────────────┐
│  Host Machine                                                    │
│                                                                  │
│  Browser ──► localhost:3000 ──► ashraak-web (Vite/nginx)      │
│                      │ proxy /api, /connect                      │
│                      ▼                                           │
│              ashraak-api :8080                                   │
│                 │    │    │                                      │
│                 ▼    ▼    ▼                                      │
│            postgres redis mongodb                                │
│            :5432   :6379  :27017                                 │
│                                                                  │
│  Optional: seq :5341, rabbitmq :5672/:15672                       │
└─────────────────────────────────────────────────────────────────┘

Docker network: ashraak-net (bridge)
```

### Compose structure

- **Root** `docker-compose.yml` includes `BackEnd/docker-compose.yml` via Compose `include` directive
- **Root** `docker-compose.override.yml` includes `BackEnd/docker-compose.override.yml` and adds web dev overrides
- Backend service definitions are not duplicated — single source of truth in `BackEnd/`

### Web container modes

| Mode | Build target | Port | Use case |
|------|--------------|------|----------|
| Development | `development` | 3000 | Vite HMR, bind-mounted source |
| Production | `production` | 80 | nginx static SPA |

---

## 4. Ports

| Service | Container | Host (default) | Env variable |
|---------|-----------|----------------|--------------|
| Web SPA | `ashraak-web` | 3000 | `WEB_PORT` |
| API | `ashraak-api` | 8080 | `API_PORT` |
| PostgreSQL | `postgres` | 5432 | `POSTGRES_PORT` |
| Redis | `redis` | 6379 | `REDIS_PORT` |
| MongoDB | `mongodb` | 27017 | `MONGO_PORT` |
| Seq UI | `seq` | 5341 | `SEQ_PORT` |
| RabbitMQ AMQP | `rabbitmq` | 5672 | `RABBITMQ_PORT` |
| RabbitMQ Mgmt | `rabbitmq` | 15672 | `RABBITMQ_MGMT_PORT` |

### Health endpoints

| Endpoint | Purpose |
|----------|---------|
| `GET http://localhost:8080/health/live` | API liveness |
| `GET http://localhost:8080/health/ready` | API readiness (all deps) |
| `GET http://localhost:8080/health` | Full health report |
| `GET http://localhost:3000/` | Web availability |

---

## 5. Startup instructions

### First-time setup

```powershell
cd C:\Jayant_Git_Local\CCTVCRM
Copy-Item .env.example .env
docker compose up -d --build
docker compose ps
```

### Database initialization

1. **Automatic (first boot):** `BackEnd/scripts/init-db.sql` creates schemas and bootstrap tables
2. **Manual:** Apply EF Core migrations:

```powershell
.\scripts\database\apply-migrations.ps1
```

3. **Optional seed:**

```powershell
.\scripts\database\run-seed.ps1
```

### Verify stack

```powershell
curl http://localhost:8080/health/live
curl http://localhost:8080/health/ready
curl http://localhost:3000/
```

### Access points

| URL | Description |
|-----|-------------|
| http://localhost:3000 | Web SPA |
| http://localhost:8080/scalar/v1 | API documentation (Development) |
| http://localhost:5341 | Seq log viewer |
| http://localhost:15672 | RabbitMQ management (ashraak / ashraak_dev) |

---

## 6. Seed data summary

Script: `scripts/test-data/seed-smoke-chain.sql`

| Entity | Count | Example ID |
|--------|-------|------------|
| Lead | 2 | LEAD-00001 (New), LEAD-00002 (Qualified) |
| Customer | 1 | CUST-00001 |
| Site | 1 | SITE-00001 |
| AMC Plan + Version | 1 + 1 | PLAN-PREM |
| AMC Contract + Term | 1 + 1 | AMC-00001 |
| Engineer | 1 | ENG-00001 |
| Service Schedule | 1 | SCHED-00001 |
| Visit | 1 | Draft visit |
| Ticket | 1 | TKT-00001 (Open) |
| Invoice | 3 | Draft, Sent, Paid |

Full GUID reference: [smoke-test-data-guide.md](docs/project/testing/smoke-test-data-guide.md)

---

## 7. Validation results

| Check | Result | Notes |
|-------|--------|-------|
| `docker compose config` | ⚠️ Not run | Docker CLI not available in build agent environment |
| `docker compose up` | ⚠️ Not run | Requires Docker Desktop on developer machine |
| API health endpoint | ⚠️ Not run | Pending local verification |
| Web startup | ⚠️ Not run | Pending local verification |
| PostgreSQL volume | ✅ Configured | `postgres_data` named volume |
| Health checks defined | ✅ Yes | API, postgres, redis, mongodb, web |
| Compose include paths | ✅ Valid | Relative to `BackEnd/` per Compose spec |

**Action required:** Run validation on a machine with Docker Desktop installed using the startup instructions in Section 5.

---

## 8. Known limitations

| # | Limitation | Impact | Mitigation |
|---|------------|--------|------------|
| 1 | **PostgreSQL, not SQL Server** | Ticket spec mentions SQL Server; project is Npgsql/PostgreSQL throughout | Documented; architecture change prohibited by code freeze |
| 2 | **React/Vite, not Angular** | Ticket spec mentions Angular; actual frontend is React 19 + Vite 6 | Node 20 containerization delivered; Angular not applicable |
| 3 | **No auto-migrate** | API does not apply EF migrations on startup | Use `scripts/database/apply-migrations.ps1` |
| 4 | **Auth users not seeded** | Seed SQL covers business entities only | OTP registration via Auth module; personas documented |
| 5 | **Platform module migrations** | Auth/Tenant/Users have no EF migration folders in repo | `init-db.sql` bootstrap tables |
| 6 | **RabbitMQ unused** | Container runs; API event bus not wired | No action needed for local testing |
| 7 | **Mobile excluded** | Flutter app not containerized | Per TP-LOCAL-1 scope |
| 8 | **First build time** | .NET + Node builds are slow | `--build` only when code changes |
| 9 | **pnpm lockfile absent** | `pnpm install` resolves latest compatible versions in Docker | Acceptable for dev; pin lockfile in future if needed |

---

## 9. Testing readiness

| Gate | Ready? | Evidence |
|------|--------|----------|
| Docker compose files at repo root | ✅ | `docker-compose.yml`, `docker-compose.override.yml` |
| Environment template | ✅ | `.env.example` |
| API containerized | ✅ | `BackEnd/Dockerfile` (pre-existing, curl added) |
| Web containerized | ✅ | `FrontEnd/Dockerfile` |
| DB init script wired | ✅ | `init-db.sql` volume mount in BackEnd compose |
| Migration tooling | ✅ | `scripts/database/apply-migrations.ps1` |
| Smoke seed data | ✅ | `scripts/test-data/seed-smoke-chain.sql` |
| Documentation | ✅ | 3 guides under `docs/project/testing/` |
| Networking (Web→API→DB) | ✅ | Container DNS + Vite proxy |
| Health checks | ✅ | All critical services |
| Business logic unchanged | ✅ | No `.cs` or frontend source modified |
| Smoke tests executed | ❌ Intentionally skipped | Per ticket STOP directive |

**Verdict:** Environment is **ready for manual testing** once Docker is available locally. Proceed to TP-2/TP-3 smoke execution.

---

## 10. Files changed (summary)

### New files

- `docker-compose.yml`
- `docker-compose.override.yml`
- `.env.example`
- `FrontEnd/Dockerfile`
- `FrontEnd/nginx.conf`
- `FrontEnd/.dockerignore`
- `scripts/test-data/seed-smoke-chain.sql`
- `scripts/database/apply-migrations.ps1`
- `scripts/database/apply-migrations.sh`
- `scripts/database/run-seed.ps1`
- `docs/project/testing/local-test-environment.md`
- `docs/project/testing/docker-setup-guide.md`
- `docs/project/testing/smoke-test-data-guide.md`
- `tp-local-1-completion-report.md`

### Modified files

- `BackEnd/Dockerfile` — added `curl` for healthcheck reliability

### Unchanged (reused)

- `BackEnd/docker-compose.yml`
- `BackEnd/docker-compose.override.yml`
- `BackEnd/scripts/init-db.sql`

---

## 11. References

- [local-test-environment.md](docs/project/testing/local-test-environment.md)
- [docker-setup-guide.md](docs/project/testing/docker-setup-guide.md)
- [smoke-test-data-guide.md](docs/project/testing/smoke-test-data-guide.md)
- [test-environment-plan.md](docs/project/testing/test-environment-plan.md)
- [BackEnd/DOCKER_ENVIRONMENT.md](BackEnd/DOCKER_ENVIRONMENT.md)

---

*TP-LOCAL-1 — Environment setup complete. No smoke tests executed. No business logic modified.*
