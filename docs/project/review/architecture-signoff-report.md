# Architecture Sign-Off Report — CF-1

**Project:** Aarvii CCTV AMC Management System  
**Date:** 2026-06-12  
**Phase:** CF-1 — Final architectural sign-off (review only)  
**Reviewer role:** Architecture validation (post-implementation)  
**Baseline:** [application-architecture.md](../application-architecture.md) · [high-level-design.md](../high-level-design.md) · ADR-0001

---

## 1. Sign-off summary

| Domain | Verdict |
|--------|---------|
| Module boundaries | ✅ **Approved** |
| Dependency rules | ✅ **Approved** |
| Schema ownership | ✅ **Approved** |
| Shared contracts | ✅ **Approved** |
| Domain events & integrations | ✅ **Approved** |
| Automated enforcement | ✅ **21/21 architecture tests passed** |

**Architecture sign-off issued** subject to code freeze conditions in [code-freeze-decision.md](./code-freeze-decision.md).

---

## 2. Module boundaries

### 2.1 Platform modules (frozen — REUSE only)

| Module | Responsibility | CCTV touch |
|--------|----------------|------------|
| Auth | Identity, tokens, password reset | Portal login; OTP APIs |
| Users | User accounts, preferences | Admin user management |
| Tenant | Tenant settings | REUSE |
| Files | Binary storage, validation | All evidence + PDFs |
| Notifications | Email templates/dispatch | CCTV templates + handlers |
| Audit | Event observer → MongoDB | DbContext registration |
| Webhooks | Outbound event delivery | Catalog extension |
| ApiKeys | M2M access | REUSE |
| Caching | Distributed cache | Password-reset OTP keys |

**No platform module boundaries were modified in Wave 4.**

### 2.2 CCTV business modules (NEW — owned)

| Module | Layers | Aggregate roots (representative) |
|--------|--------|----------------------------------|
| Lead | Domain/Application/Infrastructure/Api | Lead |
| Customer | … | Customer, Site, SiteContact, SiteAssetSummary |
| AMC | … | AmcPlan, AmcContract, AmcContractTerm |
| Service | … | ServiceSchedule, ServiceVisit |
| Ticket | … | Ticket |
| Engineer | … | Engineer |
| Invoice | … | Invoice |
| Reporting | Domain/Application/Infrastructure/Api | Read models only |
| Integration | Application/Infrastructure | Notifications, PDF, lookups, reporting data |

**Boundary rule:** CCTV domain modules do **not** reference each other's domain assemblies. Verified by `CctvLeadDomain_ShouldNotReferenceOtherCctvModuleDomains` (NetArchTest).

### 2.3 Integration layer

| Responsibility | Pattern |
|----------------|---------|
| Cross-module lookups | `I*LookupService` implementations in module Infrastructure, consumed by Integration |
| Notification dispatch | Domain event handlers → `ICctvNotificationDispatcher` |
| PDF generation | `ICctvPdfGenerationService` (QuestPDF) → Files store |
| Reporting queries | `ICctvReportingDataProvider` reads via repositories/contracts |
| SMS | `ISmsProvider` in Integration.Application (ADR-CCTV-0001) |

**Rule verified:** `CctvIntegrationApplication_ShouldNotReferenceCctvInfrastructure` — Integration.Application does not depend on module Infrastructure projects.

---

## 3. Dependencies

### 3.1 Allowed dependency graph

```
Host (Ashraak.Api)
  → Module.Api layers (platform + CCTV)
    → Module.Application
      → Module.Domain
      → SharedKernel.Contracts
    → Module.Infrastructure
      → Module.Domain
      → SharedKernel.*

Cctv.Integration.Infrastructure
  → Integration.Application
  → Module Infrastructure (lookup services only via DI registration)
  → Platform modules (Notifications, Files contracts)

SharedKernel / SharedKernel.Contracts
  → (no module implementations)
```

### 3.2 Automated rules (Ashraak.Architecture.Tests)

| Test | Purpose | CF-1 result |
|------|---------|:-----------:|
| `SharedKernel_ShouldNotReferenceAnyModule` | Kernel isolation | ✅ |
| `SharedKernelContracts_ShouldNotReferenceModuleImplementations` | Contract purity | ✅ |
| `DomainLayer_ShouldNotReferenceApplicationOrInfrastructure` | DDD layering (platform) | ✅ |
| `CctvDomainLayer_ShouldNotReferenceApplicationOrInfrastructure` | DDD layering (CCTV) | ✅ |
| `CctvLeadDomain_ShouldNotReferenceOtherCctvModuleDomains` | Modular monolith isolation | ✅ |
| `CctvIntegrationApplication_ShouldNotReferenceCctvInfrastructure` | Integration boundary | ✅ |
| `AuthModule_ShouldNotDirectlyReferenceUsersDomain` | Platform cross-module | ✅ |
| `TenantModule_ShouldNotDirectlyReferenceAuthDomain` | Platform cross-module | ✅ |
| `CommandHandlers_ShouldBeInternal` | API surface control | ✅ |
| `Repositories_ShouldBeInternal` | Persistence encapsulation | ✅ |
| `CctvNotificationTemplateTests` | Template key registry | ✅ |

**Total: 21 passed, 0 failed.**

### 3.3 Wave 4 dependency impact

| Change | New dependencies introduced |
|--------|----------------------------|
| Reporting query context | None — same Reporting + Integration projects |
| Notification DeepLink strings | None — helper in Integration.Infrastructure |
| Visit video UI | None — existing Service + Files contracts |
| Mobile auth pages | None — Flutter calls existing Auth API |
| Invoice admin UI | None — existing Invoice commands |

**No new project references or cross-boundary violations in Wave 4.**

---

## 4. Schema ownership

