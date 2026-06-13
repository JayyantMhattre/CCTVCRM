# Sprint 1 — D1-4 AMC Management Completion Report

**Date:** 2026-06-11 · **Gate:** 1 (restore + build + architecture tests)

## Delivered

| Area | Status |
|------|--------|
| Domain (`cctv_amc`) | Plan, PlanVersion, Contract, ContractTerm, ContractDocument aggregates |
| SharedKernel contracts | DTOs, enums, `IAmcPlanLookupService`, `IAmcContractLookupService`, provisioning |
| Application | 10 commands, 8 queries, permissions, authorization, mappers |
| Infrastructure | EF configs, repos, `AmcContractProvisioningService`, migration `InitialAmcSchema` |
| API | `/cctv/amc-plans/*`, `/cctv/contracts/*`, renewal + portal routes |
| Frontend | Admin plan/contract list + detail pages, endpoints, feature flag enabled |
| Tests created | `AmcDomainTests.cs` (not executed per Gate 1 scope) |

## Domain rules enforced

- V-AMC-08: Published plan version required for contract / lead conversion
- V-AMC-02: One active contract per site (partial unique index + domain guard)
- V-AMC-04: `endDate > startDate`
- V-AMC-05: Price > 0
- BR-AMC-07: Plan version immutable once referenced (`is_referenced`)

## Verification (Gate 1)

```bash
dotnet restore BackEnd/src/Host/Ashraak.Api/Ashraak.Api.csproj
dotnet build BackEnd/src/Host/Ashraak.Api/Ashraak.Api.csproj
dotnet test BackEnd/tests/Ashraak.Architecture.Tests/Ashraak.Architecture.Tests.csproj
```

## Deferred (later gates)

- Integration test execution (`AmcDomainTests`)
- Schedule auto-generation on `TermActivated` (BR-SCHED-02)
- PDF generation integration (stub remains in Integration module)
