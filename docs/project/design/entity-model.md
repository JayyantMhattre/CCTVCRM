# Entity Model

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-4 — Entity Model, ER Diagram & Database Architecture (design only — no code)
**Source of truth:** [requirements-freeze-v1.md](../requirements-freeze-v1.md) · [business-rules.md](../business-rules.md) · [module-architecture.md](../module-architecture.md)

> Design-only document. No EF entities, migrations, APIs, or UI. All entities derive from the frozen requirements and the mandated D0-4 design decisions (Option B invoicing, FileId references, summary-only assets).

---

## 1. Domain map

| Domain | Owning module(s) | Entities |
|--------|------------------|----------|
| Lead | Lead Management (2) | Lead, LeadActivity, LeadRemark, LeadAttachment |
| Customer | Customer (3) / Site (4) / Asset (5) Management | Customer, Site, SiteContact, SiteDocument, SiteAssetSummary |
| AMC | AMC Plans (6) / AMC Contracts (7) | AMCPlan, AMCPlanVersion, AMCContract, AMCContractTerm, AMCContractDocument |
| Service | Service Scheduling (8) / Visit Management (9) | ServiceSchedule, EngineerAssignment, ServiceVisit, VisitPhoto, VisitLocation, VisitSignature, VisitApproval, VisitAttachment |
| Ticket | Ticket Management (10) | Ticket, TicketComment, TicketAttachment, TicketAssignment, TicketStatusHistory |
| Engineer | Engineer Management (11) | Engineer |
| Invoice | Invoice Management (12) | Invoice, InvoiceLine, InvoiceAttachment, InvoiceStatusHistory |
| Platform (reused — **not redesigned**) | Ashraak Core | FileRecord (Files), User/Role/Permission (Auth), Audit entries, Notification dispatch |

Reporting (13) and the Portals (14, 15) own **no entities** — they read from the domains above.

---

## 2. Entity catalog

### 2.1 Lead domain

#### Lead *(aggregate root)*
| Aspect | Detail |
|--------|--------|
| Responsibility | A sales prospect from website inquiry (Contact / AMC Inquiry / Get Quote) or manual entry; carries the pipeline status (freeze §10) |
| Key data | Lead number, source, contact details (name, organization, email, phone, city/address), requirement summary, status, owner (admin user) |
| Status | New · Contacted · Qualified · Quotation Sent · Negotiation · Won · Lost · Converted (BR-LEAD-01) |
| Relationships | 1:N LeadActivity, LeadRemark, LeadAttachment; on conversion holds references to the created Customer, Site, and AMCContract (BR-LEAD-03) |
| Lifecycle ownership | Created by website submission (automatic, BR-LEAD-02) or Admin; updated by Admin; conversion is terminal alongside Lost |

#### LeadActivity
| Aspect | Detail |
|--------|--------|
| Responsibility | Chronological record of pipeline actions (status changes, calls, quotation sent, negotiation notes) |
| Key data | Activity type, from/to status (for transitions), description, performed by, occurred at |
| Relationships | N:1 Lead (composition) |
| Lifecycle | Append-only; never updated or deleted |

#### LeadRemark
| Aspect | Detail |
|--------|--------|
| Responsibility | Free-text remarks/notes on a lead |
| Relationships | N:1 Lead (composition) |
| Lifecycle | Created by Admin; append-only |

#### LeadAttachment
| Aspect | Detail |
|--------|--------|
| Responsibility | Files attached to a lead (e.g. quotation document) — stored via **FileId → platform FileRecord** (no paths) |
| Key data | FileId, title, uploaded by |
| Relationships | N:1 Lead (composition); logical reference to platform Files |
| Lifecycle | Created by Admin; removable while lead is open |

### 2.2 Customer domain

#### Customer *(aggregate root)*
| Aspect | Detail |
|--------|--------|
| Responsibility | The contracting party; anchor for sites, contracts, tickets, invoices (freeze §5) |
| Key data | Customer number, name, contact details, billing address, portal login reference (platform Auth user id), originating lead reference (when converted), status |
| Relationships | 1:N Site (BR-STRUCT-02); referenced by AMCContract, Ticket, Invoice |
| Lifecycle ownership | Created by Admin or by lead conversion (BR-LEAD-03); updated by Admin; profile fields self-served by Customer (BR-AUTH-05); deactivated, never hard-deleted |

