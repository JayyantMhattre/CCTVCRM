# Local Development

End-to-end workflow for full-stack development.

## 1. Start infrastructure

```bash
cd BackEnd
Copy-Item .env.example .env
docker compose up -d postgres redis mongodb seq
```

## 2. Start API

```bash
dotnet run --project src/Host/Ashraak.Api
```

Verify: `curl http://localhost:5000/health/ready`

## 3. Start frontend

```bash
cd FrontEnd
pnpm install
pnpm dev
```

Open `http://localhost:3000`

## 4. First tenant + user (typical flow)

1. **Provision tenant** — `POST /api/v1/tenants` (see [api/tenant.md](../api/tenant.md))
2. **Register user** — `POST /api/v1/auth/register` with `tenantId`
3. **Login** — `POST /connect/token` (password grant) via UI or Scalar
4. **Browse** — Dashboard, users, audit (Admin role)

## 5. Observability during dev

| Tool | URL |
|------|-----|
| Scalar API | http://localhost:5000/scalar/v1 |
| Seq logs | http://localhost:5341 |

## Port summary

| Process | Port |
|---------|------|
| Kestrel API | 5000 |
| Vite | 3000 |
| Postgres | 5432 |
| Redis | 6379 |
| Mongo | 27017 |

## Module toggle

To disable Audit or other modules, edit `ModuleExtensions.cs` — see [DEVELOPER_GUIDE.md](../../DEVELOPER_GUIDE.md) §4.

## Related

- [debugging-guide.md](./debugging-guide.md)
- [Documentation governance](../documentation-governance.md)
