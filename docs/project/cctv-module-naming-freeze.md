# CCTV Module Naming Freeze

**Project:** Aarvii CCTV AMC Management System  
**Status:** **FROZEN** — Sprint 0 approval condition (2026-06-11)  
**Change process:** Approved change request only ([requirements-freeze-v1.md](./requirements-freeze-v1.md) §22)

All CCTV implementation **must** use the names below. Do not introduce alternate prefixes, schemas, or folder layouts.

---

## 1. C# projects and namespaces

| Layer | Pattern | Example (Lead) |
|-------|---------|----------------|
| Domain | `Ashraak.Cctv.{Module}.Domain` | `Ashraak.Cctv.Lead.Domain` |
| Application | `Ashraak.Cctv.{Module}.Application` | `Ashraak.Cctv.Lead.Application` |
| Infrastructure | `Ashraak.Cctv.{Module}.Infrastructure` | `Ashraak.Cctv.Lead.Infrastructure` |
| Api | `Ashraak.Cctv.{Module}.Api` | `Ashraak.Cctv.Lead.Api` |

**Frozen `{Module}` values:** `Lead` · `Customer` · `Amc` · `Service` · `Ticket` · `Engineer` · `Invoice` · `Reporting`

**Integration (cross-cutting):**

| Project | Namespace |
|---------|-----------|
| `Ashraak.Cctv.Integration.Application` | SMS/PDF abstractions, RBAC constants |
| `Ashraak.Cctv.Integration.Infrastructure` | Stubs, RBAC seed, `AddCctvModules` |
| `Ashraak.Cctv.Api` | Aggregated `/cctv` endpoints + health |

**Physical path:** `BackEnd/src/Modules/Cctv/{Module}/Ashraak.Cctv.{Module}.{Layer}/`

---

## 2. PostgreSQL schemas

| Module | Schema | Frozen |
|--------|--------|:------:|
| Lead | `cctv_lead` | ✅ |
| Customer / Site / Asset | `cctv_customer` | ✅ |
| AMC | `cctv_amc` | ✅ |
| Service / Visits | `cctv_service` | ✅ |
| Ticket | `cctv_ticket` | ✅ |
| Engineer | `cctv_engineer` | ✅ |
| Invoice | `cctv_invoice` | ✅ |
| Reporting | *(none — read-only)* | ✅ |

Migration history table: `__ef_migrations_history` inside each schema.

---

## 3. HTTP API surface

| Item | Frozen value |
|------|--------------|
| Version prefix | `/api/v1` (host versioned group) |
| Business prefix | `/cctv` |
| Health | `GET /api/v1/cctv/health` |
| Route groups | `/cctv/leads` · `/customers` · `/amc` · `/service` · `/tickets` · `/engineers` · `/invoices` · `/reports` |

OpenAPI tag pattern: `CCTV — {Module}` (e.g. `CCTV — Lead`).

---

## 4. SharedKernel contracts

| Item | Frozen value |
|------|--------------|
| Namespace | `Ashraak.SharedKernel.Contracts.CctvCrm` |
| Cross-module interfaces | `Contracts/CctvCrm/` |
| Feature flags | `CctvFeatureFlags` |
| Permissions | `Ashraak.Cctv.Integration.Application.Rbac.CctvPermissions` |

---

## 5. Documentation paths

| Module | Docs folder |
|--------|-------------|
| Lead | `docs/modules/cctv-lead/` |
| Customer | `docs/modules/cctv-customer/` |
| AMC | `docs/modules/cctv-amc/` |
| Service | `docs/modules/cctv-service/` |
| Ticket | `docs/modules/cctv-ticket/` |
| Engineer | `docs/modules/cctv-engineer/` |
| Invoice | `docs/modules/cctv-invoice/` |
| Reporting | `docs/modules/cctv-reporting/` |
| Integration | `docs/modules/cctv-integration/` |

Each folder maintains the platform **7-file** layout per [documentation-governance.md](../documentation-governance.md).

---

## 6. Frontend

| Item | Pattern | Example |
|------|---------|---------|
| Feature module | `FrontEnd/apps/web/src/modules/cctv/` | — |
| Routes | `ROUTES.cctv.admin.*` · `portal.*` · `engineer.*` | `ROUTES.cctv.admin.leads` |
| Admin URL prefix | `/admin/...` | `/admin/leads` |
| Customer portal | `/portal/...` | `/portal/dashboard` |
| Engineer portal | `/engineer/...` | `/engineer/visits` |
| UI imports | `@/platform-ui` only | No theme adapter imports |

---

## 7. RBAC permission prefix

Pattern: `{resource}:{action}` — see [permission-catalog.md](./design/permission-catalog.md).  
Examples: `leads:read` · `visits:approve` · `amc:request-renewal`

**Do not rename** permission strings after Sprint 0 without change control.

---

## 8. Feature flags

Pattern: `cctv.{area}.enabled` — see [CctvFeatureFlags.cs](../BackEnd/src/Shared/Ashraak.SharedKernel.Contracts/CctvCrm/CctvFeatureFlags.cs) and [cctvFeatureFlags.ts](../FrontEnd/apps/web/src/modules/cctv/featureFlags/cctvFeatureFlags.ts).

---

## 9. Explicit prohibitions

- ❌ `CctvCrm.*` project names (use `Ashraak.Cctv.*`)
- ❌ New schemas outside the seven listed above for V1
- ❌ Routes outside `/api/v1/cctv/*` for business APIs
- ❌ Duplicate Auth, Files, Notifications, or Audit modules
- ❌ Renaming frozen schemas or permission strings without CR

---

Related: [cctv-module-map.md](./cctv-module-map.md) · [ADR-CCTV-0003](./adr/ADR-CCTV-0003-module-naming-freeze.md)
