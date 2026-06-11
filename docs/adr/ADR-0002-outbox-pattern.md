# ADR-0002: Outbox Pattern

**Status:** Accepted (design) — **Partial implementation**  
**Date:** 2026-05-01

---

## Context

Cross-module reactions (Users profile on registration, Audit on tenant delete) must survive process crashes and avoid dual-write inconsistencies between database and message broker.

---

## Decision

Adopt the **transactional outbox**:

1. `BaseDbContext` serializes domain events to `OutboxMessage` rows in the same transaction
2. `OutboxProcessorBase<TDbContext>` background worker dispatches via MediatR
3. Quartz packages included for scheduling (future wiring)

---

## Rationale

- At-least-once in-process delivery without immediate RabbitMQ dependency
- Aligns with modular monolith — same database, different schemas
- Upgrade path to external broker by publishing from processor

---

## Consequences

**Positive:** Reliable cross-module workflows when fully wired.

**Negative (current):** Scaffold creates expectation gap — processor not registered; DbContexts don't inherit `BaseDbContext`. Synchronous MediatR used for `UserRegisteredEvent` today.

**Action for implementers:** Complete wiring or document synchronous path explicitly per handler.

---

## Alternatives considered

| Alternative | Rejected because |
|-------------|------------------|
| Immediate MediatR only | Lost events on crash between commit and publish |
| RabbitMQ only | Operational overhead for template default |
| Change Data Capture | Infrastructure complexity |

---

## References

- [outbox.md](../architecture/outbox.md)
- `OutboxProcessorBase.cs`, `OutboxMessage.cs`
