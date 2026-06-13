# Sprint 1 — D1-7 Completion Report

**Phase:** D1-7 Invoice Management (Option B)  
**Date:** 2026-06-12  
**Review gate:** Gate 1 (restore + build + architecture tests only)

## Summary

Implemented Invoice Management (`cctv_invoice`) mirroring D1-1..D1-6 layered architecture: Option B domain rules, application commands/queries, EF infrastructure with CHECK constraints, REST endpoints, frontend list/detail pages, and domain tests.

## Backend

| Layer | Deliverable |
|-------|-------------|
| SharedKernel.Contracts | Invoice DTOs, enum contracts, `IInvoiceLookupService` |
| Invoice.Domain | `Invoice` aggregate, lines/attachments/history, domain events, `IInvoiceRepository` |
| Invoice.Application | 6 commands, 4 queries, permissions, customer-scoped authorization, `InvoiceMapper` |
| Invoice.Infrastructure | EF configs + CHECK constraints, `InvoiceRepository`, `InvoiceNumberGenerator` (`INV-YYYY-NNNN`), migration `InitialInvoiceSchema` |
| Invoice.Api | Full endpoint catalog §12 under `/api/v1/cctv` |

## Option B rules enforced

- V-INV-02: AMC term required for `AmcRenewal` / `NewAmc`
- V-INV-04: Edit only in `Draft`
- V-INV-05: ≥ 1 line item
- V-INV-06: Line totals + `total = subtotal + tax`
- Generate: Draft → Generated + `InvoicePdf` attachment via stub PDF service

## Frontend

- `/admin/invoices`, `/admin/invoices/:invoiceId` — admin list + detail
- `/portal/invoices` — customer invoice list
- `cctv.invoices.enabled` default `true` in dev feature flags

## Tests

- `InvoiceDomainTests.cs` — Option B term rule, draft-only edit, line totals, lifecycle transitions, number format (created, not executed per Gate 1 policy)

## Verification (Gate 1)

```bash
dotnet restore BackEnd/src/Host/Ashraak.Api/Ashraak.Api.csproj
dotnet build BackEnd/src/Host/Ashraak.Api/Ashraak.Api.csproj
dotnet test BackEnd/tests/Ashraak.Architecture.Tests/Ashraak.Architecture.Tests.csproj
```

Integration/domain test execution deferred to Review Gate 2.

## Deferred to later phases

- Files module upload on PDF generate (replace mock `fileId`)
- Invoice Generated notification
- Admin create/edit draft UI forms
- Invoice aging report (`/reports/invoices`)
- Payment gateway (out of scope BR-INV-05)

## Health endpoint

`GET /api/v1/cctv/health` phase updated to **D1-7**.
