# Outbox — Failure Recovery

## Failed messages

`OutboxMessage.Error` stores exception text; `ProcessedOnUtc` remains null — message is retried on next poll.

## Unknown type

If `Type.GetType` fails (assembly moved), message is marked failed permanently until manual fix.

## Handler exceptions

Notification or Users handler throw → outbox marks failed → retries — ensure idempotent handlers (`CreateUserProfile` is idempotent).

## Crash safety

Events are committed with aggregate state in one transaction — no loss between SaveChanges and enqueue.
