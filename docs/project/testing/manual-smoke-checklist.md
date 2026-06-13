# Manual Smoke Checklist — TP-1

**Project:** Aarvii CCTV AMC Management System  
**Date:** 2026-06-12  
**Phase:** TP-3 execution (checklist prepared in TP-1)  
**Environment:** Staging preferred · [test-data-strategy.md](./test-data-strategy.md)

**Instructions:** Execute in TP-3 only. Log defects per [defect-management-process.md](./defect-management-process.md). Mark Pass / Fail / Blocked / N/A.

---

## Pre-flight

| # | Step | Pass |
|---|------|:----:|
| P-1 | API health endpoint returns 200 | ☐ |
| P-2 | Web SPA loads login page | ☐ |
| P-3 | Test users exist: admin, engineer, customer | ☐ |
| P-4 | RBAC seed applied (CCTV permissions visible) | ☐ |
| P-5 | Primary data chain exists OR create during smoke (Lead → Invoice) | ☐ |

---

## 1. Lead → Customer

**Portal:** Admin web · **Persona:** Admin / Sales

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 1.1 | Navigate to Leads list | List loads, pagination if > page size | ☐ |
| 1.2 | Create new lead (synthetic data) | Lead saved, status New | ☐ |
| 1.3 | Open lead detail | All fields display | ☐ |
| 1.4 | Progress lead to Qualified | Status updates | ☐ |
| 1.5 | Convert lead to Customer | Customer created; link from lead | ☐ |
| 1.6 | Verify customer record | Name, contact, GST fields present | ☐ |
| 1.7 | Attempt duplicate conversion | Blocked or warned per BR | ☐ |

---

## 2. Customer → Site

**Portal:** Admin web · **Persona:** Admin

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 2.1 | Open customer detail (from 1.5) | Customer page loads | ☐ |
| 2.2 | Add site with address + contacts | Site saved | ☐ |
| 2.3 | Add second contact to site | Contact list shows both | ☐ |
| 2.4 | Edit site details | Changes persist | ☐ |
| 2.5 | List sites for customer | Both sites visible if created | ☐ |

---

## 3. Site → AMC

**Portal:** Admin web · **Persona:** Admin

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 3.1 | Select AMC plan from catalog | Plans list loads | ☐ |
| 3.2 | Create AMC contract for site | Contract in Draft or Active per flow | ☐ |
| 3.3 | Set contract terms (dates) | Terms saved | ☐ |
| 3.4 | Activate contract | Status Active | ☐ |
| 3.5 | Attempt second active AMC on same site | Rejected per one-AMC rule | ☐ |
| 3.6 | View AMC on site detail | Contract linked | ☐ |

---

## 4. AMC → Visit

**Portal:** Admin web · **Persona:** Admin

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 4.1 | Generate or view visit schedule from AMC | Schedule exists | ☐ |
| 4.2 | Open upcoming visit | Visit detail loads | ☐ |
| 4.3 | Assign engineer to visit | Assignment saved | ☐ |
| 4.4 | Engineer sees visit in list | Engineer portal/mobile | ☐ |

---

## 5. Visit → Approval

**Portals:** Engineer web/mobile + Admin web

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 5.1 | Engineer opens assigned visit | Status Scheduled/In Progress | ☐ |
| 5.2 | Start visit (if separate action) | Status In Progress | ☐ |
| 5.3 | Complete evidence checklist | Mandatory items enforced | ☐ |
| 5.4 | Upload visit photos | Files linked to visit | ☐ |
| 5.5 | Upload/link visit video (≤ 100 MB) | Video linked (Wave 4) | ☐ |
| 5.6 | Submit visit report | Status Pending Approval | ☐ |
| 5.7 | Admin approves visit | Status Approved | ☐ |
| 5.8 | Admin rejects visit (optional second visit) | Returns to engineer with reason | ☐ |

---

## 6. Ticket lifecycle

**Portals:** Customer, Engineer, Admin

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 6.1 | Customer creates ticket from portal | Ticket Open | ☐ |
| 6.2 | Customer uploads attachment | File visible on ticket | ☐ |
| 6.3 | Admin views ticket queue | Ticket listed | ☐ |
| 6.4 | Admin assigns engineer | Status Assigned | ☐ |
| 6.5 | Engineer updates ticket (web/mobile) | Status In Progress | ☐ |
| 6.6 | Engineer adds resolution notes + attachment | Saved | ☐ |
| 6.7 | Engineer resolves ticket | Status Resolved | ☐ |
| 6.8 | Admin/customer closes ticket | Status Closed | ☐ |
| 6.9 | Verify audit/history visible | Timeline or activity log | ☐ |

---

## 7. Invoice lifecycle

