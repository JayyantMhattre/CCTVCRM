# Sprint 1 / D1-2 Completion Report — Customer Management

**Project:** Aarvii CCTV AMC Management System  
**Phase:** D1-2 — Customer Management (B2 partial — Customer only, not Site)  
**Review gate:** Review Gate 1 (test execution deferred until Review Gate 2)  
**Status:** **COMPLETE ✅** (2026-06-11)

---

## 1. Executive summary

Sprint 1 D1-2 delivered the **Customer Management module**: domain model, EF migration, admin HTTP endpoints, portal profile endpoints, RBAC enforcement, feature-flag gating, real customer provisioning from lead conversion, lookup service, test assets (execution deferred), and admin customer list/detail UI.

---

## 2. Backend deliverables

| Area | Deliverable |
|------|-------------|
| Domain | `Customer` aggregate; `CustomerStatus` Active/Inactive; factories `CreateManual`, `CreateFromLead` |
| Schema | `cctv_customer.customers` — EF migration `InitialCustomerSchema` |
| APIs | GET/POST `/customers`, GET/PUT/PATCH status, GET `/customers/{id}/sites` (empty), GET/PATCH `/portal/profile` |
| Auth | `customers:read`, `customers:manage` via `CustomerAuthorization`; portal via Customer role |
| Provisioning | Real `CustomerProvisioningService` replaces Integration stub |
| Lookup | `ICustomerLookupService` implementation |
| Contracts | DTOs + `CustomerStatusContract` in `SharedKernel.Contracts.CctvCrm` |
| Tests (created) | `CustomerDomainTests` — **execution deferred to Review Gate 2** |

---

## 3. Frontend deliverables

| Route | Screen |
|-------|--------|
| `/admin/customers` | Paginated list with search/status filter |
| `/admin/customers/:customerId` | Detail and activate/deactivate |

Feature flag `cctv.customers.enabled` enabled in dev defaults.

---

## 4. Known limitations (by design)

| Item | Target phase |
|------|--------------|
| Site CRUD and `/customers/{id}/sites` data | D1-3 |
| Portal user auto-link on convert | Follow-up |
| Unit/integration test execution | Review Gate 2 |

---

## 5. Review Gate 1 verification (D1-2 exit)

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
dotnet test BackEnd/tests/Ashraak.Integration.Tests --filter Customer
```

---

## 6. References

- [Customer module README](../modules/cctv-customer/README.md)
- [ERD — Customer domain](../project/design/erd-customer-domain.md)
- [Sprint 1 / B1 completion report](./sprint-1-b1-completion-report.md)
