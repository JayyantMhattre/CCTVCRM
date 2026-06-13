# Business Requirements Document (BRD)

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0 — Project Foundation Documentation
**Source of truth:** [requirements-freeze-v1.md](./requirements-freeze-v1.md) (Approved & Frozen, V1)

> Every requirement below traces to a freeze-document section (cited as §n). This BRD adds structure and IDs only — no new requirements, features, or rule changes.

---

## 1. Business objectives

| ID | Objective | Freeze ref |
|----|-----------|-----------|
| BO-1 | Capture and convert website-generated leads into AMC customers | §2, §10 |
| BO-2 | Centralize customers, sites, contacts, and asset summaries | §5–§7 |
| BO-3 | Manage the full AMC contract lifecycle with preserved renewal history | §8, §9 |
| BO-4 | Automate and verify preventive maintenance service delivery | §11–§13 |
| BO-5 | Resolve complaints through a tracked ticket lifecycle | §14 |
| BO-6 | Bill customers per AMC contract term with self-service invoice access | §16, §19 |
| BO-7 | Provide customer and engineer self-service on web and mobile | §2, §18 |
| BO-8 | Reuse the existing base platform — no duplicate implementation | §20 |

---

## 2. Functional requirements

### 2.1 Public Website (§2, §10)

| ID | Requirement |
|----|-------------|
| FR-WEB-01 | The system shall serve a public website at www.aarvii.in with pages: Home, About Us, Services, AMC Services, Contact Us, Gallery, Testimonials, Login. |
| FR-WEB-02 | The website shall reuse existing public website content wherever possible. |
| FR-WEB-03 | The website shall provide a **Get Quote** enhancement allowing visitors to request quotations. |
| FR-WEB-04 | The website shall provide an **AMC Inquiry** enhancement allowing visitors to submit AMC inquiries. |
| FR-WEB-05 | Website inquiries (contact, AMC inquiry, quote requests) shall automatically create leads (§10). |

### 2.2 Lead Management (§10)

| ID | Requirement |
|----|-------------|
| FR-LEAD-01 | The system shall manage leads through the statuses: New, Contacted, Qualified, Quotation Sent, Negotiation, Won, Lost, Converted. |
| FR-LEAD-02 | Converting a lead shall create a Customer, a Site, and an Initial AMC Contract. |
| FR-LEAD-03 | A notification shall be sent on Lead Created and Lead Converted (§17). |

### 2.3 Customer Management (§3, §5)

| ID | Requirement |
|----|-------------|
| FR-CUST-01 | Admin shall manage customer records. |
| FR-CUST-02 | A customer shall own one or more sites (§6). |
| FR-CUST-03 | Customers shall be able to update their own profile and reset their password (OTP-based, §17). |

### 2.4 Site Management (§5, §6)

| ID | Requirement |
|----|-------------|
| FR-SITE-01 | A site shall belong to exactly one customer. |
| FR-SITE-02 | A site shall support a maximum of three contact persons. |
| FR-SITE-03 | A site shall hold at most one **active** AMC contract at a time. |
| FR-SITE-04 | A site shall aggregate: contact persons, asset summary, AMC contract, scheduled visits, tickets, and invoices (§5). |

### 2.5 Asset Management (§7)

| ID | Requirement |
|----|-------------|
| FR-ASSET-01 | Assets shall be tracked as **summary counts per site**: Camera, DVR, NVR, Hard Disk, Switch, Router, Monitor. |
| FR-ASSET-02 | The system shall **not** track individual cameras/devices. |
| FR-ASSET-03 | Asset summaries may optionally record Brand, Model, and Remarks. |

### 2.6 AMC Plans (§9)

| ID | Requirement |
|----|-------------|
| FR-PLAN-01 | Admin shall manage AMC plans (e.g. Silver, Gold, Platinum). |
| FR-PLAN-02 | A plan shall store: Price, Visit Frequency, Included Services, SLA. |
| FR-PLAN-03 | Plans shall be **versioned**; modifying a plan shall never change historical contracts. |

### 2.7 AMC Contracts (§8)

| ID | Requirement |
|----|-------------|
| FR-AMC-01 | AMC contracts shall follow a **Master + Terms** model: a permanent contract record plus renewal-term history records. |
| FR-AMC-02 | Customers shall see the current active term; Admin shall see the complete renewal history. |
| FR-AMC-03 | Customers shall be able to request AMC renewal (§3). |
| FR-AMC-04 | An AMC Expiry Reminder notification shall be sent (§17). |
| FR-AMC-05 | The system shall generate an AMC Contract PDF (§19). |

### 2.8 Service Scheduling (§11)

