# Docker Setup

## Quick start

```bash
cd BackEnd
Copy-Item .env.example .env
docker compose up -d
```

## Services

| Service | Image | Port (default) | Required by API |
|---------|-------|----------------|-----------------|
| postgres | postgres:17-alpine | 5432 | Yes |
| redis | redis:7-alpine | 6379 | Yes |
| mongodb | mongo:7 | 27017 | Yes (Audit) |
| seq | datalust/seq | 5341 | No (logging) |
| rabbitmq | rabbitmq:3.13-management | 5672, 15672 | **No** (not wired) |

## Run API in Docker

```bash
docker compose up -d ashraak-api
```

API container listens on **8080** (not 5000).

## Production overlay

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## Deep reference

[BackEnd/DOCKER_ENVIRONMENT.md](../../BackEnd/DOCKER_ENVIRONMENT.md) — full variable matrix and volume layout.

## Related

- [environment-variables.md](./environment-variables.md)
- [operations/deployment-notes.md](../operations/deployment-notes.md)
