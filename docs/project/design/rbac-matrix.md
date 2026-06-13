# RBAC Matrix

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-5 — RBAC, Navigation & Screen Inventory (base-template aware)
**Mechanism:** 100% platform reuse — Ashraak Auth RBAC/ABAC, JWT `role` + `permission` claims, `AuthGuard`/`RoleGuard`/`PermissionGuard` (web), permission-checked Minimal APIs (backend). **No duplicate authentication is created.**
**Permissions:** defined in [permission-catalog.md](./permission-catalog.md). Public Visitor is anonymous (no role) and is therefore listed only where relevant.

---

## 1. Role overview

| Role | Class | Description | Freeze ref |
|------|-------|-------------|-----------|
| `Admin` | REUSE (platform role) | Manages all modules; approves visit reports | §3 |
| `Engineer` | EXTEND (new role record, platform mechanism) | Assigned work execution; evidence capture; ticket creation | §3, §15 |
| `Customer` | EXTEND (new role record) | Self-service over own data | §3 |
| *(anonymous)* | — | Public website browsing + inquiry/quote submission | §3 |

## 2. Module access matrix

| Module | Admin | Engineer | Customer |
|--------|:-----:|:--------:|:--------:|
| Lead Management | ✅ Full | ❌ | ❌ |
| Customer Management | ✅ Full | ❌ (§15) | Own profile only |
| Site Management | ✅ Full | Read (visit context) | Own sites (read) |
| Asset Management | ✅ Full | Read (visit context) | Own sites (read) |
| AMC Plans | ✅ Full | ❌ (§15) | Read (plan info of own contract) |
| AMC Contracts | ✅ Full | ❌ (§15) | Own, **active term only** (BR-AMC-03) + renewal request |
| Service Scheduling | ✅ Full | Assigned only | Own sites (upcoming visits, read) |
| Visit Management | ✅ Review/approve | Execute assigned | Own, **approved reports only** (BR-VISIT-05) |
| Ticket Management | ✅ Full | Assigned + create | Own + create + reopen |
| Engineer Management | ✅ Full | Own profile | ❌ |
| Invoice Management | ✅ Full | ❌ | Own (read + PDF download) |
| Reporting | ✅ Full | ❌ | ❌ |
| Platform: Audit viewer | ✅ (`audit:read`) | ❌ | ❌ |
| Platform: Webhooks / API Keys admin | ✅ | ❌ | ❌ |
| Platform: Profile / sessions / MFA / password reset | ✅ | ✅ | ✅ |

## 3. Screen access matrix (by portal)

| Portal / screen group | Admin | Engineer | Customer |
|------------------------|:-----:|:--------:|:--------:|
| Public website (all pages) | ✅ (anonymous too) | ✅ | ✅ |
| Admin Portal — all screens ([admin-portal-navigation.md](./admin-portal-navigation.md)) | ✅ | ❌ | ❌ |
| Engineer Portal — visits, tickets, reporting ([engineer-portal-navigation.md](./engineer-portal-navigation.md)) | ❌* | ✅ | ❌ |
| Customer Portal — dashboard, AMC, history, tickets, invoices, profile ([customer-portal-navigation.md](./customer-portal-navigation.md)) | ❌* | ❌ | ✅ |

\* Admin administers these domains from the Admin Portal; portal shells are role-routed at login (platform RoleGuard).

## 4. Action permission matrix

Legend: ✅ allowed · 🔒 allowed with scope (own/assigned rows only) · ❌ denied

| Action | Permission | Admin | Engineer | Customer |
|--------|-----------|:-----:|:--------:|:--------:|
| Browse website, submit inquiry/quote | *(anonymous)* | ✅ | ✅ | ✅ |
| Manage lead pipeline | `leads:manage` | ✅ | ❌ | ❌ |
| Convert lead | `leads:convert` | ✅ | ❌ | ❌ |
| Manage customers/sites/assets | `customers:manage`, `sites:manage` | ✅ | ❌ | ❌ |
| Update own profile / password reset | platform Auth | ✅ | ✅ | ✅ (BR-AUTH-05) |
| Manage AMC plans (versioned) | `amcplans:manage` | ✅ | ❌ (§15) | ❌ |
| Create/renew/cancel contracts & terms | `amc:manage` | ✅ | ❌ (§15) | ❌ |
| View contract | `amc:read` | ✅ full history (BR-AMC-04) | ❌ | 🔒 active term only (BR-AMC-03) |
| Request AMC renewal | `amc:request-renewal` | — | ❌ | 🔒 own (BR-AMC-08) |
| Reschedule/cancel schedules | `schedules:manage` | ✅ | ❌ | ❌ |
| Assign engineer (visit) | `visits:assign` | ✅ (mandatory, BR-SCHED-04) | ❌ | ❌ |
| Start visit / capture evidence / submit report | `visits:execute` | ❌ | 🔒 assigned | ❌ |
| Approve / return visit report | `visits:approve` | ✅ (BR-VISIT-04) | ❌ | ❌ |
| View visit report | `visits:read` | ✅ all | 🔒 own | 🔒 approved only (BR-VISIT-05) |
| Create ticket | `tickets:create` | ✅ | ✅ (during visit, BR-TKT-05) | ✅ (BR-TKT-03) |
| Assign ticket | `tickets:assign` | ✅ | ❌ | ❌ |
| Progress ticket (In Progress/Resolved) | `tickets:update` | ✅ | 🔒 assigned | 🔒 own (comments only) |
| Close ticket | `tickets:close` | ✅ | ❌ | ❌ |
| Reopen closed ticket | `tickets:reopen` | ❌ | ❌ | 🔒 own (BR-TKT-06) |
| Manage engineers | `engineers:manage` | ✅ | ❌ | ❌ |
| Manage invoices (lifecycle) | `invoices:manage` | ✅ | ❌ | ❌ |
| View / download invoice PDF | `invoices:read`, `invoices:download` | ✅ | ❌ | 🔒 own (BR-INV-03) |
| View reports | `reports:read` | ✅ | ❌ | ❌ |

