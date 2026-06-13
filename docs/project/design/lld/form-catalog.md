# Form Catalog

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-7
**Stack (REUSE):** react-hook-form + Zod schemas per module · `PlatformFormField` · `FileUpload` · `useToast` on submit

All forms POST/PUT to endpoints in [endpoint-catalog.md](../endpoint-catalog.md). Validations cross-reference [validation-rules.md](./validation-rules.md).

---

## Form standards (platform)

| Standard | Rule |
|----------|------|
| Schema | Zod in `modules/cctv-{area}/schemas/*.ts` |
| Field wrapper | `PlatformFormField` with `control`, `name`, `label`, `error` |
| Submit | Disable while `isSubmitting`; show loading on primary button |
| Server errors | Map ProblemDetails `errors` to field errors; generic toast for 500 |
| Concurrency | Hidden `rowVersion` on all edit forms |
| File fields | Two-step: `FileUpload` → store `fileId` in form state → submit link with entity |

---

## 1. Public inquiry forms

### Contact / Get Quote / AMC Inquiry

| Field | Type | Required | Default | Validation | File upload |
|-------|------|:--------:|---------|------------|:-----------:|
| inquiryType | hidden/select | ✅ | from route | Contact \| GetQuote \| AmcInquiry | — |
| name | text | ✅ | — | 2–100 chars | — |
| organization | text | ❌ | — | max 200 | — |
| email | email | ✅ | — | valid email | — |
| phone | tel | ✅ | — | 10–15 digits, Indian format | — |
| city | text | ✅ | — | max 100 | — |
| address | textarea | ❌ | — | max 500 | — |
| requirementSummary | textarea | ✅ | — | 10–2000 chars | — |
| preferredPlanCode | select | ❌ | — | valid plan code if AmcInquiry | — |

**Permission:** Anonymous · **API:** `POST /api/v1/cctv/inquiries`

---

## 2. Lead forms

### Create / Edit Lead

| Field | Type | Required | Default | Validation |
|-------|------|:--------:|---------|------------|
| source | select | ✅ | Manual | Website \| Manual \| Referral |
| name, email, phone, city | text | ✅ | — | Same as inquiry |
| organization | text | ❌ | — | max 200 |
| requirementSummary | textarea | ✅ | — | min 10 |
| ownerUserId | user picker | ❌ | current admin | valid user id |
| status | select | ✅ | New | BR-LEAD-01 enum (edit only) |

**Permission:** `leads:manage`

### Lead status change (inline)

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| toStatus | select | ✅ | Valid transition matrix |
| notes | textarea | ❌ | max 1000 |

### Lead remark

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| text | textarea | ✅ | 1–2000 chars |

### Lead attachment link

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| fileId | hidden | ✅ | From FileUpload |
| title | text | ✅ | max 200 |

**FileUpload:** PDF, DOCX, PNG, JPEG · max 10 MB · `files:write`

### Lead conversion wizard

| Step | Fields | Validation |
|------|--------|------------|
| 1 Confirm | Read-only lead summary | status must be Won |
| 2 Site | siteName, siteAddress, city | required |
| 3 AMC | planVersionId (select published), termStartDate, termEndDate | end > start; one active contract rule |
| 4 Review | Confirm checkbox | — |

**Permission:** `leads:convert` · **API:** `POST .../convert`

---

## 3. Customer form

| Field | Type | Required | Default | Validation |
|-------|------|:--------:|---------|------------|
| name | text | ✅ | — | 2–200 chars |
| primaryEmail | email | ✅ | — | unique per tenant |
| primaryPhone | tel | ✅ | — | 10–15 digits |
| billingAddress | textarea | ✅ | — | max 500 |
| city, state, pinCode | text | ✅/❌ | — | pinCode 6 digits |
| portalUserId | user picker | ❌ | — | link existing user |
| status | select | edit | Active | Active \| Inactive |

**Permission:** `customers:manage` · Customer self-profile: subset per BR-AUTH-05 (`PATCH /portal/profile`)

---

## 4. Site form

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| customerId | select | ✅ create | valid customer |
| name | text | ✅ | max 200 |
| addressLine1, addressLine2 | text | ✅/❌ | max 300 |
| city, state, pinCode | text | ✅ | pinCode 6 digits |
| status | select | edit | Active \| Inactive |

### Site contacts (repeatable, max 3)

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| name | text | ✅ | max 100 |
| designation | text | ❌ | max 100 |
| phone | tel | ✅ | 10–15 digits |
| email | email | ❌ | valid email |
| isPrimary | checkbox | ❌ | exactly one primary |

**Permission:** `sites:manage` · **Rule:** BR-STRUCT-03 max 3

---

## 5. Asset summary form

| Field | Type | Required | Default | Validation |
|-------|------|:--------:|---------|------------|
| cameraCount | number | ✅ | 0 | int ≥ 0 |
| dvrCount | number | ✅ | 0 | int ≥ 0 |
| nvrCount | number | ✅ | 0 | int ≥ 0 |
| hardDiskCount | number | ✅ | 0 | int ≥ 0 |
| switchCount | number | ✅ | 0 | int ≥ 0 |
| routerCount | number | ✅ | 0 | int ≥ 0 |
| monitorCount | number | ✅ | 0 | int ≥ 0 |
| brand | text | ❌ | — | max 100 |
| model | text | ❌ | — | max 100 |
| remarks | textarea | ❌ | — | max 500 |

