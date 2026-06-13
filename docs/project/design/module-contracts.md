# Module Contracts

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-6 — API Architecture, Module Contracts & Integration Design
**Contract location (implementation):** `BackEnd/src/Shared/Ashraak.SharedKernel.Contracts/CctvCrm/` (new namespace — follows platform SharedKernel pattern)

> Modules communicate **only** via these contracts. No Infrastructure cross-references. HTTP APIs are the external surface; contracts are the internal cross-module surface.

---

## Contract conventions

| Aspect | Standard |
|--------|----------|
| Interface prefix | `I` + domain + role, e.g. `ILeadConversionService` |
| DTO suffix | `{Entity}Dto`, `{Entity}SummaryDto`, `{Entity}DetailDto` |
| Events | `{Entity}{Action}Event` in `Contracts/CctvCrm/Events/` — implement `IDomainEvent` |
| Queries | `{Entity}LookupDto` for cross-module read models |
| Async | All methods `Task<T>` with `CancellationToken` |
| Results | `Result<T>` from BuildingBlocks for command outcomes |

---

## 1. Lead Management

| Aspect | Detail |
|--------|--------|
| **Purpose** | Capture prospects, pipeline management, conversion orchestration |
| **Ownership** | Schema `cctv_lead`; aggregate `Lead` |
| **Dependencies** | `ICustomerProvisioningService`, `ISiteProvisioningService`, `IAmcContractProvisioningService` (downstream); platform Notifications (via events) |
| **HTTP** | `/api/v1/cctv/leads`, `/api/v1/cctv/inquiries` |

### Contracts (published)

| Interface | Methods | Consumers |
|-----------|---------|-----------|
| `ILeadLookupService` | `GetLeadAsync(id)`, `GetLeadByNumberAsync(number)` | Reporting |
| `ILeadConversionService` | `ConvertLeadAsync(ConvertLeadCommand)` → `LeadConversionResultDto` | Lead module (HTTP handler) |

### Events (published)

| Event | When |
|-------|------|
| `LeadCreatedEvent` | Website inquiry or admin create |
| `LeadStatusChangedEvent` | Pipeline transition |
| `LeadConvertedEvent` | Successful conversion (includes created entity ids) |
| `LeadLostEvent` | Marked Lost |

### External integrations

| Integration | Class |
|-------------|-------|
| Anonymous website forms | `POST /api/v1/cctv/inquiries` |
| Platform rate limiting | Anonymous inquiry protection |
| Platform Files | Lead attachments via `fileId` |

---

## 2. Customer Management

| Aspect | Detail |
|--------|--------|
| **Purpose** | Customer master records; portal account linkage |
| **Ownership** | `customers` table in `cctv_customer` |
| **Dependencies** | `IUserService` (platform Users — account link); Lead module (conversion input) |
| **HTTP** | `/api/v1/cctv/customers`, `/api/v1/cctv/portal/profile` |

### Contracts

| Interface | Methods | Consumers |
|-----------|---------|-----------|
| `ICustomerLookupService` | `GetCustomerAsync(id)`, `GetCustomerForUserAsync(userId)` | AMC, Ticket, Invoice, Reporting, Portal |
| `ICustomerProvisioningService` | `CreateCustomerAsync(CreateCustomerDto)` | Lead conversion |
| `ICustomerProfileService` | `UpdateOwnProfileAsync(userId, dto)` | Customer portal |

### Events

| Event | When |
|-------|------|
| `CustomerCreatedEvent` | Admin create or lead conversion |
| `CustomerDeactivatedEvent` | Admin deactivation |
| `CustomerProfileUpdatedEvent` | Self-service or admin update |

### External integrations

| Integration | Class |
|-------------|-------|
| Platform Users | `users:read/write` for account management |
| Platform Auth | Portal login identity |

---

## 3. Site Management

| Aspect | Detail |
|--------|--------|
| **Purpose** | Physical locations; contacts (≤3); site documents; aggregation hub |
| **Ownership** | `sites`, `site_contacts`, `site_documents` in `cctv_customer` |
| **Dependencies** | `ICustomerLookupService`; AMC/Service/Ticket/Invoice (logical refs) |
| **HTTP** | `/api/v1/cctv/sites` |

