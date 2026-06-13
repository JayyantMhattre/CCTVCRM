# API — Lead Management

**Phase:** B1 · Base path: `/api/v1/cctv`

## Public inquiries (anonymous)

| Method | Route | Permission | Status |
|--------|-------|------------|:------:|
| POST | `/inquiries` | — (rate limited) | ✅ |

## Lead management (authenticated)

| Method | Route | Permission | Status |
|--------|-------|------------|:------:|
| GET | `/leads` | `leads:read` | ✅ |
| GET | `/leads/{leadId}` | `leads:read` | ✅ |
| POST | `/leads` | `leads:manage` | ✅ |
| PUT | `/leads/{leadId}` | `leads:manage` | ✅ |
| POST | `/leads/{leadId}/status` | `leads:manage` | ✅ |
| GET | `/leads/{leadId}/activities` | `leads:read` | ✅ |
| POST | `/leads/{leadId}/activities` | `leads:manage` | ✅ |
| GET | `/leads/{leadId}/remarks` | `leads:read` | ✅ |
| POST | `/leads/{leadId}/remarks` | `leads:manage` | ✅ |
| GET | `/leads/{leadId}/attachments` | `leads:read` | ✅ |
| POST | `/leads/{leadId}/attachments` | `leads:manage` + `files:write` | ✅ |
| DELETE | `/leads/{leadId}/attachments/{attachmentId}` | `leads:manage` | ✅ |
| POST | `/leads/{leadId}/convert` | `leads:convert` | ✅ (stub downstream) |

DTOs: `Ashraak.SharedKernel.Contracts.CctvCrm.Dtos`

Feature gate: `CctvFeatureFlags.LeadsEnabled`

## Frontend

| Route | Screen |
|-------|--------|
| `/admin/leads` | Lead pipeline list |
| `/admin/leads/:leadId` | Lead detail (status, activities, remarks) |
