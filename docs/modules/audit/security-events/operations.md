# Security Audit — Operations

## Enable capture

Ensure Audit module and outbox processors are running.

## Verify failed login audit

1. Attempt login with wrong password
2. Check Mongo for `FailedLogin` / `InvalidPassword` actions
3. Check Seq for outbox processor logs

## Lockout

After 5 failures, `AccountLocked` event should appear with `LockedUntilUtc` in JSON payload.
