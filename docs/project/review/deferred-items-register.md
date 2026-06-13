# Deferred Items Register — CF-1

**Project:** Aarvii CCTV AMC Management System  
**Date:** 2026-06-12  
**Phase:** CF-1 — V1.1 and post-freeze backlog register  
**Purpose:** Single register of all items explicitly **not** in V1 code freeze scope

**Classification key:**

| Class | Meaning |
|-------|---------|
| **Deferred** | Accepted V1 omission; planned or likely for V1.1+ |
| **Rejected** | Out of product scope per freeze §21 — will not implement |
| **Future Consideration** | May revisit; no commitment; needs ADR or change request |

---

## 1. Deferred (V1.1 candidates)

| ID | Item | Source | Rationale | Est. effort |
|----|------|--------|-----------|:-----------:|
| D-01 | Auth **SMS OTP** for password reset | FR-CP-01h, Wave 4 | API accepts `PhoneNumber`; Auth sends email only; SMS in CCTV Integration module — cross-module wiring needed | S |
| D-02 | Backend **FCM push dispatch** for CCTV events | FR-NOTIF, D1-13i | Mobile deep-link handler complete; platform push send from notification service not wired | M |
| D-03 | **Flutter analyze** + CI gate | Release packaging | SDK not verified on all build agents | S |
| D-04 | **Integration / E2E test execution** | Testing policy | Tests may exist; execution deferred until Testing Phase | M |
| D-05 | Portal user **auto-link on lead convert** | D1-13f | Manual provisioning acceptable for V1 | S |
| D-06 | Dedicated **site asset edit** admin form | FR-ASSET | API complete; minimal display sufficient | S |
| D-07 | Visit PDF **embedded photos/signatures** | FR-VISIT-05, D1-13d | Production PDF exists; layout enhancement | M |
| D-08 | **Report charts** (`PlatformChart`) | FR-RPT-01 LLD | Tables + CSV meet V1 LLD; charts optional | M |
| D-09 | Live **www.aarvii.in media import** | FR-WEB-02 | Copy aligned; full asset migration time-consuming | S |
| D-10 | Public inquiry **rate limiting** config | FR-LEAD deferred | Platform middleware exists; ops configuration | S |
| D-11 | Native mobile **login/register OAuth UI** | FR-MOB, platform M2 | Password reset complete; full sign-in shell platform roadmap | M |
| D-12 | **Advanced public SEO** | Public website | Basic `PublicSeo` sufficient for V1 | S |
| D-13 | **Production DNS / SSL cutover** | Operations | Deployment phase, not development | — |
| D-14 | Traceability matrix **doc hygiene** | CF-1 review | Reconcile FR-LEAD-03 and other stale rows | S |
| D-15 | Platform **audit read API** full query | M-07 | Stub acceptable; domain histories provide forensics | L (platform) |
| D-16 | Admin **Renewal Requests** dedicated screen | M-02 | API + nav exist; may be sub-view of contracts | S |
| D-17 | Anonymous **AMC plans** public API | M-03 | Static marketing copy used on public site | S |

---

## 2. Rejected (freeze §21 — out of scope)

| ID | Item | Reason |
|----|------|--------|
| R-01 | Payment gateway integration | §21 — manual invoice paid status only |
| R-02 | Accounting / ERP integration | §21 |
| R-03 | Individual camera asset tracking | §21 / FR-ASSET-02 — summary counts only |
| R-04 | Inventory / warehouse module | §21 |
| R-05 | Multi-tenant SaaS self-signup | §21 — single-tenant deployment model |
| R-06 | Custom CCTV notification HTTP API | By design — event-driven only |
| R-07 | Duplicate platform modules (2nd Auth, Files, etc.) | §20 mandate — rejected by architecture |
| R-08 | WhatsApp / social messaging channel | §21 |
| R-09 | Customer mobile app offline write (full) | §18 scope — engineer offline only for V1 |

---

## 3. Future consideration (no V1.1 commitment)

| ID | Item | Notes |
|----|------|-------|
| F-01 | Biometric login for Customer role | Platform M4 capability; not freeze requirement |
| F-02 | In-app notification center (CCTV-specific) | Push + email sufficient for V1 |
| F-03 | Engineer ticket detail mobile screen | List + create exist; detail via web if needed |
| F-04 | Chart-based executive dashboard | Beyond report tables in LLD |
| F-05 | Webhook retry UI enhancements | Platform webhooks sufficient |
| F-06 | Multi-language public website | Not in freeze |
| F-07 | AI-assisted lead scoring | §21 adjacent — not approved |
| F-08 | Visit video inline player in report page | Files preview sufficient for V1 |

---

## 4. Wave 4 Priority 2 — disposition

Per program manager directive: Priority 2 items completed **only** because existing architecture supported them.

| Item | Completed in V1? | If not, class |
|------|:----------------:|---------------|
| Visit video evidence | ✅ Yes | — |
| Push notification deep links | ✅ Yes (payload + mobile routing) | Backend FCM dispatch → D-02 Deferred |
| Public website content | ✅ Yes (copy alignment) | Full media → D-09 Deferred |

**None marked V1.1 Deferred due to major redesign requirement** — all three were completed within existing Files, deep-link, and content modules.

---

## 5. Testing-phase items (not V1.1 features)

These are **quality gates**, not new functionality:

| ID | Item | Class |
|----|------|-------|
| T-01 | Staging database restore smoke | Testing Phase |
| T-02 | Full backend test suite execution | Testing Phase |
| T-03 | Manual smoke: admin / customer / engineer / public | Testing Phase |
| T-04 | Mobile deep-link tap verification on device | Testing Phase |
| T-05 | Performance baseline (non-blocking for freeze) | Future Consideration |

---

## 6. Register maintenance

| Action | Owner | When |
|--------|-------|------|
| Promote Deferred → implemented | Dev lead | After V1.1 delivery |
| Promote Future → Deferred | PM + Architect | After change request |
| Close Rejected permanently | PM | Only via freeze amendment §22 |

---

## 7. Summary counts

| Class | Count |
|-------|------:|
| Deferred (V1.1 candidates) | 17 |
| Rejected (out of scope) | 9 |
| Future consideration | 8 |
| Testing-phase gates | 5 |

---

Related: [code-freeze-decision.md](./code-freeze-decision.md) · [d1-13-v1-scope-completion-report.md](../d1-13-v1-scope-completion-report.md)
