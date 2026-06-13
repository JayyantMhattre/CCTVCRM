# Validation Rules

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7
**Authority:** [business-rules.md](../../business-rules.md) · [validation-rules in dto-catalog](../dto-catalog.md)

Validation layers (all required):

1. **Client (Zod)** — immediate UX feedback
2. **API (FluentValidation)** — authoritative; returns ProblemDetails
3. **Domain aggregate** — invariant enforcement before save
4. **Database CHECK** — enum/status constraints ([database-naming-standards.md](../database-naming-standards.md))

---

## 1. Customer validations

| ID | Rule | Client | Server | BR |
|----|------|:------:|:------:|-----|
| V-CUST-01 | name required, 2–200 chars | ✅ | ✅ | — |
| V-CUST-02 | primaryEmail valid + unique per tenant | ✅ | ✅ | — |
| V-CUST-03 | primaryPhone 10–15 digits | ✅ | ✅ | — |
| V-CUST-04 | billingAddress required | ✅ | ✅ | — |
| V-CUST-05 | Cannot deactivate if active contracts exist | ❌ | ✅ | — |
| V-CUST-06 | Customer self-edit: only allowed profile fields | ✅ | ✅ | BR-AUTH-05 |

---

## 2. Site validations

| ID | Rule | Client | Server | BR |
|----|------|:------:|:------:|-----|
| V-SITE-01 | site belongs to exactly one customer | ✅ | ✅ | BR-STRUCT-01 |
| V-SITE-02 | max 3 contacts per site | ✅ | ✅ | BR-STRUCT-03 |
| V-SITE-03 | exactly one isPrimary when contacts > 0 | ✅ | ✅ | — |
| V-SITE-04 | pinCode 6 digits (India) | ✅ | ✅ | — |
| V-SITE-05 | Cannot delete site with active contract | ❌ | ✅ | BR-AMC-02 |

---

## 3. Asset summary validations

| ID | Rule | Client | Server | BR |
|----|------|:------:|:------:|-----|
| V-ASSET-01 | All counts integers ≥ 0 | ✅ | ✅ | BR-STRUCT-04 |
| V-ASSET-02 | No individual device IDs accepted | ✅ | ✅ | BR-STRUCT-04 |
| V-ASSET-03 | brand/model/remarks optional max lengths | ✅ | ✅ | BR-STRUCT-05 |

---

## 4. AMC validations

| ID | Rule | Client | Server | BR |
|----|------|:------:|:------:|-----|
| V-AMC-01 | Plan version immutable once referenced | ❌ | ✅ | BR-AMC-07 |
| V-AMC-02 | One active contract per site | ❌ | ✅ | BR-AMC-02 |
| V-AMC-03 | Customer API returns active term only | ❌ | ✅ | BR-AMC-03 |
| V-AMC-04 | term endDate > startDate | ✅ | ✅ | — |
| V-AMC-05 | price > 0 on version/term | ✅ | ✅ | BR-AMC-05 |
| V-AMC-06 | visitFrequency required on version | ✅ | ✅ | BR-AMC-05 |
| V-AMC-07 | Renewal request only on active contract | ❌ | ✅ | BR-AMC-08 |
| V-AMC-08 | Lead conversion requires published plan version | ✅ | ✅ | BR-LEAD-03 |

### Lead status transitions

| From | Allowed To |
|------|------------|
| New | Contacted, Lost |
| Contacted | Qualified, Lost |
| Qualified | QuotationSent, Lost |
| QuotationSent | Negotiation, Lost |
| Negotiation | Won, Lost |
| Won | Converted (via convert action only) |
| Lost, Converted | *(terminal)* |

---

## 5. Scheduling validations

| ID | Rule | Client | Server | BR |
|----|------|:------:|:------:|-----|
| V-SCHED-01 | Status enum exact set | ✅ | ✅ | BR-SCHED-01 |
| V-SCHED-02 | Engineer required before InProgress | ✅ | ✅ | BR-SCHED-04 |
| V-SCHED-03 | Reschedule requires reason ≥ 10 chars | ✅ | ✅ | BR-SCHED-03 |
| V-SCHED-04 | Cancel requires reason | ✅ | ✅ | — |
| V-SCHED-05 | Auto-generation only from active term | ❌ | ✅ | BR-SCHED-02 |

---

## 6. Visit completion validations (critical)

| ID | Rule | Client | Server | BR |
|----|------|:------:|:------:|-----|
| V-VISIT-01 | Selfie file linked | ✅ | ✅ | BR-VISIT-01 |
| V-VISIT-02 | GPS lat/lng + timestamp | ✅ | ✅ | BR-VISIT-01/02 |
| V-VISIT-03 | ≥ 1 photo (Before/During/After) | ✅ | ✅ | BR-VISIT-01/03 |
| V-VISIT-04 | Customer signature file linked | ✅ | ✅ | BR-VISIT-01 |
| V-VISIT-05 | remarks min 20 chars | ✅ | ✅ | BR-VISIT-01 |
| V-VISIT-06 | Submit only on assigned visit | ❌ | ✅ | BR-AUTH-06 |
| V-VISIT-07 | Customer view only if Approved | ❌ | ✅ | BR-VISIT-04/05 |
| V-VISIT-08 | Return requires reason ≥ 10 chars | ✅ | ✅ | — |
| V-VISIT-09 | Engineer cannot manage customers/plans/contracts | ❌ | ✅ | BR-VISIT-07 |

