# Documentation Gap Analysis

**Audit date:** May 2026  
**Scope:** Full Ashraak monorepo (BackEnd + FrontEnd)  
**Source of truth:** Live codebase — not existing markdown assumptions.

---

## Executive summary

The Ashraak template has **strong module-level status documents** scattered under `BackEnd/src/Modules/*/` and two FrontEnd guides, but **no centralized documentation hub**, **no root README**, and **no standardized module documentation layout**. Several architecture comments and legacy docs **overstate implemented features** (outbox processor, RabbitMQ integration, persistent JWT signing).

**Frontend stack:** React 19 + Vite 6 (not Angular). All new documentation reflects this.

**Backend runtime:** .NET 10 (`net10.0`), EF Core 9.0.4 for PostgreSQL.

---

## Coverage matrix

| Area | Prior docs | Code implemented | Standard `/docs` coverage | Gap severity |
|------|------------|------------------|-------------------------|--------------|
| SharedKernel | Partial (inline XML) | Yes | Backfilled | Medium |
| SharedKernel.Contracts | Partial | Yes | Backfilled | Medium |
| BuildingBlocks (Application) | None | Yes | Backfilled | High |
| BuildingBlocks (EventBus) | Comments only | Stub (`InProcessEventBus`) | Backfilled | High |
| BuildingBlocks (Outbox) | Comments + base class | Write path only; **no processor** | Backfilled | Critical |
| BuildingBlocks (Data layer) | `DATA_LAYER.md` | Libraries unused by modules | Backfilled | Medium |
| Host / Program.cs | `API_COMPOSITION_ROOT.md` | Yes | Backfilled | Low |
| Auth module | `AUTH_MODULE_STATUS.md` | Yes | Backfilled | Low |
| Tenant module | `TENANT_MODULE_STATUS.md` | Yes | Backfilled | Low |
| Users module | `USERS_MODULE_STATUS.md` | Yes | Backfilled | Low |
| Audit module | `AUDIT_MODULE_STATUS.md` | Yes (read API stub) | Backfilled | Low |
| Caching module | `CACHING_MODULE_STATUS.md` | Yes | Backfilled | Low |
| Docker / ops | `DOCKER_ENVIRONMENT.md` | Yes | Consolidated under `/docs/operations` | Medium |
| Developer onboarding | `DEVELOPER_GUIDE.md` (root) | Yes | Linked + split getting-started | Medium |
| API reference | Scalar/OpenAPI only | Yes | Backfilled `/docs/api` | High |
| Frontend shell | `FRONTEND_STARTER.md` | Yes | Backfilled `/docs/frontend` | Medium |
| ADRs | None | N/A | Created | High |
| Documentation governance | None | N/A | Created | Critical |
| Root README | **Missing** | N/A | Created | Critical |

---

## Undocumented modules / components

### Backend — fully undocumented before this phase

- `Ashraak.BuildingBlocks.Application` — MediatR behaviors (`ValidationBehavior`, `LoggingBehavior`, `PerformanceBehavior`) exist but are **not registered globally**
- `Ashraak.BuildingBlocks.EventBus` — `IEventBus` / `InProcessEventBus` (log-only stub, not in DI)
- `Ashraak.BuildingBlocks.Infrastructure` — `BaseDbContext`, `OutboxProcessorBase` (abstract, no concrete host job)
- `Ashraak.Infrastructure.Shared` — `AddSharedInfrastructure()` defined, **never called from host**
- Cross-cutting middleware beyond Audit (e.g. `GlobalExceptionHandler`, `TenantResolutionMiddleware`) — no dedicated ops doc
- Contract events with **no publishers/handlers** (`TenantProvisionedEvent`, `UserCreatedEvent`, `ITokenService`)

### Frontend — gaps

- Centralized routing architecture (routes live in `core/router/index.tsx`, not per-module `routes.tsx`)
- Environment variable contract (`.env.production` missing)
- Known UI inconsistencies (audit role vs permission, profile nav link)
- SSO / tenant provision endpoints (defined in API layer, no UI)

---

## Partially documented modules

| Module | Existing doc | What was missing |
|--------|--------------|------------------|
| Auth | `AUTH_MODULE_STATUS.md` | API contract tables, event catalog, operations (ephemeral signing keys), middleware bypass path accuracy |
| Tenant | `TENANT_MODULE_STATUS.md` | Outbox/event gap, no delete API, `TenantAdmin` policy unused |
| Users | `USERS_MODULE_STATUS.md` | Synchronous `UserRegisteredEvent` vs documented outbox path |
| Audit | `AUDIT_MODULE_STATUS.md` | Stub read endpoint, middleware path version (`/api/v1/...`) |
| Caching | `CACHING_MODULE_STATUS.md` | Consumer map, rate-limit key builder (unused) |
| Host | `API_COMPOSITION_ROOT.md` | OpenAPI JWT scheme deferred; OTLP env not bound |

---

## Documentation vs code discrepancies (corrected in new docs)

| Topic | Old assumption | Code reality |
|-------|----------------|--------------|
| Outbox | Events auto-persisted and processed by Quartz | DbContexts do **not** inherit `BaseDbContext`; **no** `OutboxProcessor` hosted service |
| RabbitMQ | MassTransit event bus | Container only; **no** MassTransit in API |
| JWT signing | `JWT_SIGNING_KEY_BASE64` env var | OpenIddict uses **ephemeral** signing keys |
| Output cache | Redis-backed | In-memory `AddOutputCache` only |
| EF migrations | `dotnet ef database update` in guides | **No** `Migrations/` folders in repo |
| Frontend | Bootstrap 5 primary (starter table) | **CoreUI 5** SCSS primary |
| Token storage | Memory only (starter security table) | **sessionStorage** via `authStore` |
| SSO callback | Full account linking | Returns claims stub; local exchange Phase 2 |
| Audit GET | Full Mongo query | **Stub** JSON response |
| .NET version | Mixed references | Host **.NET 10**, EF **9.0.4** |

---

## Missing onboarding artifacts (now addressed)

- Root `README.md` for architects and new developers
- `/docs/index.md` master navigation hub
- Step-by-step getting-started (backend, frontend, docker, env vars)
- Extension guide aligned to existing Billing example in `DEVELOPER_GUIDE.md`
- Operations runbook (Seq, Redis, startup, deployment)
- Error catalog tied to ProblemDetails and auth error codes
- ADR set for major architectural decisions

---

## Recommended next actions (post-audit)

1. **Adopt** `/docs/documentation-governance.md` for all future features.
2. **Deprecate** scattered `*_MODULE_STATUS.md` paths gradually — link from module READMEs to `/docs/modules/{name}/`.
3. **Phase 2 code** (not this phase): wire outbox processor, fix doc/code gaps intentionally.
4. **Fix** frontend doc drift in `FRONTEND_STARTER.md` (Bootstrap → CoreUI, token storage) — optional follow-up PR.

---

## Audit method

1. Inventory all `.md` files in repository.
2. Map solution projects via `Ashraak.slnx` (35 projects).
3. Read `Program.cs`, `ModuleExtensions.cs`, all endpoint classes, module `*Module.cs` registration files.
4. Read FrontEnd `package.json`, router, auth, API layer.
5. Cross-check agent exploration reports against primary sources.
6. Backfill `/docs/**` to match implementation.
