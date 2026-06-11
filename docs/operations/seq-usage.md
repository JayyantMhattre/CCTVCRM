# Seq Usage

## Start Seq (Docker)

Included in `docker compose up -d`:

- URL: `http://localhost:5341`
- First run may require accepting EULA in UI

## API configuration

`Seq:Url` in appsettings or `Seq__Url` environment variable.

Default: `http://localhost:5341` (development).

Docker internal hostname: `http://seq:5341` when API runs in compose.

---

## What to search

| Filter | Finds |
|--------|-------|
| `@Level = 'Fatal'` | Startup crashes |
| `RequestPath like '/connect/token%'` | Login attempts |
| `TenantId` (if enriched) | Tenant-scoped logs |

Enrich logs in handlers:

```csharp
using (LogContext.PushProperty("TenantId", tenantId)) { ... }
```

---

## Correlation

Template does not yet add `X-Correlation-Id` middleware — recommended future enhancement.

Use `TraceId` from ASP.NET request logging where available.

---

## Related

- [logging.md](./logging.md)
- [observability.md](./observability.md)
