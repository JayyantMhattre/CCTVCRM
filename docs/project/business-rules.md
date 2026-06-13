# Business Rules

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0 — Project Foundation Documentation
**Source of truth:** [requirements-freeze-v1.md](./requirements-freeze-v1.md) (Approved & Frozen, V1)

> All rules below are restated from the freeze document with stable IDs for traceability. No rule has been added, modified, or removed.

---

## 1. Lead Management (§10)

| ID | Rule |
|----|------|
| BR-LEAD-01 | Lead statuses are exactly: New, Contacted, Qualified, Quotation Sent, Negotiation, Won, Lost, Converted. |
| BR-LEAD-02 | Website inquiries automatically create leads. |
| BR-LEAD-03 | Converting a lead creates a Customer, a Site, and an Initial AMC Contract. |

## 2. AMC Management (§6, §8, §9)

| ID | Rule |
|----|------|
| BR-AMC-01 | An AMC contract uses the Master + Terms model: one permanent contract record, with renewal history as contract terms. |
| BR-AMC-02 | One site can have only **one active AMC contract** at a time. |
| BR-AMC-03 | A customer sees only the **current active term** of a contract. |
| BR-AMC-04 | Admin sees the **complete renewal history** of a contract. |
| BR-AMC-05 | An AMC plan stores: Price, Visit Frequency, Included Services, SLA. |
| BR-AMC-06 | AMC plans are versioned. |
| BR-AMC-07 | Historical contracts must not change when plans are modified. |
| BR-AMC-08 | A customer may request AMC renewal (§3). |

### Customer/site structure rules (§5–§7)

| ID | Rule |
|----|------|
| BR-STRUCT-01 | One site belongs to exactly one customer. |
| BR-STRUCT-02 | A customer can have multiple sites. |
| BR-STRUCT-03 | A site can have a maximum of three contact persons. |
| BR-STRUCT-04 | Assets are tracked as summary counts per site (Camera, DVR, NVR, Hard Disk, Switch, Router, Monitor); individual cameras are not tracked. |
| BR-STRUCT-05 | Asset summary may optionally record Brand, Model, Remarks. |

## 3. Scheduling (§11)

| ID | Rule |
|----|------|
| BR-SCHED-01 | Visit schedule statuses are exactly: Planned, Assigned, In Progress, Completed, Missed, Cancelled. |
| BR-SCHED-02 | Visits are automatically generated from the AMC plan's visit frequency. |
| BR-SCHED-03 | Admin may reschedule visits. |
| BR-SCHED-04 | Engineer assignment is mandatory. |

## 4. Visits (§12, §13, §15)

| ID | Rule |
|----|------|
| BR-VISIT-01 | A visit cannot be completed without: engineer selfie, GPS coordinates, minimum one visit photo, customer signature, and visit remarks. |
| BR-VISIT-02 | GPS capture stores latitude, longitude, and timestamp. |
| BR-VISIT-03 | Supported photo categories: Before, During, After. |
| BR-VISIT-04 | Visit reports follow the approval workflow: Engineer submits → Admin reviews → Admin approves → Customer can view. |
| BR-VISIT-05 | A customer cannot view a visit report before admin approval. |
| BR-VISIT-06 | Engineers can upload photos, videos, and reports, and view assigned work. |
| BR-VISIT-07 | Engineers cannot manage customers, AMC plans, or contracts. |

## 5. Tickets (§14)

| ID | Rule |
|----|------|
| BR-TKT-01 | Ticket statuses are exactly: Open, Assigned, In Progress, Resolved, Closed, Reopened. |
| BR-TKT-02 | Ticket priorities are exactly: Low, Medium, High, Critical. |
| BR-TKT-03 | A customer may create a ticket. |
| BR-TKT-04 | An admin may create a ticket. |
| BR-TKT-05 | An engineer may create a ticket (including during visits, §3). |
| BR-TKT-06 | A customer may reopen a closed ticket. |

## 6. Invoices (§16, §19)

| ID | Rule |
|----|------|
| BR-INV-01 | Invoice statuses are exactly: Draft, Generated, Sent, Paid, Cancelled. |
| BR-INV-02 | Every invoice is linked to an AMC Contract Term. |
| BR-INV-03 | A customer can download their invoice. |
| BR-INV-04 | Invoice PDF generation is required. |
| BR-INV-05 | No accounting features in V1. |

## 7. Notifications (§17)

| ID | Rule |
|----|------|
| BR-NOTIF-01 | Notification channels are Email and SMS. |
| BR-NOTIF-02 | Notifications fire on exactly these events: Lead Created, Lead Converted, Ticket Created, Ticket Assigned, Ticket Closed, Visit Scheduled, Visit Completed, AMC Expiry Reminder, Invoice Generated, Password Reset OTP, Login OTP. |

## 8. Authentication (§2, §3, §17, §20)

| ID | Rule |
|----|------|
| BR-AUTH-01 | Authentication reuses the base platform Auth capability — no duplicate implementation (§20). |
| BR-AUTH-02 | Public Visitors require no authentication; Customer, Engineer, and Admin functions require login (§3). |
| BR-AUTH-03 | Password Reset is delivered via OTP notification (§17). |
| BR-AUTH-04 | Login OTP is a supported notification event (§17). |
| BR-AUTH-05 | Customers can update their own profile and reset their password (§3). |
| BR-AUTH-06 | Access follows the four-actor permission model (§3): customers see only their own contracts, invoices, and service history; engineers see only assigned work; admin manages all modules. |

---

## Rule count summary

| Group | Rules |
|-------|-------|
| Lead Management | 3 |
| AMC Management (incl. structure) | 13 |
| Scheduling | 4 |
| Visits | 7 |
| Tickets | 6 |
| Invoices | 5 |
| Notifications | 2 |
| Authentication | 6 |
| **Total** | **46** |

---

## Related documents

- [requirements-freeze-v1.md](./requirements-freeze-v1.md)
- [business-requirements-document.md](./business-requirements-document.md)
- [workflow-overview.md](./workflow-overview.md)
