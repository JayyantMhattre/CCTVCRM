# Grid Catalog

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7
**Component (REUSE):** `PlatformTable<TRow>` + `PlatformPagination` + filter bar in `PlatformCard`

Server pagination: `page`, `pageSize`, `sort`, filters per [api-architecture.md](../api-architecture.md). Export: **CSV optional V1** — column set matches grid; PDF export only where noted in reports.

---

## Grid standards

| Standard | Value |
|----------|-------|
| Default page size | 25 (admin), 20 (portal) |
| Max page size | 100 |
| Default sort | `-createdAt` (newest first) unless noted |
| Empty state | PlatformTable empty message + action link (e.g. "Create first lead") |
| Row click | Navigate to detail route |
| Status column | `PlatformBadge` with color map per enum |
| Row actions | Icon buttons in last column; gated by `PermissionGuard` |

---

## 1. Lead List Grid (#12)

| Column | Sortable | Filter | Search |
|--------|:--------:|:------:|:------:|
| Lead Number | ✅ | — | ✅ `q` |
| Name / Organization | ✅ | — | ✅ |
| Source | ✅ | multi-select | — |
| Status | ✅ | multi-select | — |
| Owner | ✅ | user select | — |
| Created At | ✅ | date range | — |

**Row actions:** Open · **Toolbar:** New Lead · **Export:** CSV (optional) · **Permission:** `leads:read`

---

## 2. Customer List Grid (#15)

| Column | Sortable | Filter | Search |
|--------|:--------:|:------:|:------:|
| Customer Number | ✅ | — | ✅ |
| Name | ✅ | — | ✅ |
| Primary Phone | ❌ | — | ✅ |
| City | ✅ | — | — |
| Site Count | ✅ | — | — |
| Status | ✅ | Active/Inactive | — |
| Created At | ✅ | date range | — |

**Row actions:** Open · Edit · **Toolbar:** New Customer · **Permission:** `customers:read`

---

## 3. Site List Grid (within Customer Detail #16)

| Column | Sortable | Filter |
|--------|:--------:|:------:|
| Site Number | ✅ | — |
| Name | ✅ | — |
| City | ✅ | — |
| Active Contract | ✅ | Yes/No |
| Status | ✅ | — |

**Toolbar:** Add Site · **Permission:** `sites:read`

---

## 4. AMC Plan List Grid (#21)

| Column | Sortable | Filter |
|--------|:--------:|:------:|
| Code | ✅ | — |
| Name | ✅ | — |
| Latest Version | ❌ | — |
| Published Price | ❌ | — |
| Status | ✅ | Active/Retired |

**Row actions:** Open · **Toolbar:** New Plan · **Permission:** `amcplans:read`

### Plan versions sub-grid (#22)

| Column | Sortable | Filter |
|--------|:--------:|:------:|
| Version | ✅ | — |
| Price | ✅ | — |
| Visit Frequency | ✅ | — |
| Status | ✅ | Draft/Published/Superseded |
| Effective From | ✅ | — |

---

## 5. AMC Contract List Grid (#23)

| Column | Sortable | Filter | Search |
|--------|:--------:|:------:|:------:|
| Contract Number | ✅ | — | ✅ |
| Customer | ✅ | customerId | ✅ |
| Site | ✅ | siteId | — |
| Active Term End | ✅ | expiry range | — |
| Plan Name | ❌ | — | — |
| Status | ✅ | multi | — |

**Permission:** `amc:read`

### Term history grid (#24 — admin only, BR-AMC-04)

| Column | Sortable |
|--------|:--------:|
| Term # | ✅ |
| Type | ✅ |
| Start / End | ✅ |
| Plan Version | ❌ |
| Price | ❌ |
| Status | ✅ |

Component: **CctvTermHistoryTable** (NEW on PlatformTable)

---

## 6. Schedule List / Calendar (#26)

**View toggle:** List | Calendar (month)

### List columns

| Column | Sortable | Filter |
|--------|:--------:|:------:|
| Schedule Number | ✅ | — |
| Site | ✅ | siteId |
| Scheduled Date | ✅ | date range |
| Status | ✅ | multi |
| Engineer | ✅ | engineerId |
| Contract Term | ❌ | — |

**Row actions:** Open · Assign · **Permission:** `schedules:read`

---

## 7. Visit Approval Queue Grid (#28)

| Column | Sortable | Filter |
|--------|:--------:|:------:|
| Visit Number | ✅ | — |
| Site | ✅ | — |
| Engineer | ✅ | engineerId |
| Submitted At | ✅ | date range |
| Checklist | ❌ | complete/incomplete |

Default sort: `submittedAt` asc (oldest first) · **Permission:** `visits:approve`

---

## 8. Ticket List Grid (#30, #51, #67)

| Column | Sortable | Filter | Search |
|--------|:--------:|:------:|:------:|
| Ticket Number | ✅ | — | ✅ |
| Subject | ❌ | — | ✅ |
| Site | ✅ | siteId | — |
| Priority | ✅ | multi | — |
| Status | ✅ | multi | — |
| Assigned Engineer | ✅ | engineerId | — |
| Created At | ✅ | date range | — |

**Admin row actions:** Open · Assign · **Customer:** Open · Create · **Engineer:** Open · **Permission:** `tickets:read`

Priority badge colors: Low=neutral, Medium=info, High=warning, Critical=danger

---

## 9. Engineer List Grid (#33)

| Column | Sortable | Filter | Search |
|--------|:--------:|:------:|:------:|
| Employee Code | ✅ | — | ✅ |
| Name | ✅ | — | ✅ |
| Phone | ❌ | — | — |
| Active Assignments | ✅ | — | — |
| Status | ✅ | Active/Inactive | — |

**Permission:** `engineers:read`

---

## 10. Invoice List Grid (#35, #54)

| Column | Sortable | Filter | Search |
|--------|:--------:|:------:|:------:|
| Invoice Number | ✅ | — | ✅ |
| Customer | ✅ | customerId | — |
| Type | ✅ | invoiceType | — |
| Total Amount | ✅ | — | — |
| Status | ✅ | multi | — |
| Invoice Date | ✅ | date range | — |
| Due Date | ✅ | overdue filter | — |

**Row actions:** Open · Download PDF (if Generated+) · **Permission:** `invoices:read`

---

## 11. Service History Grid (#50 — Customer)

| Column | Sortable | Filter |
|--------|:--------:|:------:|
| Visit Date | ✅ | date range |
| Site | ✅ | — |
| Engineer | ❌ | — |
| Report PDF | ❌ | download action |

**Filter:** Approved only (server-enforced BR-VISIT-05) · **Permission:** `visits:read`

---

## 12. Reports grids (#38)

Each report area uses `PlatformTable` with report-specific columns — see [report-specification.md](./report-specification.md). Common toolbar: date range filter, Export CSV.

---

## 13. Platform grids (REUSE)

| Grid | Screen | Notes |
|------|--------|-------|
| Audit log table | #41 | Server pagination, module filter ([audit-viewer filters](../../../frontend/audit-viewer/filters.md)) |
| User list | #39 | Platform Users module |
| Webhook deliveries | #43 | Platform module |
| API keys list | #42 | Platform module |

---

## 14. Mobile lists

Mobile uses scrollable list tiles (Flutter `ListView`), not `PlatformTable`. Same columns as summary DTOs — see [mobile-screen-design.md](./mobile-screen-design.md).

---

Related: [screen-design-specification.md](./screen-design-specification.md) · [platform-component-reuse.md](./platform-component-reuse.md)
