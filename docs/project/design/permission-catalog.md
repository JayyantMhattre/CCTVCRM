# Permission Catalog

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-5 — RBAC, Navigation & Screen Inventory (base-template aware)
**Platform baseline:** Ashraak V1 RBAC/ABAC — JWT `role` + `permission` claims, `resource:action` format ([auth architecture](../../modules/auth/architecture.md), [ADR-ApiKeys-0001](../../adr/ADR-ApiKeys-0001-api-keys-platform.md))

> Rule: **do not duplicate existing platform permissions.** Every permission below is classified REUSE / EXTEND / NEW. CCTV permissions follow the platform's existing `resource:action` convention so they flow through the same JWT `permission` claim, `IAuthPermissionChecker`, `PermissionGuard`, and API-key scope machinery with **zero platform changes**.

---

## 1. Existing platform permissions (inventory — verified in platform docs)

| Permission | Source | Used for |
|------------|--------|----------|
| `users:read` | ApiKeys scopes / Users module | Read user profiles |
| `users:write` | ApiKeys scopes / Users module | Create/update users |
| `files:read` | ApiKeys scopes / Files module | Download files |
| `files:write` | ApiKeys scopes / Files module | Upload files |
| `audit:read` | Audit viewer (web `PermissionGuard`) | View audit logs |
| `webhooks:read` | Webhooks module | View subscriptions/deliveries/DLQ |
| `webhooks:manage` | Webhooks module | Manage subscriptions, retry, replay |
| `apikeys:read` | ApiKeys module | List keys, view usage |
| `apikeys:manage` | ApiKeys module | Create, rotate, revoke, scopes |

**Existing platform roles:** `Admin`, `Manager` (web RoleGuards). Platform also provides MFA, sessions, invitations, password reset — all permission-governed within Auth itself.

## 2. Classification summary

| Class | Meaning | Count |
|-------|---------|-------|
| **REUSE** | Platform permission used as-is by CCTV | 9 |
| **EXTEND** | Platform mechanism reused with new role wiring (no new permission string, no platform code change) | 3 roles |
| **NEW** | CCTV business permissions (new strings, same `resource:action` machinery) | 30 |

## 3. REUSE — platform permissions consumed by CCTV

| Permission | CCTV usage |
|------------|-----------|
| `files:write` | Engineer media upload (photos/videos/selfie/signature), admin document uploads, customer ticket attachments — via platform Files API |
| `files:read` | Downloading documents/PDFs/media (always tenant- and ownership-scoped by the module API first) |
| `users:read` / `users:write` | Admin managing portal accounts for customers/engineers |
| `audit:read` | Admin audit visibility (existing audit viewer) |
| `webhooks:read` / `webhooks:manage` | Admin operating outbound CCTV webhook events |
| `apikeys:read` / `apikeys:manage` | Admin managing M2M keys for integrations |

## 4. EXTEND — roles (mechanism reuse, new wiring)

The platform role system is reused; CCTV adds **role definitions** (data, not code):

| Role | Class | Notes |
|------|-------|-------|
| `Admin` | **REUSE** | Existing platform role; CCTV grants it the full CCTV permission set |
| `Engineer` | **EXTEND** | New role *record* in the existing RBAC store; carries engineer permissions below |
| `Customer` | **EXTEND** | New role *record*; carries customer self-service permissions below |

(Public Visitor is anonymous — no role; website inquiry endpoints are public.)

## 5. NEW — CCTV business permissions

Naming: `<cctv-resource>:<action>` — actions follow platform verbs (`read`, `manage`) plus CCTV-specific actions where the freeze demands them (`approve`, `assign`, `reopen`, `convert`).

### Lead Management
| Permission | Grants | Roles |
|------------|--------|-------|
| `leads:read` | View leads, activities, remarks | Admin |
| `leads:manage` | Create/update leads, log activities, change status | Admin |
| `leads:convert` | Convert Won lead → Customer + Site + Initial Contract (BR-LEAD-03) | Admin |

