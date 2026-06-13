# Screen Inventory — V1

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0 — Project Foundation Documentation
**Source of truth:** [requirements-freeze-v1.md §2, §18](./requirements-freeze-v1.md)

All screens expected in V1, categorized by module. Every screen traces to an approved page/feature; this inventory adds no functionality. Apps: **PW** = Public Website, **CP** = Customer Portal (web), **EP** = Engineer Portal (web), **AP** = Admin Portal, **CA** = Customer App, **EA** = Engineer App.

---

## 1. Public Website module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 1 | Home | PW | §2 |
| 2 | About Us | PW | §2 |
| 3 | Services | PW | §2 |
| 4 | AMC Services | PW | §2 |
| 5 | Contact Us | PW | §2 |
| 6 | Gallery | PW | §2 |
| 7 | Testimonials | PW | §2 |
| 8 | Login | PW | §2 |
| 9 | Get Quote (form + confirmation) | PW | §2 |
| 10 | AMC Inquiry (form + confirmation) | PW | §2 |

## 2. Lead Management module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 11 | Lead List (pipeline / status filter) | AP | §10 |
| 12 | Lead Detail (status transitions, notes) | AP | §10 |
| 13 | Lead Conversion (creates Customer + Site + Initial AMC Contract) | AP | §10 |

## 3. Customer Management module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 14 | Customer List | AP | §2 |
| 15 | Customer Detail (with sites overview) | AP | §5 |
| 16 | Customer Create / Edit | AP | §3 |

## 4. Site Management module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 17 | Site List (per customer) | AP | §6 |
| 18 | Site Detail (contacts, asset summary, contract, visits, tickets, invoices) | AP | §5 |
| 19 | Site Create / Edit (incl. contact persons, max 3) | AP | §6 |

## 5. Asset Management module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 20 | Asset Summary View / Edit (counts + brand/model/remarks) | AP | §7 |

## 6. AMC Plans module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 21 | Plan List | AP | §9 |
| 22 | Plan Detail / Versions (price, frequency, services, SLA) | AP | §9 |
| 23 | Plan Create / New Version | AP | §9 |

## 7. AMC Contracts module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 24 | Contract List | AP | §8 |
| 25 | Contract Detail (master + term history) | AP | §8 |
| 26 | Contract Term Create / Renew | AP | §8 |
| 27 | AMC Contract PDF (view/download) | AP | §19 |
| 28 | AMC Details (current active term) | CP, CA | §2, §8 |
| 29 | Request AMC Renewal | CP, CA | §3 |

## 8. Service Scheduling module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 30 | Visit Schedule List / Calendar (status filters) | AP | §11 |
| 31 | Visit Detail (admin) — assign engineer, reschedule, cancel | AP | §11 |
| 32 | Upcoming Visits | CP, CA | §2 |

## 9. Visit Management module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 33 | Assigned Visits (queue) | EP, EA | §2 |
| 34 | Visit Detail (engineer) | EP, EA | §2 |
| 35 | Visit Reporting (remarks + evidence checklist) | EP, EA | §12 |
| 36 | Photo Upload (Before/During/After, video) | EP, EA | §12, §15 |
| 37 | Selfie Capture | EP, EA | §12 |
| 38 | GPS Capture (lat/long/timestamp) | EP, EA | §12 |
| 39 | Customer Signature Capture | EP, EA | §12 |
| 40 | Visit Report Review & Approval | AP | §13 |
| 41 | Service History (approved reports) | CP, CA* | §2, §13 |
| 42 | Visit Report PDF (view/download) | AP, CP | §19 |

\* Service history on mobile surfaces under the Customer App AMC/Dashboard areas (§18).

## 10. Ticket Management module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 43 | Ticket List (customer's own) | CP, CA | §14 |
| 44 | Ticket Create (customer) | CP, CA | §14 |
| 45 | Ticket Detail + Reopen (customer) | CP, CA | §14 |
| 46 | Assigned Tickets (engineer queue) | EP, EA | §2 |
| 47 | Ticket Detail (engineer, progress) | EP, EA | §14 |
| 48 | Ticket Create (engineer, during visit) | EP, EA | §3 |
| 49 | Ticket List (all, status/priority filters) | AP | §14 |
| 50 | Ticket Detail / Assign / Close (admin) | AP | §14 |
| 51 | Ticket Create (admin) | AP | §14 |

## 11. Engineer Management module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 52 | Engineer List | AP | §2 |
| 53 | Engineer Detail (assigned work overview) | AP | §2 |
| 54 | Engineer Create / Edit | AP | §3 |

## 12. Invoice Management module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 55 | Invoice List (admin, status filters) | AP | §16 |
| 56 | Invoice Detail (lifecycle actions) | AP | §16 |
| 57 | Invoice Create / Generate (linked to contract term) | AP | §16 |
| 58 | Invoice List (customer's own) | CP, CA | §2 |
| 59 | Invoice Detail / Download PDF | CP, CA | §16, §19 |

## 13. Reporting module

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 60 | Reporting Dashboard | AP | §2 |
| 61 | Report Views (leads, AMC, visits, tickets, invoices) | AP | §2 |

## 14. Customer Portal shell (cross-module)

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 62 | Customer Dashboard | CP, CA | §2, §18 |
| 63 | Profile Management | CP, CA | §2 |
| 64 | Password Reset (OTP) | CP, CA | §2, §17 |
| 65 | Notifications (in-app list) | CA | §18 |

## 15. Engineer Portal shell (cross-module)

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 66 | Engineer Home / Work Summary | EP, EA | §2 |
| 67 | Offline Sync Status | EA | §18 |

## Authentication (platform reuse, §20)

| # | Screen | App | Freeze ref |
|---|--------|-----|-----------|
| 68 | Login (portal/app) | CP, EP, AP, CA, EA | §2, §3 |
| 69 | Login OTP entry | CP, CA, EP, EA | §17 |

---

## Summary

| Category | Screens |
|----------|---------|
| Public Website | 10 |
| Admin Portal | 31 |
| Customer Portal / App | 16 |
| Engineer Portal / App | 12 |
| **Total (deduplicated rows)** | **69** |

> Counts reflect inventory rows; shared screens (web + mobile) are listed once with multiple app tags. Final screen designs are produced in D1 (Design) — see [project-roadmap.md](./project-roadmap.md).

---

## Related documents

- [navigation-map.md](./navigation-map.md)
- [application-architecture.md](./application-architecture.md)
- [mobile-architecture.md](./mobile-architecture.md)
