# Sprint 1 — D1-8 Completion Report

**Phase:** D1-8 Engineer Management  
**Date:** 2026-06-12  
**Review gate:** Gate 1 (restore + build + architecture tests)

## Summary

Upgraded the D1-5 engineer stub to full Engineer Management (`cctv_engineer`): CRUD APIs, workload summary, domain events, admin UI, and domain tests. Cross-module deactivation guard (V-ENG-02) uses schedule and ticket lookup contracts.

## Backend

| Layer | Deliverable |
|-------|-------------|
| SharedKernel.Contracts | `CreateEngineerRequest`, `UpdateEngineerRequest`, `ChangeEngineerStatusRequest`, `EngineerDetailDto`, `EngineerWorkloadDto`; extended `EngineerSummaryDto`, `ITicketLookupService.GetOpenTicketsForEngineerAsync` |
| Engineer.Domain | `Update`, `ChangeStatus`, domain events, extended `IEngineerRepository` |
| Engineer.Application | 3 commands, 3 queries, permissions, authorization, mapper, validators |
| Engineer.Infrastructure | `EngineerNumberGenerator` (`EN-YYYY-NNNN`), paged repository, module DI |
| Engineer.Api | Full endpoint catalog §11 under `/api/v1/cctv/engineers` |

## Frontend

- `/admin/engineers`, `/admin/engineers/:engineerId` — list + detail with workload and status toggle
- `cctv.engineers.enabled` default `true` in dev feature flags
- Admin nav group **Engineers** (`engineers:read`)

## Tests

- `EngineerDomainTests.cs` — create/update/status, number format (6 tests)

## Verification (Gate 1)

```bash
dotnet restore BackEnd/Ashraak.slnx
dotnet build BackEnd/src/Host/Ashraak.Api/Ashraak.Api.csproj
dotnet test BackEnd/tests/Ashraak.Architecture.Tests --no-restore
dotnet test BackEnd/tests/Ashraak.Integration.Tests --filter "FullyQualifiedName~EngineerDomainTests"
```

## Deferred to later phases

- Platform Users account provisioning on create (user picker UI)
- Engineer create/edit form in admin UI (POST/PUT wired; list/detail shipped)
- Notification on engineer deactivation
- Engineer performance reporting (B7)

## Health endpoint

`GET /api/v1/cctv/health` phase updated to **D1-8**.