| ID | Requirement |
|----|-------------|
| FR-SCHED-01 | Visits shall be automatically generated from AMC plan visit frequency. |
| FR-SCHED-02 | Visit schedule statuses shall be: Planned, Assigned, In Progress, Completed, Missed, Cancelled. |
| FR-SCHED-03 | Admin shall be able to reschedule visits. |
| FR-SCHED-04 | Engineer assignment shall be mandatory before a visit proceeds. |
| FR-SCHED-05 | Visit Scheduled and Visit Completed notifications shall be sent (§17). |

### 2.9 Visit Management (§12, §13)

| ID | Requirement |
|----|-------------|
| FR-VISIT-01 | Visit completion shall require: engineer selfie, GPS coordinates, minimum one visit photo, customer signature, and visit remarks. |
| FR-VISIT-02 | GPS capture shall store latitude, longitude, and timestamp. |
| FR-VISIT-03 | Visits shall support Before, During, and After photos. |
| FR-VISIT-04 | Engineers shall submit visit reports for **admin review**; admin approval is required before a customer can view a report. |
| FR-VISIT-05 | The system shall generate a Visit Report PDF (§19). |
| FR-VISIT-06 | Engineers shall be able to upload photos, videos, and reports (§15). |

### 2.10 Ticket Management (§14)

| ID | Requirement |
|----|-------------|
| FR-TKT-01 | Tickets shall follow the statuses: Open, Assigned, In Progress, Resolved, Closed, Reopened. |
| FR-TKT-02 | Tickets shall carry a priority: Low, Medium, High, Critical. |
| FR-TKT-03 | Customers, Admins, and Engineers shall be able to create tickets (engineers during visits, §3). |
| FR-TKT-04 | Customers shall be able to reopen closed tickets. |
| FR-TKT-05 | Ticket Created, Ticket Assigned, and Ticket Closed notifications shall be sent (§17). |

### 2.11 Engineer Management (§15)

| ID | Requirement |
|----|-------------|
| FR-ENG-01 | Admin shall manage engineers. |
| FR-ENG-02 | Engineers shall view their assigned work (visits and tickets). |
| FR-ENG-03 | Engineers shall **not** manage customers, AMC plans, or contracts. |

### 2.12 Invoice Management (§16)

| ID | Requirement |
|----|-------------|
| FR-INV-01 | Invoices shall follow the statuses: Draft, Generated, Sent, Paid, Cancelled. |
| FR-INV-02 | Every invoice shall be linked to an AMC Contract Term. |
| FR-INV-03 | Customers shall be able to download invoices as PDF (§19). |
| FR-INV-04 | An Invoice Generated notification shall be sent (§17). |
| FR-INV-05 | No accounting features shall be included in V1. |

### 2.13 Notifications (§17)

| ID | Requirement |
|----|-------------|
| FR-NOTIF-01 | The system shall send notifications via **Email** and **SMS**. |
| FR-NOTIF-02 | Notification events: Lead Created, Lead Converted, Ticket Created, Ticket Assigned, Ticket Closed, Visit Scheduled, Visit Completed, AMC Expiry Reminder, Invoice Generated, Password Reset OTP, Login OTP. |

### 2.14 Reporting (§2)

| ID | Requirement |
|----|-------------|
| FR-RPT-01 | The Admin Portal shall provide a Reporting module covering the managed business areas. |

### 2.15 Customer Portal (§2)

| ID | Requirement |
|----|-------------|
| FR-CP-01 | The Customer Portal (web + mobile) shall provide: Dashboard, AMC Details, Service History, Upcoming Visits, Tickets, Invoices, Profile Management, Password Reset. |

### 2.16 Engineer Portal (§2)

| ID | Requirement |
|----|-------------|
| FR-EP-01 | The Engineer Portal (web + mobile) shall provide: Assigned Visits, Assigned Tickets, Visit Reporting, Photo Upload, GPS Capture, Selfie Capture, Customer Signature, Ticket Creation. |

### 2.17 Mobile applications (§18)

| ID | Requirement |
|----|-------------|
| FR-MOB-01 | Mobile apps shall be built with **Flutter**. |
| FR-MOB-02 | Customer App features: Dashboard, AMC, Tickets, Invoices, Notifications, Profile. |
| FR-MOB-03 | Engineer App features: Visits, Tickets, Photo Upload, GPS Capture, Signature Capture, Offline Support. |

### 2.18 PDF documents (§19)

| ID | Requirement |
|----|-------------|
| FR-PDF-01 | The system shall generate: AMC Contract PDF, Visit Report PDF, Invoice PDF. |

---

## 3. Non-functional requirements

Derived from the freeze document and the mandated platform reuse (§20); platform-provided qualities reference the [platform discovery report](../project-bootstrap/platform-discovery-report.md).

