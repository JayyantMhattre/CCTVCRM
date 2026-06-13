# User Journeys

**Project:** Aarvii CCTV AMC Management System · **Phase:** D0-5
**Source of truth:** [requirements-freeze-v1.md](../requirements-freeze-v1.md) · screens per [screen-inventory.md](./screen-inventory.md), permissions per [rbac-matrix.md](./rbac-matrix.md)

Six end-to-end journeys across actors, screens, and platform capabilities. Statuses are the frozen vocabularies.

---

## 1. Lead → Customer

```mermaid
journey
    title Lead to Customer (Visitor → Admin)
    section Website
      Browse AMC Services: 4: Visitor
      Submit Get Quote / AMC Inquiry: 5: Visitor
    section Pipeline (Admin Portal)
      Lead auto-created + notification: 5: System
      Contact and qualify lead: 4: Admin
      Send quotation, negotiate: 4: Admin
      Mark Won: 5: Admin
    section Conversion
      Convert lead: 5: Admin
      Customer + Site + Initial AMC Contract created: 5: System
      Welcome notification to customer: 5: System
```

```mermaid
flowchart LR
    A[Visitor: Get Quote /get-quote] --> B[System: Lead created - New<br/>Notification: Lead Created]
    B --> C["Admin: /admin/leads pipeline<br/>Contacted → Qualified → QuotationSent → Negotiation"]
    C --> D{Won?}
    D -->|Yes| E[Admin: /admin/leads/:id/convert]
    E --> F[System: Customer + Site + Initial Contract<br/>Lead = Converted - Notification: Lead Converted]
    F --> G[Customer: portal account - login via platform Auth]
    D -->|No| H[Lead = Lost]
```

## 2. Customer → Ticket

```mermaid
flowchart LR
    A[Customer: /portal/tickets/new<br/>subject, description, priority, attachments] --> B[System: Ticket Open<br/>Notification: Ticket Created]
    B --> C[Admin: /admin/tickets/:id<br/>assign engineer]
    C --> D[Ticket Assigned<br/>Notification: Ticket Assigned]
    D --> E["Engineer: /engineer/tickets/:id<br/>InProgress → Resolved"]
    E --> F[Admin: close ticket]
    F --> G[Ticket Closed<br/>Notification: Ticket Closed]
    G --> H{Customer satisfied?}
    H -->|No| I[Customer: Reopen on /portal/tickets/:id]
    I --> C
    H -->|Yes| J[Done]
```

Screens: #52 → #31 → #68 → #53. Permissions: `tickets:create` → `tickets:assign` → `tickets:update` → `tickets:close` → `tickets:reopen`.

## 3. Admin → Schedule Visit

```mermaid
flowchart LR
    A[AMC term activated] --> B[System: schedules auto-generated<br/>from plan visit frequency - Planned]
    B --> C[Admin: /admin/schedules calendar]
    C --> D[Admin: assign engineer - MANDATORY<br/>visits:assign]
    D --> E["Schedule Assigned<br/>Notifications: Visit Scheduled → customer + engineer"]
    C -.reschedule/cancel.-> C2[schedules:manage<br/>reschedule trail kept]
    E --> F[Appears in Engineer's My Day<br/>web + mobile, offline-cached]
```

## 4. Engineer → Complete Visit

```mermaid
flowchart TD
    A[Engineer: /engineer/visits - assigned queue] --> B[Open Visit Detail - start visit<br/>Schedule: InProgress]
    B --> C[Capture evidence - offline capable]
    C --> C1[Photos: Before/During/After + video<br/>platform Files upload]
    C --> C2[Selfie]
    C --> C3[GPS: lat/long/timestamp]
    C --> C4[Customer Signature on device]
    C --> C5[Visit Remarks]
    C1 & C2 & C3 & C4 & C5 --> D{Checklist complete? BR-VISIT-01}
    D -->|No| C
    D -->|Yes| E[Submit Report - syncs when online]
    E --> F[Schedule: Completed<br/>Notification: Visit Completed]
    E --> G[Admin: /admin/visits/approvals review]
    G -->|Return| H[Engineer rework queue] --> E
    G -->|Approve| I[Report visible to Customer<br/>/portal/visits/history + Report PDF]
    B -.fault found.-> J[Create Ticket during visit]
```

## 5. Customer → Renewal Request

```mermaid
flowchart LR
    A[System: AMC Expiry Reminder<br/>email + SMS to customer] --> B[Customer: /portal/amc<br/>active term and expiry visible]
    B --> C[Customer: Request Renewal<br/>amc:request-renewal - BR-AMC-08]
    C --> D[Admin: /admin/amc/renewal-requests queue]
    D --> E[Admin: renew term on contract<br/>pins current plan version]
    E --> F[New Term created - history preserved<br/>Customer sees new active term only]
    F --> G[Schedules auto-generated for new term]
    F --> H[Invoice type=AmcRenewal linked to term]
```

## 6. Admin → Invoice Generation (Option B)

```mermaid
flowchart TD
    A{Billable trigger} -->|AMC renewal / new AMC| B[Invoice type=AmcRenewal or NewAmc<br/>linked to contract term]
    A -->|Emergency service / spare replacement<br/>additional charges / other| C[Invoice type=EmergencyService etc.<br/>optional ticket/visit reference]
    B & C --> D[Admin: /admin/invoices/new - Draft<br/>add invoice lines]
    D --> E["Generate → Invoice PDF created<br/>Notification: Invoice Generated"]
    E --> F[Send to customer]
    F --> G[Customer: /portal/invoices/:id<br/>view + download PDF]
    G --> H[Admin: mark Paid - manual, no gateway]
    D -.void.-> X[Cancelled]
```

---

## Journey ↔ capability reuse map

| Journey | Platform capabilities reused |
|---------|------------------------------|
| Lead → Customer | Rate-limited anonymous API, Notifications (email+SMS), Auth account provisioning, Audit |
| Customer → Ticket | Auth/RBAC scoping, Files (attachments), Notifications, Audit |
| Admin → Schedule Visit | Outbox events, Notifications, Audit |
| Engineer → Complete Visit | Files (media), mobile offline/sync foundation, Notifications, Audit |
| Customer → Renewal | Notifications (expiry reminder), Audit |
| Admin → Invoice | Files (PDF), Notifications, Audit |

Related: [workflow-overview.md](../workflow-overview.md) (process-level) · [screen-inventory.md](./screen-inventory.md) · [rbac-matrix.md](./rbac-matrix.md)
