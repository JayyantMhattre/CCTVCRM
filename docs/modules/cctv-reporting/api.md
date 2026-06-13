# API — Reporting

Reporting module (D1-11) under `/api/v1/cctv`. Cross-module data via `ICctvReportingDataProvider` in Integration.Infrastructure.

**Permission:** `reports:read` (Admin role includes all CCTV permissions)  
**Feature flag:** `cctv.reporting.enabled`

## Report endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/v1/cctv/reports/leads` | Lead pipeline / conversion report |
| GET | `/api/v1/cctv/reports/customers` | Customer summary |
| GET | `/api/v1/cctv/reports/amc?days=30` | AMC terms expiring within N days |
| GET | `/api/v1/cctv/reports/visits` | Visit submission status |
| GET | `/api/v1/cctv/reports/engineers` | Engineer workload |
| GET | `/api/v1/cctv/reports/tickets` | Ticket summary |
| GET | `/api/v1/cctv/reports/invoices` | Invoice summary |
| GET | `/api/v1/cctv/reports/revenue` | Paid invoice revenue |
| GET | `/api/v1/cctv/reports/{key}/export` | CSV export (`key` = leads, customers, amc, …) |
| GET | `/api/v1/cctv/admin/dashboard` | Admin KPI widgets |

Response shape: `ReportTableDto` (`reportKey`, `columns`, `rows`, `summary`).

## Health

| Method | Route | Status |
|--------|-------|--------|
| GET | `/api/v1/cctv/health` | Implemented (aggregate, phase D1-12) |

## Frontend

- Admin hub: `/admin/reports`
- Report view: `/admin/reports/:reportKey`

## PDF

Server PDF generation uses platform `IPdfGenerationService` (stub in V1). CSV export is implemented on report routes.
