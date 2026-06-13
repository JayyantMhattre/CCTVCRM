# Screen Inventory — V1 (Design, base-template aware)

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-5 — supersedes the preliminary [D0-2 screen inventory](../screen-inventory.md) by adding role, reuse classification, dependencies, and mobile support.

Legend — **Class:** REUSE (existing platform screen as-is) · EXTEND (existing platform screen with CCTV additions) · NEW (CCTV screen on the platform shell). **Mobile:** ✅ in V1 (freeze §18) · ➕ future-capable (foundation supports it; not in V1 scope) · — n/a.

---

## 1. Public Website (anonymous)

| # | Screen | Module | Role | Purpose | Class | Dependencies | Mobile |
|---|--------|--------|------|---------|-------|--------------|:------:|
| 1 | Home | Public Website | Visitor | Showcase | NEW (content reuse §2) | existing site content | — |
| 2 | About Us | Public Website | Visitor | Company info | NEW (content reuse) | content | — |
| 3 | Services | Public Website | Visitor | Services info | NEW (content reuse) | content | — |
| 4 | AMC Services | Public Website | Visitor | Plan information | NEW | AMC Plans (read-only marketing) | — |
| 5 | Contact Us | Public Website | Visitor | Inquiry form | NEW | Lead API (anonymous), rate limiting (REUSE) | — |
| 6 | Gallery | Public Website | Visitor | Showcase | NEW (content reuse) | content | — |
| 7 | Testimonials | Public Website | Visitor | Showcase | NEW (content reuse) | content | — |
| 8 | Get Quote | Public Website | Visitor | Quote request → lead | NEW | Lead API | — |
| 9 | AMC Inquiry | Public Website | Visitor | AMC inquiry → lead | NEW | Lead API | — |
| 10 | Login | Auth | All | Authentication | **REUSE** | platform Auth (MFA, OTP) | ✅ |

## 2. Admin Portal — business screens (role: Admin)

