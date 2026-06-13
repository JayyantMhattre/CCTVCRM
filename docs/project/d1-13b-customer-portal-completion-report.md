# Sprint 1 — D1-13b Completion Report

**Phase:** D1-13b Customer Portal Web  
**Date:** 2026-06-11  
**Wave:** D1-13 Wave 1 (stop after Wave 1)  
**Review gate:** Deferred test execution (restore + build + architecture tests)

## Summary

Replaced all customer portal `CctvPlaceholderPage` routes with functional pages reusing existing CCTV portal and ticket/invoice/AMC APIs. Enabled feature flag `cctv.portal.customer.enabled` in frontend defaults and `appsettings.json`.

## Requirements closed

| ID | Status |
|----|--------|
| FR-CP-01 (dashboard) | **Closed** — Active AMC, upcoming visits, open tickets, recent invoices, quick actions |
| FR-CP-01 (AMC) | **Closed** — Plan, coverage, dates, renewal status, history, renewal request |
| FR-CP-01 (service) | **Closed** — Service history + upcoming visits |
| FR-CP-01 (tickets) | **Closed** — List, create, detail, reopen, comments, attachments |
| FR-CP-01 (invoices) | **Closed** — List, detail, status, amount, due date, PDF download |
| FR-CP-01 (profile) | **Closed** — Platform profile/preferences/sessions reuse |
| FR-CP-01 (password reset) | **Documented dependency** — Platform Auth has no forgot-password OTP flow in SPA |

## Portal routes

| Route | Screen |
|-------|--------|
| `/portal/dashboard` | Customer dashboard |
| `/portal/amc` | AMC details + renewal |
| `/portal/service/history` | Approved visit history |
| `/portal/service/upcoming` | Upcoming schedules |
| `/portal/tickets` | Ticket list |
| `/portal/tickets/new` | Create ticket |
| `/portal/tickets/:ticketId` | Ticket detail (comments, attachments, reopen) |
| `/portal/invoices` | Invoice list |
| `/portal/invoices/:invoiceId` | Invoice detail + PDF |
| `/portal/profile` | Profile hub (platform links) |

## API reuse (no new endpoints)

| Feature | Endpoint |
|---------|----------|
| AMC | `GET /cctv/portal/amc`, `GET /cctv/contracts/{id}`, `POST /cctv/contracts/{id}/renewal-request` |
| Visits | `GET /cctv/portal/visits/upcoming`, `/history`, `/{visitId}` |
| Tickets | `GET /cctv/portal/tickets`, `POST /cctv/tickets`, `GET /cctv/tickets/{id}`, comments, attachments, reopen |
| Invoices | `GET /cctv/portal/invoices`, `GET /cctv/invoices/{id}`, `GET /cctv/invoices/{id}/pdf` |
| Sites | `GET /cctv/portal/sites` (ticket create site picker) |
| Files | Platform `FileUpload` → ticket attachments |
| Profile | Platform `/tenant/profile`, `/users/me/notifications`, `/account/sessions` |

## Frontend deliverables

| Area | Path |
|------|------|
| Portal API | `modules/cctv/portal/api.ts` |
| Pages | `modules/cctv/portal/pages/*` |
| Ticket API extensions | `tickets/api.ts` — create, comment, reopen, link attachment |
| Navigation | `cctvNavigationConfig.ts` — service + profile items |
| Feature flag | `cctvFeatureFlags.ts` — `customerPortal: true` |
| Routes | `cctv/routes.tsx` — placeholders removed |

## Backend changes

| Change | File |
|--------|------|
| Enable customer portal flag | `appsettings.json` → `cctv.portal.customer.enabled: true` |

No business logic changes.

## Password reset dependency

Customer portal profile page documents that **platform Auth does not expose forgot-password / OTP reset** in the current SPA. Administrators must reset credentials until Auth module ships self-service reset (FR-CUST-03 / FR-CP-01h remain partially open).

## Verification

```bash
dotnet build BackEnd/src/Host/Ashraak.Api
dotnet test BackEnd/tests/Ashraak.Architecture.Tests   # 20/20 PASS
```

## Deferred

- Engineer name display on visit lists (API returns engineer ID only)
- Customer password reset OTP UI (Auth dependency)
- Portal E2E tests (deferred to release gate)

## References

- [requirements-freeze-v1.md](./requirements-freeze-v1.md) §2 Customer Portal
- [customer-portal-navigation.md](./design/customer-portal-navigation.md)
