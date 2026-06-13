# API — Invoice Management (Option B)

Base path: `/api/v1/cctv` · Feature gate: `CctvFeatureFlags.InvoicesEnabled`

| Method | Route | Purpose | Permission |
|--------|-------|---------|------------|
| GET | `/invoices` | Paginated list (status/type filters) | `invoices:read` (Admin) |
| GET | `/invoices/{invoiceId}` | Detail + lines + history | `invoices:read` |
| POST | `/invoices` | Create draft | `invoices:manage` |
| PUT | `/invoices/{invoiceId}` | Update draft + lines | `invoices:manage` |
| POST | `/invoices/{invoiceId}/generate` | Generate PDF → `Generated` | `invoices:manage` |
| POST | `/invoices/{invoiceId}/send` | Mark sent | `invoices:manage` |
| POST | `/invoices/{invoiceId}/mark-paid` | Manual paid | `invoices:manage` |
| POST | `/invoices/{invoiceId}/cancel` | Cancel with reason | `invoices:manage` |
| GET | `/invoices/{invoiceId}/pdf` | PDF file metadata / download URL | `invoices:download` |
| GET | `/portal/invoices` | Customer own invoices | `invoices:read` |

## Invoice types (Option B)

`AmcRenewal` · `NewAmc` · `EmergencyService` · `SpareReplacement` · `AdditionalCharges` · `Other`

AMC term link required for `AmcRenewal` and `NewAmc` only (V-INV-02).

## DTOs

See [dto-catalog §Invoices](../../project/design/dto-catalog.md) — `CreateInvoiceRequest`, `InvoiceDetailDto`, `GenerateInvoiceResultDto`, etc.

## PDF generation

`POST .../generate` invokes `IPdfGenerationService` (stub) and stores a mock `fileId` on `InvoiceAttachment` (type `InvoicePdf`). Full Files-module upload deferred to PDF ADR implementation.