| # | Screen | Module | Purpose | Class | Dependencies | Mobile |
|---|--------|--------|---------|-------|--------------|:------:|
| 11 | Admin Dashboard | Reporting | KPIs + queues | **EXTEND** (platform dashboard page) | [dashboard-design.md](./dashboard-design.md) | ➕ |
| 12 | Lead List | Lead | Pipeline by status | NEW | `leads:read` | ➕ |
| 13 | Lead Detail | Lead | Status transitions, activities, remarks, attachments | NEW | `leads:manage`, Files (REUSE) | ➕ |
| 14 | Lead Conversion | Lead | Won → Customer+Site+Contract | NEW | `leads:convert`, Customer/AMC contracts | ➕ |
| 15 | Customer List | Customer | Manage customers | NEW | `customers:read` | ➕ |
| 16 | Customer Detail | Customer | Customer + sites overview | NEW | `customers:read`, sites | ➕ |
| 17 | Customer Create/Edit | Customer | Maintain customers; portal account link | NEW | `customers:manage`, platform Users (REUSE) | ➕ |
| 18 | Site Detail | Site | Contacts (≤3), assets, contract, visits, tickets, invoices | NEW | `sites:read`, aggregation reads | ➕ |
| 19 | Site Create/Edit | Site | Maintain sites + contacts | NEW | `sites:manage` | ➕ |
| 20 | Asset Summary Edit | Asset | Summary counts + brand/model/remarks | NEW | `sites:manage` (no device tracking, §7) | ➕ |
| 21 | AMC Plan List | AMC Plans | Plans + versions | NEW | `amcplans:read` | ➕ |
| 22 | AMC Plan Detail / New Version | AMC Plans | Versioned price/frequency/services/SLA | NEW | `amcplans:manage` | ➕ |
| 23 | Contract List | AMC Contracts | All contracts | NEW | `amc:read` | ➕ |
| 24 | Contract Detail | AMC Contracts | Master + full term history (BR-AMC-04) + documents | NEW | `amc:read`, Files (REUSE), PDF | ➕ |
| 25 | Term Create / Renew | AMC Contracts | New term (new/renewal) | NEW | `amc:manage`, plan versions | ➕ |
| 26 | Schedule Calendar/List | Scheduling | Visit slots by status/date | NEW | `schedules:read` | ➕ |
| 27 | Schedule Detail (assign/reschedule) | Scheduling | Mandatory engineer assignment; reschedule/cancel | NEW | `visits:assign`, `schedules:manage`, engineers | ➕ |
| 28 | Visit Report Review Queue | Visits | Submitted reports pending approval | NEW | `visits:approve` | ➕ |
| 29 | Visit Report Review Detail | Visits | Evidence review → approve/return (BR-VISIT-04) | NEW | `visits:approve`, Files (REUSE) | ➕ |
| 30 | Ticket List (all) | Tickets | Status/priority queues | NEW | `tickets:read` | ➕ |
| 31 | Ticket Detail (admin) | Tickets | Assign, progress, close; timeline | NEW | `tickets:assign/update/close` | ➕ |
| 32 | Ticket Create (admin) | Tickets | Raise on behalf | NEW | `tickets:create` | ➕ |
| 33 | Engineer List | Engineer | Manage engineers | NEW | `engineers:read` | ➕ |
| 34 | Engineer Detail/Edit | Engineer | Profile + workload; account link | NEW | `engineers:manage`, platform Users (REUSE) | ➕ |
| 35 | Invoice List | Invoice | All invoices by status/type | NEW | `invoices:read` | ➕ |
| 36 | Invoice Create/Edit (Draft) | Invoice | Option B types + lines | NEW | `invoices:manage` | ➕ |
| 37 | Invoice Detail | Invoice | Lifecycle actions + PDF + history | NEW | `invoices:manage`, Files (REUSE) | ➕ |
| 38 | Reports Hub | Reporting | Leads/AMC/visits/tickets/invoices views | NEW | `reports:read` | ➕ |

## 3. Admin Portal — Administration group (all REUSE)

| # | Screen | Module | Purpose | Class | Mobile |
|---|--------|--------|---------|-------|:------:|
| 39 | User List / User Profile | Platform Users | Manage portal accounts | **REUSE** | ➕ (planned by platform) |
| 40 | Tenant Profile / Settings | Platform Tenant | Workspace settings | **REUSE** | ✅ (platform) |
| 41 | Audit Logs | Platform Audit | Compliance trail (`audit:read`) | **REUSE** | ✅ (platform) |
| 42 | API Keys (list/detail) | Platform ApiKeys | M2M keys | **REUSE** | ✅ (platform, read-only) |
| 43 | Webhooks Operations Center | Platform Webhooks | Subscriptions/deliveries/DLQ | **REUSE** | ✅ (platform, read-only) |
| 44 | Sessions | Platform Auth | Session management | **REUSE** | ✅ (platform) |
| 45 | My Profile / Notification Preferences | Platform Users | Self profile | **REUSE** | ✅ (platform) |

## 4. Customer Portal (role: Customer)