## 5. API permission matrix (route-group level; detailed contracts in D0-6)

| API group (CCTV business APIs) | Permission required | Scoping |
|--------------------------------|---------------------|---------|
| `POST /api/v1/cctv/inquiries` (contact/quote/AMC inquiry) | **Anonymous** + rate-limited (platform rate limiting) | — |
| `/api/v1/cctv/leads/*` | `leads:read` / `leads:manage` / `leads:convert` | tenant |
| `/api/v1/cctv/customers/*`, `/sites/*` | `customers:*`, `sites:*` | tenant; Customer reads own via portal endpoints |
| `/api/v1/cctv/amc-plans/*` | `amcplans:read` / `amcplans:manage` | tenant |
| `/api/v1/cctv/contracts/*` | `amc:read` / `amc:manage` / `amc:request-renewal` | Customer → own + active term |
| `/api/v1/cctv/schedules/*` | `schedules:read` / `schedules:manage` / `visits:assign` | Engineer → assigned; Customer → own sites |
| `/api/v1/cctv/visits/*` | `visits:read` / `visits:execute` / `visits:approve` | Engineer → assigned; Customer → approved + own |
| `/api/v1/cctv/tickets/*` | `tickets:*` | Customer → own; Engineer → assigned |
| `/api/v1/cctv/engineers/*` | `engineers:read` / `engineers:manage` | tenant |
| `/api/v1/cctv/invoices/*` | `invoices:read` / `invoices:manage` / `invoices:download` | Customer → own |
| `/api/v1/cctv/reports/*` | `reports:read` | tenant |
| Platform APIs (auth, files, audit, webhooks, api-keys) | **Existing platform permissions — REUSE** | platform-enforced |

## 6. File permissions (platform Files module — REUSE)

All binary access goes through platform Files (`files:read` / `files:write`) **plus** CCTV module-level ownership checks before a FileId is ever disclosed:

| File class | Write (who) | Read (who) |
|------------|-------------|-----------|
| Lead attachments | Admin | Admin |
| Site documents | Admin | Admin |
| Contract PDFs / signed copies | System/Admin | Admin; Customer (own contract) |
| Visit photos / selfie / signature / videos | Engineer (assigned visit) | Admin; Engineer (own); Customer (after approval, own site) |
| Visit Report PDF | System | Admin; Customer (approved, own) |
| Ticket attachments | Creator (Admin/Engineer/Customer on own/assigned ticket) | Ticket participants per ticket scope |
| Invoice PDFs | System/Admin | Admin; Customer (own, BR-INV-03) |

Platform guarantees reused: tenant scoping (cross-tenant = 404), authenticated-only URLs, upload validation, virus-scan hook, file events → audit.

## 7. Audit visibility (platform Audit module — REUSE)

| Role | Visibility |
|------|-----------|
| Admin | Platform audit viewer (`audit:read`) — full tenant trail incl. CCTV domain events ([database-architecture.md §6](./database-architecture.md)) |
| Engineer | ❌ no audit access; sees own business histories only (e.g. own visit approval outcomes) |
| Customer | ❌ no audit access; sees own business histories rendered in-portal (ticket timeline, invoice status history, visit reports post-approval) |

## 8. Notification visibility (platform Notifications + SMS — REUSE/EXTEND)

Events per freeze §17; recipients by role:

| Event | Admin | Engineer | Customer |
|-------|:-----:|:--------:|:--------:|
| Lead Created | ✅ | ❌ | ❌ |
| Lead Converted | ✅ | ❌ | ✅ (welcome as new customer) |
| Ticket Created | ✅ | ❌ | ✅ (own, confirmation) |
| Ticket Assigned | ✅ | ✅ (assignee) | ✅ (own) |
| Ticket Closed | ✅ | ❌ | ✅ (own) |
| Visit Scheduled | ✅ | ✅ (assignee) | ✅ (own site) |
| Visit Completed | ✅ | ❌ | ✅ (own site) |
| AMC Expiry Reminder | ✅ | ❌ | ✅ (own contract) |
| Invoice Generated | ✅ | ❌ | ✅ (own) |
| Password Reset OTP | — | recipient only | recipient only |
| Login OTP | — | recipient only | recipient only |

Channel: Email (platform Notifications provider — REUSE) + SMS (new provider integration — EXTEND; see [platform-reuse-analysis.md](./platform-reuse-analysis.md)). Notification preferences UI: platform user-preferences — REUSE.

---

## Enforcement summary (all platform mechanisms)

| Layer | Mechanism | Class |
|-------|-----------|-------|
| Login/JWT/MFA/sessions | Platform Auth (OpenIddict) | REUSE |
| Role gates (portal shells) | `RoleGuard` (web), role claim (API) | REUSE |
| Permission gates (nav, buttons, endpoints) | `PermissionGuard` + `IAuthPermissionChecker` (Redis-cached) | REUSE |
| Row scoping (own/assigned) | CCTV module query filters (same pattern as platform tenant scoping) | NEW (business logic) |
| API keys for integrations | Platform ApiKeys scopes | REUSE |

---

Related: [permission-catalog.md](./permission-catalog.md) · [navigation-architecture.md](./navigation-architecture.md) · [screen-inventory.md](./screen-inventory.md)