**Permission:** `sites:manage` · **Rule:** BR-STRUCT-04 summary only

---

## 6. AMC Plan forms

### Create plan

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| code | text | ✅ | unique, uppercase slug |
| name | text | ✅ | max 100 |
| description | textarea | ❌ | max 2000 |

### New plan version

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| price | decimal | ✅ | > 0, 2 decimal places |
| visitFrequency | select | ✅ | Monthly \| Quarterly \| HalfYearly \| Yearly |
| includedServices | tag list | ✅ | min 1 item |
| slaDescription | textarea | ✅ | max 2000 |
| effectiveFrom | date | ✅ | ≥ today on draft |

**Permission:** `amcplans:manage`

---

## 7. AMC Contract forms

### Create contract

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| siteId | select | ✅ | site without active contract |
| customerId | read-only | — | from site |
| initialPlanVersionId | select | ✅ | published version only |

### New / renew term

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| planVersionId | select | ✅ | published |
| termType | select | ✅ | Initial \| Renewal |
| startDate | date | ✅ | — |
| endDate | date | ✅ | > startDate |
| price | decimal | ✅ | defaults from plan version, editable |

**Permission:** `amc:manage`

### Customer renewal request

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| message | textarea | ❌ | max 1000 |

**Permission:** `amc:request-renewal`

---

## 8. Scheduling forms

### Assign engineer

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| engineerId | select | ✅ | active engineer |
| notes | textarea | ❌ | max 500 |

**Permission:** `visits:assign` · **Rule:** BR-SCHED-04 mandatory before InProgress

### Reschedule

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| newScheduledDate | datetime | ✅ | future date |
| reason | textarea | ✅ | min 10 chars |

### Cancel schedule

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| reason | textarea | ✅ | min 10 chars |

---

## 9. Visit forms (Engineer)

### Visit remarks

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| remarks | textarea | ✅ | min 20 chars (BR-VISIT-01) |

### Photo link (per category)

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| category | select | ✅ | Before \| During \| After |
| fileId | hidden | ✅ | from FileUpload |

**FileUpload:** JPEG, PNG, WEBP · max 10 MB each · min 1 photo total before submit

### Selfie

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| fileId | camera/upload | ✅ | image only |

### GPS capture

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| latitude | readonly | ✅ | -90..90 |
| longitude | readonly | ✅ | -180..180 |
| capturedAt | readonly | ✅ | ISO timestamp |

### Signature

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| fileId | canvas | ✅ | PNG from signature pad |
| signerName | text | ✅ | max 100 |

### Admin approve / return

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| reviewRemarks | textarea | ❌ approve / ✅ return | return min 10 chars |

**Permission:** `visits:approve` / `visits:execute`

---

## 10. Ticket form

### Create ticket

| Field | Type | Required | Default | Validation |
|-------|------|:--------:|---------|------------|
| siteId | select | ✅ | from context | own site (customer) |
| subject | text | ✅ | — | 5–200 chars |
| description | textarea | ✅ | — | 10–5000 chars |
| priority | select | ✅ | Medium | BR-TKT-02 enum |
| serviceVisitId | hidden | ❌ | from visit | valid visit id |
| attachments | file list | ❌ | — | max 5 files, 10 MB each |

**Permission:** `tickets:create`

### Assign / status / reopen

| Field | Form | Required | Validation |
|-------|------|:--------:|------------|
| engineerId | assign | ✅ | active engineer |
| toStatus | status | ✅ | valid transition |
| comment | comment | ❌ | max 2000 |
| reason | reopen | ✅ | min 10 chars (BR-TKT-06) |

---

## 11. Engineer form

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| name | text | ✅ | max 200 |
| employeeCode | text | ✅ | unique |
| phone, email | tel/email | ✅ | valid formats |
| portalUserId | user picker | ❌ | link user |
| status | select | edit | Active \| Inactive |

**Permission:** `engineers:manage`

---

## 12. Invoice form (Option B)

### Header

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| customerId | select | ✅ | — |
| siteId | select | ❌ | optional |
| invoiceType | select | ✅ | Option B enum |
| amcContractTermId | select | conditional | required if AmcRenewal/NewAmc |
| ticketId / serviceVisitId | select | ❌ | optional refs |
| invoiceDate | date | ✅ | — |
| dueDate | date | ❌ | ≥ invoiceDate |

### Line items (repeatable)

| Field | Type | Required | Validation |
|-------|------|:--------:|------------|
| description | text | ✅ | max 500 |
| quantity | decimal | ✅ | > 0 |
| unitPrice | decimal | ✅ | ≥ 0 |
| taxRate | decimal | ❌ | 0–100 |

**Permission:** `invoices:manage` · Draft only editable

---

## 13. Platform forms (REUSE — no CCTV schema)

| Form | Screen | Class |
|------|--------|:-----:|
| Login / MFA / OTP | #10 | REUSE |
| Password reset | #57 | REUSE |
| Profile / notification prefs | #45, #56, #70 | REUSE |
| User admin | #39 | REUSE |
| Tenant settings | #40 | REUSE |

---

Related: [validation-rules.md](./validation-rules.md) · [file-upload-design.md](./file-upload-design.md) · [mobile-screen-design.md](./mobile-screen-design.md)
