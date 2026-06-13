# Smoke Test Data Guide — TP-LOCAL-1

**Project:** Aarvii CCTV AMC Management System  
**Script:** `scripts/test-data/seed-smoke-chain.sql`  
**Phase:** Environment setup only — data loaded manually, not by smoke tests

---

## 1. Purpose

Provide a **repeatable, synthetic dataset** covering all CCTV domain entities required for manual smoke testing:

- Lead
- Customer
- Site
- AMC (plan, contract, term)
- Visit (schedule + visit)
- Ticket
- Invoice
- Engineer

Aligned with [test-data-strategy.md](./test-data-strategy.md).

---

## 2. Prerequisites

| Step | Command |
|------|---------|
| Stack running | `docker compose up -d` |
| Migrations applied | `.\scripts\database\apply-migrations.ps1` |
| API started once | Ensures RBAC seed (`Cctv:RbacSeed:Enabled`) |

---

## 3. Load seed data

```powershell
.\scripts\database\run-seed.ps1
```

Or manually:

```powershell
Get-Content scripts\test-data\seed-smoke-chain.sql -Raw |
  docker compose exec -T postgres psql -U ashraak -d ashraak
```

The script is **idempotent** — re-running uses `ON CONFLICT DO NOTHING`.

---

## 4. Entity graph

```
LEAD-00002 (Qualified)
    └── CUST-00001 (Acme Retail)
            └── SITE-00001 (Acme HQ Bangalore)
                    └── AMC-00001 (Active)
                            └── Term 1 (Active, PLAN-PREM)
                                    └── SCHED-00001 (Assigned)
                                            └── VISIT (Draft)
                                            └── TKT-00001 (Open)
                                            └── INV-00001 (Draft)
                                            └── INV-00003 (Sent)
                                            └── INV-00004 (Paid)

ENG-00001 (Test Engineer) → assigned to SCHED-00001
```

---

## 5. Fixed identifiers

Use these GUIDs in API calls and test assertions.

### System actors

| Role | GUID | Email |
|------|------|-------|
| Admin / system actor | `22222222-2222-2222-2222-222222222222` | `admin@test.local` (Auth setup separate) |
| Engineer user | `33333333-3333-3333-3333-333333333333` | `engineer@test.local` |
| Customer portal user | `44444444-4444-4444-4444-444444444444` | `customer@test.local` |

### Business entities

| Dataset ID | Entity | GUID | Number |
|------------|--------|------|--------|
| LEAD-01 | Lead (New) | `aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1` | LEAD-00001 |
| LEAD-02 | Lead (Qualified) | `aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2` | LEAD-00002 |
| CUST-01 | Customer | `bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1` | CUST-00001 |
| SITE-01 | Site | `cccccccc-cccc-cccc-cccc-ccccccccccc1` | SITE-00001 |
| PLAN-PREM | AMC Plan | `dddddddd-dddd-dddd-dddd-dddddddddd01` | PLAN-PREM |
| PLAN-V1 | Plan Version | `dddddddd-dddd-dddd-dddd-dddddddddd02` | v1 |
| AMC-01 | Contract | `eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee1` | AMC-00001 |
| AMC-T1 | Contract Term | `eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee2` | Term 1 |
| SCHED-01 | Schedule | `ffffffff-ffff-ffff-ffff-fffffffffff1` | SCHED-00001 |
| VISIT-01 | Visit | `10101010-1010-1010-1010-101010101001` | — |
| ENG-01 | Engineer | `20202020-2020-2020-2020-202020202001` | ENG-00001 |
| TKT-01 | Ticket | `30303030-3030-3030-3030-303030303001` | TKT-00001 |
| INV-01 | Invoice (Draft) | `40404040-4040-4040-4040-404040404001` | INV-00001 |
| INV-03 | Invoice (Sent) | `40404040-4040-4040-4040-404040404003` | INV-00003 |
| INV-04 | Invoice (Paid) | `40404040-4040-4040-4040-404040404004` | INV-00004 |

---

## 6. Verification queries

Run inside PostgreSQL:

```sql
-- Entity counts
SELECT 'leads' AS entity, COUNT(*) FROM cctv_lead.leads
UNION ALL SELECT 'customers', COUNT(*) FROM cctv_customer.customers
UNION ALL SELECT 'sites', COUNT(*) FROM cctv_customer.sites
UNION ALL SELECT 'amc_contracts', COUNT(*) FROM cctv_amc.amc_contracts
UNION ALL SELECT 'service_visits', COUNT(*) FROM cctv_service.service_visits
UNION ALL SELECT 'tickets', COUNT(*) FROM cctv_ticket.tickets
UNION ALL SELECT 'invoices', COUNT(*) FROM cctv_invoice.invoices
UNION ALL SELECT 'engineers', COUNT(*) FROM cctv_engineer.engineers;

-- Primary chain
SELECT l.lead_number, l.status,
       c.customer_number, s.site_number,
       a.contract_number, t.status AS term_status
FROM cctv_lead.leads l
JOIN cctv_customer.customers c ON c.source_lead_id = l.id
JOIN cctv_customer.sites s ON s.customer_id = c.id
JOIN cctv_amc.amc_contracts a ON a.site_id = s.id
JOIN cctv_amc.amc_contract_terms t ON t.amc_contract_id = a.id
WHERE l.id = 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2';
```

Expected: 2 leads, 1 customer, 1 site, 1 contract, 1 visit, 1 ticket, 3 invoices, 1 engineer.

---

## 7. API verification (read-only)

After seeding, verify data is reachable (requires valid auth token):

```http
GET http://localhost:8080/api/v1/cctv/leads/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2
GET http://localhost:8080/api/v1/cctv/customers/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1
GET http://localhost:8080/api/v1/cctv/tickets/30303030-3030-3030-3030-303030303001
```

**Note:** Auth tokens are not seeded. User accounts (`admin@test.local`, etc.) require OTP registration via the Auth module or separate test setup. The seed provides **business entity data** only.

---

## 8. Refresh / reset

### Re-seed (non-destructive)

```powershell
.\scripts\database\run-seed.ps1
```

### Full reset

```powershell
docker compose down
docker volume rm cctvcrm_postgres_data
docker compose up -d postgres
# Wait for healthy
.\scripts\database\apply-migrations.ps1
.\scripts\database\run-seed.ps1
```

---

## 9. Data characteristics

| Field | Convention |
|-------|------------|
| Phone numbers | `+9198765432xx` synthetic range |
| Email | `*.test.local` domain |
| GSTIN | Not seeded (add via API if needed) |
| Financial amounts | `decimal(18,2)` — INV-01 total: 29500.00 |
| Dates | Relative to `CURRENT_DATE` (AMC term, schedules) |

---

## 10. What is NOT seeded

| Item | Reason |
|------|--------|
| Auth users / tokens | Platform Auth module — OTP flow required |
| Tenant record | Created via Tenant module on first admin setup |
| File attachments | Requires Files module upload |
| RBAC roles | Auto-seeded by `CctvRbacSeedHostedService` on API startup |
| Reporting bulk data | Minimum rows only; expand in TP-3 if needed |

---

## 11. References

- [test-data-strategy.md](./test-data-strategy.md)
- [manual-smoke-checklist.md](./manual-smoke-checklist.md)
- [local-test-environment.md](./local-test-environment.md)
