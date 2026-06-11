# Health checks — Operations

## Manual verification

```bash
curl -s http://localhost:5000/health | jq .
curl -s http://localhost:5000/health/ready | jq .
```

## Degraded notifications

Templates path missing → `notifications` check **Degraded** (API still runs; emails may fail). Fix `Notifications:TemplatesPath` or deploy template files with the host.

## Outbox degraded

Fewer than three outbox hosted services → misconfigured host registration. Verify `AddOutboxProcessors()` in `Program.cs`.

## Mongo unhealthy

Audit module not registered or MongoDB down — readiness fails; orchestrator should not route traffic.