### Contracts

| Interface | Methods | Consumers |
|-----------|---------|-----------|
| `ISiteLookupService` | `GetSiteAsync(id)`, `GetSitesForCustomerAsync(customerId)`, `ValidateSiteOwnershipAsync(siteId, customerId)` | All modules referencing sites |
| `ISiteProvisioningService` | `CreateSiteAsync(CreateSiteDto)` | Lead conversion |
| `ISiteContactService` | `UpsertContactsAsync(siteId, contacts)` — enforces max 3 | Site HTTP handlers |

### Events

| Event | When |
|-------|------|
| `SiteCreatedEvent` | Admin or conversion |
| `SiteUpdatedEvent` | Address/status change |
| `SiteContactChangedEvent` | Contact CRUD |

### External integrations

| Integration | Class |
|-------------|-------|
| Platform Files | Site documents |

---

## 4. Asset Management

| Aspect | Detail |
|--------|--------|
| **Purpose** | Summary asset counts per site (no device tracking) |
| **Ownership** | `site_asset_summaries` in `cctv_customer` (1:1 with Site) |
| **Dependencies** | `ISiteLookupService` |
| **HTTP** | `GET/PUT /api/v1/cctv/sites/{siteId}/asset-summary` |

### Contracts

| Interface | Methods | Consumers |
|-----------|---------|-----------|
| `ISiteAssetSummaryService` | `GetSummaryAsync(siteId)`, `UpdateSummaryAsync(siteId, dto)` | Site module (same backend slice) |
| `ISiteAssetSummaryLookup` | `GetSummaryAsync(siteId)` | Visit (context display), Reporting |

### Events

| Event | When |
|-------|------|
| `SiteAssetSummaryUpdatedEvent` | Admin updates counts |

### External integrations

None beyond Site module.

---

## 5. AMC Management (Plans + Contracts)

| Aspect | Detail |
|--------|--------|
| **Purpose** | Versioned plan catalog; master+terms contracts; renewal; expiry |
| **Ownership** | Schema `cctv_amc` |
| **Dependencies** | `ISiteLookupService`, `ICustomerLookupService`; Service (schedule generation); Invoice (term link); platform Files (PDF) |
| **HTTP** | `/api/v1/cctv/amc-plans`, `/api/v1/cctv/contracts` |

### Contracts

| Interface | Methods | Consumers |
|-----------|---------|-----------|
| `IAmcPlanLookupService` | `GetPlanVersionAsync(versionId)`, `GetPublishedVersionsAsync(planId)` | Contract, Reporting |
| `IAmcContractLookupService` | `GetActiveTermForSiteAsync(siteId)`, `GetContractAsync(id)` | Service, Invoice, Portal |
| `IAmcContractProvisioningService` | `CreateInitialContractAsync(dto)` | Lead conversion |
| `IAmcTermService` | `ActivateTermAsync`, `RenewTermAsync`, `CancelContractAsync` | Contract HTTP |
| `IAmcRenewalRequestService` | `SubmitRenewalRequestAsync(contractId, customerId)` | Customer portal |

### Events

| Event | When |
|-------|------|
| `AmcPlanVersionPublishedEvent` | Admin publishes version |
| `AmcContractCreatedEvent` | New master contract |
| `AmcContractTermActivatedEvent` | Term goes Active → triggers schedule generation |
| `AmcContractTermExpiredEvent` | Term ends |
| `AmcRenewalRequestedEvent` | Customer requests renewal |
| `AmcExpiryReminderDueEvent` | Scheduled job (30/60/90 day) |

### External integrations

| Integration | Class |
|-------------|-------|
| Platform Files | Contract PDF storage |
| Service module | Subscribes to `AmcContractTermActivatedEvent` |

---

## 6. Scheduling (Service Scheduling)

