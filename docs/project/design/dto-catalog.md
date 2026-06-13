# DTO Catalog

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-6 — request/response model inventory
**Pattern source:** Platform `UserDto`, Files upload response, `PagedResult<T>`, ProblemDetails ([users api](../../modules/users/api.md), [files api](../../modules/files/api.md))

> DTOs live in `Ashraak.SharedKernel.Contracts/CctvCrm/Dtos/` (backend authority). OpenAPI projects the same shapes to TypeScript/Dart — no hand-written client DTOs.

Classification: **REUSE** (platform DTO unchanged) · **NEW** (CCTV DTO following platform patterns)

---

## 1. Platform DTOs reused as-is

| DTO | Source | Used by |
|-----|--------|---------|
| `UserDto` | `SharedKernel.Contracts/Users/Dtos/UserDto.cs` | Customer/Engineer profile, admin user management |
| `FileUploadResponseDto` | Files module `{ id, fileName, contentType, size, uploadedOnUtc }` | All upload flows before linking |
| `AuditLogPageDto` | Audit module | Admin audit viewer |
| `ProblemDetails` | RFC 7807 via GlobalExceptionHandler | All error responses |
| `PagedResult<T>` | BuildingBlocks | All CCTV list endpoints |

---

## 2. Common CCTV base types (NEW)

| DTO | Purpose | Key fields |
|-----|---------|------------|
| `AuditableDto` | Base for mutable entities | `createdAt`, `createdBy`, `updatedAt`, `updatedBy`, `rowVersion` |
| `PagedRequest` | Standard list query | `page`, `pageSize`, `sort`, filter props |
| `PagedResponse<T>` | HTTP projection of `PagedResult<T>` | `items`, `page`, `pageSize`, `totalCount`, `totalPages`, `hasNextPage`, `hasPreviousPage` |
| `MoneyDto` | Monetary values | `amount` (decimal string), `currency` (default `INR`) |
| `GeoPointDto` | GPS capture | `latitude`, `longitude`, `capturedAt` |
| `FileReferenceDto` | Linked platform file | `fileId`, `fileName`, `contentType`, `sizeBytes` |
| `StatusTransitionRequest` | Generic status change | `toStatus`, `reason`, `rowVersion` |

---

## 3. Request DTOs (by module)

### Public inquiries

| DTO | Endpoint | Fields |
|-----|----------|--------|
| `CreateInquiryRequest` | `POST /inquiries` | `inquiryType`, `name`, `organization`, `email`, `phone`, `city`, `address`, `requirementSummary`, `preferredPlanCode?`, `sourcePage` |

### Lead

| DTO | Endpoint | Fields |
|-----|----------|--------|
| `CreateLeadRequest` | `POST /leads` | contact fields, `source`, `ownerUserId?` |
| `UpdateLeadRequest` | `PUT /leads/{id}` | mutable lead fields, `rowVersion` |
| `ChangeLeadStatusRequest` | `POST .../status` | `toStatus`, `notes?`, `rowVersion` |
| `CreateLeadActivityRequest` | `POST .../activities` | `activityType`, `description`, `fromStatus?`, `toStatus?` |
| `CreateLeadRemarkRequest` | `POST .../remarks` | `text` |
| `LinkLeadAttachmentRequest` | `POST .../attachments` | `fileId`, `title` |
| `ConvertLeadRequest` | `POST .../convert` | `planVersionId`, `siteName`, `siteAddress`, `initialTermStartDate`, `initialTermEndDate`, `rowVersion` |

### Customer

| DTO | Endpoint | Fields |
|-----|----------|--------|
| `CreateCustomerRequest` | `POST /customers` | `name`, contacts, billing address, `portalUserId?` |
| `UpdateCustomerRequest` | `PUT /customers/{id}` | mutable fields, `rowVersion` |
| `ChangeCustomerStatusRequest` | `PATCH .../status` | `status`, `rowVersion` |
| `UpdateOwnProfileRequest` | `PATCH /portal/profile` | allowed self-service fields (BR-AUTH-05) |

### Site

| DTO | Endpoint | Fields |
|-----|----------|--------|
| `CreateSiteRequest` | `POST /sites` | `customerId`, `name`, address, city |
| `UpdateSiteRequest` | `PUT /sites/{id}` | mutable fields, `rowVersion` |
| `UpsertSiteContactsRequest` | `PUT .../contacts` | `contacts[]` (max 3): `name`, `designation`, `phone`, `email`, `isPrimary` |
| `LinkSiteDocumentRequest` | `POST .../documents` | `fileId`, `title`, `documentType` |

