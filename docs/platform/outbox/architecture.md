# Outbox — Architecture

## DbContext integration

| Module | Integration |
|--------|-------------|
| Auth | `SaveChangesAsync` override + `OutboxModelConfiguration` (IdentityDbContext cannot inherit BaseDbContext) |
| Tenant | Inherits `BaseDbContext` |
| Users | Inherits `BaseDbContext` |

## Processor

`OutboxProcessorHostedService<TDbContext>` — `BackgroundService`, polls every 5s (configurable), batch 20.

Shares dispatch logic with optional Quartz `OutboxProcessorBase` via `OutboxMessageProcessor`.

## Configuration

```json
"Outbox": {
  "PollInterval": "00:00:05",
  "BatchSize": 20
}
```