| # | Screen | Module | Purpose | Class | Dependencies | Mobile (Customer App §18) |
|---|--------|--------|---------|-------|--------------|:------:|
| 46 | Customer Dashboard | Customer Portal | AMC status, next visit, open tickets, dues | NEW | scoped reads | ✅ |
| 47 | AMC Details | AMC | Active term only (BR-AMC-03); plan inclusions/SLA | NEW | `amc:read` | ✅ |
| 48 | Request Renewal | AMC | Renewal request (BR-AMC-08) | NEW | `amc:request-renewal` | ✅ |
| 49 | Upcoming Visits | Scheduling | Scheduled visits for own sites | NEW | `schedules:read` | ✅ |
| 50 | Service History | Visits | **Approved** reports + Report PDF (BR-VISIT-05) | NEW | `visits:read`, Files (REUSE) | ✅ |
| 51 | My Tickets | Tickets | Own tickets list | NEW | `tickets:read` | ✅ |
| 52 | Ticket Create | Tickets | Raise complaint (+attachments) | NEW | `tickets:create`, Files (REUSE) | ✅ |
| 53 | Ticket Detail + Reopen | Tickets | Timeline, comments, reopen (BR-TKT-06) | NEW | `tickets:reopen` | ✅ |
| 54 | My Invoices | Invoice | Own invoices | NEW | `invoices:read` | ✅ |
| 55 | Invoice Detail + PDF download | Invoice | BR-INV-03 | NEW | `invoices:download`, Files (REUSE) | ✅ |
| 56 | Profile Management | Platform | Own profile (BR-AUTH-05) | **REUSE** | platform Users | ✅ |
| 57 | Password Reset (OTP) | Platform | Self-service (§17) | **REUSE** | platform Auth + SMS (EXTEND) | ✅ |
| 58 | Notifications (in-app list) | Platform | Surface §17 events | **EXTEND** (platform notification plumbing) | mobile push foundation | ✅ |

## 5. Engineer Portal (role: Engineer)

| # | Screen | Module | Purpose | Class | Dependencies | Mobile (Engineer App §18) |
|---|--------|--------|---------|-------|--------------|:------:|
| 59 | Engineer Home / My Day | Engineer Portal | Today's assigned work | NEW | scoped reads | ✅ |
| 60 | Assigned Visits | Scheduling | Visit queue by date/status | NEW | `schedules:read` | ✅ (offline read) |
| 61 | Visit Detail | Visits | Site, contract context, checklist state | NEW | `visits:read` | ✅ (offline read) |
| 62 | Visit Reporting | Visits | Remarks + evidence checklist + submit (BR-VISIT-01) | NEW | `visits:execute` | ✅ (offline capture) |
| 63 | Photo/Video Upload | Visits | Before/During/After + video | NEW (uses platform file-upload component) | Files (REUSE) | ✅ (offline queue) |
| 64 | Selfie Capture | Visits | Mandatory selfie | NEW | Files (REUSE), camera | ✅ |
| 65 | GPS Capture | Visits | Lat/long/timestamp (BR-VISIT-02) | NEW | device GPS | ✅ |
| 66 | Customer Signature | Visits | On-device signature | NEW | Files (REUSE), touch | ✅ |
| 67 | Assigned Tickets | Tickets | Ticket queue | NEW | `tickets:read` | ✅ |
| 68 | Ticket Detail (engineer) | Tickets | Progress updates, comments | NEW | `tickets:update` | ✅ |
| 69 | Ticket Create (during visit) | Tickets | Raise fault found on site (BR-TKT-05) | NEW | `tickets:create` | ✅ |
| 70 | Profile & Sessions | Platform | Self profile/security | **REUSE** | platform | ✅ |
| 71 | Offline Sync Status | Engineer Portal | Pending queue state | NEW (on platform offline/sync foundation) | mobile core/offline (REUSE) | ✅ (mobile-only) |

---

## Summary

| Class | Screens |
|-------|---------|
| **REUSE** (platform screens as-is) | 11 (#10, 39–45, 56, 57, 70) |
| **EXTEND** (platform screen + CCTV additions) | 2 (#11 admin dashboard, #58 notifications) |
| **NEW** (CCTV screens on platform shell/components) | 58 |
| **Total** | **71** |

Every NEW screen is built from `platform-ui` primitives and platform shared components (file-upload, toasts, guards) — no theme imports, no duplicate infrastructure.

Related: [navigation-architecture.md](./navigation-architecture.md) · [dashboard-design.md](./dashboard-design.md) · [mobile-screen-inventory.md](./mobile-screen-inventory.md) · [rbac-matrix.md](./rbac-matrix.md)
