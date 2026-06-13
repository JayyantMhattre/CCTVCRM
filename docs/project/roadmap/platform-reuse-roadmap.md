# Platform Reuse Roadmap

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-8
**Purpose:** For every freeze requirement — what is **already available**, **reuse as-is**, **extend**, or **new development**. **No platform module implementation is scheduled.**

Classification: **AVAILABLE** (shipped in Platform V1) · **REUSE** (consume without code changes) · **EXTEND** (configuration/wiring only) · **NEW** (CCTV business module work)

---

## 1. Summary scoreboard

| Category | AVAILABLE | REUSE | EXTEND | NEW |
|----------|:---------:|:-----:|:------:|:---:|
| Identity & access | ✅ | ✅ | Roles/permissions seed | — |
| Files & media | ✅ | ✅ | — | Link APIs + PDF render |
| Notifications | ✅ | ✅ Email/push infra | SMS + CCTV templates | Event handlers |
| Audit | ✅ | ✅ | DbContext registration | Business history entities |
| Webhooks / API keys | ✅ | ✅ | Event catalog entries | Publishers |
| Web shell / theme | ✅ | ✅ | Nav config + dashboard shell | CCTV pages |
| Mobile foundation | ✅ | ✅ | Deep links + push wiring | CCTV feature slices |
| CI/CD & release | ✅ | ✅ | CCTV module in pipelines | — |
| Business domain | — | — | — | **All CCTV modules** |

---

## 2. Requirement-by-requirement matrix

### Applications (freeze §2)

| Requirement | AVAILABLE | Action | Scheduled in |
|-------------|:---------:|--------|--------------|
| Public website (content + inquiry forms) | Theme/auth shell | **NEW** pages + inquiry API | Sprint 1 (forms), B1 |
| Customer Portal web | Platform shell, auth, profile | **NEW** CCTV pages on platform-ui | Sprint 7 |
| Engineer Portal web | Platform shell, auth, files | **NEW** visit reporting UI | Sprint 8 |
| Admin Portal (platform admin) | Users, audit, webhooks, apikeys | **REUSE** Administration group | — |
| Admin Portal (CCTV business) | Shell only | **NEW** 38 screens | Sprints 1–6, 10 |
| Customer Mobile App | Flutter foundation | **NEW** feature slices | Sprint 9 |
| Engineer Mobile App | Flutter foundation + offline | **NEW** feature slices | Sprint 9 |

### Actors & security (freeze §3, §20)

| Requirement | Action | Sprint |
|-------------|--------|--------|
| Login / JWT / MFA | **REUSE** Auth | — |
| Admin / Engineer / Customer roles | **EXTEND** seed data | D1 |
| 30 CCTV permissions | **EXTEND** seed data | D1 |
| Row-level scoping | **NEW** query filters in modules | B2+ |
| Password reset OTP | **REUSE** Auth + **EXTEND** SMS | D1 integration |
| Login OTP | **REUSE** Auth + **EXTEND** SMS | D1 |

### Lead (freeze §10)

| Requirement | Action | Phase |
|-------------|--------|-------|
| Pipeline statuses | **NEW** Lead module | B1 |
| Auto-create from website | **NEW** inquiry API | B1 |
| Convert → Customer+Site+Contract | **NEW** orchestration | B1 (+ contracts in B3 for full path) |

### Customer / Site / Asset (freeze §5–§7)

| Requirement | Action | Phase |
|-------------|--------|-------|
| Customer master | **NEW** | B2 |
| Site 1:N, max 3 contacts | **NEW** | B2 |
| Asset summary counts | **NEW** | B2 |
| Portal profile self-service | **REUSE** Users + **NEW** scoped API | B2 / Sprint 7 |

### AMC (freeze §8–§9)

| Requirement | Action | Phase |
|-------------|--------|-------|
| Plan catalog + versioning | **NEW** | B3 |
| Master + Terms contracts | **NEW** | B3 |
| One active contract per site | **NEW** domain rule | B3 |
| Customer renewal request | **NEW** | B3 |
| Expiry reminders | **NEW** job + **EXTEND** notifications | B3 |
| Contract PDF | **NEW** PDF service + **REUSE** Files | B3/B6 |

