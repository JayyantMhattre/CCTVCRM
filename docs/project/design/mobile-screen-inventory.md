# Mobile Screen Inventory

**Project:** Aarvii CCTV AMC Management System · **Phase:** D0-5
**Foundation (REUSE — mandated):** Ashraak Flutter Mobile Foundation — auth + secure storage, go_router, Riverpod, OpenAPI SDK, offline cache + background sync, push notifications, crash reporting, analytics, deep links, build flavors, fastlane release CI ([mobile module map](../../mobile/module-map.md), [mobile-architecture.md](../mobile-architecture.md)).

Classification: **REUSE** (existing mobile foundation feature/screen as-is) · **EXTEND** (existing foundation capability wired to CCTV data) · **NEW** (new CCTV Flutter feature slice on the foundation).

> **No duplicate mobile infrastructure is created.** New screens live in `lib/features/cctv_*` slices consuming the generated SDK, per the business-module convention.

---

## 1. Customer App (freeze §18: Dashboard · AMC · Tickets · Invoices · Notifications · Profile)

| # | Screen | Tab/Area | Class | Foundation reused |
|---|--------|----------|-------|--------------------|
| 1 | Login (+ OTP) | Auth | **REUSE** | `features/auth`, secure token storage, biometrics |
| 2 | Password Reset (OTP) | Auth | **REUSE** | `features/auth` + platform Auth; SMS OTP via backend |
| 3 | Customer Dashboard | Dashboard | NEW | shell, theming, offline cache (read) |
| 4 | AMC Details (active term) | AMC | NEW | SDK, offline cache |
| 5 | Plan & SLA view | AMC | NEW | SDK |
| 6 | Contract documents (PDF view) | AMC | NEW | `features/files` preview (REUSE: download + OpenFilex PDF) |
| 7 | Request Renewal | AMC | NEW | SDK |
| 8 | Upcoming Visits | Dashboard/AMC | NEW | SDK |
| 9 | Service History (approved) + Report PDF | AMC | NEW | files preview (REUSE) |
| 10 | My Tickets list | Tickets | NEW | SDK |
| 11 | Create Ticket (+ attachments) | Tickets | NEW | `features/files` upload (REUSE: camera/gallery/picker) |
| 12 | Ticket Detail / Reopen | Tickets | NEW | SDK |
| 13 | My Invoices list | Invoices | NEW | SDK |
| 14 | Invoice Detail + PDF download | Invoices | NEW | files preview (REUSE) |
| 15 | Notifications list (in-app) | Notifications | **EXTEND** | `core/notifications` push + platform events (§17) |
| 16 | Notification Preferences | Profile | **REUSE** | `features/notifications` (existing prefs screen) |
| 17 | Profile Management | Profile | **REUSE** | `features/profile` |
| 18 | Sessions | Profile | **REUSE** | `features/sessions` |

## 2. Engineer App (freeze §18: Visits · Tickets · Photo Upload · GPS · Signature · Offline Support)

| # | Screen | Area | Class | Foundation reused |
|---|--------|------|-------|--------------------|
| 19 | Login (+ OTP, biometrics) | Auth | **REUSE** | `features/auth` |
| 20 | Engineer Home / My Day | Home | NEW | shell, offline cache |
| 21 | Assigned Visits (offline-readable) | Visits | NEW | `core/offline` cache (REUSE) |
| 22 | Visit Detail | Visits | NEW | offline cache |
| 23 | Visit Reporting (checklist) | Visits | NEW | offline queue (`core/sync` REUSE) |
| 24 | Photo Upload (Before/During/After + video) | Visits | NEW | `features/files` upload + camera (REUSE), offline queue |
| 25 | Selfie Capture | Visits | NEW | camera + files (REUSE) |
| 26 | GPS Capture (lat/long/timestamp) | Visits | NEW | device location; capture-time stamping |
| 27 | Customer Signature Capture | Visits | NEW | touch canvas → files (REUSE) |
| 28 | Submit Report (sync on reconnect) | Visits | NEW | `core/sync` background sync (REUSE) |
| 29 | Returned Reports (rework queue) | Visits | NEW | SDK |
| 30 | Assigned Tickets | Tickets | NEW | SDK, offline read |
| 31 | Ticket Detail (progress) | Tickets | NEW | SDK |
| 32 | Create Ticket (during visit) | Tickets | NEW | SDK |
| 33 | Offline Sync Status | System | NEW (UI) on **REUSE** sync core | `core/offline`, `core/sync` |
| 34 | Profile / Sessions | Profile | **REUSE** | `features/profile`, `features/sessions` |

## 3. Foundation capabilities reused without any new screens

| Capability | Class | Used for |
|------------|-------|----------|
| Secure token storage | REUSE | All authenticated calls |
| Push notifications (`core/notifications`) | REUSE/EXTEND | Surfacing §17 events (visit scheduled, ticket assigned, invoice generated, AMC expiry) |
| Deep links | REUSE | Notification → screen routing (e.g. ticket detail) |
| Crash reporting & analytics | REUSE | Both apps |
| Feature flags | REUSE | Gated rollout of CCTV features |
| Build flavors + fastlane + release CI | REUSE | Shipping both apps |
| Correlation ID handling | REUSE | Support workflows |

## 4. Classification summary

| Class | Screens |
|-------|---------|
| REUSE | 8 (#1, 2, 16, 17, 18, 19, 34 + notification prefs) |
| EXTEND | 1 (#15 notifications list) |
| NEW | 25 (all CCTV feature slices) |
| **Total** | **34** |

## 5. Offline matrix (Engineer App — freeze §18)

| Flow | Offline behavior |
|------|------------------|
| View assigned visits/tickets | Cached reads (`core/offline`) |
| Capture photos/selfie/signature/GPS/remarks | Captured + queued locally; `captured_at` = capture time |
| Submit report | Queued; background sync uploads files → FileIds → submits aggregate; server re-validates checklist (BR-VISIT-01) |
| Conflict (e.g. visit cancelled while offline) | Server rejects on sync; item surfaces in Sync Status for engineer action |

Customer App is online-first (no offline requirement in freeze §18).

Related: [mobile-architecture.md](../mobile-architecture.md) · [screen-inventory.md](./screen-inventory.md) · [engineer-portal-navigation.md](./engineer-portal-navigation.md)
