# Sprint 1 — D1-13d Completion Report

**Phase:** D1-13d Production PDF Generation  
**Date:** 2026-06-12  
**Wave:** D1-13 Wave 2 (stop after Wave 2)  
**Review gate:** Deferred test execution (restore + build + architecture tests)

## Summary

Replaced `StubPdfGenerationService` with QuestPDF-based `PdfGenerationService`. All three V1 PDF types are generated, stored through the platform Files module (`UploadFileCommand`), and referenced by domain aggregates.

## Requirements closed

| ID | Status |
|----|--------|
| FR-PDF-01 | **Closed** — AMC Contract, Visit Report, Invoice PDFs |
| FR-AMC-05 | **Closed** — Auto-generated on term activation |
| FR-VISIT-05 | **Closed** — Auto-generated on visit report approval |
| FR-INV-03 | **Closed** — Invoice generate stores real PDF via Files |
| NFR-05 (partial) | **Closed** — PDFs persisted as `FileId`; no mock GUIDs |

## PDF types

| Document | Trigger | Storage | Domain reference |
|----------|---------|---------|------------------|
| AMC Contract | `TermActivatedDomainEvent` | Files module | `AmcContract.LinkDocument(ContractPdf)` |
| Visit Report | `VisitReportApprovedDomainEvent` | Files module | `ServiceVisit.AttachReportPdf` |
| Invoice | `GenerateInvoiceCommand` | Files module | `Invoice.Generate(pdfFileId)` |

## Backend deliverables

| Area | Path |
|------|------|
| Renderer | `Integration.Infrastructure/Services/PdfGenerationService.cs` (QuestPDF) |
| File store | `Integration.Infrastructure/Services/CctvFileStore.cs` |
| Contract handler | `Integration.Infrastructure/Services/PdfHandlers/TermActivatedContractPdfHandler` |
| Visit handler | `Integration.Infrastructure/Services/PdfHandlers/VisitReportApprovedPdfHandler` |
| Invoice command | `Invoice.Application/Commands/GenerateInvoice/GenerateInvoiceCommandHandler.cs` |
| Domain | `ServiceVisit.AttachReportPdf` |
| Feature flag | `cctv.integrations.pdf.enabled` |

## PDF content (V1)

- **Contract:** customer, site, plan, coverage dates, price, SLA, included services, approval branding
- **Visit:** schedule, engineer, GPS summary, evidence summary, remarks, approval status
- **Invoice:** number, customer, dates, line items, amounts, branding header

## API compatibility

No endpoint changes. Existing consumers (`POST .../generate`, `GET .../pdf`, portal download) unchanged.

## Verification

```bash
dotnet build BackEnd/src/Host/Ashraak.Api
dotnet test BackEnd/tests/Ashraak.Architecture.Tests   # 21/21 PASS
```

## Remaining gaps (not Wave 2)

- Embedded visit photos/signatures in PDF (summary text only in V1)
- Branded letterhead assets from tenant settings
- PDF unit/integration tests (deferred execution)

## References

- [requirements-freeze-v1.md](./requirements-freeze-v1.md) §18
- ADR-CCTV-0002 (QuestPDF community license)
