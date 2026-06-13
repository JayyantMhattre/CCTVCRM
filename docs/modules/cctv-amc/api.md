# API — AMC Plans & Contracts

Base prefix: `/api/v1/cctv` · Feature flag: `cctv.amc.enabled`

## AMC Plans (`/amc-plans`)

| Method | Route | Permission |
|--------|-------|------------|
| GET | `/amc-plans` | `amcplans:read` |
| GET | `/amc-plans/{planId}` | `amcplans:read` |
| POST | `/amc-plans` | `amcplans:manage` |
| PUT | `/amc-plans/{planId}` | `amcplans:manage` |
| PATCH | `/amc-plans/{planId}/status` | `amcplans:manage` |
| POST | `/amc-plans/{planId}/versions` | `amcplans:manage` |
| GET | `/amc-plans/{planId}/versions/{versionId}` | `amcplans:read` |
| POST | `/amc-plans/{planId}/versions/{versionId}/publish` | `amcplans:manage` |

## AMC Contracts (`/contracts`)

| Method | Route | Permission |
|--------|-------|------------|
| GET | `/contracts` | `amc:read` |
| GET | `/contracts/{contractId}` | `amc:read` |
| POST | `/contracts` | `amc:manage` |
| POST | `/contracts/{contractId}/terms` | `amc:manage` |
| POST | `/contracts/{contractId}/terms/{termId}/activate` | `amc:manage` |
| PATCH | `/contracts/{contractId}/status` | `amc:manage` |
| GET | `/contracts/{contractId}/documents` | `amc:read` |
| POST | `/contracts/{contractId}/documents` | `amc:manage` |
| POST | `/contracts/{contractId}/renewal-request` | `amc:request-renewal` |
| GET | `/renewal-requests` | `amc:manage` |

## Customer portal

| Method | Route | Permission |
|--------|-------|------------|
| GET | `/portal/amc` | `amc:read` |
| GET | `/portal/amc/documents` | `amc:read` |

DTOs: `Ashraak.SharedKernel.Contracts.CctvCrm.Dtos` · Full catalog: `docs/project/design/endpoint-catalog.md` §6–7
