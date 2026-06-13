# Screen Design Specification

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7 — Low Level Design
**Source:** [screen-inventory.md](../screen-inventory.md) (71 screens) · [admin/customer/engineer portal navigation](../admin-portal-navigation.md)

Legend — **Class:** REUSE · EXTEND · NEW. **Components:** see [platform-component-reuse.md](./platform-component-reuse.md).

---

## Master screen registry

### Public Website (anonymous)

| # | Screen | Purpose | Role | Navigation | Actions | Dependencies | Class |
|---|--------|---------|------|------------|---------|--------------|:-----:|
| 1 | Home | Business showcase | Visitor | `/` | Navigate, Login | Static content | NEW |
| 2 | About Us | Company info | Visitor | `/about` | Navigate | Content reuse | NEW |
| 3 | Services | Services overview | Visitor | `/services` | Navigate | Content reuse | NEW |
| 4 | AMC Services | Plan marketing info | Visitor | `/amc-services` | Navigate, Get Quote | Static/plan marketing | NEW |
| 5 | Contact Us | Inquiry form | Visitor | `/contact` | Submit inquiry | `POST /inquiries`, rate limit | NEW |
| 6 | Gallery | Portfolio | Visitor | `/gallery` | View images | Content reuse | NEW |
| 7 | Testimonials | Social proof | Visitor | `/testimonials` | View | Content reuse | NEW |
| 8 | Get Quote | Quote → lead | Visitor | `/get-quote` | Submit | `POST /inquiries` | NEW |
| 9 | AMC Inquiry | AMC inquiry → lead | Visitor | `/amc-inquiry` | Submit | `POST /inquiries` | NEW |
| 10 | Login | Authentication | All | `/login` | Login, MFA, OTP | Platform Auth | **REUSE** |

### Admin Portal — business

| # | Screen | Purpose | Role | Navigation | Actions | Dependencies | Class |
|---|--------|---------|------|------------|---------|--------------|:-----:|
| 11 | Admin Dashboard | KPIs + queues | Admin | `/admin` | Drill-down, quick actions | `/admin/dashboard`, platform tiles | **EXTEND** |
| 12 | Lead List | Pipeline queue | Admin | `/admin/leads` | Filter, open, create | `GET /leads` | NEW |
| 13 | Lead Detail | Manage lead | Admin | `/admin/leads/:id` | Status change, activity, remark, attach | `GET/PUT /leads`, Files | NEW |
| 14 | Lead Conversion | Won → entities | Admin | `/admin/leads/:id/convert` | Convert wizard submit | `POST .../convert` | NEW |
| 15 | Customer List | Browse customers | Admin | `/admin/customers` | Search, create, open | `GET /customers` | NEW |
| 16 | Customer Detail | Customer + sites | Admin | `/admin/customers/:id` | Edit, add site | `GET /customers/:id` | NEW |
| 17 | Customer Create/Edit | Maintain customer | Admin | `/admin/customers/new`, `/:id/edit` | Save, link user | `POST/PUT /customers`, Users | NEW |
| 18 | Site Detail | Site hub | Admin | `/admin/sites/:id` | Tabs: contacts, assets, contract, visits, tickets | Aggregated reads | NEW |
| 19 | Site Create/Edit | Maintain site | Admin | `/admin/sites/new`, `/:id/edit` | Save contacts (≤3) | `POST/PUT /sites` | NEW |
| 20 | Asset Summary Edit | Summary counts | Admin | `/admin/sites/:id/assets` | Save counts | `PUT .../asset-summary` | NEW |
| 21 | AMC Plan List | Plan catalog | Admin | `/admin/amc/plans` | Open, create | `GET /amc-plans` | NEW |
| 22 | AMC Plan Detail | Versions | Admin | `/admin/amc/plans/:id` | New version, publish, retire | `GET/POST /amc-plans` | NEW |
| 23 | Contract List | All contracts | Admin | `/admin/amc/contracts` | Filter, open | `GET /contracts` | NEW |
| 24 | Contract Detail | Master + terms + PDFs | Admin | `/admin/amc/contracts/:id` | Renew term, docs | `GET /contracts/:id`, Files | NEW |
| 25 | Term Create/Renew | New term | Admin | `/admin/amc/contracts/:id/renew` | Save, activate | `POST .../terms` | NEW |
| 26 | Schedule Calendar/List | Visit schedules | Admin | `/admin/schedules` | Filter by date/status | `GET /schedules` | NEW |
| 27 | Schedule Detail | Assign/reschedule | Admin | `/admin/schedules/:id` | Assign engineer, reschedule, cancel | `POST .../assign` | NEW |
| 28 | Visit Approval Queue | Pending reports | Admin | `/admin/visits/approvals` | Open review | `GET /visits/approvals` | NEW |
| 29 | Visit Review Detail | Approve/return | Admin | `/admin/visits/:id/review` | Approve, return, view evidence | `POST .../approve|return`, Files | NEW |
| 30 | Ticket List | All tickets | Admin | `/admin/tickets` | Filter, create | `GET /tickets` | NEW |
| 31 | Ticket Detail | Manage ticket | Admin | `/admin/tickets/:id` | Assign, progress, close, comment | Ticket APIs | NEW |
| 32 | Ticket Create | Raise on behalf | Admin | `/admin/tickets/new` | Submit | `POST /tickets` | NEW |
| 33 | Engineer List | Engineers | Admin | `/admin/engineers` | Create, open | `GET /engineers` | NEW |
| 34 | Engineer Detail/Edit | Profile + workload | Admin | `/admin/engineers/:id` | Save, deactivate | `GET/PUT /engineers` | NEW |
| 35 | Invoice List | Invoices | Admin | `/admin/invoices` | Filter, create | `GET /invoices` | NEW |
| 36 | Invoice Create/Edit | Draft + lines | Admin | `/admin/invoices/new`, `/:id/edit` | Save lines | `POST/PUT /invoices` | NEW |
| 37 | Invoice Detail | Lifecycle + PDF | Admin | `/admin/invoices/:id` | Generate, send, paid, cancel | Invoice APIs, Files | NEW |
| 38 | Reports Hub | Reporting views | Admin | `/admin/reports`, `/:area` | Filter, export | `/reports/*` | NEW |

