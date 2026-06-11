# Outdated Documentation Report

**Rule applied:** Code is source of truth. This report lists existing docs that contained stale or misleading content. New `/docs/**` files correct these; legacy files are flagged for future sync.

---

## Severity legend

- **P0** — Misleads setup or security
- **P1** — Misleads architecture understanding
- **P2** — Minor drift / cosmetic

---

## P0 — Security & operations

| File | Stale content | Actual behavior |
|------|---------------|-----------------|
| `BackEnd/.env.example` + `DOCKER_ENVIRONMENT.md` | `JWT_SIGNING_KEY_BASE64` required for production | `AuthModule` uses `AddEphemeralSigningKey()` — env var **not read**; tokens invalid after API restart |
| `DOCKER_ENVIRONMENT.md` | Dev `.env` has safe defaults | `.env.example` uses `CHANGE_ME_*` placeholders |
| `DEVELOPER_GUIDE.md` §2 step 4 | `dotnet ef database update` for Auth/Tenant/Users | **No EF migration folders** exist in repository |

---

## P1 — Architecture & integration

| File | Stale content | Actual behavior |
|------|---------------|-----------------|
| `DEVELOPER_GUIDE.md`, module status docs | Outbox guarantees cross-module delivery | `OutboxProcessorBase` is abstract; **no hosted processor**; DbContexts don't inherit `BaseDbContext` |
| `DOCKER_ENVIRONMENT.md` | RabbitMQ used by API via MassTransit | RabbitMQ container runs; API has **no** MassTransit package or `IEventBus` registration |
| `API_COMPOSITION_ROOT.md` | OpenAPI JWT Bearer security scheme configured | `OpenApiExtensions.cs` **defers** security scheme to Scalar manual auth |
| `Program.cs` comments | Output cache is Redis-backed | Only `AddOutputCache()` in-memory policy |
| `Program.cs` comments | OTLP reads `OTEL_EXPORTER_OTLP_ENDPOINT` | Hardcoded `AddOtlpExporter()` defaults; compose env **not bound** |
| `AUTH_MODULE_STATUS.md` | Middleware pipeline ends before audit | `UseAuditApiCallLogging()` runs **before** `UseOutputCache` |
| `init-db.sql` comment | Auto-migrate in Program.cs | **No** migrate code in `Program.cs` |
| `Tenant` event docs | `TenantDeletedEvent` via outbox | Handler listens to contract event; domain raises `TenantDeletedDomainEvent`; **no delete API** |

---

## P1 — API paths & middleware

| File | Stale content | Actual behavior |
|------|---------------|-----------------|
| `AUDIT_MODULE_STATUS.md`, middleware comments | Exclude `/api/audit-logs` | Actual route: `/api/v1/audit-logs` |
| `TenantResolutionMiddleware` bypass list | `/api/auth/register` | Actual route: `/api/v1/auth/register` |
| `DOCKER_ENVIRONMENT.md` | Scalar at `/scalar/v1` vs `/scalar` | Mapped via `MapOpenApiDocs` — verify port 5000 (Kestrel) vs 8080 (Docker) |

---

## P2 — Frontend

| File | Stale content | Actual behavior |
|------|---------------|-----------------|
| `FRONTEND_STARTER.md` stack table | Bootstrap 5 + react-bootstrap primary | **CoreUI 5** SCSS (`coreui.scss`); `bootstrap` npm package removed |
| `FRONTEND_STARTER.md` security | Access token memory-only | Full session in **sessionStorage** (`ashraak_session`) |
| `FRONTEND_STARTER.md` structure | `packages/ui` workspace package | **`packages/` directory does not exist** |
| `core/router/index.tsx` comment | Per-module `routes.tsx` | Routes **centralized** in `index.tsx` only |
| `DEVELOPER_GUIDE.md` | React 19 + Vite 6 | **Correct** — user prompt mentioning Angular 20 is **external**; not in repo |

---

## P2 — Package / version references

| File | Stale content | Actual behavior |
|------|---------------|-----------------|
| Various comments | ".NET 8" (external prompts) | `Directory.Build.props`: **net10.0**, SDK 10.0.103 |
| NuGet alignment | All packages on 10.x | EF Core / Npgsql on **9.0.4** while host is 10.0.0 |

---

## Contract & event documentation drift

| Contract event | Documented handler expectation | Code reality |
|----------------|-------------------------------|--------------|
| `TenantProvisionedEvent` | Notification module welcome email | **No publisher** |
| `UserCreatedEvent` | Welcome email | **No publisher** |
| `UserInvitedEvent` | Invitation email | **No publisher** |
| `ITokenService` | Token revocation service | **No implementation** |

---

## Remediation status

| Action | Status |
|--------|--------|
| New `/docs/**` reflects code truth | Done (documentation foundation phase) |
| Update legacy `*_MODULE_STATUS.md` in place | Done (drift cleanup phase) — canonical links + key fixes |
| Update `FRONTEND_STARTER.md` | Done — CoreUI, sessionStorage, routing |
| Update `DEVELOPER_GUIDE.md` migration section | Done — init-db.sql + optional EF |
| Update `DOCKER_ENVIRONMENT.md` | Done — RabbitMQ, JWT, Scalar, migrations |
| Update `API_COMPOSITION_ROOT.md` | Done — OpenAPI JWT, register path |
| CI doc validation | Done — `scripts/validate-docs.*`, GitHub workflow |
| Fix code to match docs (outbox, JWT) | **Out of scope** — future implementation phase |

---

## Verification checklist for doc maintainers

When changing code, re-verify these high-churn areas:

- [ ] `ModuleExtensions.cs` registration order
- [ ] `Program.cs` middleware order
- [ ] Endpoint route prefixes (`/api/v1/...` vs unversioned `/connect/...`)
- [ ] Audit capture paths (middleware exclusions)
- [ ] Environment variables actually read in `*Module.cs` / `Program.cs`
- [ ] Frontend `ENDPOINTS` in `endpoints.ts` vs backend routes
- [ ] Stub vs implemented endpoints (Audit GET)
