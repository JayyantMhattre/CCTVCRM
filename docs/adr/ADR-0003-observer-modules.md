# ADR-0003: Observer Modules (Audit)

**Status:** Accepted  
**Date:** 2026-05-01

---

## Context

Login logging, API tracing, and entity change history are cross-cutting concerns. Implementing them inside each business module duplicates code and violates single responsibility.

---

## Decision

Implement **observer modules** at registration Layer 3:

- **Audit** captures HTTP requests (middleware), EF changes (`SaveChangesInterceptor`), and all `IDomainEvent` (MediatR handler)
- Writes asynchronously to MongoDB via channel + `BackgroundService`
- Exposes read API for admins (query stub in Phase 1)

Observers depend on **contracts** (`IAuditService`) not on business module internals.

---

## Rationale

| Benefit | Explanation |
|---------|-------------|
| Single capture pipeline | Consistent `AuditEntryDto` shape |
| Non-blocking | Queue decouples request latency from Mongo writes |
| Tamper evidence | Per-tenant hash chain |
| Toggleable | Disable Audit in `ModuleExtensions` for lean deployments |

---

## Consequences

**Positive:** Login and security events documented uniformly (e.g. `UserLoggedInDomainEvent`).

**Negative:** MongoDB operational dependency; middleware ordering sensitive; risk of capture loops if audit endpoints not excluded.

---

## Alternatives considered

| Alternative | Rejected because |
|-------------|------------------|
| Serilog-only audit | Not structured for compliance queries |
| Per-module audit tables | Schema sprawl, no hash chain |
| Synchronous Mongo writes | Latency on every request |

---

## References

- [modules/audit/README.md](../modules/audit/README.md)
- `AuditApiCallMiddleware`, `AuditEntityChangeInterceptor`, `DomainEventAuditHandler`
