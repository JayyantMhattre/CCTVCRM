# API Reuse Analysis

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-6 — **most important document**: maps every API capability to platform reality.
**Classification:** **REUSE** (existing platform API as-is) · **EXTEND** (existing API/mechanism + CCTV wiring) · **NEW** (CCTV HTTP surface or contract)

Effort scale (indicative, per area): **S** ≤ 1 week · **M** 1–3 weeks · **L** 3–6 weeks (single experienced developer including module docs).

---

## 1. Executive summary

| Category | REUSE | EXTEND | NEW |
|----------|:-----:|:------:|:---:|
| Platform Core APIs | 8 areas | 3 areas | 0 |
| CCTV business APIs | 0 | 2 convenience aggregations | 11 module route groups |
| Cross-cutting integration | 4 patterns | 3 patterns | 1 (PDF generation service) |

**~70% of HTTP traffic** (auth, files, profile, sessions) hits **existing platform endpoints unchanged**. All CCTV domain CRUD and workflows are **NEW** route groups on the existing host — not a new API gateway or duplicate stack.

---

## 2. Platform capabilities — full matrix

### Authentication & identity

| Capability | Class | Platform API / mechanism | CCTV usage | Effort |
|------------|-------|--------------------------|------------|:------:|
| Login (password grant) | **REUSE** | `POST /connect/token` | All portals + mobile | — |
| Registration | **REUSE** | `POST /api/v1/auth/register` | Admin-provisioned customer/engineer accounts | — |
| MFA / OTP flows | **REUSE** | Auth module endpoints | Login OTP, password reset OTP (freeze §17) | — |
| Sessions | **REUSE** | `GET/POST /api/v1/auth/sessions/*` | Profile/security on all portals | — |
| JWT roles + permissions | **EXTEND** | JWT `role` + `permission` claims | Seed `Engineer`, `Customer` roles + 30 CCTV permissions | S |
| Password reset | **REUSE** | Platform Auth flows | Customer self-service | — |
| User profiles | **REUSE** | `GET/PATCH /api/v1/users/{userId}` | Customer profile (BR-AUTH-05) | — |
| Notification preferences | **REUSE** | `PATCH /api/v1/users/{userId}/preferences` | Email toggle | — |

### Files

| Capability | Class | Platform API | CCTV usage | Effort |
|------------|-------|--------------|------------|:------:|
| Upload | **REUSE** | `POST /api/v1/files` | All media, attachments, signatures | — |
| Download | **REUSE** | `GET /api/v1/files/{fileId}` | PDFs, photos, invoices | — |
| Delete | **REUSE** | `DELETE /api/v1/files/{fileId}` | Attachment removal | — |
| URL hint | **REUSE** | `GET /api/v1/files/{fileId}/url` | Mobile preview paths | — |
| Business attachment linking | **NEW** | CCTV `POST .../attachments { fileId }` | After platform upload | included in modules |
| PDF generation | **NEW** | Internal service → Files upload | Contract, visit report, invoice PDFs | M |

### Notifications

| Capability | Class | Platform mechanism | CCTV usage | Effort |
|------------|-------|-------------------|------------|:------:|
| Email dispatch | **REUSE** | `INotificationService` + templates | All §17 events | — |
| Template infrastructure | **EXTEND** | Add `Templates/cctv/*.txt` | 11 CCTV templates | S |
| Event handlers | **EXTEND** | `INotificationHandler<TEvent>` | Subscribe to CCTV contract events | S |
| SMS channel | **EXTEND** | New `ISmsProvider` in CCTV or Notifications | OTP + critical alerts | M |
| Push (mobile) | **REUSE** | Mobile `core/notifications` | Surface §17 events via push | S (wiring) |
| Send notification API | **N/A** | No public send API (by design) | Event-driven only | — |

### Audit

| Capability | Class | Platform mechanism | CCTV usage | Effort |
|------------|-------|-------------------|------------|:------:|
| Audit storage | **REUSE** | MongoDB observer + hash chain | All CCTV domain events | — |
| Domain event capture | **REUSE** | `DomainEventAuditHandler` | Auto when events published | — |
| EF change capture | **REUSE** | `AuditEntityChangeInterceptor` | CCTV DbContexts register interceptor | S |
| API call logging | **REUSE** | `AuditApiCallMiddleware` | Automatic | — |
| Read API | **REUSE** | `GET /api/v1/audit-logs` (`audit:read`) | Admin viewer | — |
| Custom audit calls | **EXTEND** | `IAuditService.LogAsync` | Sensitive operations if needed | S |

### Webhooks

| Capability | Class | Platform API / mechanism | CCTV usage | Effort |
|------------|-------|--------------------------|------------|:------:|
| Subscription management | **REUSE** | `/api/v1/webhooks/subscriptions/*` | Admin configures integrations | — |
| Delivery / DLQ / retry | **REUSE** | `/api/v1/webhooks/deliveries/*`, `/deadletters/*` | Ops center | — |
| Event catalog entries | **EXTEND** | [event-catalog.md](../../modules/webhooks/event-catalog.md) | Add `lead.*`, `ticket.*`, … CCTV events | S |
| Event publishing | **EXTEND** | `IWebhookPublisher` from outbox bridge | Fan-out on CCTV contract events | S |

### API Keys

| Capability | Class | Platform API | CCTV usage | Effort |
|------------|-------|--------------|------------|:------:|
| Key management | **REUSE** | `/api/v1/api-keys/*` | Admin M2M keys | — |
| Scope format | **REUSE** | `resource:action` = RBAC permission | CCTV permissions valid scopes automatically | — |
| Scoped access to CCTV APIs | **EXTEND** | Same middleware | Keys with e.g. `tickets:read` | S |

