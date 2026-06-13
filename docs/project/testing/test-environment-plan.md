# Test Environment Plan — TP-1

**Project:** Aarvii CCTV AMC Management System  
**Date:** 2026-06-12  
**Phase:** TP-1 — Environment preparation (no deployment in this phase)

---

## 1. Environment overview

| Environment | Purpose | TP phase | Deploy trigger |
|-------------|---------|----------|----------------|
| **Local** | Developer + QA ad-hoc | TP-2, TP-3 | Manual |
| **QA / Dev** | CI integration, automated tests | TP-2 | Merge to `main` |
| **Staging** | Manual smoke, integration, UAT prep | TP-3+ | Release candidate tag |

Platform and CCTV share **Ashraak.Api** host — single deploy unit per environment ([release-plan.md](../roadmap/release-plan.md)).

---

## 2. Local environment

### 2.1 Prerequisites

| Component | Version / notes |
|-----------|-----------------|
| .NET SDK | Per `BackEnd/global.json` (.NET 10) |
| Node.js | **20+** (Vite build requires modern JS; CF-1 noted Node `||=` issue on older agents) |
| PostgreSQL | 15+ recommended |
| Redis | Local `localhost:6379` |
| MongoDB | Local audit store `mongodb://localhost:27017/ashraak_audit` |
| Flutter SDK | Stable channel (mobile tests) |
| Docker (optional) | Postgres/Redis/Mongo compose if used |

### 2.2 Local services (from appsettings)

| Service | Default connection |
|---------|-------------------|
| PostgreSQL | `Host=localhost;Port=5432;Database=ashraak;Username=ashraak;Password=ashraak_dev` |
| Redis | `localhost:6379` |
| MongoDB | `mongodb://localhost:27017/ashraak_audit` |
| Seq (optional) | `http://localhost:5341` |
| OTEL (optional) | `http://localhost:4317` |

**Schema search paths:** Platform schemas (`auth`, `tenant`, `users`, `files`, …) + CCTV schemas (`cctv_lead`, `cctv_customer`, `cctv_amc`, `cctv_service`, `cctv_ticket`, `cctv_engineer`, `cctv_invoice`).

### 2.3 Local run sequence (TP-2 verification)

1. Start PostgreSQL, Redis, MongoDB  
2. Apply EF migrations (all modules) or restore from backup  
3. Start API: `dotnet run --project BackEnd/src/Host/Ashraak.Api`  
4. Start web: `cd FrontEnd/apps/web && npm run dev`  
5. Mobile: point `EnvironmentConfig` dev URL to host machine IP / emulator bridge  

### 2.4 Local limitations

- Email: `Notifications:Provider = console` — OTP codes in logs  
- SMS: stub/log per ADR-CCTV-0001  
- Push: FCM requires device + credentials; deep-link parser testable without push send  
- Files: local disk storage via Files module  

---

## 3. QA environment

### 3.1 Purpose

- **CI automated test execution** (TP-2): GitHub Actions `ci.yml` — restore, build, test on `ubuntu-latest`  
- Integration deploy on merge to `main` (per release plan)  
- Not primary manual QA environment — use Staging for structured smoke  

### 3.2 CI pipeline (existing)

| Job | Steps |
|-----|-------|
| `package-audit` | Central package version verify |
| `backend` | `dotnet restore` → `build Release` → `dotnet test` → TRX upload |
| `validate-docs` | Doc validation script |

**TP-2 action:** Confirm CI green on **frozen branch/tag**; archive TRX artifacts.

### 3.3 QA environment gaps (TP-2 setup)

| Item | Action | Owner |
|------|--------|-------|
| Dedicated QA URL documented | Record API + SPA URLs in runbook | DevOps |
| QA connection strings | Secrets in vault — not in repo | DevOps |
| Feature flags | Match production-like toggles for CCTV modules | DevOps |
| Test user accounts | Admin, Engineer, Customer seeded | QA + DevOps |

---

## 4. Staging environment

### 4.1 Purpose

- Manual smoke ([manual-smoke-checklist.md](./manual-smoke-checklist.md)) — TP-3  
- Database restore + migrate verification — **freeze condition C-04**  
- Realistic data volume for reports  
- UAT preparation (post TP-5 — out of TP-1 scope)  

### 4.2 Staging requirements

| Requirement | Detail |
|-------------|--------|
| API | Ashraak.Api deployed with CCTV modules enabled |
| Web SPA | Static host or CDN for `FrontEnd/apps/web` build |
| PostgreSQL | Dedicated staging DB; **no production data** |
| Redis / Mongo | Staging instances |
| Files storage | Writable path or blob — sufficient for video upload smoke (100 MB limit) |
| Email | Console or test SMTP — OTP retrievable for password reset smoke |
| TLS | HTTPS for staging URLs |
| Mobile | Staging API base URL in QA/UAT flavor |

### 4.3 Staging verification checklist (TP-2 — C-04)

