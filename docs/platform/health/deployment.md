# Health checks — Deployment

## Docker Compose

Map container port and probe `/health/ready` after Postgres, Redis, and Mongo are healthy.

## Load balancers

Use `/health/ready` for pool membership; use `/health/live` for restart decisions only.

## CI smoke test

After deploy:

1. `GET /health` → status not `Unhealthy`
2. `GET /health/ready` → postgres + redis + mongodb healthy

## Backward compatibility

`/health/live` remains available (same behaviour as before). `/health` is the aggregate probe added in host hardening phase.