### Asset

| DTO | Endpoint | Fields |
|-----|----------|--------|
| `UpdateSiteAssetSummaryRequest` | `PUT .../asset-summary` | `cameraCount`, `dvrCount`, `nvrCount`, `hardDiskCount`, `switchCount`, `routerCount`, `monitorCount`, `brand?`, `model?`, `remarks?`, `rowVersion` |

### AMC Plans

| DTO | Endpoint | Fields |
|-----|----------|--------|
| `CreateAmcPlanRequest` | `POST /amc-plans` | `code`, `name`, `description` |
| `UpdateAmcPlanRequest` | `PUT /amc-plans/{id}` | identity fields, `rowVersion` |
| `CreateAmcPlanVersionRequest` | `POST .../versions` | `price`, `visitFrequency`, `includedServices[]`, `slaDescription`, `effectiveFrom` |
| `PublishAmcPlanVersionRequest` | `POST .../publish` | `rowVersion` |

### AMC Contracts

| DTO | Endpoint | Fields |
|-----|----------|--------|
| `CreateAmcContractRequest` | `POST /contracts` | `siteId`, `customerId`, `planVersionId` |
| `CreateAmcContractTermRequest` | `POST .../terms` | `planVersionId`, `startDate`, `endDate`, `price`, `termType` (`Initial`/`Renewal`) |
| `ActivateAmcTermRequest` | `POST .../activate` | `rowVersion` |
| `AmcRenewalRequestDto` | `POST .../renewal-request` | `message?` |
| `LinkContractDocumentRequest` | `POST .../documents` | `fileId`, `documentType` |

### Scheduling

| DTO | Endpoint | Fields |
|-----|----------|--------|
| `CreateAdHocScheduleRequest` | `POST /schedules` | `contractTermId`, `siteId`, `scheduledDate`, `notes?` |
| `AssignEngineerRequest` | `POST .../assign` | `engineerId`, `rowVersion` |
| `RescheduleVisitRequest` | `POST .../reschedule` | `newScheduledDate`, `reason`, `rowVersion` |
| `CancelScheduleRequest` | `POST .../cancel` | `reason`, `rowVersion` |

### Visits

| DTO | Endpoint | Fields |
|-----|----------|--------|
| `StartVisitRequest` | `POST .../start` | `startedAt?`, `rowVersion` |
| `UpdateVisitRemarksRequest` | `PUT .../remarks` | `remarks`, `rowVersion` |
| `LinkVisitPhotoRequest` | `POST .../photos` | `fileId`, `category` (`Before`/`During`/`After`), `capturedAt?` |
| `LinkVisitSelfieRequest` | `POST .../selfie` | `fileId`, `capturedAt?` |
| `CaptureVisitLocationRequest` | `POST .../location` | `latitude`, `longitude`, `capturedAt` |
| `LinkVisitSignatureRequest` | `POST .../signature` | `fileId`, `signerName`, `capturedAt?` |
| `LinkVisitAttachmentRequest` | `POST .../attachments` | `fileId`, `attachmentType` (`Video`/`ReportPdf`), `title?` |
| `SubmitVisitReportRequest` | `POST .../submit` | `rowVersion`, `clientCorrelationId?` (mobile offline) |
| `ApproveVisitRequest` | `POST .../approve` | `reviewRemarks?` |
| `ReturnVisitRequest` | `POST .../return` | `returnReason` (required) |
| `OfflineVisitSyncBatchRequest` | `POST /engineer/visits/sync` | `items[]` of offline payloads + `clientCorrelationId` |

### Tickets

| DTO | Endpoint | Fields |
|-----|----------|--------|
| `CreateTicketRequest` | `POST /tickets` | `siteId`, `subject`, `description`, `priority`, `serviceVisitId?`, `attachmentFileIds?[]` |
| `AssignTicketRequest` | `POST .../assign` | `engineerId`, `rowVersion` |
| `UpdateTicketStatusRequest` | `PATCH .../status` | `toStatus`, `comment?`, `rowVersion` |
| `CreateTicketCommentRequest` | `POST .../comments` | `text` |
| `LinkTicketAttachmentRequest` | `POST .../attachments` | `fileId`, `title?` |
| `ReopenTicketRequest` | `POST .../reopen` | `reason` (required, BR-TKT-06) |