Per [database-architecture.md](../design/database-architecture.md):

| Schema | Owner module | Tables (representative) |
|--------|--------------|-------------------------|
| `cctv_lead` | Lead | leads, lead_attachments, inquiries |
| `cctv_customer` | Customer | customers, sites, site_contacts, site_documents |
| `cctv_amc` | AMC | amc_plans, amc_contracts, amc_contract_terms |
| `cctv_service` | Service | service_schedules, service_visits, visit_attachments, visit_photos |
| `cctv_ticket` | Ticket | tickets, ticket_attachments, comments |
| `cctv_engineer` | Engineer | engineers, assignments |
| `cctv_invoice` | Invoice | invoices, invoice_lines, invoice_attachments |

**Rules enforced:**

- FileId references only — no blob paths in business tables ✅
- One active AMC per site — domain + index ✅
- Invoice Option B (term link by type) — documented design decision M-01 ✅
- Wave 4: **no schema migrations** — video uses existing `visit_attachments` table

---

## 5. Contracts

### 5.1 SharedKernel.Contracts.CctvCrm

| Contract type | Usage |
|---------------|-------|
| DTOs | API request/response across modules and Integration |
| Enums | Status, attachment types, report keys |
| Interfaces | `I*LookupService`, `ICctvReportingDataProvider`, feature flags |

**Cross-module communication:** Contracts + domain events only — no direct repository calls across domains.

### 5.2 API surface

| Prefix | Owner |
|--------|-------|
| `/api/v1/auth/*` | Auth |
| `/api/v1/files/*` | Files |
| `/api/v1/cctv/*` | CCTV module Api projects |
| `/connect/token` | OpenIddict |

CCTV routes registered via `Ashraak.Cctv.Api` composition — no collision with platform routes.

---

## 6. Events and integrations

### 6.1 Domain events (§17 notification mapping)

| Event area | Handlers | Channel |
|------------|----------|---------|
| Lead created/converted | `LeadNotificationHandlers` | Email |
| AMC renewal requested, expiry reminder | `AmcNotificationHandlers`, hosted service | Email + SMS |
| Visit scheduled/completed/approved | `ServiceNotificationHandlers` | Email + SMS (where flagged) |
| Ticket created/assigned/closed | `TicketNotificationHandlers` | Email |
| Invoice generated | `InvoiceNotificationHandlers` | Email |

**Wave 4:** All customer/engineer-facing handlers enriched with `DeepLink` payload for mobile routing.

### 6.2 Integration patterns

| Pattern | Implementation |
|---------|----------------|
| Outbox / async dispatch | Platform outbox processors (ADR-0007) |
| Lead conversion orchestration | Integration command handlers |
| Schedule generation on term activation | Domain event handler in Service |
| PDF on generate | Invoice/AMC/Visit domain → Integration PDF service |
| Offline sync | `POST /cctv/engineer/visits/sync` — batch replay |

### 6.3 Webhooks

CCTV events registered in webhook catalog; published via platform `IWebhookPublisher` — no duplicate webhook engine.

---

## 7. Frontend architecture

| App | Pattern | Compliance |
|-----|---------|:----------:|
| Web SPA | React 19, TanStack Query, platform shared components | ✅ |
| Route guards | Auth + role + permission guards on admin/reports | ✅ |
| Mobile | Flutter, Riverpod, GoRouter, feature slices | ✅ |
| API client | Typed HTTP; no duplicate OpenAPI client in web | ✅ |

---

## 8. ADR alignment

| ADR | Topic | Implementation status |
|-----|-------|----------------------|
| ADR-0001 | Modular monolith | ✅ Enforced |
| ADR-0006 | Notifications module | ✅ REUSE |
| ADR-0011 | Files storage | ✅ REUSE |
| ADR-0012 | Flutter mobile platform | ✅ REUSE |
| ADR-CCTV-0001 | SMS provider strategy | ✅ ConfiguredSmsProvider in Integration |
| ADR-CCTV-0002 | PDF generation strategy | ✅ QuestPDF in Integration |
| ADR-CCTV-0003 | Module naming freeze | ✅ Consistent `Ashraak.Cctv.*` |
| ADR-Mobile-0002 | GoRouter navigation | ✅ Deep links extend, not replace |

**No new ADR required for Wave 4** — changes fit existing decisions.

---

## 9. Known architectural acceptances (V1)

| Item | Acceptance rationale |
|------|---------------------|
| Platform audit read API stub | Domain history tables + observer capture sufficient for V1 |
| Auth SMS OTP not wired | Email OTP meets freeze; SMS is V1.1 extension |
| Backend FCM not dispatching CCTV pushes | Mobile infra ready; email/SMS primary for V1 |
| Mobile login UI placeholder | Password reset complete; OAuth shell platform scope |
| Invoice Option B vs literal BR-INV-02 | Documented design override M-01 |

These are **documented deferrals**, not boundary violations.

---

## 10. Architecture sign-off statement

The implemented Aarvii CCTV AMC V1 solution:

1. **Maintains** modular monolith boundaries per ADR-0001  
2. **Enforces** layer and cross-module rules via automated tests  
3. **Owns** business data in isolated PostgreSQL schemas  
4. **Integrates** exclusively through SharedKernel contracts and domain events  
5. **Reuses** platform Auth, Files, Notifications, Audit, Theme, and Mobile foundation without duplication  

**Architecture Sign-Off: GRANTED** for code freeze, conditional on testing-phase gates defined in [code-freeze-decision.md](./code-freeze-decision.md).

---

**Signed (review artifact):** CF-1 Architecture Validation  
**Date:** 2026-06-12

Related: [code-freeze-review.md](./code-freeze-review.md) · [platform-reuse-audit.md](./platform-reuse-audit.md)