#### Site *(aggregate root)*
| Aspect | Detail |
|--------|--------|
| Responsibility | A physical customer location holding contacts, asset summary, AMC contract, visits, tickets, invoices (freeze §5) |
| Key data | Site number, name, address, city, status |
| Relationships | N:1 Customer (BR-STRUCT-01: one site → one customer); 1:N SiteContact (**max 3**, BR-STRUCT-03); 1:1 SiteAssetSummary; 1:N SiteDocument; at most **one active** AMCContract (BR-AMC-02) |
| Lifecycle ownership | Created by Admin or lead conversion; updated by Admin; deactivated, never hard-deleted |

#### SiteContact
| Aspect | Detail |
|--------|--------|
| Responsibility | Contact person at a site (name, designation, phone, email, primary flag) |
| Constraint | **Maximum 3 per site** (BR-STRUCT-03) — enforced in the aggregate (application invariant) with DB support documented in [database-naming-standards.md](./database-naming-standards.md) |
| Relationships | N:1 Site (composition) |
| Lifecycle | Managed by Admin within the Site aggregate |

#### SiteDocument
| Aspect | Detail |
|--------|--------|
| Responsibility | Documents attached to a site (layouts, agreements) — **FileId → platform FileRecord** |
| Relationships | N:1 Site (composition) |
| Lifecycle | Managed by Admin |

#### SiteAssetSummary
| Aspect | Detail |
|--------|--------|
| Responsibility | **Summary counts only** (freeze §7, mandated): Camera, DVR, NVR, Hard Disk, Switch, Router, Monitor counts + optional Brand, Model, Remarks. **Individual cameras are NOT tracked.** |
| Relationships | 1:1 Site (composition; one summary per site) |
| Lifecycle | Created with the site; counts updated by Admin (e.g. after visits report changes) |

### 2.3 AMC domain

#### AMCPlan *(aggregate root)*
| Aspect | Detail |
|--------|--------|
| Responsibility | Plan identity (e.g. Silver, Gold, Platinum) and lifecycle status (freeze §9) |
| Key data | Plan code, name, description, status (Active/Retired) |
| Relationships | 1:N AMCPlanVersion (composition) |
| Lifecycle ownership | Admin only (BR-VISIT-07: engineers cannot manage plans) |

#### AMCPlanVersion
| Aspect | Detail |
|--------|--------|
| Responsibility | **Immutable versioned snapshot** of plan commercial terms: Price, Visit Frequency, Included Services, SLA (BR-AMC-05, BR-AMC-06) |
| Invariant | Once referenced by any contract term, a version is frozen — historical contracts never change when plans are modified (BR-AMC-07) |
| Relationships | N:1 AMCPlan (composition); referenced by AMCContractTerm (pinning) |
| Lifecycle | Created as new version on every plan change; never updated after activation; never deleted |

#### AMCContract *(aggregate root)*
| Aspect | Detail |
|--------|--------|
| Responsibility | **Permanent master contract record** for a site (freeze §8, mandated Master + Terms model) |
| Key data | Contract number, site reference, customer reference, status (Active/Expired/Cancelled) |
| Invariant | **One active contract per site** (BR-AMC-02) |
| Relationships | N:1 Site; 1:N AMCContractTerm (composition); 1:N AMCContractDocument |
| Lifecycle ownership | Created by lead conversion (initial contract, BR-LEAD-03) or Admin; managed by Admin only; never hard-deleted (permanent record) |

#### AMCContractTerm
| Aspect | Detail |
|--------|--------|
| Responsibility | **Renewal-history record** — one row per contracted period (Term 2026, 2027, …) pinning a plan version and price (freeze §8) |
| Key data | Term number/sequence, plan version reference, start/end dates, agreed price, status, origin (New/Renewal), renewal-request marker (customer-requested, BR-AMC-08) |
| Invariant | One active term per contract; customer sees **active term only**, admin sees full history (BR-AMC-03/04) |
| Relationships | N:1 AMCContract (composition); N:1 AMCPlanVersion (pin); referenced by ServiceSchedule and (optionally) Invoice |
| Lifecycle | Created on contract creation/renewal by Admin; status moves with dates; never deleted (history) |