| ID | Requirement | Basis |
|----|-------------|-------|
| NFR-01 | **Security/Auth** — All portals authenticate via the platform Auth module (JWT/OAuth2); role-based access enforces the four-actor permission model. | §3, §20 |
| NFR-02 | **Authorization** — Engineer restrictions (§15) and customer data ownership ("own contracts/invoices") are enforced server-side via platform RBAC/ABAC. | §3, §15 |
| NFR-03 | **Tenancy & data isolation** — Customer-facing data is scoped so customers access only their own sites, contracts, visits, tickets, and invoices. | §3 |
| NFR-04 | **Auditability** — Business actions are captured by the platform Audit capability (visit approval, ticket transitions, contract changes available for audit). | §20 |
| NFR-05 | **File handling** — Photos, videos, selfies, signatures, and PDFs are stored via the platform Files capability (tenant-scoped, provider-pluggable). | §12, §19, §20 |
| NFR-06 | **Notifications delivery** — Email via the platform Notifications capability; SMS via an SMS provider integration. | §17, §20 |
| NFR-07 | **Offline capability** — The Engineer App must operate offline and synchronize when connectivity returns. | §18 |
| NFR-08 | **Mobile platform** — Both apps use the existing Flutter Mobile Foundation (secure token storage, push/notification plumbing, release engineering). | §18, §20 |
| NFR-09 | **Historical integrity** — Plan versioning guarantees historical contracts never change when plans are modified. | §9 |
| NFR-10 | **Evidence integrity** — GPS records latitude, longitude, and timestamp; mandatory completion evidence cannot be bypassed. | §12 |
| NFR-11 | **Observability** — Logging, tracing, correlation IDs, and health checks are inherited from the platform Host. | §20 |
| NFR-12 | **Documentation governance** — All CCTV modules follow the platform documentation governance (7-file module docs, ADRs, docs validation CI). | Platform policy |

---

## 4. Business constraints

| ID | Constraint | Source |
|----|-----------|--------|
| BC-01 | Scope is frozen; changes only via approved change request. | §22 |
| BC-02 | The Core Platform is frozen at v1.0.0 — CCTV functionality ships as business modules; no Core modifications. | §20, [platform freeze policy](../governance/platform-freeze-policy.md) |
| BC-03 | No duplicate implementation of Authentication, Roles, Permissions, Files, Notifications, Audit, Theme Engine, Mobile Foundation, API Keys, Webhooks. | §20 |
| BC-04 | Existing public website content must be reused wherever possible. | §2 |
| BC-05 | One active AMC contract per site; max 3 contact persons per site. | §6 |
| BC-06 | Assets tracked as summary counts only. | §7 |
| BC-07 | No accounting features in invoicing. | §16 |
| BC-08 | Out-of-scope list (§21) must not be implemented in V1. | §21 |
| BC-09 | Web frontend is React 19 + Theme Engine; mobile is Flutter — per the base platform stack. | §20, platform |

## 5. Assumptions

| ID | Assumption |
|----|-----------|
| AS-01 | Aarvii Technologies operates as a single tenant of the platform; multi-tenancy isolation still applies between customer accounts at the application level. |
| AS-02 | Existing website content (copy, images, testimonials) is available for reuse (§2). |
| AS-03 | An SMS gateway provider will be selected and contracted for SMS notifications (§17); email uses the platform provider model. |
| AS-04 | Engineers have Android/iOS smartphones with camera and GPS for the Engineer App (§12, §18). |
| AS-05 | Customer signature is captured on the engineer's device screen during the visit (§12). |
| AS-06 | Visit frequency semantics (e.g. visits per term) are defined per AMC plan (§9) and drive auto-generation (§11). |
| AS-07 | Invoice amounts derive from the AMC plan/term pricing; payment recording is a manual status update (no payment gateway, §21). |

## 6. Dependencies

| ID | Dependency | Needed by |
|----|-----------|-----------|
| DEP-01 | Ashraak Platform V1 (frozen) — Auth, Roles/Permissions, Files, Notifications (email), Audit, Theme Engine, Mobile Foundation, API Keys, Webhooks | All modules (§20) |
| DEP-02 | SMS gateway provider | Notifications (§17) |
| DEP-03 | Email provider (SMTP) configuration | Notifications (§17) |
| DEP-04 | PDF generation capability (server-side) | AMC Contract / Visit Report / Invoice PDFs (§19) |
| DEP-05 | Existing www.aarvii.in content | Public Website (§2) |
| DEP-06 | Device capabilities: camera, GPS, touch signature | Engineer App (§12, §18) |
| DEP-07 | App store accounts + platform release pipelines (fastlane CI) | Mobile distribution (§18) |

---

## 7. Traceability

- All FRs cite their freeze-document section inline.
- Business rules are enumerated separately in [business-rules.md](./business-rules.md).
- Workflows: [workflow-overview.md](./workflow-overview.md) · Design: [high-level-design.md](./high-level-design.md)
- Screens: [screen-inventory.md](./screen-inventory.md) · Navigation: [navigation-map.md](./navigation-map.md)