| Aspect | Detail |
|--------|--------|
| **Purpose** | Auto-generated and ad-hoc visit schedules; engineer assignment |
| **Ownership** | `service_schedules`, `engineer_assignments` in `cctv_service` |
| **Dependencies** | `IAmcContractLookupService`, `IEngineerLookupService`, `ISiteLookupService` |
| **HTTP** | `/api/v1/cctv/schedules` |

### Contracts

| Interface | Methods | Consumers |
|-----------|---------|-----------|
| `IScheduleLookupService` | `GetScheduleAsync(id)`, `GetSchedulesForEngineerAsync(engineerId, dateRange)` | Visit, Portal, Engineer app |
| `IScheduleGenerationService` | `GenerateSchedulesForTermAsync(termId)` | AMC event handler |
| `IScheduleAssignmentService` | `AssignEngineerAsync(scheduleId, engineerId)` | Admin HTTP |

### Events

| Event | When |
|-------|------|
| `VisitScheduleCreatedEvent` | Auto or ad-hoc create |
| `VisitScheduleAssignedEvent` | Engineer assigned (mandatory before execution) |
| `VisitScheduleRescheduledEvent` | Admin reschedule |
| `VisitScheduleCancelledEvent` | Admin cancel |
| `VisitScheduleMissedEvent` | System job detects overdue |

### External integrations

| Integration | Class |
|-------------|-------|
| Notifications | Visit Scheduled event |

---

## 7. Visit Management

| Aspect | Detail |
|--------|--------|
| **Purpose** | Field execution, evidence capture, admin approval, report PDF |
| **Ownership** | `service_visits`, visit evidence tables in `cctv_service` |
| **Dependencies** | `IScheduleLookupService`, `IEngineerLookupService`; platform Files; Ticket (raise during visit) |
| **HTTP** | `/api/v1/cctv/visits`, `/api/v1/cctv/engineer/visits` |

### Contracts

| Interface | Methods | Consumers |
|-----------|---------|-----------|
| `IVisitLookupService` | `GetVisitAsync(id)`, `GetApprovedVisitsForSiteAsync(siteId)` | Portal, Reporting |
| `IVisitExecutionService` | `StartVisitAsync`, `CaptureEvidenceAsync`, `SubmitReportAsync` | Engineer HTTP/mobile |
| `IVisitApprovalService` | `ApproveAsync`, `ReturnAsync` | Admin HTTP |

### Events

| Event | When |
|-------|------|
| `VisitStartedEvent` | Engineer starts |
| `VisitEvidenceCapturedEvent` | Each evidence item (optional granular) |
| `VisitReportSubmittedEvent` | Submitted for approval |
| `VisitReportApprovedEvent` | Admin approves → customer visibility |
| `VisitReportReturnedEvent` | Admin returns for rework |
| `VisitCompletedEvent` | Schedule marked Completed |

### External integrations

| Integration | Class |
|-------------|-------|
| Platform Files | Photos, selfie, signature, video, report PDF |
| Ticket module | `CreateTicketDuringVisit` command |
| Mobile offline sync | Idempotent submit with client correlation id |

---

## 8. Ticket Management

| Aspect | Detail |
|--------|--------|
| **Purpose** | Complaint lifecycle; tri-actor creation; customer reopen |
| **Ownership** | Schema `cctv_ticket` |
| **Dependencies** | `ICustomerLookupService`, `ISiteLookupService`, `IEngineerLookupService` |
| **HTTP** | `/api/v1/cctv/tickets` |

### Contracts

| Interface | Methods | Consumers |
|-----------|---------|-----------|
| `ITicketLookupService` | `GetTicketAsync(id)`, `GetTicketsForCustomerAsync(customerId)` | Reporting, Portal |
| `ITicketAssignmentService` | `AssignEngineerAsync(ticketId, engineerId)` | Admin HTTP |
| `ITicketLifecycleService` | `CreateAsync`, `UpdateStatusAsync`, `ReopenAsync`, `CloseAsync` | All actor HTTP handlers |

### Events

| Event | When |
|-------|------|
| `TicketCreatedEvent` | Customer/Admin/Engineer create |
| `TicketAssignedEvent` | Engineer assigned |
| `TicketStatusChangedEvent` | Any transition |
| `TicketClosedEvent` | Admin closes |
| `TicketReopenedEvent` | Customer reopens |

