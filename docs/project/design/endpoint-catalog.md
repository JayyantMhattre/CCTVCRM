# Endpoint Catalog

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-6 — complete V1 HTTP inventory
**Base path:** `/api/v1/cctv` (versioned host group) · Platform endpoints documented in [api-reuse-analysis.md](./api-reuse-analysis.md)

Legend: **R** = REUSE (platform) · **E** = EXTEND · **N** = NEW (CCTV)

---

## Platform endpoints consumed (REUSE — not duplicated)

| Method | Route | Purpose | Roles | Class |
|--------|-------|---------|-------|:-----:|
| POST | `/connect/token` | Login | All | R |
| POST | `/api/v1/auth/register` | Register user | Admin flow | R |
| GET/POST | `/api/v1/auth/sessions/*` | Session management | All authenticated | R |
| POST | `/api/v1/files` | Upload binary | All with `files:write` | R |
| GET | `/api/v1/files/{fileId}` | Download | All with `files:read` + module authz | R |
| DELETE | `/api/v1/files/{fileId}` | Soft delete file | Owner contexts | R |
| GET | `/api/v1/files/{fileId}/url` | Auth download path | All with `files:read` | R |
| GET | `/api/v1/users/{userId}` | Profile read | Self / Admin | R |
| PATCH | `/api/v1/users/{userId}/preferences` | Notification prefs | Self | R |
| GET | `/api/v1/audit-logs` | Audit viewer | Admin (`audit:read`) | R |
| GET/POST | `/api/v1/webhooks/*` | Webhook ops | Admin | R |
| GET/POST | `/api/v1/api-keys/*` | API key mgmt | Admin | R |
| GET | `/api/v1/tenants/current` | Tenant info | Authenticated | R |

---

## 1. Public inquiries (anonymous)

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| POST | `/inquiries` | Submit Contact / Get Quote / AMC Inquiry → auto-lead | Anonymous | — (rate limited) | N |

Body includes `inquiryType`: `Contact` | `GetQuote` | `AmcInquiry`.

---

## 2. Lead Management

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| GET | `/leads` | Paginated pipeline list | Admin | `leads:read` | N |
| GET | `/leads/{leadId}` | Lead detail + summary counts | Admin | `leads:read` | N |
| POST | `/leads` | Manual lead create | Admin | `leads:manage` | N |
| PUT | `/leads/{leadId}` | Update lead fields | Admin | `leads:manage` | N |
| POST | `/leads/{leadId}/status` | Pipeline status transition | Admin | `leads:manage` | N |
| GET | `/leads/{leadId}/activities` | Activity timeline | Admin | `leads:read` | N |
| POST | `/leads/{leadId}/activities` | Log activity | Admin | `leads:manage` | N |
| GET | `/leads/{leadId}/remarks` | Remarks list | Admin | `leads:read` | N |
| POST | `/leads/{leadId}/remarks` | Add remark | Admin | `leads:manage` | N |
| GET | `/leads/{leadId}/attachments` | Attachment metadata | Admin | `leads:read` | N |
| POST | `/leads/{leadId}/attachments` | Link platform FileId | Admin | `leads:manage` + `files:write` | N |
| DELETE | `/leads/{leadId}/attachments/{attachmentId}` | Remove attachment link | Admin | `leads:manage` | N |
| POST | `/leads/{leadId}/convert` | Won → Customer+Site+Contract | Admin | `leads:convert` | N |

---

## 3. Customer Management

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| GET | `/customers` | Paginated customer list | Admin | `customers:read` | N |
| GET | `/customers/{customerId}` | Customer detail | Admin | `customers:read` | N |
| POST | `/customers` | Create customer (+ optional user link) | Admin | `customers:manage` | N |
| PUT | `/customers/{customerId}` | Update customer | Admin | `customers:manage` | N |
| PATCH | `/customers/{customerId}/status` | Activate/deactivate | Admin | `customers:manage` | N |
| GET | `/customers/{customerId}/sites` | Sites for customer | Admin | `sites:read` | N |
| GET | `/portal/profile` | Own customer profile | Customer | (scoped) | N |
| PATCH | `/portal/profile` | Update own profile fields | Customer | (scoped, BR-AUTH-05) | N |

