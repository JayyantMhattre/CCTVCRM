# Audit — Extending

## Implement real GET /audit-logs

Replace stub in `AuditEndpoints.cs`:

1. Add query handler in Application (e.g. `GetAuditLogsQuery`)
2. Query MongoDB via `IMongoDatabase.GetCollection<AuditEntry>("audit_entries")`
3. Apply filters: tenantId, module, date range
4. Return `PagedList<AuditEntryDto>`

Or use Dapper read model on PostgreSQL (Phase 2 plan in status doc).

## Add hash-chain validation endpoint

```csharp
group.MapGet("/integrity/{tenantId:guid}", ValidateHashChain)
    .RequireAuthorization("AdminOnly");
```

Walk `audit_entries` for tenant ordered by `OccurredOnUtc`, verify each `Hash` links to `PreviousHash`.

Logic partially exists in `AuditMongoWriterHostedService` — extract for validation.

## Audit custom module actions

From any module Infrastructure:

```csharp
public class MyService(IAuditService audit, ICurrentUser user, ITenantContext tenant)
{
    public async Task DoWorkAsync()
    {
        await audit.LogAsync(new AuditEntryDto(
            tenant.TenantId, user.UserId, "MyModule", "DoWork", ...));
    }
}
```

`IAuditService` is singleton — safe for fire-and-forget enqueue.

## Extend interceptor filtering

Modify `AuditEntityChangeInterceptor` to skip additional entity types:

```csharp
if (entityType.Name.Contains("OutboxMessage") || entityType.Name.Contains("MySensitive"))
    continue;
```

## Extend middleware paths

Update skip list in `AuditApiCallMiddleware` for high-volume health/metrics paths.

## Migrate to BuildingBlocks Data.Mongo

Optional refactor:

```csharp
services.AddMongoDataLayer(configuration);
services.AddMongoRepository<AuditEntry>();
```

Replace hand-rolled registration in `AuditModule.cs` — functional equivalent.

## Add TTL index

For retention policy:

```csharp
// In EnsureIndexes
collection.Indexes.CreateOne(new CreateIndexModel<AuditEntry>(
    Builders<AuditEntry>.IndexKeys.Ascending(e => e.OccurredOnUtc),
    new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(365) }));
```

## Performance tuning

| Knob | Location |
|------|----------|
| Channel capacity | `AuditWriteQueue` |
| Batch insert | `AuditMongoWriterHostedService` |
| Skip large JSON snapshots | Interceptor serialization |

## Module boundaries

- Audit must not reference other modules' Domain projects
- Use reflection in `DomainEventAuditHandler` intentionally to avoid coupling