| Step | Pass criteria |
|------|---------------|
| Restore latest backup to staging DB | Restore completes without error |
| Run pending migrations | All modules at head revision |
| API health endpoint | 200 OK |
| RBAC seed | CCTV permissions + roles present (`Cctv:RbacSeed:Enabled`) |
| Web loads | Login page reachable |
| Smoke login | Admin, Customer, Engineer tokens issued |

**Owner:** DevOps  
**Evidence:** Screenshot/log + migration version table archived.

---

## 5. Database requirements

### 5.1 Schemas (ownership)

| Schema | Module | Migration project |
|--------|--------|-------------------|
| `auth`, `tenant`, `users` | Platform | Auth, Tenant, Users Infrastructure |
| `files`, `notifications`, … | Platform | Respective Infrastructure |
| `cctv_lead` | Lead | `Ashraak.Cctv.Lead.Infrastructure` |
| `cctv_customer` | Customer | `Ashraak.Cctv.Customer.Infrastructure` |
| `cctv_amc` | AMC | `Ashraak.Cctv.Amc.Infrastructure` |
| `cctv_service` | Service | `Ashraak.Cctv.Service.Infrastructure` |
| `cctv_ticket` | Ticket | `Ashraak.Cctv.Ticket.Infrastructure` |
| `cctv_engineer` | Engineer | `Ashraak.Cctv.Engineer.Infrastructure` |
| `cctv_invoice` | Invoice | `Ashraak.Cctv.Invoice.Infrastructure` |

### 5.2 TP-2 database tasks

| Task | Environment | Notes |
|------|-------------|-------|
| Migration idempotency check | Local + Staging | No Wave 4 schema changes expected |
| Backup/restore drill | Staging | C-04 |
| Clean DB integration test | Local (optional Testcontainers) | TP-3 extension |

### 5.3 Data retention (testing)

- Staging refreshed from anonymized seed or synthetic data — see [test-data-strategy.md](./test-data-strategy.md)  
- No PII from production  
- Files bucket cleared between major regression cycles (optional)

---

## 6. Seed data requirements

### 6.1 Automatic seeds (runtime)

| Seed | Source | When |
|------|--------|------|
| CCTV RBAC permissions + roles | `CctvRbacSeedHostedService` | API startup (`Cctv:RbacSeed:Enabled=true`) |
| Webhook event catalog | `WebhookEventDefinitionSeeder` | API startup |
| Auth baseline roles | Auth Infrastructure | First run |

### 6.2 Manual / scripted test data (TP-2/TP-3)

Required personas and entities — full matrix in [test-data-strategy.md](./test-data-strategy.md):

| Persona | Role | Needed for |
|---------|------|------------|
| `admin@test.local` | Admin | All admin flows, reports |
| `engineer@test.local` | Engineer | Visit, ticket create |
| `customer@test.local` | Customer | Portal, tickets, invoices |
| Tenant ID | GUID | Auth OTP flows |

**TP-2 deliverable:** Document or script `scripts/seed-test-data` (creation in TP-2 if missing — test infra only, not product feature).

### 6.3 Minimum entity graph for smoke

One complete chain:

```
Lead → Convert → Customer → Site → AMC Contract/Term → Schedule → Visit → Approve
                                                              ↓
                                                          Ticket → Close
                                                              ↓
                                                          Invoice (Draft → Generated → Sent → Paid)
```

---

## 7. External dependencies by environment

| Dependency | Local | QA/CI | Staging |
|------------|-------|-------|---------|
| PostgreSQL | Required | CI uses in-memory/mock for most tests | Required |
| Redis | Required (cache, OTP) | Optional in CI | Required |
| MongoDB | Optional (audit) | Mock OK | Required |
| SMTP | Console | N/A | Test SMTP |
| SMS gateway | Stub | N/A | Stub or test gateway |
| FCM | N/A | N/A | Optional — V1.1 partial |

---

## 8. Environment readiness gates

| Gate | Local | QA/CI | Staging | TP phase |
|------|:-----:|:-----:|:-------:|----------|
| API builds | ✅ | TP-2 | TP-2 | TP-2 |
| Migrations current | ✅ | N/A | TP-2 C-04 | TP-2 |
| Seed personas exist | TP-2 | TP-2 | TP-3 | TP-2/3 |
| Web points to API | ✅ | TP-2 | TP-3 | TP-2 |
| Mobile points to API | TP-2 | N/A | TP-3 | TP-2 |

---

## 9. References

- [test-data-strategy.md](./test-data-strategy.md)
- [test-execution-plan.md](./test-execution-plan.md)
- [test-readiness-review.md](./test-readiness-review.md)
- [release-plan.md](../roadmap/release-plan.md)
- `BackEnd/src/Host/Ashraak.Api/appsettings.json`

---

*TP-1 — Environment plan only. No environments provisioned or modified in this phase.*