Offline sync: server re-runs V-VISIT-01..05 on batch submit; rejects with 422 + item error.

---

## 7. Ticket validations

| ID | Rule | Client | Server | BR |
|----|------|:------:|:------:|-----|
| V-TKT-01 | Status enum exact set | ✅ | ✅ | BR-TKT-01 |
| V-TKT-02 | Priority enum exact set | ✅ | ✅ | BR-TKT-02 |
| V-TKT-03 | Customer creates only on own sites | ❌ | ✅ | BR-TKT-03 |
| V-TKT-04 | Reopen only Closed → Reopened by customer | ✅ | ✅ | BR-TKT-06 |
| V-TKT-05 | Reopen reason ≥ 10 chars | ✅ | ✅ | BR-TKT-06 |
| V-TKT-06 | Close only from Resolved (admin) | ❌ | ✅ | BR-TKT-01 |
| V-TKT-07 | Max 5 attachments, 10 MB each | ✅ | ✅ | — |

---

## 8. Invoice validations (Option B)

| ID | Rule | Client | Server | BR |
|----|------|:------:|:------:|-----|
| V-INV-01 | Status enum exact set | ✅ | ✅ | BR-INV-01 |
| V-INV-02 | `amcContractTermId` required when type is AmcRenewal or NewAmc | ✅ | ✅ | Option B |
| V-INV-03 | Optional term link for other types | ✅ | ✅ | Option B |
| V-INV-04 | Edit only in Draft | ✅ | ✅ | BR-INV-01 |
| V-INV-05 | ≥ 1 line item | ✅ | ✅ | — |
| V-INV-06 | total = sum(lines) + tax | ✅ | ✅ | — |
| V-INV-07 | Generate produces PDF (BR-INV-04) | ❌ | ✅ | BR-INV-04 |
| V-INV-08 | Customer download own invoices only | ❌ | ✅ | BR-INV-03 |
| V-INV-09 | No payment gateway fields | ✅ | ✅ | BR-INV-05 |

> Note: BR-INV-02 in business-rules states term link for all invoices; **Option B** (approved D0-4) requires term link only for AMC types — server implements Option B.

---

## 9. Lead / inquiry validations

| ID | Rule | Client | Server | BR |
|----|------|:------:|:------:|-----|
| V-LEAD-01 | Website inquiry auto-creates lead | ❌ | ✅ | BR-LEAD-02 |
| V-LEAD-02 | Convert only from Won | ✅ | ✅ | BR-LEAD-03 |
| V-LEAD-03 | Anonymous inquiry rate limited | ❌ | ✅ | platform |
| V-LEAD-04 | email + phone required on inquiry | ✅ | ✅ | — |

---

## 10. Engineer validations

| ID | Rule | Client | Server | BR |
|----|------|:------:|:------:|-----|
| V-ENG-01 | employeeCode unique | ✅ | ✅ | — |
| V-ENG-02 | Cannot deactivate with active assignments | ❌ | ✅ | — |
| V-ENG-03 | portalUserId unique per engineer | ✅ | ✅ | — |

---

## 11. Password reset / login OTP (REUSE platform)

| ID | Rule | Layer | BR |
|----|------|-------|-----|
| V-AUTH-01 | OTP 6 digits, expires 10 min | Platform Auth | BR-AUTH-03/04 |
| V-AUTH-02 | Password min length/complexity per platform policy | Platform | — |
| V-AUTH-03 | No duplicate auth implementation | Architecture | BR-AUTH-01 |

---

## 12. Mobile form validations

| Area | Rule |
|------|------|
| Visit offline queue | Same V-VISIT-01..05 before queue submit |
| Signature canvas | Non-empty stroke detection before export PNG |
| GPS | Block submit if permission denied; show settings link |
| Camera | Fallback to gallery if camera unavailable |
| Sync batch | Each item includes `clientCorrelationId` UUID |
| Ticket create mobile | Same V-TKT-03 + attachment limits |

Flutter: use same validation messages as web (shared copy keys in LLD implementation).

---

## 13. File upload validations

See [file-upload-design.md](./file-upload-design.md) for size/type limits per category.

---

## 14. Error presentation

| Layer | UX |
|-------|-----|
| Field error | Inline under `PlatformFormField` |
| Form-level | `PlatformAlert` at top |
| API 422 | Map to fields + toast summary |
| API 403/404 | Toast error, no field detail |
| Correlation | Show in toast for 500/network ([toasts usage](../../../frontend/toasts/usage.md)) |

---

Related: [form-catalog.md](./form-catalog.md) · [notification-ux-design.md](./notification-ux-design.md)