---

## 4. Site Management

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| GET | `/sites` | Paginated site list (admin) | Admin | `sites:read` | N |
| GET | `/sites/{siteId}` | Site detail + aggregation summary | Admin | `sites:read` | N |
| POST | `/sites` | Create site | Admin | `sites:manage` | N |
| PUT | `/sites/{siteId}` | Update site | Admin | `sites:manage` | N |
| PATCH | `/sites/{siteId}/status` | Activate/deactivate site | Admin | `sites:manage` | N |
| GET | `/sites/{siteId}/contacts` | List contacts (≤3) | Admin | `sites:read` | N |
| PUT | `/sites/{siteId}/contacts` | Replace contact set (max 3) | Admin | `sites:manage` | N |
| GET | `/sites/{siteId}/documents` | Site documents | Admin | `sites:read` | N |
| POST | `/sites/{siteId}/documents` | Link document FileId | Admin | `sites:manage` | N |
| DELETE | `/sites/{siteId}/documents/{documentId}` | Remove document link | Admin | `sites:manage` | N |
| GET | `/portal/sites` | Own sites list | Customer | `sites:read` (scoped) | N |
| GET | `/portal/sites/{siteId}` | Own site detail | Customer | `sites:read` (scoped) | N |

---

## 5. Asset Management

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| GET | `/sites/{siteId}/asset-summary` | Summary counts + brand/model | Admin; Engineer (visit ctx) | `sites:read` | N |
| PUT | `/sites/{siteId}/asset-summary` | Update summary counts | Admin | `sites:manage` | N |

---

## 6. AMC Plans

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| GET | `/amc-plans` | Plan catalog | Admin | `amcplans:read` | N |
| GET | `/amc-plans/{planId}` | Plan detail + versions | Admin | `amcplans:read` | N |
| POST | `/amc-plans` | Create plan | Admin | `amcplans:manage` | N |
| PUT | `/amc-plans/{planId}` | Update plan identity | Admin | `amcplans:manage` | N |
| POST | `/amc-plans/{planId}/versions` | Create new version (Draft) | Admin | `amcplans:manage` | N |
| GET | `/amc-plans/{planId}/versions/{versionId}` | Version detail | Admin | `amcplans:read` | N |
| POST | `/amc-plans/{planId}/versions/{versionId}/publish` | Publish version | Admin | `amcplans:manage` | N |
| PATCH | `/amc-plans/{planId}/status` | Retire plan | Admin | `amcplans:manage` | N |

---

## 7. AMC Contracts

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| GET | `/contracts` | Contract list | Admin | `amc:read` | N |
| GET | `/contracts/{contractId}` | Master + full term history | Admin | `amc:read` | N |
| POST | `/contracts` | Create contract for site | Admin | `amc:manage` | N |
| POST | `/contracts/{contractId}/terms` | Add term (new/renewal) | Admin | `amc:manage` | N |
| POST | `/contracts/{contractId}/terms/{termId}/activate` | Activate draft term | Admin | `amc:manage` | N |
| PATCH | `/contracts/{contractId}/status` | Cancel contract | Admin | `amc:manage` | N |
| GET | `/contracts/{contractId}/documents` | Contract PDFs | Admin | `amc:read` | N |
| POST | `/contracts/{contractId}/documents` | Link/generate contract PDF | Admin | `amc:manage` | N |
| GET | `/renewal-requests` | Customer renewal queue | Admin | `amc:manage` | N |
| POST | `/contracts/{contractId}/renewal-request` | Customer renewal request | Customer | `amc:request-renewal` | N |
| GET | `/portal/amc` | Active term + plan inclusions | Customer | `amc:read` (active only) | N |
| GET | `/portal/amc/documents` | Own contract PDFs | Customer | `amc:read` + `files:read` | N |

