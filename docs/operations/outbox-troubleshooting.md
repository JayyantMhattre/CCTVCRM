# Outbox Troubleshooting

## Current state

The outbox is **scaffold only**:

- No rows written on `SaveChanges` (DbContexts don't inherit `BaseDbContext`)
- No `OutboxProcessor` hosted service
- Quartz packages referenced but not configured

**Do not troubleshoot outbox delivery failures** — use MediatR synchronous path instead.

---

## If you implement outbox later

### Symptom: Events never consumed

| Check | Action |
|-------|--------|
| Processor registered? | `AddHostedService<YourOutboxProcessor>()` |
| Table exists? | EF migration for `OutboxMessages` |
| Rows stuck? | Inspect `ProcessedOnUtc` null + `Error` column |
| Handler exception | Fix handler; message retries on next poll |

### Symptom: Duplicate side effects

Handlers must be **idempotent** — outbox is at-least-once delivery.

---

## Working cross-module flow today

`UserRegisteredEvent` published synchronously in `RegisterUserCommandHandler` after `SaveChanges`.

If Users profile missing:

1. Check MediatR handler registration in `UsersModule`
2. Check logs for handler exceptions
3. Verify Auth registration succeeded (201)

---

## Related

- [architecture/outbox.md](../architecture/outbox.md)
- [ADR-0002](../adr/ADR-0002-outbox-pattern.md)
