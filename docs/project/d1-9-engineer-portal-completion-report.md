# Sprint 1 — D1-9 Completion Report

**Phase:** D1-9 Engineer Portal  
**Date:** 2026-06-11  
**Review gate:** Deferred test execution (restore + build + architecture tests)

## Summary

Delivered the Engineer web portal on top of existing D1-5 visit execution APIs and platform reuse (Auth, RBAC, Files, Notifications, Audit, Theme). Engineers can view their day, manage assigned visits with mandatory evidence, update assigned tickets, and access profile/notification preferences via platform routes.

## Backend (reuse + extensions)

| Layer | Deliverable |
|-------|-------------|
| Service.Application | `GetEngineerDashboardQuery` + handler, `EngineerDashboardDto` |
| Service.Api | `GET /api/v1/cctv/engineer/dashboard` |
| Service (existing) | Visit start/complete, evidence endpoints (selfie, GPS, photos, signature, remarks, submit) via Files `FileRecord` |
| Ticket (existing) | Engineer ticket list/detail/status update APIs |

## Frontend

| Screen | Route | Status |
|--------|-------|--------|
| Engineer Dashboard | `/engineer` | Implemented |
| Assigned Visits | `/engineer/visits` | Implemented |
| Visit Detail | `/engineer/visits/:visitId` | Implemented |
| Visit Reporting | `/engineer/visits/:visitId/report` | Implemented (evidence checklist) |
| Assigned Tickets | `/engineer/tickets` | Implemented |
| Ticket Detail / Updates | `/engineer/tickets/:ticketId` | Implemented |
| Profile & Preferences | `/engineer/profile` | Platform reuse links |

- Feature flag: `cctv.portal.engineer.enabled = true`
- Navigation: `CCTV_ENGINEER_NAV` (My Day, Visits, Tickets, Profile)

## Mandatory visit evidence (BRD)

Captured via existing visit execution flow + web report page:

- Engineer selfie, GPS coordinates + timestamp, before/during/after photos, customer signature, visit remarks
- All uploads use platform Files module (`uploadFile` → `FileRecord`)

## Verification

```bash
dotnet build BackEnd/Ashraak.slnx
dotnet test BackEnd/tests/Ashraak.Architecture.Tests
```

## Deferred

- Engineer returned/completed visit history pages (admin visit list covers approvals)
- Dedicated engineer notification preferences UI (platform `/users/me/notifications` reused)
- Full E2E portal tests (deferred to release gate)

## Health endpoint

Aggregate health remains under CCTV API; phase advanced to **D1-12** at release candidate.
