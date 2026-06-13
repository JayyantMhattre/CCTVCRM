# API — Engineer Management

**Base:** `/api/v1/cctv/engineers`  
**Phase:** D1-8 — full CRUD + workload

| Method | Route | Permission | Status |
|--------|-------|------------|--------|
| GET | `/api/v1/cctv/engineers` | `engineers:read` | Implemented |
| GET | `/api/v1/cctv/engineers/{engineerId}` | `engineers:read` | Implemented |
| POST | `/api/v1/cctv/engineers` | `engineers:manage` | Implemented |
| PUT | `/api/v1/cctv/engineers/{engineerId}` | `engineers:manage` | Implemented |
| PATCH | `/api/v1/cctv/engineers/{engineerId}/status` | `engineers:manage` | Implemented |
| GET | `/api/v1/cctv/engineers/{engineerId}/workload` | `engineers:read` | Implemented |

Cross-module contracts: `IEngineerLookupService` (read), schedule/ticket lookups for workload and deactivation guard.
