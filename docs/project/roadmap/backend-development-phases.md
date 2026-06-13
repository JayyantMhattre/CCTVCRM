# Backend Development Phases

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-8
**Stack:** .NET 10 · PostgreSQL · MediatR · Minimal APIs · schema-per-module
**Platform:** Register at Host Layer 2; observers unchanged ([add-backend-module.md](../../extending/add-backend-module.md))

---

## Phase D1 — Foundation (prerequisite for B1)

| Deliverable | Details |
|-------------|---------|
| Projects | `CctvCrm.*` module skeletons (Lead, Customer, Amc, Service, Ticket, Engineer, Invoice, Reporting) |
| Host wiring | `ModuleExtensions.cs` registration order |
| RBAC | Seed `Engineer`, `Customer` roles + 30 permissions ([permission-catalog.md](../design/permission-catalog.md)) |
| SharedKernel | `Contracts/CctvCrm/` namespace stub |
| Health | `GET /api/v1/cctv/health` |
| Integrations | SMS adapter interface; PDF service interface; CCTV email template placeholders |
| Tests | Architecture boundary tests for all projects |

**Dependencies:** Platform Auth, Outbox, BuildingBlocks  
**Not in scope:** Business logic

---

## Phase B1 — Lead Management

| Aspect | Detail |
|--------|--------|
| **Schema** | `cctv_lead` |
| **Entities** | Lead, LeadActivity, LeadRemark, LeadAttachment |
| **APIs** | `/inquiries`, `/leads/*` ([endpoint-catalog.md](../design/endpoint-catalog.md)) |
| **Events** | LeadCreated, LeadStatusChanged, LeadConverted, LeadLost |
| **Notifications** | Lead Created → admin; Lead Converted → admin + customer welcome |
| **Audit** | All domain events + EF interceptor |
| **Dependencies** | D1; conversion contracts stub until B2/B3 |
| **Complexity** | M (2–3 pw) |

**Exit:** Website inquiry creates lead; admin manages pipeline; convert creates customer+site (contract when B3 ready).

---

## Phase B2 — Customer · Site · Asset

| Aspect | Detail |
|--------|--------|
| **Schema** | `cctv_customer` |
| **Entities** | Customer, Site, SiteContact, SiteDocument, SiteAssetSummary |
| **APIs** | `/customers/*`, `/sites/*`, asset-summary, `/portal/profile` |
| **Events** | CustomerCreated, SiteCreated, SiteContactChanged, SiteAssetSummaryUpdated |
| **Notifications** | (none mandatory beyond conversion welcome) |
| **Audit** | Standard |
| **Dependencies** | B1 (conversion consumer) |
| **Complexity** | M (3–4 pw) |

**Exit:** Customer/site CRUD; max 3 contacts; asset summary; customer scoped reads.

---

## Phase B3 — AMC Plans · Contracts · Terms

| Aspect | Detail |
|--------|--------|
| **Schema** | `cctv_amc` |
| **Entities** | AMCPlan, AMCPlanVersion, AMCContract, AMCContractTerm, AMCContractDocument |
| **APIs** | `/amc-plans/*`, `/contracts/*`, renewal-request, renewal queue |
| **Events** | PlanVersionPublished, ContractCreated, TermActivated, TermExpired, RenewalRequested, ExpiryReminderDue |
| **Notifications** | AMC Expiry Reminder; completes lead conversion welcome path |
| **Audit** | Standard |
| **Dependencies** | B2 (site/customer) |
| **Complexity** | L (4–5 pw) |

**Exit:** Full lead conversion; term activation publishes schedule-generation event.

---

## Phase B4 — Scheduling · Visits · Approval

| Aspect | Detail |
|--------|--------|
| **Schema** | `cctv_service` |
| **Entities** | ServiceSchedule, EngineerAssignment, ServiceVisit, VisitPhoto, VisitLocation, VisitSignature, VisitApproval, VisitAttachment |
| **APIs** | `/schedules/*`, `/visits/*`, `/engineer/visits/sync`, approval queue |
| **Events** | ScheduleCreated/Assigned/Rescheduled/Cancelled/Missed; VisitStarted/Submitted/Approved/Returned/Completed |
| **Notifications** | Visit Scheduled, Visit Completed |
| **Audit** | Standard; high-volume photo rows |
| **Dependencies** | B3 (term activation → schedules); engineer lookup (B5 or stub) |
| **Complexity** | L (5–6 pw) |

**Exit:** Auto schedules; mandatory assignment; visit checklist enforced; admin approve/return; customer sees approved only.

---

## Phase B5 — Tickets · Engineer Operations

| Aspect | Detail |
|--------|--------|
| **Schemas** | `cctv_ticket`, `cctv_engineer` |
| **Entities** | Ticket + composition; Engineer |
| **APIs** | `/tickets/*`, `/engineers/*` |
| **Events** | TicketCreated/Assigned/StatusChanged/Closed/Reopened; EngineerCreated/Deactivated |
| **Notifications** | Ticket Created, Assigned, Closed |
| **Audit** | Standard + TicketStatusHistory as business history |
| **Dependencies** | B2; B4 optional for visit-origin tickets |
| **Complexity** | M (3–4 pw) |

**Exit:** Full ticket lifecycle; engineer master; workload reads.

---

## Phase B6 — Invoices · PDF Generation

| Aspect | Detail |
|--------|--------|
| **Schema** | `cctv_invoice` |
| **Entities** | Invoice, InvoiceLine, InvoiceAttachment, InvoiceStatusHistory |
| **APIs** | `/invoices/*` |
| **Events** | InvoiceCreated/Generated/Sent/Paid/Cancelled |
| **Notifications** | Invoice Generated |
| **PDF** | Invoice PDF + complete PDF service for contract/visit report ([pdf-document-design.md](../design/lld/pdf-document-design.md)) |
| **Dependencies** | B2, B3; optional ticket/visit refs |
| **Complexity** | M (3–4 pw) |

**Exit:** Option B invoicing; generate PDF → Files; customer download authorized.

---

## Phase B7 — Reports · Analytics

| Aspect | Detail |
|--------|--------|
| **Schema** | None (read-only module) |
| **APIs** | `/reports/*`, `/admin/dashboard`, `/portal/dashboard`, `/engineer/dashboard` |
| **Events** | Consumes only (optional cache invalidation) |
| **Dependencies** | All B1–B6 |
| **Complexity** | M (2–3 pw) |

**Exit:** All reports in [report-specification.md](../design/lld/report-specification.md); dashboard aggregation APIs.

---

## Cross-phase backend checklist (every phase)

- [ ] DbContext + migrations in module schema
- [ ] `*Module.cs` + `*Endpoints.cs` registered
- [ ] FluentValidation for all commands
- [ ] Permissions on every endpoint
- [ ] Domain events → Outbox → MediatR
- [ ] Notification handlers for §17 events in scope
- [ ] Webhook catalog entries for new events
- [ ] 7-file module docs
- [ ] OpenAPI `WithSummary`/`WithTags` on endpoints
- [ ] Integration tests for happy path + key business rules

---

Related: [database-implementation-plan.md](./database-implementation-plan.md) · [integration-roadmap.md](./integration-roadmap.md)