---

## 8. Service Scheduling

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| GET | `/schedules` | Calendar/list (filters: date, status, engineer) | Admin | `schedules:read` | N |
| GET | `/schedules/{scheduleId}` | Schedule detail + assignment | Admin | `schedules:read` | N |
| POST | `/schedules` | Ad-hoc schedule within active term | Admin | `schedules:manage` | N |
| POST | `/schedules/{scheduleId}/assign` | Assign/reassign engineer | Admin | `visits:assign` | N |
| POST | `/schedules/{scheduleId}/reschedule` | Reschedule with reason | Admin | `schedules:manage` | N |
| POST | `/schedules/{scheduleId}/cancel` | Cancel schedule | Admin | `schedules:manage` | N |
| GET | `/portal/visits/upcoming` | Upcoming visits (own sites) | Customer | `schedules:read` | N |
| GET | `/engineer/schedules` | Assigned schedules | Engineer | `schedules:read` (assigned) | N |
| GET | `/engineer/schedules/today` | Today's assigned work | Engineer | `schedules:read` | N |

---

## 9. Visit Management

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| GET | `/visits` | Visit list (admin filters) | Admin | `visits:read` | N |
| GET | `/visits/{visitId}` | Visit detail + evidence | Admin; Engineer (own) | `visits:read` | N |
| POST | `/visits/{visitId}/start` | Start visit (schedule → InProgress) | Engineer | `visits:execute` | N |
| PUT | `/visits/{visitId}/remarks` | Update visit remarks | Engineer | `visits:execute` | N |
| POST | `/visits/{visitId}/photos` | Link photo FileId + category | Engineer | `visits:execute` + `files:write` | N |
| POST | `/visits/{visitId}/selfie` | Link selfie FileId | Engineer | `visits:execute` | N |
| POST | `/visits/{visitId}/location` | Capture GPS lat/long/timestamp | Engineer | `visits:execute` | N |
| POST | `/visits/{visitId}/signature` | Link signature FileId + signer | Engineer | `visits:execute` | N |
| POST | `/visits/{visitId}/attachments` | Link video/report FileId | Engineer | `visits:execute` | N |
| POST | `/visits/{visitId}/submit` | Submit report for approval | Engineer | `visits:execute` | N |
| GET | `/visits/approvals` | Pending approval queue | Admin | `visits:approve` | N |
| POST | `/visits/{visitId}/approve` | Approve report | Admin | `visits:approve` | N |
| POST | `/visits/{visitId}/return` | Return for rework | Admin | `visits:approve` | N |
| GET | `/portal/visits/history` | Approved service history | Customer | `visits:read` (approved) | N |
| GET | `/portal/visits/{visitId}` | Approved report detail | Customer | `visits:read` | N |
| GET | `/engineer/visits/{visitId}` | Assigned visit detail | Engineer | `visits:read` | N |
| POST | `/engineer/visits/sync` | Batch offline sync (idempotent) | Engineer | `visits:execute` | N |

---

## 10. Ticket Management

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| GET | `/tickets` | Ticket list | Admin; scoped for C/E | `tickets:read` | N |
| GET | `/tickets/{ticketId}` | Ticket detail + timeline | Admin; scoped | `tickets:read` | N |
| POST | `/tickets` | Create ticket | Admin, Engineer, Customer | `tickets:create` | N |
| POST | `/tickets/{ticketId}/assign` | Assign engineer | Admin | `tickets:assign` | N |
| PATCH | `/tickets/{ticketId}/status` | Progress status | Admin, Engineer (assigned) | `tickets:update` | N |
| POST | `/tickets/{ticketId}/comments` | Add comment | All participants | `tickets:update` / scoped | N |
| POST | `/tickets/{ticketId}/attachments` | Link attachment FileId | Creator/participants | `tickets:create` + `files:write` | N |
| POST | `/tickets/{ticketId}/close` | Close ticket | Admin | `tickets:close` | N |
| POST | `/tickets/{ticketId}/reopen` | Customer reopen | Customer (own) | `tickets:reopen` | N |
| GET | `/portal/tickets` | Own tickets | Customer | `tickets:read` | N |
| GET | `/engineer/tickets` | Assigned tickets | Engineer | `tickets:read` | N |

