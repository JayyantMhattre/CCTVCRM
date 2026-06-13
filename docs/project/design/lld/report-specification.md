# Report Specification

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7
**Access:** Admin only · **Permission:** `reports:read` · **Route hub:** `/admin/reports` (#38)

**Implementation status (D1-13g, 2026-06-12):** All eight reports implemented with server pagination, status/priority/search filters, days window (AMC/visits), CSV export with active filters, drill-down links to admin entity detail routes, and `reports:read` guard on hub. See [d1-13g-reporting-completion-report.md](../../d1-13g-reporting-completion-report.md).

Reports use `PlatformTable` + filter bar + optional CSV export. Charts deferred to `PlatformChart` contract (V1 = tables + summary cards).

---

## 1. AMC Expiry Report

**Route:** `/admin/reports/amc` · **API:** `GET /api/v1/cctv/reports/amc`

| Aspect | Detail |
|--------|--------|
| Purpose | Terms expiring in selected window; renewal pipeline |
| Filters | Expiry bucket (30/60/90/custom days), customer, site, plan |
| Default sort | `termEndDate` asc |

| Column | Description |
|--------|-------------|
| Contract Number | Link to contract detail |
| Customer / Site | Names |
| Plan | Plan name + version |
| Term End Date | — |
| Days Remaining | Computed |
| Status | Active term status |
| Renewal Requested | Yes/No badge |

**Export:** CSV · **Drill-down:** Contract detail #24

---

## 2. Ticket Report

**Route:** `/admin/reports/tickets` · **API:** `GET /reports/tickets`

| Aspect | Detail |
|--------|--------|
| Purpose | Ticket volume, priority mix, resolution time |
| Filters | Date range (created), status, priority, engineer, site |
| Default sort | `-createdAt` |

| Column | Description |
|--------|-------------|
| Ticket Number | Link |
| Subject | Truncated |
| Priority | Badge |
| Status | Badge |
| Site / Customer | — |
| Assigned Engineer | — |
| Created At | — |
| Closed At | Nullable |
| Resolution Hours | Computed if closed |

**Summary row:** Count by priority · avg resolution hours · open vs closed

---

## 3. Visit Report (completion)

**Route:** `/admin/reports/visits` · **API:** `GET /reports/visits`

| Aspect | Detail |
|--------|--------|
| Purpose | Visit completion rates, missed visits, approval backlog |
| Filters | Date range (scheduled), status, engineer, site |
| Default sort | `scheduledDate` desc |

| Column | Description |
|--------|-------------|
| Schedule Number | Link |
| Site | — |
| Scheduled Date | — |
| Status | Planned/Assigned/…/Missed |
| Engineer | — |
| Visit Submitted | Yes/No |
| Approved | Yes/No/Pending |
| Completed Date | — |

**Summary cards:** Completion rate % · Missed count · Pending approval count

---

## 4. Revenue Report (Invoice)

**Route:** `/admin/reports/invoices` · **API:** `GET /reports/invoices`

| Aspect | Detail |
|--------|--------|
| Purpose | Invoice aging and revenue by type (Option B) — **not accounting** |
| Filters | Date range (invoice date), status, invoice type, customer |
| Default sort | `-invoiceDate` |

| Column | Description |
|--------|-------------|
| Invoice Number | Link |
| Customer | — |
| Type | AmcRenewal, etc. |
| Invoice Date / Due Date | — |
| Total Amount | ₹ |
| Status | Badge |
| Days Outstanding | If Sent unpaid |

**Summary:** Total by type · Paid vs outstanding · Count by status

> BR-INV-05: informational only — no GL, no payment reconciliation

---

## 5. Engineer Performance Report

**Route:** `/admin/reports/engineers` · **API:** `GET /reports/engineers`

| Aspect | Detail |
|--------|--------|
| Purpose | Workload and completion metrics per engineer |
| Filters | Date range, engineer (multi), active only |
| Default sort | `completedVisits` desc |

| Column | Description |
|--------|-------------|
| Engineer | Name + code |
| Assigned Visits | Count in period |
| Completed Visits | Count |
| Missed Visits | Count |
| Avg Approval Turnaround | Hours submit→approve |
| Open Tickets | Current |
| Tickets Closed | In period |

---

## 6. Customer Summary Report

**Route:** `/admin/reports/customers` · **API:** `GET /reports/customers` (or combined hub)

| Aspect | Detail |
|--------|--------|
| Purpose | Customer portfolio overview |
| Filters | Status, city, has active AMC |
| Default sort | `customerName` asc |

| Column | Description |
|--------|-------------|
| Customer Number | Link |
| Name | — |
| Site Count | — |
| Active AMC | Yes/No |
| Open Tickets | Count |
| Outstanding Invoices | Count + amount |
| Last Visit Date | Approved visit |

---

## 7. Lead Pipeline Report (bonus — from Reports Hub)

**Route:** `/admin/reports/leads` · **API:** `GET /reports/leads`

| Column | Description |
|--------|-------------|
| Status | Pipeline stage |
| Count | In period |
| Conversion Rate | Won+Converted / total |

Presentation: summary table + optional **CctvStatusPipeline** visual (counts, not chart library V1)

---

## 8. Export & print

| Feature | V1 |
|---------|-----|
| CSV export | ✅ All reports |
| PDF export | ❌ Use dedicated PDF docs for contract/visit/invoice |
| Print-friendly view | Optional CSS print on report page |

---

## 9. Component reuse

| Element | Component |
|---------|-----------|
| Report shell | `PlatformCard` + breadcrumb |
| Data table | `PlatformTable` |
| Filters | `PlatformFormField` inline |
| Summary cards | `PlatformCard` row |
| Status | `PlatformBadge` |

---

Related: [grid-catalog.md](./grid-catalog.md) · [dashboard-specification.md](./dashboard-specification.md)