#### AMCContractDocument
| Aspect | Detail |
|--------|--------|
| Responsibility | Contract artifacts — generated AMC Contract PDF (freeze §19), signed copies — **FileId → platform FileRecord** |
| Relationships | N:1 AMCContract; optional N:1 AMCContractTerm (term-specific documents) |
| Lifecycle | Generated by system / uploaded by Admin; immutable once issued |

### 2.4 Service domain

#### ServiceSchedule *(aggregate root)*
| Aspect | Detail |
|--------|--------|
| Responsibility | A planned preventive-maintenance visit slot, **auto-generated from AMC plan visit frequency** (BR-SCHED-02) |
| Key data | Schedule number, contract term reference, site reference, scheduled date, sequence within term, status, reschedule trail (previous date) |
| Status | Planned · Assigned · In Progress · Completed · Missed · Cancelled (BR-SCHED-01) |
| Relationships | N:1 AMCContractTerm; 1:N EngineerAssignment; 0..1 ServiceVisit (the execution) |
| Lifecycle ownership | System-generated; Admin reschedules/cancels (BR-SCHED-03); status driven by assignment and visit execution |

#### EngineerAssignment
| Aspect | Detail |
|--------|--------|
| Responsibility | Assignment of an engineer to a schedule — **mandatory before execution** (BR-SCHED-04); reassignment history preserved |
| Key data | Engineer reference, assigned by, assigned at, active flag |
| Relationships | N:1 ServiceSchedule (composition); N:1 Engineer (logical reference) |
| Lifecycle | Created by Admin; superseded (not deleted) on reassignment |

#### ServiceVisit *(aggregate root)*
| Aspect | Detail |
|--------|--------|
| Responsibility | The actual visit execution and report: remarks + evidence container (freeze §12) |
| Key data | Schedule reference, engineer reference, started/completed timestamps, **mandatory remarks**, report status, generated Visit Report PDF (FileId) |
| Completion invariant | Cannot complete without: selfie, GPS coordinates, ≥1 visit photo, customer signature, remarks (BR-VISIT-01) — enforced at the aggregate boundary |
| Relationships | 1:1 ServiceSchedule; 1:N VisitPhoto, VisitAttachment, VisitApproval; 1:1 VisitLocation, VisitSignature |
| Lifecycle ownership | Created/submitted by Engineer; approved by Admin (VisitApproval); customer-visible **only after approval** (BR-VISIT-04/05) |

#### VisitPhoto
| Aspect | Detail |
|--------|--------|
| Responsibility | Visit photos by category: **Before, During, After, Selfie** (BR-VISIT-01/03) — **FileId → platform FileRecord** |
| Key data | FileId, category, caption, captured at |
| Relationships | N:1 ServiceVisit (composition) |
| Lifecycle | Uploaded by Engineer (offline-capable, freeze §18); immutable after report approval |

#### VisitLocation
| Aspect | Detail |
|--------|--------|
| Responsibility | GPS evidence: **latitude, longitude, timestamp** (BR-VISIT-02) |
| Relationships | 1:1 ServiceVisit (composition) |
| Lifecycle | Captured by Engineer at the visit; immutable |

#### VisitSignature
| Aspect | Detail |
|--------|--------|
| Responsibility | Customer signature captured on the engineer's device — **FileId → platform FileRecord** (signature image) + signer name + captured at |
| Relationships | 1:1 ServiceVisit (composition) |
| Lifecycle | Captured by Engineer; immutable |

#### VisitApproval
| Aspect | Detail |
|--------|--------|
| Responsibility | Admin review decision record for a submitted report (freeze §13): pending → approved / returned, reviewer, time, remarks |
| Relationships | N:1 ServiceVisit (composition; history of review rounds) |
| Lifecycle | Created on submission; decided by Admin; append-only history |

#### VisitAttachment
| Aspect | Detail |
|--------|--------|
| Responsibility | Additional visit media — **videos** and report files (BR-VISIT-06) — **FileId → platform FileRecord** |
| Relationships | N:1 ServiceVisit (composition) |
| Lifecycle | Uploaded by Engineer; immutable after approval |

