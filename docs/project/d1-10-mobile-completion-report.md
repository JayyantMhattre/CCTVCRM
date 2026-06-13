# Sprint 1 — D1-10 Completion Report

**Phase:** D1-10 Mobile Applications  
**Date:** 2026-06-11  
**Review gate:** Deferred test execution (Flutter analyze + build verification recommended)

## Summary

Extended the existing Flutter foundation (`FrontEnd.Mobile`) with CCTV Customer and Engineer feature modules. Reuses Dio client, secure storage, offline cache mixin, notifications, profile, and release engineering — no duplicate platform implementations.

## Customer Mobile App

| Screen | Route | API |
|--------|-------|-----|
| Dashboard | `/cctv/customer` | Portal summaries |
| AMC Details | `/cctv/customer/amc` | `GET /cctv/portal/amc` |
| Upcoming Visits | `/cctv/customer/visits` | `GET /cctv/portal/visits/upcoming` |
| Tickets | `/cctv/customer/tickets` | `GET /cctv/portal/tickets` |
| Invoices | `/cctv/customer/invoices` | `GET /cctv/portal/invoices` |
| Notifications | Platform route | Notification preferences page |
| Profile / Password | Platform routes | Profile + auth flows |

Offline: read-through cache on tickets, invoices, AMC, visits lists.

## Engineer Mobile App

| Screen | Route | API |
|--------|-------|-----|
| Dashboard | `/cctv/engineer` | `GET /cctv/engineer/dashboard` |
| Assigned Visits | `/cctv/engineer/visits` | `GET /cctv/engineer/schedules` |
| Visit Report | `/cctv/engineer/visits/:visitId/report` | Start, GPS, remarks, submit |
| Assigned Tickets | `/cctv/engineer/tickets` | `GET /cctv/engineer/tickets` |

Offline: cached schedules, tickets, dashboard KPIs.

## Feature flags

- `cctv.mobile.customer.enabled`
- `cctv.mobile.engineer.enabled`

Home page shows Customer/Engineer app entry cards when JWT role matches.

## Deferred

- Native camera/GPS plugins for selfie and auto-GPS (manual coordinates + platform Files hooks documented)
- Ticket creation form on engineer mobile (API exists; web portal primary)
- Store release binaries and push notification tenant config

## Files

- `FrontEnd.Mobile/lib/features/cctv_customer/`
- `FrontEnd.Mobile/lib/features/cctv_engineer/`
- `FrontEnd.Mobile/lib/features/cctv/` (shared API paths, models, flags)
