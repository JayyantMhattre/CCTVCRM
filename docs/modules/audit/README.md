# Audit Module

Cross-cutting audit logging: HTTP API calls, EF entity changes, and domain events. Writes asynchronously to MongoDB with per-tenant hash chains. Read API is a stub.

**Source:** `BackEnd/src/Modules/Audit/`

## Projects

| Layer | Project |
|-------|---------|
| Domain | `Ashraak.Audit.Domain` |
| Application | `Ashraak.Audit.Application` |
| Infrastructure | `Ashraak.Audit.Infrastructure` |
| Api | `Ashraak.Audit.Api` |

## Key facts

- **MongoDB** storage — collection `audit_entries`
- **No EF DbContext** — hand-rolled Mongo registration (not BuildingBlocks Data.Mongo)
- **Write path:** Channel queue → `AuditMongoWriterHostedService` (non-blocking)
- **Capture:** Middleware + EF interceptor + MediatR handler
- **GET /api/v1/audit-logs** — **stub**, does not query MongoDB
- Registered as **Layer 3** (last) in host

## Module documentation

- [Architecture](./architecture.md) — write pipeline, hash chain, sequence diagrams
- [Registration](./registration.md)
- [API](./api.md)
- [Events](./events.md)
- [Extending](./extending.md)
- [Operations](./operations.md)

## Dependencies

- [Host](../host/README.md) — middleware pipeline position
- [Shared Kernel](../shared-kernel/README.md) — `IAuditService`, `AuditEntryDto`