### External integrations

| Integration | Class |
|-------------|-------|
| Platform Files | Ticket attachments |

---

## 9. Engineer Management

| Aspect | Detail |
|--------|--------|
| **Purpose** | Engineer records; workload; platform account link |
| **Ownership** | Schema `cctv_engineer` |
| **Dependencies** | Platform Users/Auth |
| **HTTP** | `/api/v1/cctv/engineers` |

### Contracts

| Interface | Methods | Consumers |
|-----------|---------|-----------|
| `IEngineerLookupService` | `GetEngineerAsync(id)`, `GetEngineerForUserAsync(userId)`, `GetActiveEngineersAsync()` | Scheduling, Ticket, Visit |
| `IEngineerManagementService` | `CreateAsync`, `UpdateAsync`, `DeactivateAsync` | Admin HTTP |

### Events

| Event | When |
|-------|------|
| `EngineerCreatedEvent` | Admin create |
| `EngineerDeactivatedEvent` | Admin deactivate |

### External integrations

| Integration | Class |
|-------------|-------|
| Platform Users | Engineer portal account |

---

## 10. Invoice Management (Option B)

| Aspect | Detail |
|--------|--------|
| **Purpose** | Invoice lifecycle; diverse billable types; PDF; manual paid status |
| **Ownership** | Schema `cctv_invoice` |
| **Dependencies** | `ICustomerLookupService`, `IAmcContractLookupService`; platform Files |
| **HTTP** | `/api/v1/cctv/invoices` |

### Contracts

| Interface | Methods | Consumers |
|-----------|---------|-----------|
| `IInvoiceLookupService` | `GetInvoiceAsync(id)`, `GetInvoicesForCustomerAsync(customerId)` | Portal, Reporting |
| `IInvoiceLifecycleService` | `CreateDraftAsync`, `GenerateAsync`, `SendAsync`, `MarkPaidAsync`, `CancelAsync` | Admin HTTP |

### Events

| Event | When |
|-------|------|
| `InvoiceCreatedEvent` | Draft created |
| `InvoiceGeneratedEvent` | PDF generated, status Generated |
| `InvoiceSentEvent` | Marked Sent |
| `InvoicePaidEvent` | Manual paid |
| `InvoiceCancelledEvent` | Cancelled |

### External integrations

| Integration | Class |
|-------------|-------|
| Platform Files | Invoice PDF |
| Optional refs | `ticketId`, `serviceVisitId`, `amcContractTermId` |

---

## 11. Reporting

| Aspect | Detail |
|--------|--------|
| **Purpose** | Read-only aggregations for admin dashboards and report views |
| **Ownership** | No schema — query-only module |
| **Dependencies** | All `*LookupService` contracts above |
| **HTTP** | `/api/v1/cctv/reports`, `/api/v1/cctv/admin/dashboard` |

### Contracts

| Interface | Methods | Consumers |
|-----------|---------|-----------|
| `IReportingQueryService` | `GetLeadPipelineSummaryAsync`, `GetAmcExpirySummaryAsync`, … | Reporting HTTP |
| `IAdminDashboardQueryService` | `GetDashboardAsync()` | Admin dashboard HTTP |

### Events

Reporting **consumes** events for cache invalidation only (optional V1) — publishes none.

### External integrations

None.

---

## Cross-module orchestration patterns

| Flow | Pattern |
|------|---------|
| Lead conversion | Lead module command → calls provisioning contracts → single Outbox transaction per aggregate save → `LeadConvertedEvent` |
| Term activation → schedules | AMC publishes `AmcContractTermActivatedEvent` → Service handler calls `GenerateSchedulesForTermAsync` |
| Visit submit → approval queue | Visit module state change → `VisitReportSubmittedEvent` |
| Invoice generate → PDF | Invoice command → internal PDF renderer → `POST` equivalent Files store → `InvoiceGeneratedEvent` |

---

Related: [api-architecture.md](./api-architecture.md) · [event-catalog.md](./event-catalog.md) · [endpoint-catalog.md](./endpoint-catalog.md)
