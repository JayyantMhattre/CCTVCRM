# Health checks — Catalog

| Name | Tags | What it verifies |
|------|------|------------------|
| `postgres` | `ready`, `db` | `ConnectionStrings:DefaultConnection` |
| `redis` | `ready` | `ConnectionStrings:Redis` |
| `mongodb` | `ready` | `IMongoClient` (Audit module) |
| `notifications` | `ready`, `platform` | `INotificationService` + templates directory |
| `outbox_processors` | `ready`, `platform` | Three `OutboxProcessorHostedService<>` workers + `Outbox` config |

## Readiness probe

Kubernetes example:

```yaml
readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 10
  periodSeconds: 10
```

## Liveness probe

```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
```

## Response format

JSON via `HealthChecks.UI.Client` (`UIResponseWriter.WriteHealthCheckUIResponse`) — includes per-check duration and status.
