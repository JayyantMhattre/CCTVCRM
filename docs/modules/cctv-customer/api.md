# API — Customer / Site / Asset

Base prefix: `/api/v1/cctv`. Auth required unless noted.

## Customer (D1-2)

| Method | Route | Name | Permission | Notes |
|--------|-------|------|------------|-------|
| GET | `/customers` | ListCctvCustomers | `customers:read` | Paginated; query: `page`, `pageSize`, `status`, `search` |
| GET | `/customers/{customerId}` | GetCctvCustomer | `customers:read` | Detail with `rowVersion` |
| POST | `/customers` | CreateCctvCustomer | `customers:manage` | Manual create; number `CU-YYYY-NNNN` |
| PUT | `/customers/{customerId}` | UpdateCctvCustomer | `customers:manage` | Requires `rowVersion` |
| PATCH | `/customers/{customerId}/status` | ChangeCctvCustomerStatus | `customers:manage` | `Active` ↔ `Inactive` |
| GET | `/customers/{customerId}/sites` | GetCctvCustomerSites | `sites:read` | Returns `SiteSummaryDto[]` |

## Customer portal (D1-2 / D1-3)

| Method | Route | Name | Role | Notes |
|--------|-------|------|------|-------|
| GET | `/portal/profile` | GetCctvPortalProfile | Customer | Scoped via `portal_user_id` |
| PATCH | `/portal/profile` | UpdateCctvPortalProfile | Customer | BR-AUTH-05: name, phone, email only |
| GET | `/portal/sites` | GetCctvPortalSites | Customer | Own sites list |
| GET | `/portal/sites/{siteId}` | GetCctvPortalSiteDetail | Customer | Own site detail (ownership validated) |

## Site (D1-3)

| Method | Route | Name | Permission | Notes |
|--------|-------|------|------------|-------|
| GET | `/sites` | ListCctvSites | `sites:read` | Paginated; query: `page`, `pageSize`, `customerId`, `status`, `search` |
| GET | `/sites/{siteId}` | GetCctvSite | `sites:read` | Detail with `rowVersion` |
| POST | `/sites` | CreateCctvSite | `sites:manage` | Manual create; number `ST-YYYY-NNNN` |
| PUT | `/sites/{siteId}` | UpdateCctvSite | `sites:manage` | Requires `rowVersion` |
| PATCH | `/sites/{siteId}/status` | ChangeCctvSiteStatus | `sites:manage` | `Active` ↔ `Inactive` |
| GET | `/sites/{siteId}/contacts` | GetCctvSiteContacts | `sites:read` | Max 3 contacts |
| PUT | `/sites/{siteId}/contacts` | UpsertCctvSiteContacts | `sites:manage` | Replace set; V-SITE-02/03 |
| GET | `/sites/{siteId}/documents` | GetCctvSiteDocuments | `sites:read` | Linked FileIds |
| POST | `/sites/{siteId}/documents` | LinkCctvSiteDocument | `sites:manage` | Requires `rowVersion` |
| DELETE | `/sites/{siteId}/documents/{documentId}` | RemoveCctvSiteDocument | `sites:manage` | Query: `rowVersion` |
| GET | `/sites/{siteId}/asset-summary` | GetCctvSiteAssetSummary | `sites:read` | Summary counts |
| PUT | `/sites/{siteId}/asset-summary` | UpdateCctvSiteAssetSummary | `sites:manage` | Upsert counts; V-ASSET-01 |

## Feature flag

All customer and site endpoints gated by `cctv.customers.enabled`.
