# Debugging Guide

## Backend (.NET)

### Visual Studio / Rider

- Startup project: `Ashraak.Api`
- Launch profile: `http` → port 5000
- Breakpoints in command handlers (`*Handler.cs`) and endpoints (`*Endpoints.cs`)

### VS Code

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Ashraak.Api",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/BackEnd/src/Host/Ashraak.Api/bin/Debug/net10.0/Ashraak.Api.dll",
      "cwd": "${workspaceFolder}/BackEnd/src/Host/Ashraak.Api",
      "env": { "ASPNETCORE_ENVIRONMENT": "Development" }
    }
  ]
}
```

### Common breakpoints

| Area | File |
|------|------|
| Token issue | `AuthEndpoints.IssueToken` |
| Tenant resolution | `TenantResolutionMiddleware` |
| Registration | `RegisterUserCommandHandler` |
| Audit capture | `AuditApiCallMiddleware`, `DomainEventAuditHandler` |

### Logs

Serilog level in `appsettings.Development.json`. View structured logs in Seq.

---

## Frontend (React)

### Browser

- React DevTools extension
- Network tab for `/api` and `/connect` calls
- Application → Session Storage → `ashraak_session`

### Vite

Source maps enabled by default in dev.

### Common issues

| Symptom | Check |
|---------|-------|
| 401 loop | Refresh token in store; interceptor in `interceptors.ts` |
| CORS | Use Vite proxy — don't call API on different origin without CORS |
| Empty tenant | JWT `tenant_id` claim; login form `tenantId` |

---

## Database

```bash
docker exec -it ashraak-postgres psql -U ashraak -d ashraak
\dn    # list schemas: auth, tenant, users
```

Mongo audit:

```bash
docker exec -it ashraak-mongodb mongosh ashraak_audit
db.audit_entries.find().limit(5)
```

---

## Related

- [errors/common-failures.md](../errors/common-failures.md)
- [operations/startup-troubleshooting.md](../operations/startup-troubleshooting.md)
