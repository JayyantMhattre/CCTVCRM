# Data Model Review

**Project:** Aarvii CCTV AMC Management System  
**Phase:** D1-0 — Entity & relationship validation  
**Sources:** entity-model.md · erd-overview.md · entity-lifecycle-matrix.md · database-architecture.md

---

## 1. Executive summary

| Check | Result |
|-------|:------:|
| Entity count vs freeze domains | ✅ 32 entities cover all domains |
| Aggregate boundaries | ✅ 10 roots correctly identified |
| Relationship cardinality | ✅ Matches freeze §5–§16 |
| Normalization | ✅ No redundancy issues |
| File reference pattern | ✅ FileId only — no paths |
| Lifecycle ownership | ✅ Per entity-lifecycle-matrix |
| Renewal model | ✅ Master + Terms validated |
| Invoice model | ✅ Option B validated |
| Visit model | ✅ Evidence entities complete |
| Ticket model | ✅ History + assignment validated |

**Data model: APPROVED for implementation**

---

## 2. Entity inventory validation

| Domain | Entities | Count | Freeze alignment |
|--------|----------|:-----:|------------------|
| Lead | Lead, LeadActivity, LeadRemark, LeadAttachment | 4 | §10 |
| Customer/Site/Asset | Customer, Site, SiteContact, SiteDocument, SiteAssetSummary | 5 | §5–§7 |
| AMC | AMCPlan, AMCPlanVersion, AMCContract, AMCContractTerm, AMCContractDocument | 5 | §8–§9 |
| Service | ServiceSchedule, EngineerAssignment, ServiceVisit, VisitPhoto, VisitLocation, VisitSignature, VisitApproval, VisitAttachment | 8 | §11–§13 |
| Ticket | Ticket, TicketComment, TicketAttachment, TicketAssignment, TicketStatusHistory | 5 | §14 |
| Engineer | Engineer | 1 | §15 |
| Invoice | Invoice, InvoiceLine, InvoiceAttachment, InvoiceStatusHistory | 4 | §16 |
| **Total** | | **32** | |

Reporting + portals: **0 entities** — correct (read models only).

---

## 3. Relationship validation

### 3.1 Customer hierarchy (freeze §5)

```
Customer 1──N Site
Site 1──N SiteContact (max 3)
Site 1──1 SiteAssetSummary
Site 1──N SiteDocument
Site 1──0..1 active AMCContract
```

**Validated:** BR-STRUCT-01..05 enforced in entity invariants.

### 3.2 AMC Master + Terms (freeze §8)

```
AMCContract (master, permanent)
  └── AMCContractTerm (renewal history, 1 active)
        └── AMCPlanVersion (immutable snapshot)
```

**Validated:**
- One active contract per site (BR-AMC-02)
- Plan versioning immutable for historical terms (BR-AMC-05)
- Customer sees active term; admin sees full history

### 3.3 Service chain

```
AMCContractTerm → ServiceSchedule → ServiceVisit
ServiceVisit → VisitPhoto, VisitLocation, VisitSignature, VisitApproval
EngineerAssignment links Engineer to Schedule/Visit
```

**Validated:** Auto-generation from frequency (BR-SCHED-01); mandatory evidence (BR-VISIT-01).

### 3.4 Ticket relationships

```
Ticket → Customer, Site, optional ServiceVisit
Ticket → TicketComment, TicketAttachment, TicketAssignment, TicketStatusHistory
```

**Validated:** Tri-actor creation (BR-TKT-03); reopen by customer (BR-TKT-06).

### 3.5 Invoice relationships (Option B)

```
Invoice → Customer (required)
Invoice → AMCContractTerm (required for AmcRenewal/NewAmc only)
Invoice → optional Ticket/Visit reference for service types
Invoice 1──N InvoiceLine
```

**Validated:** Matches validation-rules V-INV-02/03; no accounting fields (BR-INV-05).

---

## 4. Lifecycle ownership review

