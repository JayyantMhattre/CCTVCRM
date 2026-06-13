# Sprint 1 — D1-5 Completion Report

**Phase:** D1-5 Scheduling & Visits  
**Date:** 2026-06-11  
**Review gate:** Gate 1 (restore + build + architecture tests only)

## Summary

Implemented Service Scheduling and Visit Management (`cctv_service`) plus a minimal Engineer master stub (`cctv_engineer`) for assignment validation. Mirrors D1-1..D1-4 layered architecture.

## Backend

| Layer | Deliverable |
|-------|-------------|
| SharedKernel.Contracts | Schedule/Visit DTOs, enums, `IScheduleLookupService`, `IScheduleGenerationService`, `IVisitLookupService`, `IEngineerLookupService` |
| Service.Domain | `ServiceSchedule`, `ServiceVisit`, evidence entities, domain events, repositories |
| Service.Application | 15 commands, 11 queries, permissions, mappers, `TermActivatedScheduleGenerationHandler` |
| Service.Infrastructure | EF configs, repos, `ScheduleGenerationService`, migration `InitialServiceSchema` |
| Service.Api | Full endpoint catalog §8–9 |
| Engineer (stub) | `Engineer` aggregate, EF migration `InitialEngineerSchema`, `EngineerLookupService` |

## Frontend

- `/admin/schedules` — schedule list
- `/admin/visits`, `/admin/visits/:visitId` — visit list + detail with evidence checklist
- `/engineer/visits` — engineer assigned schedules
- `cctv.service.enabled` default `true` in dev feature flags

## Tests

- `ServiceDomainTests.cs` — schedule transitions, reschedule reason, visit submit validation (created, not executed per Gate 1 policy)

## Verification (Gate 1)

```bash
dotnet restore BackEnd/Ashraak.slnx
dotnet build BackEnd/src/Host/Ashraak.Api/Ashraak.Api.csproj
dotnet test BackEnd/tests/Ashraak.Architecture.Tests --no-restore
```

Integration/domain test execution deferred to Review Gate 2.

## Deferred to later phases

- Full Engineer CRUD (B5)
- Outbox processor registration for AMC/Service DbContexts
- Notification integration (Visit Scheduled / Completed)
- Mobile offline sync full implementation
- Visit PDF generation attachment

## Health endpoint

`GET /api/v1/cctv/health` phase updated to **D1-5**.
