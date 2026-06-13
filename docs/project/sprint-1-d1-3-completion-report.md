# Sprint 1 / D1-3 Completion Report — Site Management

**Project:** Aarvii CCTV AMC Management System  
**Phase:** D1-3 — Site Management (within Customer module, schema `cctv_customer`)  
**Review gate:** Review Gate 1 (integration test execution deferred until Review Gate 2)  
**Status:** **COMPLETE ✅** (2026-06-11)

---

## 1. Executive summary

Sprint 1 D1-3 delivered **Site Management** inside the existing Customer module: `Site` aggregate with contacts, documents, and asset summary; EF migration `AddSiteTables`; admin and portal HTTP endpoints; RBAC (`sites:read`, `sites:manage`); real site provisioning from lead conversion; lookup service; domain tests (created, not executed); and admin site list/detail UI.

---

## 2. Backend deliverables

| Area | Deliverable |
|------|-------------|
| Domain | `Site` aggregate; `SiteContact`, `SiteDocument`, `SiteAssetSummary`; enums; domain events; `ISiteRepository` |
| Schema | `sites`, `site_contacts`, `site_documents`, `site_asset_summaries` — migration `AddSiteTables` |
| APIs | Full `/sites` CRUD + contacts/documents/asset-summary; portal `/portal/sites`; customer sites list |
| Auth | `SiteAuthorization` with `sites:read` / `sites:manage` + Admin fallback |
| Provisioning | Real `SiteProvisioningService` replaces Integration stub registration |
| Lookup | `ISiteLookupService` with ownership validation for portal |
| Contracts | DTOs + enums in `SharedKernel.Contracts.CctvCrm` |
| Tests (created) | `SiteDomainTests` — **execution deferred to Review Gate 2** |

---

## 3. Frontend deliverables

| Route | Screen |
|-------|--------|
| `/admin/sites` | Paginated site list with search/status filter |
| `/admin/sites/:siteId` | Detail, contacts display, asset summary, status toggle |

Gated by `cctv.customers.enabled` (sites share customer module flag).

---

## 4. Known limitations (by design)

| Item | Target phase |
|------|--------------|
| Site document upload UI (FileId linking only) | Follow-up |
| Full contact edit form on detail page | Follow-up |
| Unit/integration test execution | Review Gate 2 |

---

## 5. Review Gate 1 verification (D1-3 exit)

| Criterion | Result |
|-----------|:------:|
| `dotnet restore` | ✅ |
| `dotnet build` (host) | ✅ |
| Architecture tests (`Ashraak.Architecture.Tests`) | ✅ |
| Module documentation updated | ✅ |
| Completion report | ✅ (this document) |

```bash
dotnet restore BackEnd/src/Host/Ashraak.Api/Ashraak.Api.csproj
dotnet build BackEnd/src/Host/Ashraak.Api/Ashraak.Api.csproj
dotnet test BackEnd/tests/Ashraak.Architecture.Tests/Ashraak.Architecture.Tests.csproj

# Migration (when PostgreSQL available — not a Review Gate 1 blocker)
dotnet ef database update \
  --project BackEnd/src/Modules/Cctv/Customer/Ashraak.Cctv.Customer.Infrastructure \
  --context CustomerDbContext
```

**Deferred to Review Gate 2:**

```bash
dotnet test BackEnd/tests/Ashraak.Integration.Tests --filter Site
```

---

## 6. References

- [Customer module README](../modules/cctv-customer/README.md)
- [ERD — Customer domain](./design/erd-customer-domain.md)
- [Sprint 1 / D1-2 completion report](./sprint-1-d1-2-completion-report.md)
