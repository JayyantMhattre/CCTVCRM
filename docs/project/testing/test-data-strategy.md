# Test Data Strategy — TP-1

**Project:** Aarvii CCTV AMC Management System  
**Date:** 2026-06-12  
**Phase:** TP-1 — Data planning only (no seed scripts executed)

---

## 1. Principles

| Principle | Rule |
|-----------|------|
| **Synthetic only** | No production PII in any test environment |
| **Repeatable** | Same seed produces same IDs where possible (fixed GUIDs for personas) |
| **Minimal sufficient** | One happy-path chain + edge-case variants per domain |
| **RBAC-aware** | Data owned by test tenant; permissions match [rbac-matrix.md](../design/rbac-matrix.md) |
| **Lifecycle aligned** | States match [entity-lifecycle-matrix.md](../design/entity-lifecycle-matrix.md) |

**Reference:** [test-environment-plan.md](./test-environment-plan.md) · [business-rules.md](../business-rules.md)

---

## 2. Tenant and users (foundation)

### 2.1 Test tenant

| Field | Value (example) |
|-------|-----------------|
| TenantId | `11111111-1111-1111-1111-111111111111` (document actual after seed) |
| Tenant name | `Aarvii Test Tenant` |
| Time zone | `Asia/Kolkata` |

### 2.2 Personas

| User | Email | Role | Portal |
|------|-------|------|--------|
| Admin | `admin@test.local` | Admin (full CCTV permissions) | Admin web |
| Engineer | `engineer@test.local` | Engineer | Engineer web + mobile |
| Customer | `customer@test.local` | Customer | Customer web + mobile |
| Sales (optional) | `sales@test.local` | Lead-only permissions | Admin web |

**Auth:** OTP via console/log in dev/staging. Password reset smoke uses email capture or log.

---

## 3. Lead data

### 3.1 Purpose

Exercise lead pipeline, conversion, and rejection paths.

### 3.2 Datasets

| Dataset ID | Name | Status | Use case |
|------------|------|--------|----------|
| `LEAD-01` | Acme Retail Lead | New | Create, assign, qualify |
| `LEAD-02` | Beta Corp Lead | Qualified | Convert to customer |
| `LEAD-03` | Gamma Lost Lead | Lost | Terminal state smoke |
| `LEAD-04` | Duplicate Phone Lead | New | Duplicate detection (if applicable) |

### 3.3 Required fields (per entity model)

| Field | Example |
|-------|---------|
| Company name | `Acme Retail Pvt Ltd` |
| Contact name | `Rajesh Kumar` |
| Phone | `+919876543210` (synthetic) |
| Email | `acme.lead@test.local` |
| Source | `Website` / `Referral` |
| Address | Valid Indian address (synthetic) |

### 3.4 Conversion target

`LEAD-02` converts to `CUST-01` + `SITE-01` + `AMC-01` (primary chain).

---

## 4. Customer data

### 4.1 Datasets

| Dataset ID | Name | Source | Use case |
|------------|------|--------|----------|
| `CUST-01` | Acme Retail | Lead conversion | Primary portal + invoice |
| `CUST-02` | Standalone Corp | Manual create | Direct customer create |
| `CUST-03` | Inactive Customer | Manual | Read-only / archive rules |

### 4.2 Required fields

| Field | Example |
|-------|---------|
| Legal name | `Acme Retail Pvt Ltd` |
| GSTIN | Valid format synthetic `29ABCDE1234F1Z5` |
| Billing address | Matches site or separate |
| Primary contact | Link to site contact |

### 4.3 Portal linkage

`customer@test.local` linked to `CUST-01` for customer portal smoke.

---

## 5. Site data

### 5.1 Datasets

| Dataset ID | Customer | Name | Use case |
|------------|----------|------|----------|
| `SITE-01` | CUST-01 | Acme HQ Bangalore | Primary AMC site |
| `SITE-02` | CUST-01 | Acme Warehouse | Second site (no AMC overlap rule) |
| `SITE-03` | CUST-02 | Standalone Office | One AMC per site validation |

### 5.2 Site contacts

| Site | Contact | Role | Phone |
|------|---------|------|-------|
| SITE-01 | Priya Sharma | Site Manager | `+919876543211` |
| SITE-01 | Security Desk | Alternate | `+919876543212` |

### 5.3 Business rules to validate

- One active AMC per site (BR from site/AMC modules)
- Site required before AMC contract

---

## 6. AMC data

### 6.1 Plan catalog (reference data)

| Plan ID | Name | Visits/year | SLA |
|---------|------|-------------|-----|
| `PLAN-BASIC` | Basic AMC | 4 | Standard |
| `PLAN-PREM` | Premium AMC | 12 | Priority |

### 6.2 Contract datasets

| Dataset ID | Site | Plan | Status | Use case |
|------------|------|------|--------|----------|
| `AMC-01` | SITE-01 | PLAN-PREM | Active | Visit schedule generation |
| `AMC-02` | SITE-03 | PLAN-BASIC | Draft | Activation flow |
| `AMC-03` | SITE-01 | — | Expired | Renewal smoke (manual) |

### 6.3 Terms

