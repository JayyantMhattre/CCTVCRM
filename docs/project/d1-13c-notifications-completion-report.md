# Sprint 1 — D1-13c Completion Report

**Phase:** D1-13c CCTV Notifications  
**Date:** 2026-06-12  
**Wave:** D1-13 Wave 2 (stop after Wave 2)  
**Review gate:** Deferred test execution (restore + build + architecture tests)

## Summary

Delivered production CCTV notification handlers reusing platform `INotificationService`, user email preferences, and a configuration-driven SMS provider. All freeze §17 CCTV business events are wired via domain event handlers dispatched through module outbox processors.

## Requirements closed

| ID | Status |
|----|--------|
| FR-NOTIF-01 | **Closed** — Email via platform templates; SMS via `ConfiguredSmsProvider` |
| FR-NOTIF-02 | **Closed** — User `emailNotificationsEnabled` respected for admin/engineer/customer (email match) |
| FR-LEAD-03 | **Closed** — Lead Created, Lead Converted (+ customer welcome) |
| FR-AMC-04 | **Closed** — Renewal Requested + AMC Expiry Reminder (30-day job) |
| FR-SCHED-05 | **Closed** — Visit Scheduled (admin/customer); engineer SMS on assignment |
| FR-TKT-05 | **Closed** — Ticket Created, Assigned, Closed |
| FR-INV-04 | **Closed** — Invoice Generated |

## Events implemented

| Event | Handler | Template(s) | SMS |
|-------|---------|-------------|:---:|
| Lead Created | `LeadCreatedNotificationHandler` | `cctv-lead-created` | — |
| Lead Converted | `LeadConvertedNotificationHandler` | `cctv-lead-converted`, `cctv-customer-welcome` | — |
| AMC Renewal Requested | `RenewalRequestedNotificationHandler` | `cctv-amc-renewal-requested` | — |
| AMC Expiry Reminder | `CctvAmcExpiryReminderHostedService` | `cctv-amc-expiry-reminder` | ✅ |
| Visit Scheduled | `VisitScheduledNotificationHandler` | `cctv-visit-scheduled` | ✅ (customer) |
| Engineer Assigned | `EngineerAssignedNotificationHandler` | `cctv-visit-scheduled` | ✅ |
| Visit Completed | `VisitCompletedNotificationHandler` | `cctv-visit-completed` | — |
| Visit Approved | `VisitApprovedNotificationHandler` | `cctv-visit-approved` | — |
| Ticket Created | `TicketCreatedNotificationHandler` | `cctv-ticket-created` | — |
| Ticket Assigned | `TicketAssignedNotificationHandler` | `cctv-ticket-assigned` | ✅ |
| Ticket Closed | `TicketClosedNotificationHandler` | `cctv-ticket-closed` | — |
| Invoice Generated | `InvoiceGeneratedNotificationHandler` | `cctv-invoice-generated` | — |

## Backend deliverables

| Area | Path |
|------|------|
| Dispatcher | `Integration.Infrastructure/Notifications/CctvNotificationDispatcher.cs` |
| Handlers | `Integration.Infrastructure/Notifications/EventHandlers/*.cs` |
| SMS provider | `Integration.Infrastructure/Services/ConfiguredSmsProvider.cs` |
| Expiry job | `Integration.Infrastructure/Services/CctvAmcExpiryReminderHostedService.cs` |
| Templates | `Host/Ashraak.Api/Templates/cctv/*.txt` (12 files) |
| Outbox | CCTV DbContext processors registered in `CctvIntegrationModule.cs` |
| Config | `Cctv:Notifications`, `Sms` sections in `appsettings.json` |

## Architecture notes

- No parallel notification system — all sends go through `ICctvNotificationDispatcher` → platform `INotificationService`.
- Background/outbox handlers resolve tenant via `Cctv:Notifications:DefaultTenantId` when HTTP tenant context is empty.
- Push/in-app channels remain future-ready per notification-mapping (not in Wave 2 scope).

## Verification

```bash
dotnet build BackEnd/src/Host/Ashraak.Api
dotnet test BackEnd/tests/Ashraak.Architecture.Tests   # 21/21 PASS
```

## Remaining gaps (not Wave 2)

- Push notification deep-link wiring (mobile)
- Per-tenant admin distribution lists (V1 uses all Admin role users)
- SMS HTTP gateway production credentials (config-only; console default)

## References

- [notification-mapping.md](./design/notification-mapping.md)
- [requirements-freeze-v1.md](./requirements-freeze-v1.md) §17
