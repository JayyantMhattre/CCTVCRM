# ADR-0005: OpenTelemetry

**Status:** Accepted  
**Date:** 2026-05-01

---

## Context

Operators need distributed traces and metrics for the API host without vendor lock-in to a single APM SaaS in the template.

---

## Decision

Use **OpenTelemetry** in `Ashraak.Api`:

- Resource: service name `Ashraak.Api`
- Traces: ASP.NET Core + HttpClient instrumentation
- Metrics: ASP.NET Core instrumentation
- Export: OTLP exporter (collector endpoint configurable in future)

Complement with **Serilog** for logs (not replaced by OTel logs in template).

---

## Rationale

- Industry standard; works with Jaeger, Grafana Tempo, Datadog agents
- Minimal module changes — host-level concern
- Pairs with health checks for K8s operations

---

## Consequences

**Positive:** Unified telemetry pipeline for template adopters.

**Negative:** No custom spans per module yet; docker `OpenTelemetry__Endpoint` env not bound in code; no frontend RUM.

---

## Alternatives considered

| Alternative | Rejected because |
|-------------|------------------|
| Application Insights only | Azure-specific |
| Serilog-only | Weak distributed tracing |
| Per-module telemetry | Duplication and config sprawl |

---

## References

- [observability.md](../architecture/observability.md)
- `Program.cs` OpenTelemetry section
