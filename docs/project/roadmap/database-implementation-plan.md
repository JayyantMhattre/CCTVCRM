# Database Implementation Plan

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-8
**Strategy:** PostgreSQL · single DB · schema-per-module ([database-architecture.md](../design/database-architecture.md))

---

## 1. Schema rollout order

Roll out in **dependency order** — each schema migrated before dependent modules consume logical refs.

| Order | Schema | Module phase | Tables (primary) |
|:-----:|--------|--------------|------------------|
| 1 | `cctv_lead` | B1 | leads, lead_activities, lead_remarks, lead_attachments |
| 2 | `cctv_customer` | B2 | customers, sites, site_contacts, site_documents, site_asset_summaries |
| 3 | `cctv_amc` | B3 | amc_plans, amc_plan_versions, amc_contracts, amc_contract_terms, amc_contract_documents |
| 4 | `cctv_engineer` | B5* | engineers |
| 5 | `cctv_service` | B4 | service_schedules, engineer_assignments, service_visits, visit_* |
| 6 | `cctv_ticket` | B5 | tickets, ticket_* |
| 7 | `cctv_invoice` | B6 | invoices, invoice_* |

\* `cctv_engineer` before `cctv_service` if assignments need engineer FK within service schema (logical ref only cross-schema — engineer schema can ship in B4 with stub or parallel B5 start).

**Platform schemas (`auth`, `files`, …):** **no migrations** — frozen.

---

## 2. Migration strategy

| Rule | Detail |
|------|--------|
| Tool | EF Core migrations per module DbContext |
| History table | `__ef_migrations_history` **inside each schema** (platform pattern) |
| Naming | `{timestamp}_{description}` |
| One context | One schema per DbContext — no shared context across modules |
| Cross-schema | **No physical FKs** — UUID logical refs only |
| Roll forward | Apply migrations in CI against ephemeral DB; then dev/staging/prod |
| Breaking changes | New migration only — never edit applied migrations |

### CI pipeline addition

```yaml
# Pseudocode — extend ci.yml
- dotnet ef database update --context LeadDbContext
- dotnet ef database update --context CustomerDbContext
# ... per module
```

---

## 3. Seed data strategy

| Data | When | Content |
|------|------|---------|
| RBAC roles + permissions | D1 | Admin (existing), Engineer, Customer + 30 CCTV permissions |
| AMC plan catalog | B3 dev/staging | Silver/Gold/Platinum sample plans with published versions |
| Demo tenant | Dev only | Sample admin user, 2 engineers, 3 customers |
| Production | Go-live | **No business seed** — admin creates plans/customers |
| Reference enums | N/A | Status values enforced by CHECK constraints, not seed tables |

Seeds via idempotent SQL scripts or `IHostedService` dev-only seeder — **never** auto-seed production business data.

---

## 4. Reference data strategy

| Type | Storage |
|------|---------|
| Lead statuses | CHECK constraint on `leads.status` |
| Ticket priority/status | CHECK constraints |
| Invoice type/status | CHECK constraints |
| Visit photo category | CHECK constraint |
| AMC visit frequency | CHECK on plan version |

No separate lookup tables V1 — reduces join complexity; enums documented in [dto-catalog.md](../design/dto-catalog.md).

---

## 5. Rollback strategy

| Level | Approach |
|-------|----------|
| **Migration rollback** | Prefer **forward-fix** migration — EF down migrations discouraged in prod |
| **Failed deploy** | Redeploy previous application version; DB schema forward-compatible (additive-only in V1) |
| **Bad migration** | New migration to revert column/table; restore from backup if catastrophic |
| **Backup** | Daily PostgreSQL backup; point-in-time recovery for production |
| **Mongo audit** | Independent — rollback app does not delete audit entries |

### Additive-only rule (V1)

- Add columns/tables freely
- Do not drop/rename columns without change request + migration plan + backup window

---

## 6. Integrity enforcement timeline

| Rule | Enforced at |
|------|-------------|
| Max 3 site contacts | B2 app + optional CHECK/trigger |
| One active AMC per site | B3 partial unique index |
| Visit checklist | B4 domain + API |
| Invoice Option B term link | B6 app validation |
| Soft delete | All modules from first migration |

---

## 7. File references

All `file_id` columns are **logical refs** to platform `files` schema — no FK migration to platform.

---

## 8. Environment matrix

| Env | Migration apply | Seed |
|-----|-----------------|------|
| Local dev | Developer on pull | Demo seed optional |
| CI | Every PR | Test database fresh + migrations |
| Staging | On deploy | AMC sample plans |
| Production | On deploy (manual approval gate) | RBAC seeds only (D1) |

---

Related: [backend-development-phases.md](./backend-development-phases.md) · [database-architecture.md](../design/database-architecture.md)
