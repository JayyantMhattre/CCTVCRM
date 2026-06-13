# Product Vision Document

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0 — Project Foundation Documentation
**Source of truth:** [requirements-freeze-v1.md](./requirements-freeze-v1.md) (Approved & Frozen, V1)
**Status:** Derived documentation — no requirement changes introduced.

---

## 1. Business vision

Build a **centralized CCTV AMC (Annual Maintenance Contract) Management System** for **Aarvii Technologies** that manages the complete AMC lifecycle in one platform:

- Public website ([www.aarvii.in](http://www.aarvii.in)) with lead generation
- Customers, sites, and asset summaries
- AMC plans and AMC contracts (with renewal terms)
- Preventive maintenance scheduling and service visits
- Complaint tickets
- Engineers and their field activities
- Invoices
- Customer self-service portal and Engineer portal (web + mobile)

The platform provides **complete visibility** of the customer AMC lifecycle, engineer activities, service history, contract renewals, invoices, and complaint resolution (freeze §1).

The system is delivered as a **business module suite** on top of the frozen Ashraak Enterprise Platform V1, reusing Authentication, Roles, Permissions, Files, Notifications, Audit, Theme Engine, Mobile Foundation, API Keys, and Webhooks with **no duplicate implementation** (freeze §20).

---

## 2. Objectives

| # | Objective | Freeze reference |
|---|-----------|------------------|
| O1 | Convert public website traffic into qualified leads and AMC customers | §2 (Public Website), §10 (Lead Lifecycle) |
| O2 | Maintain a single source of truth for customers, sites, and asset summaries | §5, §6, §7 |
| O3 | Manage AMC contracts with a permanent master record and renewal-term history | §8 |
| O4 | Automate preventive maintenance visit generation from AMC plan frequency | §11 |
| O5 | Enforce verifiable field service evidence (selfie, GPS, photos, signature) | §12 |
| O6 | Gate customer-visible reports behind admin approval | §13 |
| O7 | Provide end-to-end complaint ticket management with reopen capability | §14 |
| O8 | Issue and track invoices per AMC contract term with PDF download | §16, §19 |
| O9 | Keep customers and engineers informed via Email and SMS notifications | §17 |
| O10 | Deliver self-service via Customer and Engineer portals on web and Flutter mobile apps | §2, §18 |

---

## 3. Problem statement

Without a centralized system, a CCTV AMC business faces:

- **Lost leads** — website inquiries and quote requests are not captured or tracked through a sales pipeline.
- **Fragmented customer data** — customers, sites, contacts, and installed asset counts live in spreadsheets or on paper.
- **Untracked AMC lifecycle** — contract renewals and plan history are not preserved; expiries are missed.
- **Unmanaged preventive maintenance** — visits owed under AMC frequency are not planned, assigned, or verified.
- **No field accountability** — no proof an engineer attended a site (selfie, GPS, photos, customer signature).
- **Opaque complaint handling** — tickets lack lifecycle, priority, assignment, and reopen tracking.
- **Manual invoicing** — invoices are not tied to contract terms; customers cannot self-serve downloads.
- **No customer self-service** — customers must call to learn AMC status, visit dates, or invoice details.

The Aarvii CCTV AMC Management System addresses all of the above within the approved V1 scope.

---

## 4. Success criteria

V1 is successful when:

| # | Criterion |
|---|-----------|
| S1 | Every website inquiry / quote request automatically creates a lead (freeze §10) |
| S2 | A lead can be converted in one flow into Customer + Site + Initial AMC Contract (freeze §10) |
| S3 | Each site holds at most one active AMC contract; renewal history is preserved as terms (freeze §6, §8) |
| S4 | Scheduled visits are auto-generated from AMC plan frequency, with mandatory engineer assignment (freeze §11) |
| S5 | No visit can be completed without selfie, GPS, ≥1 photo, customer signature, and remarks (freeze §12) |
| S6 | Customers see visit reports only after admin approval (freeze §13) |
| S7 | Tickets follow the approved lifecycle and can be raised by Customer, Admin, or Engineer, and reopened by Customer (freeze §14) |
| S8 | Invoices are linked to AMC contract terms and downloadable as PDF by customers (freeze §16) |
| S9 | All approved notification events fire on Email and SMS (freeze §17) |
| S10 | Customer and Engineer apps (Flutter) ship with the approved feature sets, including engineer offline support (freeze §18) |
| S11 | AMC Contract, Visit Report, and Invoice PDFs are generated (freeze §19) |
| S12 | Zero duplicate implementation of base platform capabilities (freeze §20) |

---

## 5. Target users

The four approved actors (freeze §3); detailed personas in [actors-and-personas.md](./actors-and-personas.md):

| Actor | Description |
|-------|-------------|
| **Public Visitor** | Anonymous website user browsing services, submitting inquiries and quote requests |
| **Customer** | Contract holder using the Customer Portal / App for AMC, tickets, invoices, and profile |
| **Engineer** | Field service engineer executing visits and tickets via the Engineer Portal / App |
| **Admin** | Aarvii back-office staff managing all modules through the Admin Portal |

---

## 6. Expected benefits

| Beneficiary | Benefits |
|-------------|----------|
| **Aarvii Technologies (business)** | Captured leads and conversion pipeline; preserved AMC renewal history; automated visit planning; verified field work; ticket accountability; invoice traceability; reporting across the operation |
| **Customers** | Self-service visibility of AMC, visits, service history, tickets, and invoices; ability to raise/reopen tickets and request renewals; PDF invoice downloads |
| **Engineers** | Clear assigned work queue; structured mobile reporting (photos, GPS, selfie, signature); offline support in the field |
| **Admins** | Single console for leads, customers, sites, assets, plans, contracts, scheduling, tickets, engineers, invoices, and reports; approval control over customer-visible reports |

---

## 7. Guardrails

- The scope is **frozen** per [requirements-freeze-v1.md §22](./requirements-freeze-v1.md); changes only via approved change request.
- Out-of-scope items (freeze §21) are explicitly excluded from this vision — see [scope-freeze-v1.md](./scope-freeze-v1.md).
- All design and build phases (BRD, HLD, ERD, LLD, APIs, UI, Mobile, Development) must trace back to the freeze document.

---

## Related documents

- [scope-freeze-v1.md](./scope-freeze-v1.md)
- [business-requirements-document.md](./business-requirements-document.md)
- [high-level-design.md](./high-level-design.md)
- [project-roadmap.md](./project-roadmap.md)
- Platform discovery: [../project-bootstrap/platform-discovery-report.md](../project-bootstrap/platform-discovery-report.md)
