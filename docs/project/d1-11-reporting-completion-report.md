# Sprint 1 — D1-11 Completion Report

**Phase:** D1-11 Reporting  
**Date:** 2026-06-11  
**Review gate:** Deferred test execution (restore + build + architecture tests)

## Summary

Implemented the CCTV reporting module using platform reporting patterns: cross-module read provider in Integration.Infrastructure, MediatR queries in Reporting.Application, REST endpoints, and admin UI hub + report viewer with sorting and CSV export.

## Backend

| Layer | Deliverable |
|-------|-------------|
| SharedKernel.Contracts | `ReportTableDto`, `AdminDashboardDto`, `ICctvReportingDataProvider` |
| Integration.Infrastructure | `CctvReportingDataProvider` (Lead, Customer, AMC, Service, Ticket, Invoice, Engineer repos) |
| Reporting.Application | `GetCctvReportQuery`, `GetAdminDashboardQuery`, RBAC (`reports:read`) |
| Reporting.Api | 8 report routes + CSV export + admin dashboard KPI route |

## Reports

| Report | Route |
|--------|-------|
| Lead Conversion | `GET /cctv/reports/leads` |
| Customer Summary | `GET /cctv/reports/customers` |
| AMC Expiry | `GET /cctv/reports/amc?days=30` |
| Visit Report | `GET /cctv/reports/visits` |
| Engineer Performance | `GET /cctv/reports/engineers` |
| Ticket Report | `GET /cctv/reports/tickets` |
| Invoice Report | `GET /cctv/reports/invoices` |
| Revenue Report | `GET /cctv/reports/revenue` |
| Admin Dashboard KPIs | `GET /cctv/admin/dashboard` |

Export: `GET /cctv/reports/{key}/export` (CSV). PDF generation remains on platform stub (`IPdfGenerationService`) for future wiring.

## Frontend

- `/admin/reports` — Reports hub
- `/admin/reports/:reportKey` — Sortable table + summary badges + CSV export
- Feature flag: `cctv.reporting.enabled = true`

## Verification

```bash
dotnet build BackEnd/Ashraak.slnx
dotnet test BackEnd/tests/Ashraak.Architecture.Tests
```

## Deferred

- Server-side PDF report generation (stub integration only)
- Report pagination beyond first 100 rows (V1 admin scale assumption)
- Dedicated reporting integration tests (deferred to release gate)
