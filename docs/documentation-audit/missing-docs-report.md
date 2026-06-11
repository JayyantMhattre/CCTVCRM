# Missing Documentation Report

**Generated:** Documentation audit phase — May 2026  
**Purpose:** Inventory what did not exist before `/docs` backfill.

---

## Critical — blocking onboarding

| Item | Location in codebase | Was missing |
|------|----------------------|-------------|
| Root README | Repository root | Entire file |
| Documentation hub | `/docs/index.md` | Entire file |
| Documentation governance | `/docs/documentation-governance.md` | Entire file |
| Architecture overview set | `/docs/architecture/*` | Entire directory |
| ADR directory | `/docs/adr/*` | Entire directory |
| Getting-started guides | `/docs/getting-started/*` | Entire directory |
| API documentation (human-readable) | `/docs/api/*` | Entire directory |
| Extension guide | `/docs/extending/*` | Entire directory |
| Operations runbook | `/docs/operations/*` | Entire directory |
| Error catalog | `/docs/errors/*` | Entire directory |

---

## High — module documentation (standard layout)

Each module below lacked the required seven-file layout under `/docs/modules/{name}/`:

| Module | Code path | Required files added |
|--------|-----------|----------------------|
| SharedKernel | `BackEnd/src/Shared/` | README, architecture, registration, api, events, extending, operations |
| Auth | `BackEnd/src/Modules/Auth/` | All 7 |
| Tenant | `BackEnd/src/Modules/Tenant/` | All 7 |
| Users | `BackEnd/src/Modules/Users/` | All 7 |
| Audit | `BackEnd/src/Modules/Audit/` | All 7 |
| Caching | `BackEnd/src/Modules/Caching/` | All 7 |
| Host | `BackEnd/src/Host/Ashraak.Api/` | All 7 |
| BuildingBlocks | `BackEnd/src/BuildingBlocks/` | All 7 (covers EventBus, Outbox, Data) |

---

## High — frontend documentation

| Topic | Code evidence | Was missing |
|-------|---------------|-------------|
| Frontend architecture | `FrontEnd/apps/web/src/` | `/docs/frontend/architecture.md` |
| Routing & guards | `core/router/`, `shared/guards/` | `/docs/frontend/routing-and-guards.md` |
| Auth & session | `core/auth/` | `/docs/frontend/auth.md` |
| API client layer | `core/api/` | `/docs/frontend/api-layer.md` |
| Feature modules map | `src/modules/*` | `/docs/frontend/feature-modules.md` |
| Environment config | `.env.example`, `.env.development` | `/docs/frontend/environment.md` |

---

## Medium — cross-cutting backend topics

| Topic | Evidence | Was missing |
|-------|----------|-------------|
| Eventing (in-process MediatR) | `DomainEventPublisher`, contract events | Dedicated architecture + module events docs |
| Outbox (scaffolding) | `OutboxProcessorBase`, `OutboxMessage` | Honest outbox doc (scaffold vs runtime) |
| Middleware pipeline | `Program.cs` order | Consolidated host operations doc |
| Redis integration | `CachingModule`, health check | Operations troubleshooting |
| Seq logging | `Program.cs` Serilog | Operations doc |
| OpenTelemetry | `Program.cs` OTLP | Architecture + operations |
| SharedKernel.Contracts catalog | `SharedKernel.Contracts/**` | Contracts section in architecture + per-module events |

---

## Medium — API documentation

| Gap | Detail |
|-----|--------|
| Endpoint catalog | Only OpenAPI/Scalar; no human narrative with examples |
| Auth flows | Password grant form fields not documented centrally |
| Error responses | OAuth-style errors vs ProblemDetails mixed |
| Stub endpoints | Audit GET stub not called out in API index |

---

## Low — optional artifacts (not required but noted)

| Item | Status |
|------|--------|
| Per-module `sequence-diagram.md` | Optional — added only for Audit (complex flow) |
| Per-module `troubleshooting.md` | Optional — added for Auth, Audit, Host |
| FrontEnd root README | Still missing — covered by `/docs/getting-started/frontend-setup.md` |
| BackEnd root README | Still missing — covered by root README + getting-started |

---

## Legacy docs retained (not deleted)

These files remain at original paths; `/docs` links to them where useful:

- `DEVELOPER_GUIDE.md` — comprehensive rename + new module tutorial
- `BackEnd/DOCKER_ENVIRONMENT.md` — detailed Docker reference
- `BackEnd/src/BuildingBlocks/DATA_LAYER.md` — data abstraction design
- `BackEnd/src/Host/Ashraak.Api/API_COMPOSITION_ROOT.md` — host wiring
- `BackEnd/src/Modules/*/*_MODULE_STATUS.md` — implementation status snapshots
- `FrontEnd/FRONTEND_STARTER.md` — React starter (some tables stale)
- `FrontEnd/COREUI_INTEGRATION.md` — UI kit integration

**Action:** Module READMEs in `/docs/modules/` are the canonical entry points; legacy status files are supplementary.

---

## Count summary

| Category | Files created in backfill |
|----------|---------------------------|
| Documentation audit | 4 |
| Index + governance | 2 |
| Architecture | 9 |
| ADR | 6 |
| Getting started | 6 |
| Module docs (8 modules × 7 files) | 56 |
| Frontend docs | 6 |
| API docs | 4 |
| Extending | 4 |
| Operations | 7 |
| Errors | 3 |
| Root README | 1 |
| **Approximate total** | **~107** |
