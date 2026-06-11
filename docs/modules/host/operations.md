# Host — Operations

## Run locally

```powershell
cd BackEnd/src/Host/Ashraak.Api
dotnet run
```

Default URL: `http://localhost:5000` (check launchSettings.json)

Dependencies: PostgreSQL, Redis, MongoDB (see docker-compose).

## Docker

```powershell
cd BackEnd
docker compose up -d
```

API container maps port 8080. Connection strings injected via environment variables.

## Configuration

### appsettings.json

File: `BackEnd/src/Host/Ashraak.Api/appsettings.json`

| Key | Purpose |
|-----|---------|
| `ConnectionStrings:DefaultConnection` | Postgres (health checks) |
| `ConnectionStrings:Auth` | Auth schema (`Search Path=auth`) |
| `ConnectionStrings:Tenant` | Tenant schema |
| `ConnectionStrings:Users` | Users schema |
| `ConnectionStrings:Redis` | Output cache + Caching module |
| `ConnectionStrings:MongoDB` | Audit module |
| `Seq:Url` | Serilog Seq sink (default `http://localhost:5341`) |
| `Logging:LogLevel:*` | ASP.NET log levels |
| `AllowedHosts` | `*` in base config |

### Environment variables (Docker / Production)

| Env var | Maps to |
|---------|---------|
| `ConnectionStrings__Auth` | Auth DB |
| `ConnectionStrings__DefaultConnection` | Default DB |
| `ConnectionStrings__Tenant` | Tenant DB |
| `ConnectionStrings__Users` | Users DB |
| `ConnectionStrings__Redis` | Redis |
| `ConnectionStrings__MongoDB` | MongoDB |
| `Seq__Url` | Seq |
| `OTEL_EXPORTER_OTLP_ENDPOINT` | OpenTelemetry OTLP |
| `Authentication__Google__ClientId/ClientSecret` | Google SSO |
| `Authentication__Microsoft__ClientId/ClientSecret` | Microsoft SSO |
| `Auth__SigningKeyBase64` | **Injected in Docker but not read by AuthModule** — ephemeral keys used |
| `ASPNETCORE_ENVIRONMENT` | Development / Production |
| `ASPNETCORE_URLS` | Bind URLs |

Production secrets via env vars — `appsettings.Production.json` has warning-level logging only, no secrets.

## Health check operations

```powershell
curl http://localhost:5000/health/live
curl http://localhost:5000/health/ready
```

Ready check fails if Postgres, Redis, or MongoDB unreachable.

## Observability

| Tool | Config | Purpose |
|------|--------|---------|
| Serilog | `Seq:Url` | Structured logs |
| OpenTelemetry | `OTEL_EXPORTER_OTLP_ENDPOINT` | Traces + metrics |
| Health endpoints | Built-in | K8s liveness/readiness |

Service name: `Ashraak.Api`

## Build and deploy

```powershell
dotnet publish BackEnd/src/Host/Ashraak.Api/Ashraak.Api.csproj -c Release -o ./publish
```

Single deployable artifact includes all module assemblies.

## Known operational gaps

| Gap | Impact |
|-----|--------|
| Ephemeral OpenIddict keys | All JWTs invalid after restart |
| Outbox processor not hosted | No async event replay |
| RabbitMQ in compose unused | No message broker failover |
| No CORS on host | Frontend must use dev proxy or same-origin gateway |
| EF migrations not in repo | DB schema must be applied manually or migrations generated |

## Troubleshooting

| Symptom | Check |
|---------|-------|
| 503 on `/health/ready` | Postgres, Redis, Mongo connectivity |
| 401 on all API calls | Token expired (restart invalidates ephemeral keys) |
| 403 tenant inactive | `ITenantService.IsActiveAsync` |
| 400 tenant mismatch | JWT tenant vs `X-Tenant-ID` header |
| Module not found at runtime | ProjectReference in csproj, `AddModules` registration |
| Redis connection failed | `ConnectionStrings:Redis`, docker redis container |

## Frontend dev workflow

1. Start API: `dotnet run` in Ashraak.Api (port 5000)
2. Start SPA: `npm run dev` in `FrontEnd/apps/web` (port 3000)
3. Vite proxy forwards `/api` and `/connect` to API

See `FrontEnd/apps/web/vite.config.ts` and `.env.example`.

## Related module operations

- [Auth operations](../auth/operations.md) — token endpoint, SSO
- [Caching operations](../caching/operations.md) — Redis required at startup
- [Audit operations](../audit/operations.md) — MongoDB required