### Admin Portal — Administration (REUSE)

| # | Screen | Purpose | Role | Navigation | Actions | Dependencies | Class |
|---|--------|---------|------|------------|---------|--------------|:-----:|
| 39 | User List / Profile | Portal accounts | Admin | `/users`, `/users/:id` | CRUD users | Platform Users | **REUSE** |
| 40 | Tenant Profile/Settings | Workspace | Admin | `/tenant/*` | Settings | Platform Tenant | **REUSE** |
| 41 | Audit Logs | Compliance | Admin | `/audit` | Filter, paginate | Platform Audit | **REUSE** |
| 42 | API Keys | M2M keys | Admin | api-keys routes | Manage keys | Platform ApiKeys | **REUSE** |
| 43 | Webhooks Center | Integrations | Admin | webhooks routes | Subscriptions, DLQ | Platform Webhooks | **REUSE** |
| 44 | Sessions | Security | Admin | sessions route | Revoke | Platform Auth | **REUSE** |
| 45 | My Profile / Prefs | Self service | Admin | profile routes | Update prefs | Platform Users | **REUSE** |

### Customer Portal

| # | Screen | Purpose | Role | Navigation | Actions | Dependencies | Class |
|---|--------|---------|------|------------|---------|--------------|:-----:|
| 46 | Customer Dashboard | Overview | Customer | `/portal` | Drill-down | `/portal/dashboard` | NEW |
| 47 | AMC Details | Active term | Customer | `/portal/amc` | View plan/SLA | `/portal/amc` | NEW |
| 48 | Request Renewal | Renewal request | Customer | `/portal/amc/renewal` | Submit | `POST .../renewal-request` | NEW |
| 49 | Upcoming Visits | Scheduled visits | Customer | `/portal/visits/upcoming` | View detail | `/portal/visits/upcoming` | NEW |
| 50 | Service History | Approved reports | Customer | `/portal/visits/history` | View PDF | `/portal/visits/history`, Files | NEW |
| 51 | My Tickets | Ticket list | Customer | `/portal/tickets` | Open, create | `GET /portal/tickets` | NEW |
| 52 | Ticket Create | Raise complaint | Customer | `/portal/tickets/new` | Submit + attach | `POST /tickets`, Files | NEW |
| 53 | Ticket Detail | Timeline, reopen | Customer | `/portal/tickets/:id` | Comment, reopen | Ticket APIs | NEW |
| 54 | My Invoices | Invoice list | Customer | `/portal/invoices` | Open | `GET /portal/invoices` | NEW |
| 55 | Invoice Detail | View/download PDF | Customer | `/portal/invoices/:id` | Download | Files | NEW |
| 56 | Profile Management | Own profile | Customer | `/portal/profile` | Save | Platform + `/portal/profile` | **REUSE** |
| 57 | Password Reset | OTP reset | Customer | Auth flow | Reset | Platform Auth | **REUSE** |
| 58 | Notifications | In-app feed | Customer | `/portal/notifications` | Mark read | Push + feed API | **EXTEND** |

