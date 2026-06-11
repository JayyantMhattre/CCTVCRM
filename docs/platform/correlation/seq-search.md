# Correlation — Seq search

## Filter by correlation ID

```
CorrelationId = '7f3c2a1b9e0d4f6a8b2c1d3e4f5a6b7c'
```

## Login failure investigation

1. Capture `X-Correlation-Id` from API response (or client header).
2. Seq query above across the incident window.
3. Cross-reference:
   - `Rate limit exceeded` (auth abuse)
   - Audit security events (Mongo — separate store; correlate by timestamp + user)
   - Outbox processor errors (`Outbox processor` in message)

## OpenTelemetry

Baggage key `correlation.id` is set on the current activity. Export to Tempo/Jaeger when OTLP collector is configured (`OTEL_EXPORTER_OTLP_ENDPOINT`).
