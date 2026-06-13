# Integration Roadmap

**Project:** Aarvii CCTV AMC Management System
**Phase:** D0-8
**Principle:** Integrate with platform capabilities — never rebuild ([integration-design.md](../design/integration-design.md))

---

## 1. Integration matrix

| Integration | Platform status | CCTV action | Phase | Effort |
|-------------|-----------------|-------------|:-----:|:------:|
| **Authentication** | ✅ Available | REUSE — wire portal roles | D1 | S |
| **RBAC** | ✅ Available | EXTEND — seed permissions | D1 | S |
| **Files** | ✅ Available | REUSE upload/download; NEW link endpoints | B1+ | — |
| **Notifications email** | ✅ Available | EXTEND — CCTV templates + handlers | B1+ | S |
| **Notifications SMS** | ❌ Not in platform | EXTEND — `ISmsProvider` adapter | D1 | M |
| **Audit** | ✅ Available | REUSE observer + interceptor register | D1 per module | S |
| **Webhooks** | ✅ Available | EXTEND — catalog + publishers | B1+ | S |
| **API Keys** | ✅ Available | REUSE — scopes auto from permissions | — | — |
| **PDF generation** | ❌ Not in platform | NEW service → Files | B4/B6 | M |
| **Push (mobile)** | ✅ Available | EXTEND — deep links + payloads | Sprint 9 | S |
| **Payment gateway** | ❌ Out of scope (§21) | **Future** — see §8 | Post-V1 | — |

---

## 2. Notifications integration schedule

| Event (§17) | Handler phase | Email | SMS |
|-------------|:-------------:|:-----:|:---:|
| Lead Created | B1 | ✅ | — |
| Lead Converted / Welcome | B1/B3 | ✅ | — |
| Ticket Created/Assigned/Closed | B5 | ✅ | Assign: optional |
| Visit Scheduled | B4 | ✅ | ✅ |
| Visit Completed | B4 | ✅ | — |
| AMC Expiry Reminder | B3 | ✅ | ✅ |
| Invoice Generated | B6 | ✅ | — |
| Login/Password OTP | D1 | ✅ (platform) | ✅ |

Templates: `Ashraak.Api/Templates/cctv/*.txt` — [notification-mapping.md](../design/notification-mapping.md)

---

## 3. Files integration schedule

| Use case | Phase | Pattern |
|----------|:-----:|---------|
| Lead attachments | B1 | FileUpload → link |
| Site documents | B2 | Same |
| Visit photos/selfie/signature/video | B4 | Same + mobile offline queue |
| Ticket attachments | B5 | Same |
| Contract/Visit/Invoice PDFs | B3/B4/B6 | Server render → Files store |

Validation: [file-upload-design.md](../design/lld/file-upload-design.md)

---

## 4. Audit integration schedule

| Phase | Action |
|-------|--------|
| D1 | Register `AuditEntityChangeInterceptor` on each CCTV DbContext |
| B1+ | Publish domain events (auto audit via handler) |
| All | Business histories in module tables (ticket/invoice/lead activity) |
| Admin UI | REUSE `/audit` — no CCTV audit module |

Mapping: [audit-mapping.md](../design/audit-mapping.md)

---

## 5. Webhooks integration schedule

| Phase | Action |
|-------|--------|
| D1 | Document planned event names in platform event-catalog |
| B1+ | Implement `IWebhookPublisher` calls from outbox bridge per event |
| B7 | Verify admin can subscribe to `lead.created`, `ticket.closed`, etc. |

Admin UI: **REUSE** existing Webhooks Operations Center

---

## 6. PDF generation integration

| Document | Phase | Trigger |
|----------|:-----:|---------|
| AMC Contract PDF | B3/B6 | Admin generate / term activation |
| Visit Report PDF | B4/B6 | Admin approve visit |
| Invoice PDF | B6 | Admin generate invoice |

**Flow:** Render → byte[] → platform `IFileService` → business `file_id`  
**Design:** [pdf-document-design.md](../design/lld/pdf-document-design.md)

**Spike:** D1 — evaluate QuestPDF vs iText; ADR required

---

## 7. SMS integration

| Use case | Provider | Phase |
|----------|----------|:-----:|
| OTP (auth) | Config-driven `ISmsProvider` | D1 |
| Visit scheduled alert | Same | B4 |
| AMC expiry | Same | B3 |
| Ticket assigned (High/Critical) | Same | B5 optional |

**Fallback:** Email-only if SMS unavailable (log warning, do not fail transaction)

---

## 8. Future payment gateway integration (post-V1)

| Aspect | Approach |
|--------|----------|
| Scope | Explicitly out of freeze §21 |
| Database headroom | Invoice `Paid` manual today; future `payment` entity ([database-future-considerations.md](../design/database-future-considerations.md)) |
| Integration point | Webhook from gateway → update invoice status |
| Effort | Change request + new phase Post-V1 |

**Not scheduled in V1 roadmap.**

---

## 9. Email integration

**REUSE** platform `INotificationService` + SMTP/console/SendGrid config — no CCTV SMTP code.

---

## 10. Verification checklist (per phase)

- [ ] Events published for integration triggers
- [ ] Notification handlers idempotent where possible
- [ ] Files tenant-scoped (platform enforced)
- [ ] Audit entries appear in viewer (when API live)
- [ ] Webhook test subscription receives payload in staging

---

Related: [platform-reuse-roadmap.md](./platform-reuse-roadmap.md) · [backend-development-phases.md](./backend-development-phases.md)
