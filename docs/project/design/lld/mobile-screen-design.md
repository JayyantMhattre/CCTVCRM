# Mobile Screen Design

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7
**Apps:** Customer (18 screens) · Engineer (16 CCTV + platform) · [mobile-screen-inventory.md](../mobile-screen-inventory.md)

Flutter features on platform foundation — **no duplicate mobile infrastructure**.

---

## Customer Mobile

| # | Screen | Fields displayed | Primary actions | Offline | Camera | GPS | Signature | Files |
|---|--------|------------------|-----------------|:-------:|:------:|:---:|:---------:|:-----:|
| 1 | Login | email, password, OTP | Login | ❌ | ❌ | ❌ | ❌ | ❌ |
| 2 | Password reset | phone/email, OTP, new password | Reset | ❌ | ❌ | ❌ | ❌ | ❌ |
| 3 | Dashboard | AMC summary, next visit, tickets, invoices | Navigate tabs | read cache | ❌ | ❌ | ❌ | ❌ |
| 4 | AMC Details | plan, term dates, SLA, inclusions | Request renewal | cache | ❌ | ❌ | ❌ | ❌ |
| 5 | Plan & SLA | services list, SLA text | Back | cache | ❌ | ❌ | ❌ | ❌ |
| 6 | Contract PDFs | document list | Open PDF preview | ❌ | ❌ | ❌ | ❌ | **REUSE** download |
| 7 | Request Renewal | message (optional) | Submit | ❌ | ❌ | ❌ | ❌ | ❌ |
| 8 | Upcoming Visits | date, site, status list | Open detail | cache | ❌ | ❌ | ❌ | ❌ |
| 9 | Service History | approved visits list | Open report, PDF | cache | ❌ | ❌ | ❌ | **REUSE** PDF |
| 10 | My Tickets | ticket number, subject, status | Open, FAB create | cache | ❌ | ❌ | ❌ | ❌ |
| 11 | Create Ticket | site, subject, description, priority, attachments | Submit | ❌ | **REUSE** gallery/camera | ❌ | ❌ | **REUSE** upload |
| 12 | Ticket Detail | timeline, comments | Comment, reopen | ❌ | ❌ | ❌ | ❌ | view attachments |
| 13 | My Invoices | number, amount, status | Open detail | cache | ❌ | ❌ | ❌ | ❌ |
| 14 | Invoice Detail | lines, totals, dates | Download PDF | ❌ | ❌ | ❌ | ❌ | **REUSE** |
| 15 | Notifications | event list | Tap → deep link | ❌ | ❌ | ❌ | ❌ | ❌ |
| 16 | Notification Prefs | email toggle | Save | ❌ | ❌ | ❌ | ❌ | ❌ REUSE |
| 17 | Profile | name, phone, email | Save | ❌ | ❌ | ❌ | ❌ | ❌ REUSE |
| 18 | Sessions | session list | Revoke | ❌ | ❌ | ❌ | ❌ | ❌ REUSE |

**Navigation:** Bottom tabs — Dashboard · AMC · Tickets · Invoices · Profile (Notifications via bell icon)

---

## Engineer Mobile

| # | Screen | Fields | Actions | Offline | Camera | GPS | Signature | Files |
|---|--------|--------|---------|:-------:|:------:|:---:|:---------:|:-----:|
| 19 | Login | credentials, biometrics | Login | ❌ | ❌ | ❌ | ❌ | ❌ |
| 20 | Home / My Day | today visits, ticket count, sync badge | Open visit/ticket | **read** | ❌ | ❌ | ❌ | ❌ |
| 21 | Assigned Visits | list by date/status | Open visit | **read** | ❌ | ❌ | ❌ | ❌ |
| 22 | Visit Detail | site, schedule, checklist summary | Start visit | **read** | ❌ | ❌ | ❌ | ❌ |
| 23 | Visit Reporting | checklist tiles, remarks field | Navigate evidence | **write queue** | ❌ | ❌ | ❌ | ❌ |
| 24 | Photo Upload | category tabs B/D/A + video | Capture/select, upload queue | **queue** | **✅** | ❌ | ❌ | **REUSE** |
| 25 | Selfie | camera preview | Capture | **queue** | **✅** front | ❌ | ❌ | **REUSE** |
| 26 | GPS Capture | lat, lng, time readonly | Capture location | **queue** | ❌ | **✅** | ❌ | ❌ |
| 27 | Signature | canvas | Clear, save | **queue** | ❌ | ❌ | **✅** canvas | **REUSE** |
| 28 | Submit Report | checklist status | Submit / queue | **sync** | ❌ | ❌ | ❌ | ❌ |
| 29 | Returned Reports | list | Open rework | read | ❌ | ❌ | ❌ | ❌ |
| 30 | Assigned Tickets | list | Open | read | ❌ | ❌ | ❌ | ❌ |
| 31 | Ticket Detail | status, comments | Update status, comment | online | ❌ | ❌ | ❌ | ❌ |
| 32 | Create Ticket | subject, description, priority | Submit | online | optional photo | ❌ | ❌ | REUSE |
| 33 | Sync Status | pending uploads, failed items | Retry, discard | local | ❌ | ❌ | ❌ | ❌ |
| 34 | Profile / Sessions | REUSE platform | REUSE | ❌ | ❌ | ❌ | ❌ | ❌ |

**Navigation:** Bottom tabs — Home · Visits · Tickets · Sync (badge) · Profile

---

## Visit reporting UX (Engineer — critical path)

```
Visit Detail → Start → Reporting hub (checklist)
  ├─ Selfie (camera, required)
  ├─ GPS (one-tap capture, required)
  ├─ Photos (≥1 across Before/During/After, required)
  ├─ Video (optional)
  ├─ Signature (canvas, required)
  ├─ Remarks (required, min 20 chars)
  └─ Submit (disabled until all green)
```

**Offline:** Each capture writes to local queue with `clientCorrelationId` · Sync Status #33 shows progress · Submit queues until files uploaded

---

## Platform mobile components (REUSE)

| Capability | Usage |
|------------|--------|
| `features/files` | All uploads/downloads |
| `features/auth` | Login, OTP, biometrics |
| `core/offline` | Engineer read cache |
| `core/sync` | Background upload + batch submit |
| `core/notifications` | Push + deep links |
| OpenAPI SDK | All CCTV API calls |

## NEW Flutter widgets

| Widget | Screen |
|--------|--------|
| `VisitChecklistTile` | #23 |
| `SignatureCanvas` | #27 |
| `GpsCaptureTile` | #26 |
| `SyncQueueList` | #33 |
| `EvidenceCategoryTabs` | #24 |

Styled with app theme — not vendor-specific business UI.

---

## Responsive / device rules

| Rule | Detail |
|------|--------|
| Orientation | Portrait primary for signature/camera |
| Tablet | Engineer app uses two-pane list/detail where width > 600dp |
| Permissions | Runtime camera, location, storage prompts with rationale dialogs |
| Min OS | Per platform manifest (Android/iOS) |

---

Related: [mobile-api-consumption.md](../mobile-api-consumption.md) · [file-upload-design.md](./file-upload-design.md) · [validation-rules.md](./validation-rules.md)