| Contract | Start | End | Auto-renew |
|----------|-------|-----|------------|
| AMC-01 | Today − 30d | Today + 335d | false |
| AMC-02 | — | — | — |

---

## 7. Visit data

### 7.1 Schedule

| Schedule ID | AMC | Frequency | Next visit |
|-------------|-----|-----------|------------|
| `SCHED-01` | AMC-01 | Monthly | Upcoming within 7 days |

### 7.2 Visit instances

| Visit ID | Schedule | Status | Engineer | Use case |
|----------|----------|--------|----------|----------|
| `VISIT-01` | SCHED-01 | Scheduled | engineer@test | Assignment smoke |
| `VISIT-02` | SCHED-01 | In Progress | engineer@test | Evidence capture |
| `VISIT-03` | SCHED-01 | Completed | engineer@test | Pending approval |
| `VISIT-04` | SCHED-01 | Approved | engineer@test | Closed loop |

### 7.3 Evidence checklist (per BR)

| Visit | Photos | Checklist items | Video |
|-------|--------|-----------------|-------|
| VISIT-02 | 2+ synthetic JPEG | All mandatory checked | Optional |
| VISIT-03 | Complete set | Complete | **1 MP4 ≤ 100 MB** (Wave 4 smoke) |

**Files:** Upload via Files module; link via visit API (`linkVideo`).

---

## 8. Ticket data

### 8.1 Datasets

| Ticket ID | Customer/Site | Status | Source | Use case |
|-----------|---------------|--------|--------|----------|
| `TKT-01` | CUST-01 / SITE-01 | Open | Customer portal | Customer create |
| `TKT-02` | CUST-01 / SITE-01 | Assigned | Admin | Engineer assignment |
| `TKT-03` | CUST-01 / SITE-01 | In Progress | Engineer | Mobile update |
| `TKT-04` | CUST-01 / SITE-01 | Resolved | Engineer | Close workflow |
| `TKT-05` | CUST-01 / SITE-01 | Closed | Admin | Terminal state |

### 8.2 Attachments

| Ticket | File | Purpose |
|--------|------|---------|
| TKT-01 | `ticket-photo.jpg` | Customer upload |
| TKT-03 | `engineer-note.pdf` | Engineer attachment |

---

## 9. Invoice data

### 9.1 Option B lifecycle datasets

| Invoice ID | Customer | Status | Use case |
|------------|----------|--------|----------|
| `INV-01` | CUST-01 | Draft | Edit line items |
| `INV-02` | CUST-01 | Generated | PDF generation |
| `INV-03` | CUST-01 | Sent | Admin Send action (Wave 4) |
| `INV-04` | CUST-01 | Paid | Mark Paid (Wave 4) |
| `INV-05` | CUST-01 | Cancelled | Cancel from Draft/Sent |

### 9.2 Line items (example INV-01)

| Description | Qty | Rate | GST |
|-------------|-----|------|-----|
| Premium AMC — Q1 | 1 | 25000.00 | 18% |
| Emergency visit charge | 1 | 1500.00 | 18% |

**Financial type:** `decimal` in API; verify rounding in smoke.

### 9.3 Portal visibility

Customer portal shows `INV-03`, `INV-04` — not Draft.

---

## 10. Engineer data

| Engineer ID | User | Skills | Assignment |
|-------------|------|--------|------------|
| `ENG-01` | engineer@test.local | CCTV install | VISIT-01, TKT-02 |

---

## 11. Reporting data (Wave 4)

Minimum rows for filter/pagination smoke:

| Report | Min rows | Filters to test |
|--------|----------|-----------------|
| Leads | 15+ | Status, date range |
| Visits | 15+ | Engineer, status |
| Tickets | 15+ | Priority, status |
| Invoices | 10+ | Status, customer |

Seed script or SQL insert batch in TP-2 — **test infrastructure only**.

---

## 12. Data creation methods

| Method | When | Owner |
|--------|------|-------|
| API sequence (Postman/REST) | TP-3 manual setup | QA |
| Integration test builders | TP-2 automated | Dev |
| SQL seed script | Staging refresh | DevOps |
| RBAC auto-seed | Every API start | System |

**Recommended TP-2 artifact:** `scripts/test-data/seed-smoke-chain.sql` or documented Postman collection — creation deferred to TP-2 if not present.

---

## 13. Data refresh policy

| Event | Action |
|-------|--------|
| Start of TP-3 | Full smoke chain verified or re-seeded |
| After destructive test | Re-run seed for affected entities |
| End of TP-5 | Staging snapshot archived |
| Production | **Never** use test data |

---

## 14. Privacy and compliance

- All phone numbers use `+91987654xxxx` test range  
- No real GSTINs or Aadhaar  
- Video files: generated test MP4, not customer premises  
- Audit logs may contain test actions — acceptable on staging  

---

## 15. References

- [manual-smoke-checklist.md](./manual-smoke-checklist.md)
- [entity-model.md](../design/entity-model.md)
- [erd-overview.md](../design/erd-overview.md)
- [business-rules.md](../business-rules.md)

---

*TP-1 — Strategy only. No data seeded in this phase.*
