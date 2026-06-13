# Workflow Overview

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0 — Project Foundation Documentation
**Source of truth:** [requirements-freeze-v1.md](./requirements-freeze-v1.md) (Approved & Frozen, V1)

Six core workflows derived from the frozen requirements. Statuses and transitions are exactly those approved in the freeze document.

---

## 1. Lead Conversion (§10)

Website inquiries automatically create leads; a converted lead creates Customer + Site + Initial AMC Contract.

```mermaid
flowchart TD
    A[Public Visitor submits inquiry / Get Quote / AMC Inquiry] --> B[Lead auto-created - Status: New]
    B --> N1[/Notification: Lead Created - Email + SMS/]
    B --> C[Contacted]
    C --> D[Qualified]
    D --> E[Quotation Sent]
    E --> F[Negotiation]
    F --> G{Outcome}
    G -->|Won| H[Won]
    G -->|Lost| I[Lost - End]
    H --> J[Convert Lead]
    J --> K[Create Customer]
    J --> L[Create Site]
    J --> M[Create Initial AMC Contract]
    K & L & M --> O[Lead Status: Converted]
    O --> N2[/Notification: Lead Converted - Email + SMS/]
```

### Lead status lifecycle

```mermaid
stateDiagram-v2
    [*] --> New
    New --> Contacted
    Contacted --> Qualified
    Qualified --> QuotationSent: Quotation Sent
    QuotationSent --> Negotiation
    Negotiation --> Won
    Negotiation --> Lost
    Won --> Converted
    Lost --> [*]
    Converted --> [*]
```

---

## 2. AMC Renewal (§8, §3, §17)

The AMC contract is permanent (master); each renewal adds a **Contract Term**. Customers see the current active term; admin sees full history.

```mermaid
flowchart TD
    A[Active AMC Contract Term] --> B[/Notification: AMC Expiry Reminder - Email + SMS/]
    B --> C{Renewal initiated}
    C -->|Customer requests renewal| D[Renewal Request]
    C -->|Admin initiates| D
    D --> E[Admin manages contract renewal]
    E --> F[New AMC Contract Term created<br/>versioned plan pricing applies]
    F --> G[Term history preserved under Contract Master]
    F --> H[Invoice linked to new Contract Term]
    H --> I[/Notification: Invoice Generated/]
    F --> J[Visits auto-generated from plan frequency]
    G --> K[Customer sees current active term<br/>Admin sees complete renewal history]
```

---

## 3. Ticket Resolution (§14, §17)

```mermaid
flowchart TD
    A{Ticket created by} -->|Customer| B[Ticket: Open]
    A -->|Admin| B
    A -->|Engineer during visit| B
    B --> N1[/Notification: Ticket Created/]
    B --> C[Admin assigns engineer]
    C --> D[Ticket: Assigned]
    D --> N2[/Notification: Ticket Assigned/]
    D --> E[Engineer works the ticket]
    E --> F[Ticket: In Progress]
    F --> G[Ticket: Resolved]
    G --> H[Ticket: Closed]
    H --> N3[/Notification: Ticket Closed/]
    H --> I{Customer satisfied?}
    I -->|No| J[Customer reopens ticket]
    J --> K[Ticket: Reopened]
    K --> C
    I -->|Yes| L[End]
```

### Ticket status lifecycle

```mermaid
stateDiagram-v2
    [*] --> Open
    Open --> Assigned
    Assigned --> InProgress: In Progress
    InProgress --> Resolved
    Resolved --> Closed
    Closed --> Reopened: Customer reopens
    Reopened --> Assigned
    Closed --> [*]
```

Priorities (applied at creation, §14): Low · Medium · High · Critical.

---

## 4. Service Visit (§11, §12, §13)

Covers scheduling lifecycle, mandatory completion evidence, and the approval gate.