| Aggregate | Owner actions | Terminal states |
|-----------|---------------|-----------------|
| Lead | Admin; auto-create from web | Converted, Lost |
| Customer/Site | Admin; conversion; customer profile self-service | Deactivated (soft) |
| AMCContract | Admin | Active → Expired |
| AMCContractTerm | Admin activate/renew | Superseded |
| ServiceSchedule | System generate; Admin reschedule | Completed, Missed, Cancelled |
| ServiceVisit | Engineer execute; Admin approve | Completed + Approved |
| Ticket | Customer/Admin/Engineer create; Admin/Engineer resolve | Closed, Reopened |
| Invoice | Admin manage | Paid, Cancelled |

Source: entity-lifecycle-matrix.md — **no gaps vs freeze lifecycles**.

---

## 5. Domain-specific model review

### 5.1 Renewal model

| Aspect | Design | Issue |
|--------|--------|:-----:|
| Customer renewal request | AMCContract.renewalRequested flag or queue entity | ✅ API + customer screen #48 |
| Admin renew-term flow | New AMCContractTerm on existing master | ✅ Screen #25 |
| Expiry reminders | Scheduled job on term end date | ✅ Event ExpiryReminderDue |
| Lead conversion creates initial contract | B3 completes B1 conversion | ✅ Documented partial path |

### 5.2 Invoice model

| Aspect | Design | Issue |
|--------|--------|:-----:|
| Status enum | Draft/Generated/Sent/Paid/Cancelled | ✅ BR-INV-01 |
| Types (Option B) | AmcRenewal, NewAmc, EmergencyService, SpareReplacement, AdditionalCharges, Other | ✅ |
| Term link | Required AmcRenewal/NewAmc only | ✅ Option B |
| PDF | InvoiceAttachment → FileId | ✅ BR-INV-04 |
| No payment fields | No gateway columns | ✅ §21 |

### 5.3 Visit model

| Aspect | Design | Issue |
|--------|--------|:-----:|
| Selfie | VisitPhoto category OR dedicated capture | ✅ BR-VISIT-01 |
| GPS | VisitLocation (lat, lng, timestamp) | ✅ |
| Signature | VisitSignature → FileId | ✅ |
| Photos min 1 | VisitPhoto count invariant | ✅ |
| Approval gate | VisitApproval before customer visibility | ✅ §13 |
| Categories | Before/During/After + Selfie | ✅ Design extends §12 |

### 5.4 Ticket model

| Aspect | Design | Issue |
|--------|--------|:-----:|
| Priority enum | Low/Medium/High/Critical | ✅ |
| Status enum | Open→Closed→Reopened | ✅ |
| Assignment history | TicketAssignment | ✅ |
| Status audit | TicketStatusHistory | ✅ |
| Reopen | Customer action with history | ✅ BR-TKT-06 |

---

## 6. Normalization review

| Check | Finding |
|-------|---------|
| Duplicate customer data on Lead | Lead holds prospect data; Customer created on conversion — **correct denormalization for pipeline** |
| Plan data on ContractTerm | AMCPlanVersion snapshot — **correct** (BR-AMC-05 immutability) |
| File blobs in business tables | **None** — FileId references only |
| Redundant visit/ticket status | Status on aggregate + history table — **correct** (current + audit) |
| Invoice lines | Normalized InvoiceLine — **correct** |
| Engineer vs User | Engineer links platform UserId — **correct** (no duplicate identity) |

**No normalization issues requiring redesign.**

---

## 7. Schema-per-module mapping

| Schema | Tables (entities) | Cross-schema refs |
|--------|---------------------|-------------------|
| `cctv_lead` | 4 | UserId → platform (logical) |
| `cctv_customer` | 5 | LeadId optional |
| `cctv_amc` | 5 | CustomerId, SiteId |
| `cctv_service` | 8 | SiteId, ContractId, EngineerId |
| `cctv_ticket` | 5 | CustomerId, SiteId, VisitId |
| `cctv_engineer` | 1 | UserId |
| `cctv_invoice` | 4 | CustomerId, TermId optional |

**No cross-schema FK constraints** — validated per database-architecture.md.

---

## 8. Conclusion

Entity model, ERDs, and lifecycle matrix are **consistent, normalized, and complete** for V1. Option B invoicing and Master+Terms AMC are correctly modeled. Ready for EF implementation starting B1 schema (lead only).

---

Related: [architecture-validation-report.md](./architecture-validation-report.md) · [database-implementation-plan.md](../roadmap/database-implementation-plan.md)
