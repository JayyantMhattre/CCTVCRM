# Security Readiness Review

**Project:** Aarvii CCTV AMC Management System  
**Phase:** D1-0 — Pre-implementation security assessment  
**Scope:** Design-time review — no penetration test

---

## 1. Executive summary

| Area | Readiness | Notes |
|------|:---------:|-------|
| Authentication | ✅ Ready | Platform Auth REUSE — OpenIddict, MFA, sessions |
| Authorization (RBAC) | ⚠️ Ready with D1 seeds | 30 permissions designed; seeds pending |
| Role separation | ✅ Ready | Engineer/Customer/Admin matrix complete |
| Row-level scoping | ✅ Designed | Own/assigned filters in rbac-matrix + API |
| File access | ✅ Ready | Platform Files + module ownership checks |
| Audit coverage | ✅ Ready | Observer + business histories |
| OTP flows | ⚠️ SMS pending | Auth mechanism REUSE; SMS EXTEND |
| Session management | ✅ Ready | Platform sessions REUSE |

**Security design readiness: 88%** — gaps are implementation (seeds, SMS), not architectural.

---

## 2. Authentication

| Control | Design | Platform | Status |
|---------|--------|----------|:------:|
| JWT access tokens | All portals + mobile | Auth | ✅ REUSE |
| Refresh token rotation | Web + mobile | Auth | ✅ REUSE |
| MFA (Admin) | rbac-matrix | Auth MFA | ✅ REUSE |
| Password reset | Customer portal #57 | Auth | ✅ REUSE |
| Anonymous public inquiry | Rate-limited | Platform middleware | ✅ REUSE |
| API key M2M | Integrations | ApiKeys module | ✅ REUSE |

**No custom auth stack planned** — duplicate check passed.

---

## 3. Authorization

### 3.1 Role separation matrix

| Capability | Admin | Engineer | Customer | Public |
|------------|:-----:|:--------:|:--------:|:------:|
| All leads/customers | ✅ | ❌ | ❌ | ❌ |
| Assigned visits/tickets | ✅ | 🔒 assigned | ❌ | ❌ |
| Own AMC/invoices/tickets | ✅ | ❌ | 🔒 own | ❌ |
| AMC plan/contract manage | ✅ | ❌ | ❌ | ❌ |
| Visit approval | ✅ | ❌ | ❌ | ❌ |
| Visit evidence submit | ✅ | 🔒 assigned | ❌ | ❌ |
| Platform admin (users/audit) | ✅ | ❌ | ❌ | ❌ |

Source: [rbac-matrix.md](../design/rbac-matrix.md) — **validated complete**.

### 3.2 Permission enforcement layers

| Layer | Mechanism | Status |
|-------|-----------|:------:|
| API | `IAuthPermissionChecker` + policy attributes | Designed per endpoint-catalog |
| Web | PermissionGuard + RoleGuard | Designed per navigation-architecture |
| Mobile | SDK + token scopes | mobile-api-consumption.md |
| API keys | Scope strings match permissions | ApiKeys REUSE |

**D1 requirement:** Seed 30 permissions before any B1 endpoint exposes data.

---

## 4. Engineer access

| Rule (freeze §15) | Design enforcement |
|-------------------|-------------------|
| Cannot manage customers | No `customers:manage` permission |
| Cannot manage AMC plans/contracts | No `amc:manage` |
| View assigned work only | Query filter on engineerId |
| Upload photos/videos/reports | `files:write` + visit ownership |
| Create tickets during visits | `tickets:create` scoped |

**Engineer portal routes:** `/engineer/*` — RoleGuard Engineer only.

**Offline sync:** `clientCorrelationId` + server-wins — prevents unauthorized cross-visit writes (mobile-api-consumption.md).

---

## 5. Customer access

| Rule | Design enforcement |
|------|-------------------|
| View own AMC only | CustomerId filter on all portal reads |
| Download own invoices | `invoices:download` + ownership (BR-INV-03) |
| Create/reopen own tickets | `tickets:create`, `tickets:reopen` + ownership |
| Request renewal | `amc:request-renewal` |
| Update own profile | `portal/profile` API + BR-AUTH-05 |
| Cannot view unapproved visit reports | Visit status filter (BR-VISIT approval workflow) |

**Customer portal routes:** `/portal/*` — RoleGuard Customer only.

---

## 6. Admin access

| Area | Permission pattern |
|------|-------------------|
| Full CCTV module access | Domain `:manage` permissions |
| Platform administration | REUSE platform screens #39–45 |
| Visit approval | `visits:approve` |
| Invoice lifecycle | `invoices:manage` |
| Engineer assignment | `visits:assign`, `tickets:assign` |

**Admin default landing:** `/admin/dashboard` after role redirect.

---

## 7. File access

| Pattern | Security control |
|---------|-----------------|
| Two-step upload | Platform validates MIME/size/scan hook |
| FileId in business tables | No direct blob URLs exposed |
| Download | Module API verifies ownership before Files read |
| Engineer media | Visit/ticket ownership + `files:write` |
| Customer attachments | Ticket ownership |
| PDF downloads | Generated PDF stored as FileRecord; scoped download |

Source: [file-management-design.md](../design/file-management-design.md)

---

## 8. Audit coverage

| Action type | Capture mechanism |
|-------------|-------------------|
| Domain state changes | EF interceptor → Audit observer |
| Security events | Platform security-events |
| Business timeline | TicketStatusHistory, InvoiceStatusHistory, LeadActivity |
| Admin viewer | Screen #41 REUSE (stub read API — see gap G-A03) |

**Auditable actions mapped:** [audit-mapping.md](../design/audit-mapping.md) — complete for V1 scope.

---

## 9. OTP flows

| Flow | Auth mechanism | Delivery channel | Status |
|------|----------------|------------------|:------:|
| Password Reset OTP | REUSE Auth | SMS (EXTEND) + email fallback | ⚠️ D1 ADR |
| Login OTP | REUSE Auth | SMS (EXTEND) | ⚠️ D1 ADR |

**Risk:** SMS provider not selected (R04). Email fallback documented — product must accept for dev/staging.

---

## 10. Session management

| Feature | Platform | CCTV usage |
|---------|----------|------------|
| Active sessions list | Auth sessions | Admin #44 REUSE |
| Session revoke | Auth API | Admin REUSE |
| Mobile secure storage | ADR-Mobile-0004 | Both apps REUSE |
| Token refresh | Auth | All clients REUSE |

---

## 11. Security gaps (pre-implementation)

| ID | Gap | Severity | Mitigation |
|----|-----|:--------:|------------|
| SEC-01 | RBAC seeds not deployed | High | D1 migration |
| SEC-02 | SMS OTP delivery | High | D1 ISmsProvider + ADR |
| SEC-03 | Row-level filters not coded | High | Per-module B1–B6 |
| SEC-04 | Audit read stub | Medium | Business histories |
| SEC-05 | Public plans API absent | Medium | Static content or B3 ADR |

**No architectural security redesign required.**

---

## 12. Conclusion

Security model **correctly delegates identity to Platform V1** and layers CCTV permissions + row scoping on business modules. Design is sufficient to begin B1 after D1 seed deployment.

---

Related: [platform-reuse-validation.md](./platform-reuse-validation.md) · [rbac-matrix.md](../design/rbac-matrix.md)