**Portals:** Admin + Customer · **Wave 4 admin actions**

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 7.1 | Create invoice (Draft) for customer | Draft saved | ☐ |
| 7.2 | Add/edit line items | Totals correct (decimal) | ☐ |
| 7.3 | Generate invoice | Status Generated; PDF if applicable | ☐ |
| 7.4 | **Send** invoice (admin detail page) | Status Sent | ☐ |
| 7.5 | Customer views sent invoice in portal | Visible, not Draft | ☐ |
| 7.6 | **Mark Paid** (admin) | Status Paid | ☐ |
| 7.7 | Create second invoice → **Cancel** | Status Cancelled | ☐ |
| 7.8 | Customer cannot see cancelled draft incorrectly | Per BR | ☐ |

---

## 8. Customer portal

**Persona:** Customer · **Platform:** Web (+ mobile subset in §11)

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 8.1 | Login (OTP) | Dashboard/home loads | ☐ |
| 8.2 | View sites | Site list matches seed | ☐ |
| 8.3 | View AMC summary | Active contract shown | ☐ |
| 8.4 | View visit history | Approved visits visible | ☐ |
| 8.5 | Create ticket (see §6) | — | ☐ |
| 8.6 | View invoices | Sent/Paid visible | ☐ |
| 8.7 | Profile / logout | Session ends cleanly | ☐ |
| 8.8 | Access admin route as customer | 403 or redirect | ☐ |

---

## 9. Engineer portal

**Persona:** Engineer · **Platform:** Web

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 9.1 | Login | Engineer dashboard loads | ☐ |
| 9.2 | View assigned visits | List matches assignments | ☐ |
| 9.3 | Complete visit flow (see §5) | — | ☐ |
| 9.4 | View/update assigned tickets | — | ☐ |
| 9.5 | Access admin-only routes | Denied | ☐ |

---

## 10. Admin — reports (Wave 4)

**Persona:** Admin · **Permission:** `reports:read`

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 10.1 | Open Reports hub | Hub loads; guarded if no permission | ☐ |
| 10.2 | Run Leads report with filters | Filtered results | ☐ |
| 10.3 | Paginate results | Page 2 loads | ☐ |
| 10.4 | Drill-down link to entity | Navigates to detail | ☐ |
| 10.5 | Export CSV | File downloads | ☐ |
| 10.6 | Repeat for Visits, Tickets, Invoices reports | Same behaviors | ☐ |

---

## 11. Mobile — engineer

**Platform:** Flutter app · **Persona:** Engineer

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 11.1 | Login (OTP) | Home/dashboard | ☐ |
| 11.2 | View visit list | Synced with web assignment | ☐ |
| 11.3 | Open visit; capture evidence | Photos upload | ☐ |
| 11.4 | Link/upload visit video | Video linked | ☐ |
| 11.5 | Submit visit report | Pending approval | ☐ |
| 11.6 | Update ticket from mobile | Status updates | ☐ |
| 11.7 | Offline queue (if applicable) | Sync on reconnect | ☐ |

---

## 12. Mobile — customer

**Platform:** Flutter app · **Persona:** Customer

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 12.1 | Login | Customer home | ☐ |
| 12.2 | View tickets | List loads | ☐ |
| 12.3 | Create ticket | Ticket created | ☐ |
| 12.4 | View invoices | Sent/Paid visible | ☐ |

---

## 13. Mobile — auth (Wave 4)

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 13.1 | Forgot password flow | Email/OTP step completes | ☐ |
| 13.2 | Reset password | New password works | ☐ |
| 13.3 | Login with new password | Success | ☐ |

---

## 14. Public website

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 14.1 | Load public home | Content loads (aarvii.in messaging) | ☐ |
| 14.2 | Navigate to login links | Routes to portal login | ☐ |

---

## 15. Notifications & deep links (best effort)

**Note:** FCM send partially deferred (V1.1). Validate parser + in-app routing where possible.

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 15.1 | Trigger in-app notification (if testable) | Banner/toast appears | ☐ |
| 15.2 | Tap notification with visit deep link | Opens visit screen | ☐ |
| 15.3 | Tap notification with ticket deep link | Opens ticket screen | ☐ |

---

## 16. RBAC negative tests

| # | Step | Expected | Pass |
|---|------|----------|:----:|
| 16.1 | Engineer accesses invoice admin | 403 | ☐ |
| 16.2 | Customer accesses lead admin | 403 | ☐ |
| 16.3 | User without `reports:read` | Reports hub blocked | ☐ |

---

## Sign-off (TP-3)

| Field | Value |
|-------|-------|
| Executed by | |
| Date | |
| Environment | Staging / Local |
| Build / tag | |
| Total Pass | / |
| Total Fail | / |
| Blocked | / |
| QA lead sign-off | ☐ |

**Exit criteria:** Zero **Critical** or **High** open defects for smoke scope, OR explicit waiver documented by PM.

---

*TP-1 — Checklist prepared. Do not execute until TP-3.*
