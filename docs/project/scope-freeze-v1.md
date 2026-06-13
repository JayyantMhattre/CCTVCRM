# Scope Freeze — V1

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0 — Project Foundation Documentation
**Source of truth:** [requirements-freeze-v1.md](./requirements-freeze-v1.md) (Approved & Frozen, V1)

> This document restates the frozen scope for quick reference. It introduces **no** additions, removals, or modifications. On any discrepancy, `requirements-freeze-v1.md` wins.

---

## 1. In scope (V1)

| Area | Scope | Freeze ref |
|------|-------|-----------|
| Public website | Business showcase, lead generation, AMC plan info, contact inquiries, quote requests at [www.aarvii.in](http://www.aarvii.in); reuse existing content where possible | §2 |
| Lead management | Lead capture (auto from website), lifecycle to conversion (Customer + Site + Initial AMC Contract) | §10 |
| Customer & site management | Customers with multiple sites; ≤3 contact persons per site | §5, §6 |
| Asset management | Summary counts per site (Camera, DVR, NVR, Hard Disk, Switch, Router, Monitor) + optional brand/model/remarks; **no individual camera tracking** | §7 |
| AMC plans | Plans (e.g. Silver/Gold/Platinum) with price, visit frequency, included services, SLA; **plan versioning required** | §9 |
| AMC contracts | Master + Terms model; one active contract per site; renewal history preserved | §6, §8 |
| Service scheduling | Auto-generated visits from AMC frequency; reschedule by admin; mandatory engineer assignment | §11 |
| Visit management | Field evidence (selfie, GPS, photos before/during/after, signature, remarks); admin approval before customer visibility | §12, §13 |
| Ticket management | Full lifecycle with priorities; created by Customer/Admin/Engineer; customer reopen | §14 |
| Engineer management | Engineer work queues and restrictions | §3, §15 |
| Invoice management | Lifecycle Draft→Cancelled; linked to AMC Contract Term; customer PDF download | §16 |
| Notifications | Email + SMS on the 11 approved events | §17 |
| Reporting | Admin reporting module | §2 (Admin Portal) |
| Customer Portal | Web + mobile self-service | §2 |
| Engineer Portal | Web + mobile field operations | §2 |
| Mobile apps | Flutter Customer App + Engineer App (engineer offline support) | §18 |
| PDF documents | AMC Contract PDF, Visit Report PDF, Invoice PDF | §19 |
| Platform reuse | Reuse base platform capabilities; no duplicate implementation | §20 |

## 2. Out of scope (V1)

Per freeze §21 — **not included**:

- Inventory Stock Management
- Purchase Orders
- Vendor Management
- Accounting (and no accounting features in invoicing, §16)
- GST Filing
- ERP
- CRM (generic CRM beyond the approved lead/customer modules)
- Payroll
- Attendance
- Geo Tracking (continuous tracking; visit GPS *capture* remains in scope per §12)
- WhatsApp Integration
- Payment Gateway
- AI Features

Additionally out of scope by definition:

- Individual camera/asset tracking (only summary counts, §7)
- Modifying historical contracts when plans change (plan versioning protects history, §9)
- Customer visibility of unapproved visit reports (§13)

## 3. Approved modules (15)

Per freeze §4:

| # | Module |
|---|--------|
| 1 | Public Website |
| 2 | Lead Management |
| 3 | Customer Management |
| 4 | Site Management |
| 5 | Asset Management |
| 6 | AMC Plans |
| 7 | AMC Contracts |
| 8 | Service Scheduling |
| 9 | Visit Management |
| 10 | Ticket Management |
| 11 | Engineer Management |
| 12 | Invoice Management |
| 13 | Reporting |
| 14 | Customer Portal |
| 15 | Engineer Portal |

## 4. Approved actors (4)

Per freeze §3 — detailed in [actors-and-personas.md](./actors-and-personas.md):

| Actor | Summary of permitted actions |
|-------|------------------------------|
| **Public Visitor** | Browse website, submit inquiries, request quotations |
| **Customer** | View own AMC contracts, invoices, service history; raise tickets; reopen closed tickets; request AMC renewal; update profile |
| **Engineer** | View assigned work; create tickets during visits; upload visit reports, photos, selfie; capture GPS; capture customer signature |
| **Admin** | Manage all modules; approve visit reports; manage contracts, invoices, engineers, customers |

## 5. Approved mobile scope

Per freeze §18 — **Flutter**, two apps:

| App | Features |
|-----|----------|
| **Customer App** | Dashboard · AMC · Tickets · Invoices · Notifications · Profile |
| **Engineer App** | Visits · Tickets · Photo Upload · GPS Capture · Signature Capture · **Offline Support** |

## 6. Approved website scope

Per freeze §2 — Public Website at [www.aarvii.in](http://www.aarvii.in), reusing existing public website content wherever possible.

**Pages:** Home · About Us · Services · AMC Services · Contact Us · Gallery · Testimonials · Login

**Enhancements:** Get Quote · AMC Inquiry

## 7. Scope freeze declaration

Per freeze §22: this baseline is **approved and frozen**. All future BRD, HLD, ERD, LLD, APIs, UI designs, mobile designs, and development phases must follow it. Requirements may only change through an approved change request process.

---

## Related documents

- [requirements-freeze-v1.md](./requirements-freeze-v1.md) — authoritative source
- [business-requirements-document.md](./business-requirements-document.md)
- [module-architecture.md](./module-architecture.md)
- [project-roadmap.md](./project-roadmap.md)