### 2.5 Ticket domain

#### Ticket *(aggregate root)*
| Aspect | Detail |
|--------|--------|
| Responsibility | A complaint/service request raised by Customer, Admin, or Engineer (during a visit) (BR-TKT-03..05) |
| Key data | Ticket number, customer + site references, subject, description, priority (Low/Medium/High/Critical), status, source (Customer/Admin/EngineerVisit), originating visit reference (optional), resolved/closed timestamps |
| Status | Open · Assigned · In Progress · Resolved · Closed · Reopened (BR-TKT-01) |
| Relationships | N:1 Customer, N:1 Site (logical); optional N:1 ServiceVisit (raised-during); 1:N TicketComment, TicketAttachment, TicketAssignment, TicketStatusHistory |
| Lifecycle ownership | Created by any of the three actors; assigned/progressed by Admin/Engineer; closed by Admin; **reopened by Customer** (BR-TKT-06) |

#### TicketComment
| Responsibility | Conversation/progress notes on a ticket (author, role, text, time) | Composition under Ticket; append-only |
|----------------|---------------------------------------------------------------------|--------------------------------------|

#### TicketAttachment
| Responsibility | Files on a ticket (photos of faults etc.) — **FileId → platform FileRecord** | Composition under Ticket |
|----------------|--------------------------------------------------------------------------------|---------------------------|

#### TicketAssignment
| Responsibility | Engineer assignment history for the ticket (engineer, assigned by/at, active flag) | Composition under Ticket; superseded on reassignment |
|----------------|---------------------------------------------------------------------------------------|------------------------------------------------------|

#### TicketStatusHistory
| Responsibility | Every status transition (from, to, changed by, changed at, reason — e.g. reopen reason) | Composition under Ticket; append-only |
|----------------|--------------------------------------------------------------------------------------------|----------------------------------------|

### 2.6 Engineer domain

#### Engineer *(aggregate root)*
| Aspect | Detail |
|--------|--------|
| Responsibility | Field engineer profile managed by Admin (freeze §2, §15); links the platform Auth account to business assignments |
| Key data | Employee code, name, phone, email, platform user reference, status (Active/Inactive) |
| Relationships | Referenced (logically) by EngineerAssignment and TicketAssignment |
| Lifecycle ownership | Admin creates/updates/deactivates; never hard-deleted (assignment history) |

### 2.7 Invoice domain — **Option B (approved)**

> **Mandated design decision:** invoices may be generated for **AMC Renewal, New AMC, Emergency Service, Spare Replacement, Additional Charges, and Other Billable Activities**. The invoice is **not tightly coupled to AMC terms** — the AMC Contract Term reference is **optional** and populated for AMC-type invoices (which preserves freeze §16 linkage for AMC billing).

#### Invoice *(aggregate root)*
| Aspect | Detail |
|--------|--------|
| Responsibility | A billable document to a customer (freeze §16 + Option B) |
| Key data | Invoice number, customer reference, optional site reference, **invoice type** (AMCRenewal / NewAMC / EmergencyService / SpareReplacement / AdditionalCharges / Other), **optional** AMC contract term reference, optional ticket / service visit reference (for service-originated billing), invoice date, due date, amounts (subtotal, tax, total — `decimal`), status |
| Status | Draft · Generated · Sent · Paid · Cancelled (BR-INV-01) |
| Invariant | AMC-type invoices (AMCRenewal, NewAMC) reference an AMCContractTerm (BR-INV-02); other types may reference a ticket/visit or stand alone |
| Relationships | N:1 Customer; optional N:1 Site / AMCContractTerm / Ticket / ServiceVisit (logical); 1:N InvoiceLine, InvoiceAttachment, InvoiceStatusHistory |
| Lifecycle ownership | Admin manages; customer downloads PDF (BR-INV-03); **no accounting features** (BR-INV-05) |

#### InvoiceLine
| Responsibility | Billed line items (line no, description, quantity, unit price, line total — `decimal`) | Composition under Invoice; editable only in Draft |
|----------------|--------------------------------------------------------------------------------------------|-----------------------------------------------------|