```mermaid
flowchart TD
    A[Visit auto-generated from AMC frequency] --> B[Status: Planned]
    B --> C[Admin assigns engineer - mandatory]
    C --> D[Status: Assigned]
    D --> N1[/Notification: Visit Scheduled/]
    D --> E{Visit day}
    E -->|Engineer starts| F[Status: In Progress]
    E -->|Not attended| M[Status: Missed]
    B -->|Admin cancels| X[Status: Cancelled]
    B -->|Admin reschedules| B
    F --> G[Capture mandatory evidence]
    G --> G1[Engineer Selfie]
    G --> G2[GPS: latitude, longitude, timestamp]
    G --> G3[Min. one visit photo<br/>Before / During / After]
    G --> G4[Customer Signature]
    G --> G5[Visit Remarks]
    G1 & G2 & G3 & G4 & G5 --> H[Engineer submits visit report]
    H --> I[Status: Completed]
    I --> N2[/Notification: Visit Completed/]
    H --> J[Admin review]
    J -->|Approve| K[Report approved]
    K --> L[Customer can view report<br/>Visit Report PDF available]
    J -->|Not approved| H2[Back to engineer / re-review]
```

### Visit approval gate (§13)

```mermaid
sequenceDiagram
    participant E as Engineer
    participant S as System
    participant A as Admin
    participant C as Customer

    E->>S: Submit visit report (selfie, GPS, photos, signature, remarks)
    S->>A: Queue for review
    A->>S: Approve
    S->>C: Report visible in Customer Portal
    Note over C: Customer cannot view reports before approval
```

---

## 5. Invoice Generation (§16, §17, §19)

```mermaid
flowchart TD
    A[AMC Contract Term created or renewed] --> B[Invoice: Draft]
    B --> C[Invoice: Generated]
    C --> N1[/Notification: Invoice Generated/]
    C --> D[Invoice PDF produced]
    C --> E[Invoice: Sent]
    E --> F{Payment received?}
    F -->|Yes| G[Invoice: Paid]
    F -->|Cancelled| H[Invoice: Cancelled]
    D --> I[Customer downloads PDF<br/>from Customer Portal / App]
```

### Invoice status lifecycle

```mermaid
stateDiagram-v2
    [*] --> Draft
    Draft --> Generated
    Generated --> Sent
    Sent --> Paid
    Draft --> Cancelled
    Generated --> Cancelled
    Sent --> Cancelled
    Paid --> [*]
    Cancelled --> [*]
```

Rules: invoice is linked to an AMC Contract Term; no accounting features in V1 (§16).

---

## 6. Engineer Assignment (§11, §14, §15)

Engineer assignment is mandatory for visits; tickets are assigned through the ticket lifecycle.

```mermaid
flowchart TD
    subgraph visits [Visit assignment]
        A[Visit: Planned] --> B[Admin selects engineer]
        B --> C[Visit: Assigned]
        C --> D[Appears in engineer's Assigned Visits<br/>web + mobile]
    end

    subgraph tickets [Ticket assignment]
        E[Ticket: Open] --> F[Admin assigns engineer]
        F --> G[Ticket: Assigned]
        G --> H[Appears in engineer's Assigned Tickets]
        G --> N1[/Notification: Ticket Assigned/]
    end

    D --> I[Engineer executes work<br/>uploads photos, videos, reports]
    H --> I
    I --> J[Engineer may create new tickets during visit]
    
    K[Engineer restrictions - freeze §15:<br/>cannot manage customers, AMC plans, contracts] -.-> I
```

---

## Workflow ↔ rules traceability

| Workflow | Business rules | Freeze sections |
|----------|----------------|-----------------|
| Lead Conversion | BR-LEAD-01..03 | §10 |
| AMC Renewal | BR-AMC-01..08 | §3, §8, §9, §17 |
| Ticket Resolution | BR-TKT-01..06 | §14, §17 |
| Service Visit | BR-SCHED-01..04, BR-VISIT-01..07 | §11–§13 |
| Invoice Generation | BR-INV-01..05 | §16, §19 |
| Engineer Assignment | BR-SCHED-04, BR-VISIT-06..07 | §11, §14, §15 |

Rules detail: [business-rules.md](./business-rules.md)