### Customer / Site / Asset
| Permission | Grants | Roles |
|------------|--------|-------|
| `customers:read` | View customers | Admin |
| `customers:manage` | Create/update/deactivate customers | Admin |
| `sites:read` | View sites, contacts, asset summaries | Admin; Customer (own, scoped) |
| `sites:manage` | Manage sites, contacts (≤3), asset summaries | Admin |

### AMC
| Permission | Grants | Roles |
|------------|--------|-------|
| `amcplans:read` | View plans/versions | Admin |
| `amcplans:manage` | Create plans, publish versions, retire (BR-VISIT-07: never Engineer) | Admin |
| `amc:read` | View contracts/terms (Customer: own + active term only, BR-AMC-03) | Admin; Customer (scoped) |
| `amc:manage` | Create contracts, renew terms, cancel | Admin |
| `amc:request-renewal` | Request renewal of own contract (BR-AMC-08) | Customer |

### Scheduling & Visits
| Permission | Grants | Roles |
|------------|--------|-------|
| `schedules:read` | View visit schedules (Engineer: assigned only; Customer: own sites) | Admin; Engineer, Customer (scoped) |
| `schedules:manage` | Generate ad-hoc, reschedule, cancel (BR-SCHED-03) | Admin |
| `visits:assign` | Assign/reassign engineer (mandatory, BR-SCHED-04) | Admin |
| `visits:read` | View visit reports (Customer: **approved only**, BR-VISIT-05) | Admin; Engineer (own), Customer (scoped) |
| `visits:execute` | Start visit, capture evidence, submit report (BR-VISIT-01) | Engineer |
| `visits:approve` | Approve/return submitted reports (BR-VISIT-04) | Admin |

### Tickets
| Permission | Grants | Roles |
|------------|--------|-------|
| `tickets:read` | View tickets (Customer: own; Engineer: assigned) | Admin; Engineer, Customer (scoped) |
| `tickets:create` | Create tickets (BR-TKT-03..05 — all three actors) | Admin, Engineer, Customer |
| `tickets:assign` | Assign/reassign engineer | Admin |
| `tickets:update` | Progress status, comment on assigned/own tickets | Admin, Engineer (assigned), Customer (own: comments) |
| `tickets:close` | Resolve→Close transition | Admin |
| `tickets:reopen` | Reopen own closed ticket (BR-TKT-06) | Customer |

### Engineers
| Permission | Grants | Roles |
|------------|--------|-------|
| `engineers:read` | View engineer records/workload | Admin |
| `engineers:manage` | Create/update/deactivate engineers | Admin |

### Invoices (Option B)
| Permission | Grants | Roles |
|------------|--------|-------|
| `invoices:read` | View invoices (Customer: own only) | Admin; Customer (scoped) |
| `invoices:manage` | Create/edit Draft, generate, send, mark paid, cancel (BR-INV-01) | Admin |
| `invoices:download` | Download invoice PDF (BR-INV-03) | Admin, Customer (own) |

### Reporting
| Permission | Grants | Roles |
|------------|--------|-------|
| `reports:read` | View reporting dashboards/views | Admin |

## 6. Scoping rules (ABAC layer — reused platform mechanism)

Permission strings authorize the **operation**; **row-level scope** is enforced by module APIs (same pattern the platform uses for tenant scoping):

| Role | Scope rule |
|------|-----------|
| Customer | Only rows where `customer_id` = caller's customer (own contracts, invoices, tickets, sites, visit reports — and visit reports only when approved) |
| Engineer | Only rows where the engineer holds the **active assignment** (schedules, visits, tickets); §15 restrictions: no customer/plan/contract management permissions at all |
| Admin | Tenant-wide |

## 7. API-key scope compatibility

CCTV permission strings are valid API-key scopes automatically (platform scope format = RBAC format). Example: an integration key could carry `tickets:create` + `tickets:read`. No ApiKeys module change required.

---

## Related documents

- [rbac-matrix.md](./rbac-matrix.md) — role × module/screen/action/API matrix
- [platform-reuse-analysis.md](./platform-reuse-analysis.md)