### Tenant & infrastructure

| Capability | Class | Platform API | CCTV usage | Effort |
|------------|-------|--------------|------------|:------:|
| Tenant context | **REUSE** | JWT `tenant_id` + `ITenantContext` | All CCTV modules | — |
| Rate limiting | **REUSE** | Platform middleware | Anonymous inquiries | — |
| Outbox | **REUSE** | `BaseDbContext` + Quartz processors | Cross-module orchestration | — |
| Health checks | **REUSE** | `/health/*` | Host liveness | — |
| Correlation ID | **REUSE** | Middleware + `X-Correlation-Id` | Support/debug | — |

---

## 3. CCTV business APIs — full matrix

| Domain | Class | Route prefix | Endpoints | Effort |
|--------|-------|--------------|-----------|:------:|
| Public inquiries | **NEW** | `/api/v1/cctv/inquiries` | 1 POST (anonymous) | S |
| Lead Management | **NEW** | `/api/v1/cctv/leads` | ~12 | M |
| Customer Management | **NEW** | `/api/v1/cctv/customers` | ~8 | M |
| Site Management | **NEW** | `/api/v1/cctv/sites` | ~14 | M |
| Asset Management | **NEW** | `/api/v1/cctv/sites/{id}/asset-summary` | 2 (GET/PUT) | S *(same backend slice as Site)* |
| AMC Plans | **NEW** | `/api/v1/cctv/amc-plans` | ~8 | M |
| AMC Contracts | **NEW** | `/api/v1/cctv/contracts` | ~12 | L |
| Service Scheduling | **NEW** | `/api/v1/cctv/schedules` | ~10 | M |
| Visit Management | **NEW** | `/api/v1/cctv/visits` | ~18 | **L** |
| Ticket Management | **NEW** | `/api/v1/cctv/tickets` | ~14 | M |
| Engineer Management | **NEW** | `/api/v1/cctv/engineers` | ~8 | S |
| Invoice Management | **NEW** | `/api/v1/cctv/invoices` | ~12 | M |
| Reporting | **NEW** | `/api/v1/cctv/reports` | ~8 read-only | M |
| Customer portal aggregations | **NEW** | `/api/v1/cctv/portal/*` | ~6 read | S |
| Engineer portal aggregations | **NEW** | `/api/v1/cctv/engineer/*` | ~4 read | S |

**Portals (14, 15)** and **Public Website (1)** have **no separate backend** — they consume platform + CCTV APIs above.

---

## 4. Requirement traceability

| Freeze requirement | API approach | Class |
|--------------------|--------------|-------|
| Website inquiries → auto-lead (§10) | `POST /api/v1/cctv/inquiries` | NEW |
| Lead conversion → Customer+Site+Contract (§10) | `POST .../leads/{id}/convert` orchestrates via contracts + outbox | NEW |
| Max 3 site contacts (§6) | Validated in Site API | NEW |
| One active AMC per site (§6) | Enforced in Contract API | NEW |
| Summary-only assets (§7) | Site asset-summary endpoint | NEW |
| Plan versioning (§9) | AMC Plans API | NEW |
| Master + Terms contracts (§8) | Contracts API | NEW |
| Customer renewal request (§8) | `POST .../contracts/{id}/renewal-request` | NEW |
| Auto schedule generation (§11) | Internal on term activation — no public trigger | NEW |
| Mandatory engineer assignment (§11) | `POST .../schedules/{id}/assign` | NEW |
| Visit evidence checklist (§12) | Visit execute/submit APIs | NEW |
| Admin visit approval (§13) | `POST .../visits/{id}/approve|return` | NEW |
| Customer approved reports only (§13) | `visits:read` + server filter | NEW |
| Ticket lifecycle + reopen (§14) | Tickets API | NEW |
| Invoice Option B (§16) | Invoices API with `invoiceType` | NEW |
| §17 notifications | Event → platform Notifications | EXTEND |
| §18 mobile offline | Engineer visit APIs idempotent + sync | NEW + REUSE mobile sync |
| §19 PDFs | Generate → Files | NEW service + REUSE Files |
| §20 platform reuse | This document | REUSE/EXTEND |

---

## 5. What we explicitly do NOT build

| Avoided duplicate | Use instead |
|--------------------|-------------|
| Custom auth endpoints | `/connect/token`, Auth module |
| CCTV file storage API | `/api/v1/files` |
| CCTV audit write/read API | Platform Audit observer + `/api/v1/audit-logs` |
| CCTV notification send API | Event-driven Notifications module |
| CCTV webhook engine | Platform Webhooks module |
| Mobile HTTP client | OpenAPI-generated `api_client` |
| Theme/layout APIs | Out of scope (D0-6 rules) |

---

## 6. Effort summary

| Workstream | Effort |
|------------|:------:|
| Platform wiring (permissions, templates, webhook catalog, SMS) | S–M |
| PDF generation (3 document types) | M |
| CCTV backend modules (7 schemas + reporting) | **L** (dominant) |
| Portal aggregation endpoints | S |
| OpenAPI + SDK regeneration | S (per release) |
| **Total relative split** | ~12% platform extend · ~88% new CCTV APIs |

---

Related: [api-architecture.md](./api-architecture.md) · [platform-reuse-analysis.md](./platform-reuse-analysis.md) · [endpoint-catalog.md](./endpoint-catalog.md)