### Engineers

| DTO | Endpoint | Fields |
|-----|----------|--------|
| `CreateEngineerRequest` | `POST /engineers` | `name`, `phone`, `email`, `employeeCode`, `portalUserId?` |
| `UpdateEngineerRequest` | `PUT /engineers/{id}` | mutable fields, `rowVersion` |
| `ChangeEngineerStatusRequest` | `PATCH .../status` | `status`, `rowVersion` |

### Invoices

| DTO | Endpoint | Fields |
|-----|----------|--------|
| `CreateInvoiceRequest` | `POST /invoices` | `customerId`, `siteId?`, `invoiceType`, `amcContractTermId?`, `ticketId?`, `serviceVisitId?`, `invoiceDate`, `dueDate?`, `lines[]` |
| `UpdateInvoiceDraftRequest` | `PUT /invoices/{id}` | header fields + `lines[]`, `rowVersion` |
| `InvoiceLineRequest` | nested | `description`, `quantity`, `unitPrice`, `taxRate?`, `sortOrder` |
| `GenerateInvoiceRequest` | `POST .../generate` | `rowVersion` |
| `SendInvoiceRequest` | `POST .../send` | `rowVersion` |
| `MarkInvoicePaidRequest` | `POST .../mark-paid` | `paidAt?`, `rowVersion` |
| `CancelInvoiceRequest` | `POST .../cancel` | `reason`, `rowVersion` |

---

## 4. Response DTOs — Summary (list/card views)

| DTO | Used in |
|-----|---------|
| `LeadSummaryDto` | Lead pipeline list |
| `CustomerSummaryDto` | Customer list |
| `SiteSummaryDto` | Site list, customer site picker |
| `AmcPlanSummaryDto` | Plan catalog |
| `AmcPlanVersionSummaryDto` | Version list within plan |
| `AmcContractSummaryDto` | Contract list |
| `AmcContractTermSummaryDto` | Term row in admin history |
| `ScheduleSummaryDto` | Calendar/list |
| `VisitSummaryDto` | Visit lists, engineer queue |
| `TicketSummaryDto` | Ticket queues |
| `EngineerSummaryDto` | Engineer list |
| `InvoiceSummaryDto` | Invoice list |
| `RenewalRequestSummaryDto` | Admin renewal queue |

Common summary fields: `id`, `{entity}Number`, `status`, key display labels, `createdAt`.

---

## 5. Response DTOs — Detail (single resource)

| DTO | Used in |
|-----|---------|
| `LeadDetailDto` | Lead detail — includes status, owner, conversion refs, counts |
| `CustomerDetailDto` | Customer detail — includes site count, portal link |
| `SiteDetailDto` | Site detail — contacts, asset summary stub, active contract stub |
| `SiteContactDto` | Embedded in site detail |
| `SiteAssetSummaryDto` | Asset summary detail |
| `SiteDocumentDto` | Document metadata + `FileReferenceDto` |
| `AmcPlanDetailDto` | Plan + version list |
| `AmcPlanVersionDetailDto` | Full commercial terms |
| `AmcContractDetailDto` | Master + **full term history** (admin) |
| `AmcContractTermDetailDto` | Term with pinned plan version snapshot |
| `AmcContractDocumentDto` | Contract PDF metadata |
| `ScheduleDetailDto` | Schedule + current assignment + visit stub |
| `EngineerAssignmentDto` | Assignment history row |
| `VisitDetailDto` | Full evidence checklist state |
| `VisitPhotoDto` | Photo metadata + category |
| `VisitLocationDto` | GPS record |
| `VisitSignatureDto` | Signature metadata |
| `VisitApprovalDto` | Review round history |
| `VisitAttachmentDto` | Video/report attachments |
| `TicketDetailDto` | Ticket + current assignment |
| `TicketCommentDto` | Comment row |
| `TicketAttachmentDto` | Attachment metadata |
| `TicketStatusHistoryDto` | Status transition row |
| `EngineerDetailDto` | Engineer profile + workload counts |
| `InvoiceDetailDto` | Invoice header + lines + status history |
| `InvoiceLineDto` | Line item |
| `InvoiceAttachmentDto` | PDF attachment metadata |

---

## 6. Command result DTOs