### Scheduling & visits (freeze §11–§13)

| Requirement | Action | Phase |
|-------------|--------|-------|
| Auto visit generation | **NEW** | B4 |
| Mandatory engineer assignment | **NEW** | B4 |
| Visit evidence checklist | **NEW** | B4 |
| Admin approval workflow | **NEW** | B4 |
| Visit report PDF | **NEW** PDF + **REUSE** Files | B4/B6 |
| Customer approved-only view | **NEW** API filter | B4 / Sprint 7 |

### Tickets (freeze §14)

| Requirement | Action | Phase |
|-------------|--------|-------|
| Full lifecycle + reopen | **NEW** | B5 |
| Tri-actor creation | **NEW** | B5 |
| Attachments | **REUSE** Files + **NEW** link | B5 |

### Engineer (freeze §15)

| Requirement | Action | Phase |
|-------------|--------|-------|
| Engineer master | **NEW** | B5 |
| Assigned work queues | **NEW** read APIs | B4/B5 |
| §15 restrictions | **NEW** permission enforcement | B5 |

### Invoices (freeze §16, Option B)

| Requirement | Action | Phase |
|-------------|--------|-------|
| Invoice lifecycle | **NEW** | B6 |
| Option B types | **NEW** | B6 |
| Invoice PDF | **NEW** PDF + **REUSE** Files | B6 |
| No payment gateway | **N/A** out of scope | — |

### Notifications (freeze §17)

| Event | Action |
|-------|--------|
| All 11 events | **EXTEND** templates + handlers (**REUSE** dispatch) |
| Email | **REUSE** |
| SMS | **EXTEND** provider |
| Push | **REUSE** mobile infra + **EXTEND** wiring |

### Reporting (freeze §2)

| Requirement | Action | Phase |
|-------------|--------|-------|
| Admin reports | **NEW** read module | B7 / Sprint 10 |
| Dashboards | **NEW** widgets on **EXTEND** dashboard page | FP / Sprint 10 |

### Platform mandates (freeze §20)

| Mandate | Compliance |
|---------|------------|
| Reuse Auth | ✅ No duplicate auth scheduled |
| Reuse Files | ✅ FileUpload component + Files API only |
| Reuse Notifications | ✅ Event-driven only |
| Reuse Audit | ✅ Observer only |
| Reuse Theme Engine | ✅ platform-ui only |
| Reuse Mobile foundation | ✅ Feature slices only |
| Business modules only | ✅ All work in CctvCrm.* |

---

## 3. What we explicitly do NOT build

| Avoided | Use instead |
|---------|-------------|
| Custom identity server | OpenIddict `/connect/token` |
| Blob storage module | Platform Files |
| Email SMTP stack | Platform Notifications |
| Audit MongoDB pipeline | Platform Audit |
| Webhook delivery engine | Platform Webhooks |
| React component library | platform-ui |
| Mobile HTTP client | OpenAPI SDK |
| New CI pipelines | Extend existing 5 workflows |

---

## 4. Extension work package (D1 — not full modules)

| Extension | Effort | Owner |
|-----------|:------:|-------|
| CCTV permission + role seeds | S | Backend |
| Portal route trees + nav config | S | Frontend |
| SMS provider adapter | M | Backend |
| CCTV email templates (11) | S | Backend |
| Webhook catalog entries | S | Backend |
| OpenAPI + SDK regen pipeline | S | DevOps |
| PDF service abstraction | M | Backend |
| Push deep-link routes (mobile) | S | Mobile |

Total platform **extension** effort: ~2–3 person-weeks (fits D1 / parallel to B1).

---

## 5. New development effort (CCTV only)

| Area | Indicative effort |
|------|-------------------|
| 7 PostgreSQL schemas + modules | ~60% of total |
| ~115 REST endpoints | included above |
| Admin UI (38 screens) | ~20% |
| Customer + Engineer portals | ~10% |
| Mobile (34 screens) | ~10% |
| Reports + hardening | ~5% |

Aligns with [platform-reuse-analysis.md](../design/platform-reuse-analysis.md) (~85% new business / ~15% wiring).

---

Related: [implementation-roadmap.md](./implementation-roadmap.md) · [backend-development-phases.md](./backend-development-phases.md)
