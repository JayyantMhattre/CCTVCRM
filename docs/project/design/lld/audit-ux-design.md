# Audit UX Design

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7
**Mandate:** REUSE platform Audit module — no duplicate audit UI or storage ([audit-mapping.md](../audit-mapping.md))

---

## 1. Admin audit screens (REUSE)

| Screen | Route | Class | Components |
|--------|-------|:-----:|------------|
| Audit Logs | `/audit` (#41) | **REUSE** | Platform audit viewer module |

### Existing viewer capabilities (REUSE)

| Feature | UI control | API param |
|---------|------------|-----------|
| Module filter | Text input | `module` (e.g. `CctvCrm.Lead`) |
| Date range | From/To date | `from`, `to` ISO UTC |
| Pagination | Prev/Next | `page`, `pageSize` (default 50) |
| Search | Client-side on stub | action/module/userId when data available |

**Permission:** `audit:read` + Admin role gate · **Note:** Platform GET endpoint is stub — viewer shows info banner until MongoDB query wired; CCTV business histories compensate for operational UX.

Reference: [audit-viewer filters](../../../frontend/audit-viewer/filters.md)

---

## 2. Entity history (business-visible — NEW CCTV UI)

Platform audit ≠ user-facing timelines. These are **first-class CCTV screens**:

| Entity | UI location | Component | Data source |
|--------|-------------|-----------|-------------|
| Ticket | Ticket Detail #31/#53/#68 tab "History" | **CctvTicketTimeline** | `ticket_status_histories` + comments |
| Invoice | Invoice Detail #37 tab "History" | Status list | `invoice_status_histories` |
| Visit approval | Visit Review #29 sidebar | Approval rounds list | `visit_approvals` |
| Lead | Lead Detail #13 tab "Activities" | Activity feed | `lead_activities` |
| Contract terms | Contract Detail #24 | **CctvTermHistoryTable** | `amc_contract_terms` (admin full history) |

**Customer visibility:** Ticket history (own) · Invoice status (own) · No platform audit viewer

**Engineer visibility:** Own visit approval outcomes · Assigned ticket updates · No audit viewer

---

## 3. Activity tracking UX patterns

| Pattern | Implementation |
|---------|----------------|
| Timeline | Vertical list with timestamp, actor, action label, optional comment |
| Status badge | `PlatformBadge` for new status |
| Actor display | `PlatformAvatar` + name from user lookup |
| Immutable entries | No edit/delete UI on history rows |
| Real-time | Refresh on navigation; no live updates V1 |

---

## 4. Security event visibility

| Event type | Where visible | Class |
|------------|---------------|:-----:|
| Login / logout / MFA | Platform security audit (future) + Auth screens | REUSE |
| Failed login | Admin audit when platform publishes | REUSE |
| Permission denied API calls | API middleware audit log | REUSE |
| CCTV domain mutations | Domain event audit entries | Auto via REUSE handler |
| Password reset OTP | Auth flow only — not shown in business UI | REUSE |

Engineer and Customer roles: **no access** to `/audit` route — `RoleGuard` + missing `audit:read`.

---

## 5. Correlation support (REUSE)

When admin investigates issues:

| Tool | Usage |
|------|--------|
| Toast correlation ID | Copy from error toast → search Seq/logs |
| Audit entry | Match `correlationId` in payload when present |
| Support workflow | [correlation-support](../../../frontend/correlation-support/support-workflow.md) |

---

## 6. CCTV modules in audit filter

Suggested module filter values for admin searching CCTV events:

| Module value | Domain |
|--------------|--------|
| `CctvCrm.Lead` | Leads |
| `CctvCrm.Customer` | Customers/sites |
| `CctvCrm.Amc` | Plans/contracts |
| `CctvCrm.Service` | Schedules/visits |
| `CctvCrm.Ticket` | Tickets |
| `CctvCrm.Engineer` | Engineers |
| `CctvCrm.Invoice` | Invoices |

---

## 7. Classification summary

| UX area | Class |
|---------|:-----:|
| Audit log viewer page | REUSE |
| Audit storage / capture | REUSE |
| Business timeline components | NEW (on platform-ui) |
| Security event screens | REUSE (platform Auth) |
| Custom audit API | Not created |

---

Related: [audit-mapping.md](../audit-mapping.md) · [screen-design-specification.md](./screen-design-specification.md) · [platform-component-reuse.md](./platform-component-reuse.md)
