# Correlation ID

End-to-end request tracing via `X-Correlation-Id` across API, Serilog, OpenTelemetry, Audit, and Outbox logs.

**Implementation:** `BackEnd/src/Host/Ashraak.Api/Middleware/CorrelationMiddleware.cs`

## Behaviour

| Request header | Action |
|----------------|--------|
| Present | Preserved and echoed on response |
| Absent | New GUID (`N` format) generated |

Enrichment:

- Serilog `LogContext` property `CorrelationId`
- OpenTelemetry baggage `correlation.id`
- Activity tag `correlation.id`

## Middleware order

Immediately after `UseExceptionHandler()`, before Serilog request logging.

## Related docs

- [tracing.md](./tracing.md)
- [seq-search.md](./seq-search.md)
- [support-guide.md](./support-guide.md)
