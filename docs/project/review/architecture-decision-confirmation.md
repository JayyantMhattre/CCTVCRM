# Architecture Decision Confirmation

**Project:** Aarvii CCTV AMC Management System  
**Phase:** D1-0 — Frozen decisions registry  
**Rule:** **No requirement changes.** Decisions below are design-time confirmations for implementation. Freeze §22 prohibits BRD/requirements edits without change request.

Status: **Confirmed** · **Needs Review** · **Potential Future Enhancement**

---

## 1. Confirmed decisions (implement as documented)

| ID | Decision | Source | Freeze relation | Status |
|----|----------|--------|-----------------|:------:|
| AD-01 | Schema-per-module (7 CCTV PostgreSQL schemas) | database-architecture.md | Aligns §4 modules | **Confirmed** |
| AD-02 | FileId-only references — no blob paths in business tables | database-architecture §7 | Aligns §20 Files reuse | **Confirmed** |
| AD-03 | Summary-only asset tracking — no individual cameras | entity-model, freeze §7 | Direct match | **Confirmed** |
| AD-04 | AMC Master + Terms contract model | entity-model, freeze §8 | Direct match | **Confirmed** |
| AD-05 | AMC plan versioning with immutable snapshots on terms | entity-model, BR-AMC-05 | Direct match | **Confirmed** |
| AD-06 | One active AMC contract per site | BR-AMC-02, entity invariants | Direct match | **Confirmed** |
| AD-07 | **Option B invoicing** — term link required only for AmcRenewal/NewAmc | entity-model §2.7, validation-rules V-INV-02/03 | **Design extends freeze §16 / BR-INV-02** — server implements Option B | **Confirmed** |
| AD-08 | Invoice types: AmcRenewal, NewAmc, EmergencyService, SpareReplacement, AdditionalCharges, Other | validation-rules, endpoint-catalog | Extends §16 | **Confirmed** |
| AD-09 | Two-step file upload: platform POST /files then CCTV link endpoint | file-management-design.md | Aligns §20 | **Confirmed** |
| AD-10 | API prefix `/api/v1/cctv/*` for all business routes | api-architecture.md | New — no freeze conflict | **Confirmed** |
| AD-11 | Cross-module integration via SharedKernel contracts + domain events only | module-contracts.md | Aligns platform pattern | **Confirmed** |
| AD-12 | 30 NEW CCTV permissions + 9 REUSE platform permissions | permission-catalog.md | Aligns §20 | **Confirmed** |
| AD-13 | Roles: Admin REUSE; Engineer + Customer EXTEND (new records) | permission-catalog.md | Aligns §3 actors | **Confirmed** |
| AD-14 | Row-level scoping: own/assigned query filters per module | rbac-matrix.md | Implements actor rules | **Confirmed** |
| AD-15 | Lead conversion orchestration: partial B1, complete B3 | module-contracts, playbook | Aligns §10 | **Confirmed** |
| AD-16 | Visit approval gate before customer visibility | workflow-screen-design §4 | Aligns §13 | **Confirmed** |
| AD-17 | VisitPhoto includes Selfie category (plus Before/During/After) | entity-model | Clarifies §12 + BR-VISIT-01 | **Confirmed** |
| AD-18 | Engineer offline sync: server-wins, clientCorrelationId | mobile-api-consumption.md | Aligns §18 offline | **Confirmed** |
| AD-19 | PDF generation: CCTV-owned service, output via Files module | pdf-document-design.md | Aligns §19 | **Confirmed** |
| AD-20 | Three PDF types: Contract, Visit Report, Invoice | freeze §19 | Direct match | **Confirmed** |
| AD-21 | Web UI: `@/platform-ui` only — no theme adapter imports | theme-usage-design.md | Aligns §20 Theme reuse | **Confirmed** |
| AD-22 | Mobile: OpenAPI-generated Dart SDK only | mobile-api-consumption.md | Aligns §20 Mobile reuse | **Confirmed** |
| AD-23 | Dashboards/reports: cards and tables — no chart library V1 | dashboard-specification.md | Within §21 scope | **Confirmed** |
| AD-24 | 11 notification events mapped to email + SMS (when provider ready) | notification-mapping.md | Aligns §17 | **Confirmed** |
| AD-25 | Webhook catalog entries for CCTV domain events | event-catalog.md | Aligns §20 Webhooks reuse | **Confirmed** |
| AD-26 | No payment gateway, accounting, ERP V1 | freeze §21 | Direct match | **Confirmed** |
| AD-27 | Public inquiry rate limiting via platform middleware | api-architecture.md | Security REUSE | **Confirmed** |
| AD-28 | Reporting module: read-only, no schema | database-architecture.md | Aligns module-architecture | **Confirmed** |
| AD-29 | 71 web screens (D0-5 authoritative) supersedes D0-2 69-screen inventory | screen-inventory.md | Inventory update | **Confirmed** |
| AD-30 | Platform modules frozen — zero Core code changes for CCTV | platform-reuse-validation | Aligns §20 | **Confirmed** |

