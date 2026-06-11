# Platform health checks

Operational visibility for dependencies and platform components.

**Registration:** `Program.cs` + `HostPlatformExtensions.MapPlatformHealthChecks()`

## Endpoints

| Endpoint | Purpose |
|----------|---------|
| `GET /health` | All checks — structured JSON |
| `GET /health/live` | Liveness — process up (no dependency checks) |
| `GET /health/ready` | Readiness — tagged `ready` checks only |

## Status values

ASP.NET Core health check UI response:

- **Healthy** — all evaluated checks passed
- **Degraded** — at least one check degraded, none unhealthy
- **Unhealthy** — at least one check failed

## Related docs

- [checks.md](./checks.md)
- [ops.md](./ops.md)
- [deployment.md](./deployment.md)