### Engineer Portal

| # | Screen | Purpose | Role | Navigation | Actions | Dependencies | Class |
|---|--------|---------|------|------------|---------|--------------|:-----:|
| 59 | Engineer Home | My Day summary | Engineer | `/engineer` | Open visits/tickets | `/engineer/dashboard` | NEW |
| 60 | Assigned Visits | Visit queue | Engineer | `/engineer/visits` | Open visit | `/engineer/schedules` | NEW |
| 61 | Visit Detail | Context + start | Engineer | `/engineer/visits/:id` | Start visit | Visit read APIs | NEW |
| 62 | Visit Reporting | Evidence + submit | Engineer | `/engineer/visits/:id/report` | Capture all evidence, submit | Visit execute APIs, Files | NEW |
| 63–66 | Photo/Video/Selfie/GPS/Signature | Evidence capture | Engineer | Report sub-sections | Capture/upload | Files + visit link APIs | NEW |
| 67 | Assigned Tickets | Ticket queue | Engineer | `/engineer/tickets` | Open | `/engineer/tickets` | NEW |
| 68 | Ticket Detail | Progress ticket | Engineer | `/engineer/tickets/:id` | Update status, comment | Ticket APIs | NEW |
| 69 | Ticket Create (visit) | Fault on site | Engineer | `/engineer/tickets/new` | Submit | `POST /tickets` | NEW |
| 70 | Profile & Sessions | Security | Engineer | `/engineer/profile` | Manage | Platform Users/Auth | **REUSE** |
| 71 | Offline Sync Status | Queue state | Engineer | `/engineer/sync` (mobile-primary) | Retry sync | Sync API | NEW |

---

## Page layout standard (all NEW web screens)

```
PlatformBreadcrumb (admin deep pages)
PlatformCard [title + header actions]
  ├─ Filter bar (lists) — PlatformFormField inline
  ├─ PlatformTable | form body | PlatformTabs
  └─ PlatformPagination (lists)
PlatformDialog / PlatformConfirmDialog (destructive actions)
```

---

## Key screen interaction notes

### Lead Detail (#13)
- **Status dropdown:** only valid transitions per BR-LEAD-01; invalid → toast warning
- **Convert button:** visible only at `Won` + `leads:convert`
- **Tabs:** Details | Activities | Remarks | Attachments

### Site Detail (#18)
- **PlatformTabs:** Overview | Contacts | Assets | Contract | Visits | Tickets | Invoices
- Max 3 contacts enforced in edit form (disable Add at 3)

### Visit Review Detail (#29)
- Split view: evidence gallery (Files download) + checklist validation display + remarks
- **Approve** disabled until server checklist satisfied
- **Return** requires reason (min 10 chars)

### Visit Reporting (#62)
- **CctvEvidenceChecklist** sticky sidebar showing BR-VISIT-01 progress
- Submit disabled until all mandatory items green

### Invoice Draft (#36)
- **CctvInvoiceLineEditor** for line items; totals computed client-side for preview, server authoritative

---

## Mobile screen parity

Web screens with mobile equivalents: see [mobile-screen-design.md](./mobile-screen-design.md). Engineer visit reporting (#62–66) is **mobile-first**; web offers same flow when online.

---

## Permission gating (every screen)

Apply existing guards — no custom auth UI:

| Guard | Usage |
|-------|-------|
| `AuthGuard` | All authenticated routes |
| `RoleGuard` | Portal shell (`Admin`, `Customer`, `Engineer`) |
| `PermissionGuard` | Nav items + action buttons per [rbac-matrix.md](../rbac-matrix.md) |

---

Related: [platform-component-reuse.md](./platform-component-reuse.md) · [form-catalog.md](./form-catalog.md) · [grid-catalog.md](./grid-catalog.md) · [workflow-screen-design.md](./workflow-screen-design.md)
