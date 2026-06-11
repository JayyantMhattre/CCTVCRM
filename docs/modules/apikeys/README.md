# API Keys Platform

**Status:** Implemented — final V1 core capability.

Enterprise machine-to-machine authentication for integrations, CRM, LMS, HRMS, ERP, and future AI agents.

## Start here

| I want to… | Go to |
|------------|-------|
| Understand architecture | [architecture.md](./architecture.md) |
| Security model | [security.md](./security.md) |
| Scope conventions | [scopes.md](./scopes.md) |
| Rotation & revocation | [rotation.md](./rotation.md) |
| Usage tracking | [usage-tracking.md](./usage-tracking.md) |
| API reference | [api.md](./api.md) |
| Operations | [operations.md](./operations.md) |
| Admin UI (web) | [admin-ui/README.md](./admin-ui/README.md) |
| Mobile visibility | [../../mobile/modules/apikeys/README.md](../../mobile/modules/apikeys/README.md) |
| Capability status | [platform-manifest.md](./platform-manifest.md) |

## ADR

- [ADR-ApiKeys-0001](../../adr/ADR-ApiKeys-0001-api-keys-platform.md)

## V1 scope

| Included | Excluded (future) |
|----------|-------------------|
| Tenant-scoped API keys | OAuth provider |
| Argon2id hashed secrets | Developer portal |
| Scope-based M2M auth | Partner portal |
| Create / rotate / revoke | External IdP for keys |
| Usage metrics | |