---

## 2. Needs review (decide during D1 — not requirement changes)

| ID | Decision | Question | Default if no decision | Target |
|----|----------|----------|------------------------|--------|
| NR-01 | SMS provider selection | Which gateway (MSG91, Twilio, etc.)? | Email-only dev/staging; SMS stub logs | D1 ADR |
| NR-02 | PDF rendering library | QuestPDF, iText, Puppeteer? | HTML-to-PDF fallback for dev | D1 ADR |
| NR-03 | Public AMC plan data | Anonymous API vs static marketing content? | **Static content** on website | D1/B3 |
| NR-04 | Admin renewal requests screen | Separate screen #72 vs tab on Contracts #24? | Tab on Contracts list | FP-3 |
| NR-05 | Two Flutter apps vs single app with role switch | Store policy and UX | Two flavors per mobile-architecture | Sprint 9 |
| NR-06 | Option B formal traceability | Add freeze errata vs UAT script only? | **UAT script references Option B** — no freeze edit | D1-1 |

**NR-06 note:** Per scope freeze §22, requirements document is not modified. Option B is a **design interpretation** documented in validation-rules and this register — implementation and UAT follow design pack.

---

## 3. Potential future enhancements (out of V1 — do not implement)

| ID | Enhancement | Reason deferred |
|----|-------------|-----------------|
| FE-01 | Platform audit read API (MongoDB query) | Platform Phase 2; stub acceptable V1 |
| FE-02 | Chart library / PlatformChart widgets | dashboard-specification defers |
| FE-03 | Payment gateway integration | freeze §21 out of scope |
| FE-04 | WhatsApp notifications | freeze §21 out of scope |
| FE-05 | Individual camera asset tracking | freeze §7 summary-only |
| FE-06 | Geo tracking / live engineer map | freeze §21 out of scope |
| FE-07 | Accounting / GST / ERP integration | freeze §21 out of scope |
| FE-08 | Anonymous public AMC plans API | NR-03 — only if marketing needs dynamic pricing |
| FE-09 | Platform audit viewer full Mongo integration | FE-01 related |
| FE-10 | Inventory / purchase orders | freeze §21 out of scope |

---

## 4. Confirmed vs freeze tension points (no change — implement design)

| Freeze text | Design decision | Implementation rule |
|-------------|-----------------|---------------------|
| §16 "Invoice linked to AMC Contract Term" | Option B — term optional for non-AMC invoice types | Server: V-INV-02/03 |
| BR-INV-02 all invoices need term | Option B override | Tests use Option B |
| §17 SMS mandatory | SMS via EXTEND not yet built | D1 stub; production before REL |
| BR-VISIT-03 photo categories | Selfie as VisitPhoto category | BR-VISIT-01 satisfied separately |

---

## 5. Decision authority

| Change type | Process |
|-------------|---------|
| Confirmed AD (§1) | Implement — no re-debate without CR |
| Needs Review (§2) | ADR in D1; default stands if deadline missed |
| Future Enhancement (§3) | Change request + scope amendment required |
| Freeze/requirements text | Change request §22 only — **not in D1-0 scope** |

---

## 6. Conclusion

**30 confirmed architecture decisions** ready for implementation. **6 items need review during D1** with documented defaults. **10 future enhancements** explicitly excluded from V1.

No requirement changes proposed or permitted in this review.

---

Related: [architecture-validation-report.md](./architecture-validation-report.md) · [final-implementation-recommendation.md](./final-implementation-recommendation.md)
