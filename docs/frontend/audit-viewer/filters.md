# Audit viewer — Filters

Query parameters sent to the API (when supported):

| Filter | UI control | Query param |
|--------|------------|-------------|
| Module | Text input | `module` |
| Event type | Select | (client filter on `eventType`) |
| Search | Text | (client filter on action/module/userId) |
| From / To | Date inputs | `from`, `to` (ISO UTC) |
| Page | Prev/Next | `page`, `pageSize` |

## Stub backend

When the API returns `{ Message, Filters }` (Phase 2 stub), the page shows an info banner and empty table. Filters still work once real `AuditEntryDto[]` data is returned.

## Normalization

`audit/api.ts` → `normalizeAuditResponse()` accepts:

- `AuditEntryDto[]`
- `{ items, page, pageSize, totalCount }`
- Stub `{ Message, Filters }`
