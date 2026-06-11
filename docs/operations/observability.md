# Operations — Observability

## Health endpoints

| URL | K8s probe | Checks |
|-----|------------|--------|
| `/health/live` | liveness | Process running |
| `/health/ready` | readiness | postgres, redis, mongodb (tag `ready`) |

```bash
curl -s http://localhost:5000/health/ready | jq
```

Unhealthy readiness: API should not receive traffic.

---

## OpenTelemetry

Exported via OTLP from `Program.cs`:

- Traces: ASP.NET Core, HttpClient
- Metrics: ASP.NET Core

**Gap:** `OpenTelemetry__Endpoint` env in compose is not bound — configure exporter in code or use default collector on localhost.

Suggested local stack: Grafana Tempo + Grafana UI.

---

## Logs

Serilog → Console + Seq. See [logging.md](./logging.md).

---

## Audit trail

Compliance-oriented capture in MongoDB — separate from application logs.

Query: stub HTTP API; use Mongo shell for operations until Phase 2.

```bash
docker exec -it ashraak-mongodb mongosh ashraak_audit --eval "db.audit_entries.countDocuments()"
```

---

## Dashboards (recommended)

| Tool | Data |
|------|------|
| Seq | Logs |
| Grafana | OTel traces/metrics |
| MongoDB Compass | Audit entries |

---

## Related

- [architecture/observability.md](../architecture/observability.md)
- [ADR-0005](../adr/ADR-0005-open-telemetry.md)
