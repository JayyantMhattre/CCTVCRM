# Dashboard Design

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-5 — RBAC, Navigation & Screen Inventory (base-template aware)

Dashboards are composed from **platform-ui primitives** (`PlatformCard`, `PlatformTable`, `PlatformBadge`, `PlatformChart` contract) rendered through the existing theme adapters. Widget classification: **REUSE** (existing platform widget) · **EXTEND** (existing widget pattern with CCTV data) · **NEW** (CCTV widget built on platform-ui).

> Note: the platform's `ChartContract` is library-agnostic and no chart library is committed in the platform (known limitation). V1 dashboards therefore favor **count cards, tables, and badges**; chart visuals can be added later through the existing contract without redesign.

---

## 1. Admin Dashboard (`/admin` — EXTEND of the platform dashboard page)

| Widget | Content | Class | Source |
|--------|---------|-------|--------|
| Lead pipeline summary | Counts per status (New → Converted), this-month trend | NEW | `leads:read` |
| AMC expiry radar | Terms expiring in 30/60/90 days (reminder pipeline, §17) | NEW | `amc:read` |
| Visit schedule today/this week | Planned/Assigned/InProgress counts + overdue (Missed) alert | NEW | `schedules:read` |
| Pending report approvals | Submitted visit reports awaiting review (BR-VISIT-04) — action queue | NEW | `visits:approve` |
| Open tickets by priority | Low/Medium/High/Critical with status split | NEW | `tickets:read` |
| Invoice status summary | Draft/Generated/Sent/Paid this month; unpaid total | NEW | `invoices:read` |
| Engineer workload | Active assignments per engineer | NEW | `engineers:read` |
| Recent audit activity card | Existing audit tile (gated `audit:read`) | **REUSE** | platform Audit viewer |
| Webhook/API-key health cards | Existing platform admin cards | **REUSE** | platform Webhooks/ApiKeys dashboards |
| Quick actions | New Lead, New Ticket, New Invoice, Schedule Visit | NEW | respective `*:manage` |

## 2. Customer Dashboard (`/portal` — NEW page on REUSE shell)

| Widget | Content | Class | Source |
|--------|---------|-------|--------|
| AMC status card | Active term, plan name, validity, days to expiry | NEW | `amc:read` (active term only, BR-AMC-03) |
| Renewal call-to-action | Visible near expiry → Request Renewal (BR-AMC-08) | NEW | `amc:request-renewal` |
| Next scheduled visit | Date + status of nearest upcoming visit | NEW | `schedules:read` (own sites) |
| Service history snapshot | Last approved visits (BR-VISIT-05) | NEW | `visits:read` |
| Open tickets card | Own tickets with status badges + raise-ticket action | NEW | `tickets:read/create` |
| Invoices due card | Generated/Sent invoices + PDF download | NEW | `invoices:read/download` |
| Notifications feed | Recent §17 events for this customer | **EXTEND** | platform notification plumbing |
| Profile completeness / security | Existing profile & sessions entry points | **REUSE** | platform Users/Auth |

## 3. Engineer Dashboard (`/engineer` — NEW page on REUSE shell)

| Widget | Content | Class | Source |
|--------|---------|-------|--------|
| My Day | Today's assigned visits with site + time | NEW | `schedules:read` (assigned) |
| Visit completion checklist state | Started visit's evidence progress (selfie/GPS/photo/signature/remarks, BR-VISIT-01) | NEW | `visits:execute` |
| Assigned tickets | Open/InProgress tickets with priority | NEW | `tickets:read` (assigned) |
| Returned reports | Reports returned by admin needing rework | NEW | `visits:read` (own) |
| Offline sync status (mobile) | Pending uploads/submissions queue | NEW (on platform offline/sync core) | mobile foundation (REUSE) |
| Profile/security entry | Existing platform profile | **REUSE** | platform |

## 4. Widget classification summary

| Class | Count | Notes |
|-------|-------|-------|
| REUSE | 4 | Audit tile, webhook/API-key cards, profile/security entries |
| EXTEND | 2 | Admin dashboard page itself; customer notifications feed |
| NEW | 18 | All CCTV business widgets — built exclusively on `platform-ui` primitives |

## 5. Design rules

1. Widgets are **permission-gated** with the existing `PermissionGuard` — a user missing a permission sees no widget (same pattern as the platform's audit tile).
2. All data comes from module read APIs — widgets own no business logic.
3. No chart library is introduced in V1; the `ChartContract` slot remains available for a future change request.
4. Mobile dashboards (Customer App, Engineer App) render the same data through the corresponding Flutter features ([mobile-screen-inventory.md](./mobile-screen-inventory.md)).

Related: [screen-inventory.md](./screen-inventory.md) · [navigation-architecture.md](./navigation-architecture.md)
