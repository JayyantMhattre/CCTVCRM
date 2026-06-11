# Observability

## Logging (Serilog)

**Configuration:** `BackEnd/src/Host/Ashraak.Api/appsettings.json` → `Serilog` section

**Bootstrap:** `Program.cs` creates bootstrap logger before host build.

**Sinks:**

- Console (structured)
- Seq — `Seq:Url` default `http://localhost:5341`

**Enrichment:** `FromLogContext`, machine name, thread id

**HTTP:** `UseSerilogRequestLogging()` after exception handler

See [operations/logging.md](../operations/logging.md) and [operations/seq-usage.md](../operations/seq-usage.md).

---

## OpenTelemetry

**Registration:** `Program.cs` → `AddOpenTelemetry()`

| Signal | Instrumentation |
|--------|-----------------|
| Traces | ASP.NET Core, HttpClient |
| Metrics | ASP.NET Core |

**Export:** OTLP exporter (default collector endpoint)

**Service name:** `Ashraak.Api` v1.0.0

**Note:** `OpenTelemetry__Endpoint` in docker-compose is **not bound** in code — exporter uses SDK defaults unless extended.

See [ADR-0005](../adr/ADR-0005-open-telemetry.md).

---

## Health checks

| Endpoint | Purpose | Checks |
|----------|---------|--------|
| `GET /health` | Aggregate status (JSON) | All registered checks |
| `GET /health/live` | Liveness | Process up (no dependency probes) |
| `GET /health/ready` | Readiness | Tags `ready` only |

**Readiness dependencies:**

- PostgreSQL (`DefaultConnection`)
- Redis (`ConnectionStrings:Redis`)
- MongoDB (`IMongoClient` from Audit module)
- Notifications module + templates path
- Outbox hosted processors (Auth, Tenant, Users)

See [platform/health/](../platform/health/README.md).

If Audit module is disabled, remove Mongo health check registration.

## Correlation ID

Host middleware propagates `X-Correlation-Id` to Serilog (`CorrelationId`) and OpenTelemetry baggage (`correlation.id`). See [platform/correlation/](../platform/correlation/README.md).

## Rate limiting

Redis-backed limits for auth token and registration routes. See [platform/rate-limits/](../platform/rate-limits/README.md).

---

## Audit as compliance observability

Separate from application logs:

- Tamper-evident hash chain per tenant in MongoDB
- Captures API calls, EF changes, domain events

Not a replacement for Serilog — complementary.

---

## Frontend observability

No OpenTelemetry browser SDK in template.

**Practical approach:**

- Browser devtools Network tab
- API correlation: send `X-Correlation-Id` on API calls (echoed in response)
- TanStack Query Devtools (optional, not pre-wired)

---

## Related

- [operations/observability.md](../operations/observability.md)
- [modules/audit/operations.md](../modules/audit/operations.md)
