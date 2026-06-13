# Customer / Site / Asset

**Status:** D1-2 Customer + D1-3 Site Management **COMPLETE ✅**  
**Schema:** `cctv_customer`  
**Implementation phase:** D1-2 (Customer), D1-3 (Site)

Aarvii CCTV AMC business module on Ashraak Platform V1.

## Projects

```
BackEnd/src/Modules/Cctv/Customer/
  Ashraak.Cctv.Customer.Domain/
  Ashraak.Cctv.Customer.Application/
  Ashraak.Cctv.Customer.Infrastructure/
  Ashraak.Cctv.Customer.Api/
```

## D1-2 deliverables

| Area | Status |
|------|:------:|
| Customer aggregate + EF migration | ✅ |
| Admin CRUD + status endpoints | ✅ |
| Portal profile GET/PATCH | ✅ |
| Real `CustomerProvisioningService` (lead convert) | ✅ |
| `ICustomerLookupService` | ✅ |
| Admin UI (`/admin/customers`) | ✅ |
| Site aggregate + EF migration `AddSiteTables` | ✅ |
| Site admin CRUD + contacts/documents/asset-summary | ✅ |
| Portal sites GET endpoints | ✅ |
| Real `SiteProvisioningService` (lead convert) | ✅ |
| `ISiteLookupService` | ✅ |
| Admin UI (`/admin/sites`) | ✅ |
| Customer portal web (D1-13b) | ✅ |

### D1-13b — Customer portal web

| Screen | Route |
|--------|-------|
| Dashboard | `/portal/dashboard` |
| AMC + renewal | `/portal/amc` |
| Service history | `/portal/service/history` |
| Upcoming visits | `/portal/service/upcoming` |
| Tickets | `/portal/tickets`, `/portal/tickets/new`, `/portal/tickets/:id` |
| Invoices | `/portal/invoices`, `/portal/invoices/:id` |
| Profile | `/portal/profile` |

Feature flag: `cctv.portal.customer.enabled = true`

Report: [d1-13b-customer-portal-completion-report.md](../../project/d1-13b-customer-portal-completion-report.md)

## Documentation

- [Architecture](./architecture.md)
- [Registration](./registration.md)
- [API](./api.md)
- [Events](./events.md)
- [Extending](./extending.md)
- [Operations](./operations.md)

## Completion report

[Sprint 1 / D1-2 completion report](../../project/sprint-1-d1-2-completion-report.md) · [D1-3 completion report](../../project/sprint-1-d1-3-completion-report.md)
