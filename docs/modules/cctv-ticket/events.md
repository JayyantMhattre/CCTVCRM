# Events — Ticket Management

Domain events raised by the `Ticket` aggregate (outbox via `TicketDbContext`):

| Event | Trigger |
|-------|---------|
| `TicketCreatedDomainEvent` | Ticket created (source actor recorded) |
| `TicketAssignedDomainEvent` | Engineer assigned or reassigned |
| `TicketStatusChangedDomainEvent` | Any status transition (companion history row) |
| `TicketClosedDomainEvent` | Resolved → Closed |
| `TicketReopenedDomainEvent` | Closed → Reopened (reason, reopen count) |

Notification integration (Ticket Created / Assigned / Closed) deferred to later sprint per requirements freeze §17.
