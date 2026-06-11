# Outbox — Retry Behavior

- Poll interval: default 5 seconds (`Outbox:PollInterval`)
- Batch size: 20 (`Outbox:BatchSize`)
- Failed messages: retried every cycle (at-least-once delivery)
- Handlers must be idempotent

No exponential backoff in Phase 2A — future enhancement.
