# Backend Setup

## Prerequisites

| Tool | Version |
|------|---------|
| .NET SDK | 10.0.103+ (`BackEnd/global.json`) |
| Docker Desktop | For PostgreSQL, Redis, MongoDB, Seq |

Verify:

```bash
dotnet --version
```

## Build

```bash
cd BackEnd
dotnet restore Ashraak.slnx
dotnet build Ashraak.slnx --no-restore
```

Expected: **0 errors**.

### Package audit (CPM)

All NuGet versions live in `Directory.Packages.props`. Before pushing:

```bash
# PowerShell (repo root)
./scripts/package-audit.ps1
```

If restore fails with **NU1010**, add the missing `PackageVersion` — see [platform/packages/troubleshooting.md](../platform/packages/troubleshooting.md).

SDK policy: [platform/packages/sdk-policy.md](../platform/packages/sdk-policy.md).

## Configuration

```bash
Copy-Item .env.example .env   # PowerShell
# Edit .env — see environment-variables.md
```

Connection strings in `src/Host/Ashraak.Api/appsettings.json` override via environment variables in Docker.

## Database

Schemas `auth`, `tenant`, `users` are created by `BackEnd/scripts/init-db.sql` when Postgres container starts.

**Note:** Repository has **no EF migration folders** yet. For schema creation either:

- Add migrations per module (`dotnet ef migrations add`), or
- Apply SQL scripts manually

See [operations/startup-troubleshooting.md](../operations/startup-troubleshooting.md).

## Run API

```bash
dotnet run --project src/Host/Ashraak.Api
```

- API: `http://localhost:5000`
- Scalar: `http://localhost:5000/scalar/v1`
- Health: `http://localhost:5000/health/live`

## Tests

```bash
dotnet test Ashraak.slnx
```

## Related

- [Docker setup](./docker-setup.md)
- [modules/host](../modules/host/README.md)
