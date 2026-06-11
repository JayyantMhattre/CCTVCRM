# Audit Module Status

> **Canonical:** [docs/modules/audit/](../../../docs/modules/audit/README.md)

This document validates the Audit module against the requested design and records incremental changes made in this phase.

**HTTP query:** `GET /api/v1/audit-logs` is a **stub** until Phase 2 Mongo read model.

## Coverage Against Requirements

### 1) Capture

Implemented capture paths:

- **API calls** via `AuditApiCallMiddleware`
- **Entity changes** via `AuditEntityChangeInterceptor` (EF Core `SaveChangesInterceptor`)
- **User actions** via `DomainEventAuditHandler` (domain event stream)

### 2) Store in MongoDB

Audit entries are stored in MongoDB collection `audit_entries` through a background writer service:

- queue producers: middleware/interceptor/domain handler
- consumer: `AuditMongoWriterHostedService`

### 3) Include TenantId, UserId, Before/After

All audit producers emit `AuditEntryDto` containing:

- `TenantId`
- `UserId`
- `OldValues` (before snapshot)
- `NewValues` (after snapshot)

### 4) Integrate Middleware + EF Core Interceptors

Integrated:

- request pipeline: `UseAuditApiCallLogging()`
- EF interception registration through DI `IInterceptor`
- module `DbContext` registrations dynamically attach all interceptors

### 5) Async + Non-Blocking

Achieved using an internal channel queue:

- capture points enqueue quickly (`IAuditService.LogAsync`)
- Mongo persistence occurs in `AuditMongoWriterHostedService`
- request/transaction paths do not wait on Mongo operations

## Architecture

Audit module uses a producer-consumer architecture:

1. Producers:
   - `AuditApiCallMiddleware`
   - `AuditEntityChangeInterceptor`
   - `DomainEventAuditHandler`
2. Queue:
   - `AuditWriteQueue` (`Channel<AuditEntryDto>`)
3. Consumer:
   - `AuditMongoWriterHostedService`
4. Storage:
   - MongoDB `audit_entries` with tenant-scoped hash chain

`AuditRepository` is the capture facade implementing `IAuditService`; it enqueues, not writes directly.

## Event Flow

### API Call Flow

1. HTTP request enters middleware.
2. Middleware awaits downstream pipeline.
3. Middleware creates `AuditEntryDto` with request/response metadata.
4. Entry is enqueued.
5. Background writer persists to Mongo with hash chaining.

### Entity Change Flow

1. EF `SaveChanges` triggers `AuditEntityChangeInterceptor`.
2. Interceptor extracts changed entries (Added/Modified/Deleted).
3. For each entry it captures before/after JSON snapshots.
4. Entry is enqueued.
5. Background writer persists to Mongo.

### Domain Event/User Action Flow

1. Domain event published through MediatR.
2. `DomainEventAuditHandler` maps event to audit DTO.
3. Entry is enqueued.
4. Background writer persists to Mongo.

## Storage Design

Collection: `audit_entries`

Document model: `AuditEntry`

- `Id` (string GUID)
- `TenantId`
- `UserId` nullable
- `Module`
- `Action`
- `EntityType`
- `EntityId`
- `OldValues`
- `NewValues`
- `IpAddress`
- `UserAgent`
- `OccurredOnUtc`
- `PreviousHash`
- `Hash`

Indexes:

- `ix_tenant_id` on `TenantId`
- `ix_tenant_occurred` on `TenantId ASC, OccurredOnUtc DESC`

Integrity:

- SHA-256 hash chain per tenant (`PreviousHash` + current payload).

