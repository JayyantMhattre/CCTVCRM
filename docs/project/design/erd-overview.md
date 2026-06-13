# ERD Overview — Complete System

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-4 — Entity Model, ER Diagram & Database Architecture (design only)
**Source of truth:** [requirements-freeze-v1.md](../requirements-freeze-v1.md) · [entity-model.md](./entity-model.md)

Complete system ERD across all CCTV domains. Attribute-level detail lives in the per-domain ERDs (linked below). Dashed/cross-schema references are **logical** (no physical FK — see [database-architecture.md §3](./database-architecture.md)).

---

## 1. Full system ERD (relationships)

```mermaid
erDiagram
    %% ----- Lead domain (cctv_lead) -----
    Lead ||--o{ LeadActivity : "logs"
    Lead ||--o{ LeadRemark : "has"
    Lead ||--o{ LeadAttachment : "has"
    Lead |o..o| Customer : "converts to"
    Lead |o..o| Site : "creates"
    Lead |o..o| AMCContract : "creates initial"

    %% ----- Customer domain (cctv_customer) -----
    Customer ||--o{ Site : "owns 1:N"
    Site ||--o{ SiteContact : "max 3"
    Site ||--o| SiteAssetSummary : "summary counts only"
    Site ||--o{ SiteDocument : "has"

    %% ----- AMC domain (cctv_amc) -----
    AMCPlan ||--|{ AMCPlanVersion : "versioned"
    Site ||..o{ AMCContract : "one ACTIVE at a time"
    AMCContract ||--|{ AMCContractTerm : "renewal history"
    AMCPlanVersion ||..o{ AMCContractTerm : "pins price-frequency-SLA"
    AMCContract ||--o{ AMCContractDocument : "contract PDFs"

    %% ----- Service domain (cctv_service) -----
    AMCContractTerm ||..o{ ServiceSchedule : "auto-generates visits"
    ServiceSchedule ||--o{ EngineerAssignment : "mandatory assignment"
    Engineer ||..o{ EngineerAssignment : "assigned"
    ServiceSchedule ||--o| ServiceVisit : "executed as"
    ServiceVisit ||--o{ VisitPhoto : "before-during-after-selfie"
    ServiceVisit ||--o| VisitLocation : "GPS lat-long-timestamp"
    ServiceVisit ||--o| VisitSignature : "customer signature"
    ServiceVisit ||--o{ VisitApproval : "admin review rounds"
    ServiceVisit ||--o{ VisitAttachment : "videos and report PDF"

    %% ----- Ticket domain (cctv_ticket) -----
    Customer ||..o{ Ticket : "raises"
    Site ||..o{ Ticket : "located at"
    ServiceVisit |o..o{ Ticket : "raised during visit"
    Ticket ||--o{ TicketComment : "conversation"
    Ticket ||--o{ TicketAttachment : "has"
    Ticket ||--o{ TicketAssignment : "engineer history"
    Engineer ||..o{ TicketAssignment : "assigned"
    Ticket ||--|{ TicketStatusHistory : "transitions"

    %% ----- Invoice domain (cctv_invoice) — Option B -----
    Customer ||..o{ Invoice : "billed"
    Site |o..o{ Invoice : "optional"
    AMCContractTerm |o..o{ Invoice : "OPTIONAL link - AMC types"
    Ticket |o..o{ Invoice : "optional - service billing"
    Invoice ||--|{ InvoiceLine : "line items"
    Invoice ||--o{ InvoiceAttachment : "invoice PDF"
    Invoice ||--|{ InvoiceStatusHistory : "transitions"

    %% ----- Platform (frozen, reused) -----
    FileRecord ||..o{ LeadAttachment : "file_id"
    FileRecord ||..o{ SiteDocument : "file_id"
    FileRecord ||..o{ AMCContractDocument : "file_id"
    FileRecord ||..o{ VisitPhoto : "file_id"
    FileRecord ||..o{ VisitSignature : "file_id"
    FileRecord ||..o{ VisitAttachment : "file_id"
    FileRecord ||..o{ TicketAttachment : "file_id"
    FileRecord ||..o{ InvoiceAttachment : "file_id"
    PlatformUser ||..o| Customer : "portal login"
    PlatformUser ||..o| Engineer : "portal login"
```

> `FileRecord` and `PlatformUser` are **frozen platform entities** (Files / Auth modules) shown for reference only — they are reused, never redesigned (freeze §20).

## 2. Domain-to-schema map

```mermaid
flowchart LR
    subgraph cctv_lead
        L[Lead +3 children]
    end
    subgraph cctv_customer
        C[Customer, Site +3 children]
    end
    subgraph cctv_amc
        A[AMCPlan/Version<br/>AMCContract/Term/Document]
    end
    subgraph cctv_service
        S[ServiceSchedule, ServiceVisit<br/>+6 children]
    end
    subgraph cctv_ticket
        T[Ticket +4 children]
    end
    subgraph cctv_engineer
        E[Engineer]
    end
    subgraph cctv_invoice
        I[Invoice +3 children]
    end

    L -.convert.-> C
    L -.convert.-> A
    C -.site.-> A
    A -.term.-> S
    E -.assign.-> S
    E -.assign.-> T
    C -.context.-> T
    S -.visit ticket.-> T
    C -.billing.-> I
    A -.optional term link.-> I
    T -.optional.-> I
```

## 3. Mandated design decisions reflected in this ERD

| Decision | Where visible |
|----------|---------------|
| Customer 1:N Site | `Customer ||--o{ Site` |
| Max 3 site contacts | `Site ||--o{ SiteContact` + slot constraint ([naming standards §5](./database-naming-standards.md)) |
| One site = one **active** AMC contract | `Site ||..o{ AMCContract` + partial unique index |
| AMC Contract = master record | `AMCContract` permanent root |
| AMC Contract Term = renewal history | `AMCContract ||--|{ AMCContractTerm` |
| Asset tracking = summary counts only (no individual cameras) | Single `SiteAssetSummary` 1:1 with Site — **no device/camera entity exists** |
| **Invoices — Option B approved** | `AMCContractTerm |o..o{ Invoice` is **optional**; `invoice_type` covers AMC Renewal, New AMC, Emergency Service, Spare Replacement, Additional Charges, Other |
| Files via platform FileRecord (no path columns) | All `file_id` references to `FileRecord` |
| Audit via platform module | No custom audit entities; histories shown are business data |

## 4. Entity count summary

| Domain | Entities | Detail ERD |
|--------|----------|-----------|
| Lead | 4 | [erd-lead-domain.md](./erd-lead-domain.md) |
| Customer | 5 | [erd-customer-domain.md](./erd-customer-domain.md) |
| AMC | 5 | [erd-amc-domain.md](./erd-amc-domain.md) |
| Service | 8 | [erd-service-domain.md](./erd-service-domain.md) |
| Ticket | 5 | [erd-ticket-domain.md](./erd-ticket-domain.md) |
| Invoice | 4 | [erd-invoice-domain.md](./erd-invoice-domain.md) |
| Engineer | 1 | (in [entity-model.md §2.6](./entity-model.md)) |
| **Total CCTV entities** | **32** | + reused platform entities |