| DTO | Returns from |
|-----|--------------|
| `LeadConversionResultDto` | `POST .../convert` — `{ customerId, siteId, contractId, termId }` |
| `CreateInquiryResultDto` | `POST /inquiries` — `{ leadId, leadNumber }` |
| `OfflineSyncResultDto` | `POST /engineer/visits/sync` — `{ accepted[], rejected[] }` with per-item errors |
| `GenerateInvoiceResultDto` | `POST .../generate` — `{ invoiceId, invoiceNumber, pdfFileId }` |

---

## 7. Dashboard / aggregation DTOs (NEW)

| DTO | Endpoint |
|-----|----------|
| `AdminDashboardDto` | `/admin/dashboard` — widget payloads per [dashboard-design.md](./dashboard-design.md) |
| `CustomerPortalDashboardDto` | `/portal/dashboard` |
| `EngineerDashboardDto` | `/engineer/dashboard` |
| `LeadPipelineReportDto` | `/reports/leads` |
| `AmcExpiryReportDto` | `/reports/amc` |
| `VisitCompletionReportDto` | `/reports/visits` |
| `TicketPriorityReportDto` | `/reports/tickets` |
| `InvoiceAgingReportDto` | `/reports/invoices` |
| `EngineerWorkloadReportDto` | `/reports/engineers` |

Widget sub-DTOs: `CountCardDto`, `QueueItemDto`, `ExpiryBucketDto` — built on platform-ui card contracts (presentation layer maps these in LLD).

---

## 8. Mobile DTO considerations

Mobile consumes **the same DTOs** via OpenAPI-generated Dart classes — no parallel mobile DTO layer.

| Concern | Approach |
|---------|----------|
| Payload size | List endpoints use **Summary DTOs**; detail fetched on navigation |
| Offline sync | `OfflineVisitSyncBatchRequest` adds `clientCorrelationId` + `capturedAt` per evidence item |
| File references | Upload via platform Files first; sync batch sends `fileId` only after background upload completes |
| Optimistic concurrency | Mobile stores `rowVersion` from last fetch; sends on mutating calls |
| Partial connectivity | Engineer app caches `ScheduleSummaryDto`, `VisitDetailDto` reads; writes queue locally |

---

## 9. Enum DTOs (serialized as strings)

All enums serialize as **PascalCase strings** matching CHECK constraints in [database-naming-standards.md](./database-naming-standards.md):

| Enum | Values (sample) |
|------|-----------------|
| `LeadStatus` | `New`, `Contacted`, `Qualified`, `QuotationSent`, `Negotiation`, `Won`, `Lost`, `Converted` |
| `ScheduleStatus` | `Planned`, `Assigned`, `InProgress`, `Completed`, `Missed`, `Cancelled` |
| `VisitReportStatus` | `Started`, `EvidenceCaptured`, `Submitted`, `Approved`, `Returned` |
| `TicketStatus` | `Open`, `Assigned`, `InProgress`, `Resolved`, `Closed`, `Reopened` |
| `TicketPriority` | `Low`, `Medium`, `High`, `Critical` |
| `InvoiceStatus` | `Draft`, `Generated`, `Sent`, `Paid`, `Cancelled` |
| `InvoiceType` | `AmcRenewal`, `NewAmc`, `EmergencyService`, `SpareReplacement`, `AdditionalCharges`, `Other` |
| `PhotoCategory` | `Before`, `During`, `After`, `Selfie` |

---

## 10. Validation rules (DTO-level — LLD will expand)

| Rule | DTO / field |
|------|-------------|
| Max 3 contacts | `UpsertSiteContactsRequest.contacts` |
| Required AMC term link | `CreateInvoiceRequest.amcContractTermId` when type is `AmcRenewal` or `NewAmc` |
| GPS range | `CaptureVisitLocationRequest` lat ∈ [-90,90], lng ∈ [-180,180] |
| Money precision | All amounts `decimal(18,2)` — never float in JSON (string or number per OpenAPI config) |
| Reopen reason required | `ReopenTicketRequest.reason` min length 10 |

FluentValidation validators co-located in `{Module}.Application/Validators/` (platform `ValidationBehavior`).

---

Related: [endpoint-catalog.md](./endpoint-catalog.md) · [api-architecture.md](./api-architecture.md) · [openapi-roadmap.md](./openapi-roadmap.md)