#### InvoiceAttachment
| Responsibility | Generated Invoice PDF (freeze §19) and supporting files — **FileId → platform FileRecord** | Composition under Invoice; immutable once Generated |
|----------------|------------------------------------------------------------------------------------------------|-------------------------------------------------------|

#### InvoiceStatusHistory
| Responsibility | Every status transition (from, to, changed by/at) | Composition under Invoice; append-only |
|----------------|-----------------------------------------------------|------------------------------------------|

---

## 3. Aggregate boundaries (summary)

| Aggregate root | Members (composition) | Consistency rule enforced at the boundary |
|----------------|------------------------|--------------------------------------------|
| Lead | LeadActivity, LeadRemark, LeadAttachment | Status transitions; conversion creates references atomically |
| Customer | — (Site is its own aggregate) | Identity/profile integrity |
| Site | SiteContact, SiteDocument, SiteAssetSummary | **Max 3 contacts**; one asset summary |
| AMCPlan | AMCPlanVersion | Version immutability after first reference |
| AMCContract | AMCContractTerm, AMCContractDocument | One active term; one active contract per site (cross-aggregate, DB-supported) |
| ServiceSchedule | EngineerAssignment | Mandatory assignment before In Progress |
| ServiceVisit | VisitPhoto, VisitLocation, VisitSignature, VisitApproval, VisitAttachment | **Completion evidence checklist**; approval gate |
| Ticket | TicketComment, TicketAttachment, TicketAssignment, TicketStatusHistory | Valid status transitions; reopen only from Closed by Customer |
| Engineer | — | Active/inactive state |
| Invoice | InvoiceLine, InvoiceAttachment, InvoiceStatusHistory | Lines locked after Draft; type↔reference consistency |

**Cross-aggregate references are by identifier only** (logical references) — no object graphs across aggregates, no physical FKs across module schemas (see [database-architecture.md](./database-architecture.md) §5).

---

## 4. Ownership matrix (module → entities)

| Backend module slice | Schema | Owns |
|----------------------|--------|------|
| CctvCrm.Lead | `cctv_lead` | Lead, LeadActivity, LeadRemark, LeadAttachment |
| CctvCrm.Customer | `cctv_customer` | Customer, Site, SiteContact, SiteDocument, SiteAssetSummary |
| CctvCrm.Amc | `cctv_amc` | AMCPlan, AMCPlanVersion, AMCContract, AMCContractTerm, AMCContractDocument |
| CctvCrm.Service | `cctv_service` | ServiceSchedule, EngineerAssignment, ServiceVisit, VisitPhoto, VisitLocation, VisitSignature, VisitApproval, VisitAttachment |
| CctvCrm.Ticket | `cctv_ticket` | Ticket, TicketComment, TicketAttachment, TicketAssignment, TicketStatusHistory |
| CctvCrm.Engineer | `cctv_engineer` | Engineer |
| CctvCrm.Invoice | `cctv_invoice` | Invoice, InvoiceLine, InvoiceAttachment, InvoiceStatusHistory |
| Platform (frozen) | `files`, `auth`, … | FileRecord, User/Role, audit store (MongoDB) — **reused, never redesigned** |

---

## 5. Platform reference strategy (mandated)

| Reference | How |
|-----------|-----|
| Files | Every binary (photo, video, selfie, signature, document, PDF) is a **FileId (UUID) referencing the platform `FileRecord`**. **No file path columns anywhere in business tables.** |
| Identity | `created_by` / actor columns store platform Auth user ids; Customer and Engineer entities hold their portal user reference |
| Audit | No custom audit tables for platform-captured trails; domain events flow to the platform Audit observer (see [database-architecture.md](./database-architecture.md) §6). Domain-meaningful histories that users must query in-app (TicketStatusHistory, InvoiceStatusHistory, VisitApproval, LeadActivity) **are** first-class entities, because they are business data, not audit logs. |

---

## Related documents

- [database-architecture.md](./database-architecture.md)
- [database-naming-standards.md](./database-naming-standards.md)
- [entity-lifecycle-matrix.md](./entity-lifecycle-matrix.md)
- [erd-overview.md](./erd-overview.md)
