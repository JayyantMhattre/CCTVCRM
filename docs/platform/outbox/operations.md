# Outbox — Operations

## Verify processors started

Startup log: `Outbox processor started for AuthDbContext` (and Tenant, Users).

## Database

Tables: `auth.outbox_messages`, `tenant.outbox_messages`, `users.outbox_messages`

Created by `scripts/init-db.sql` or EF migrations.

## Inspect pending

```sql
SELECT "Id", "Type", "CreatedOnUtc", "Error"
FROM auth.outbox_messages
WHERE "ProcessedOnUtc" IS NULL;
```

## Stuck messages

Fix handler or delete row after manual replay.
