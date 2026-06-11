# Logging

## Serilog configuration

**File:** `BackEnd/src/Host/Ashraak.Api/appsettings.json` → `Serilog` section

**Bootstrap:** Early console logger before host build (`Program.cs`).

**Production host:** `builder.Host.UseSerilog(...)` reads configuration, enriches with:

- Log context
- Machine name
- Thread id

**Sinks:**

| Sink | Purpose |
|------|---------|
| Console | Dev / container stdout |
| Seq | Structured search — `Seq:Url` |

---

## Request logging

`UseSerilogRequestLogging()` — HTTP access log after exception handler.

---

## Log levels (typical)

| Namespace | Dev | Prod |
|-----------|-----|------|
| Default | Information | Warning |
| Microsoft.AspNetCore | Warning | Warning |
| Module handlers | Information | Information |

Override in `appsettings.Development.json` or environment.

---

## MediatR / behaviors

`LoggingBehavior` exists in BuildingBlocks but is **not registered** globally. Handler logs rely on explicit `ILogger` injection.

---

## Frontend

Browser console + Network tab. No centralized frontend log shipping in template.

---

## Related

- [seq-usage.md](./seq-usage.md)
- [modules/host/operations.md](../modules/host/operations.md)
