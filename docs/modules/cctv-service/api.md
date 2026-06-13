# Service module — HTTP API

Base path: `/api/v1/cctv` · Feature flag: `cctv.service.enabled`

## Schedules (Admin)

| Method | Route | Permission | Description |
|--------|-------|------------|-------------|
| GET | `/schedules` | `schedules:read` | Paginated schedule list |
| GET | `/schedules/{scheduleId}` | `schedules:read` | Schedule detail + assignment history |
| POST | `/schedules` | `schedules:manage` | Ad-hoc schedule within active term |
| POST | `/schedules/{scheduleId}/assign` | `visits:assign` | Assign/reassign engineer (creates draft visit) |
| POST | `/schedules/{scheduleId}/reschedule` | `schedules:manage` | Reschedule with reason (min 10 chars) |
| POST | `/schedules/{scheduleId}/cancel` | `schedules:manage` | Cancel with reason |

## Visits (Admin + Engineer)

| Method | Route | Permission | Description |
|--------|-------|------------|-------------|
| GET | `/visits` | `visits:read` | Paginated visit list |
| GET | `/visits/{visitId}` | `visits:read` | Visit detail + evidence checklist |
| POST | `/visits/{visitId}/start` | `visits:execute` | Start visit → schedule InProgress |
| PUT | `/visits/{visitId}/remarks` | `visits:execute` | Update remarks |
| POST | `/visits/{visitId}/photos` | `visits:execute` | Link Before/During/After photo |
| POST | `/visits/{visitId}/selfie` | `visits:execute` | Link selfie |
| POST | `/visits/{visitId}/location` | `visits:execute` | Capture GPS |
| POST | `/visits/{visitId}/signature` | `visits:execute` | Link signature |
| POST | `/visits/{visitId}/attachments` | `visits:execute` | Link video/report attachment |
| POST | `/visits/{visitId}/submit` | `visits:execute` | Submit for approval (BR-VISIT-01) |
| GET | `/visits/approvals` | `visits:approve` | Pending approval queue |
| POST | `/visits/{visitId}/approve` | `visits:approve` | Approve report |
| POST | `/visits/{visitId}/return` | `visits:approve` | Return for rework |

## Portal

| Method | Route | Permission | Description |
|--------|-------|------------|-------------|
| GET | `/portal/visits/upcoming` | `schedules:read` | Upcoming visits for customer sites |
| GET | `/portal/visits/history` | `visits:read` | Approved visit history |
| GET | `/portal/visits/{visitId}` | `visits:read` | Approved visit detail |

## Engineer

| Method | Route | Permission | Description |
|--------|-------|------------|-------------|
| GET | `/engineer/schedules` | `schedules:read` | Assigned schedules |
| GET | `/engineer/schedules/today` | `schedules:read` | Today's work |
| GET | `/engineer/visits/{visitId}` | `visits:read` | Own visit detail |
| POST | `/engineer/visits/sync` | `visits:execute` | Offline sync batch (stub) |