---

## 11. Engineer Management

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| GET | `/engineers` | Engineer list | Admin | `engineers:read` | N |
| GET | `/engineers/{engineerId}` | Detail + workload summary | Admin | `engineers:read` | N |
| POST | `/engineers` | Create engineer (+ user link) | Admin | `engineers:manage` | N |
| PUT | `/engineers/{engineerId}` | Update engineer | Admin | `engineers:manage` | N |
| PATCH | `/engineers/{engineerId}/status` | Activate/deactivate | Admin | `engineers:manage` | N |
| GET | `/engineers/{engineerId}/workload` | Active assignments | Admin | `engineers:read` | N |

---

## 12. Invoice Management (Option B)

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| GET | `/invoices` | Invoice list (type/status filters) | Admin | `invoices:read` | N |
| GET | `/invoices/{invoiceId}` | Invoice detail + lines | Admin; Customer (own) | `invoices:read` | N |
| POST | `/invoices` | Create draft | Admin | `invoices:manage` | N |
| PUT | `/invoices/{invoiceId}` | Update draft + lines | Admin | `invoices:manage` | N |
| POST | `/invoices/{invoiceId}/generate` | Generate PDF + status | Admin | `invoices:manage` | N |
| POST | `/invoices/{invoiceId}/send` | Mark sent | Admin | `invoices:manage` | N |
| POST | `/invoices/{invoiceId}/mark-paid` | Manual paid | Admin | `invoices:manage` | N |
| POST | `/invoices/{invoiceId}/cancel` | Cancel invoice | Admin | `invoices:manage` | N |
| GET | `/invoices/{invoiceId}/pdf` | Download invoice PDF (authorizes FileId) | Admin, Customer (own) | `invoices:download` | N |
| GET | `/portal/invoices` | Own invoices | Customer | `invoices:read` | N |

`invoiceType`: `AmcRenewal` | `NewAmc` | `EmergencyService` | `SpareReplacement` | `AdditionalCharges` | `Other`.

---

## 13. Reporting & dashboards

| Method | Route | Purpose | Role | Permission | Class |
|--------|-------|---------|------|------------|:-----:|
| GET | `/admin/dashboard` | Admin KPI widgets | Admin | multiple read perms | N |
| GET | `/portal/dashboard` | Customer dashboard widgets | Customer | scoped reads | N |
| GET | `/engineer/dashboard` | Engineer My Day summary | Engineer | scoped reads | N |
| GET | `/reports/leads` | Lead pipeline report | Admin | `reports:read` | N |
| GET | `/reports/amc` | AMC expiry / renewal report | Admin | `reports:read` | N |
| GET | `/reports/visits` | Visit completion report | Admin | `reports:read` | N |
| GET | `/reports/tickets` | Ticket SLA/priority report | Admin | `reports:read` | N |
| GET | `/reports/invoices` | Invoice aging report | Admin | `reports:read` | N |
| GET | `/reports/engineers` | Engineer workload report | Admin | `reports:read` | N |

---

## Summary

| Class | Endpoint count (approx.) |
|-------|--------------------------|
| **REUSE** (platform) | 13+ platform routes |
| **NEW** (CCTV) | **~115** CCTV routes |
| **EXTEND** | 0 HTTP endpoints — extensions are permissions, templates, webhook catalog only |

Total unique CCTV routes: **~115** across 11 logical modules + portal/engineer convenience paths.

---

Related: [api-architecture.md](./api-architecture.md) · [dto-catalog.md](./dto-catalog.md) · [rbac-matrix.md](./rbac-matrix.md)
