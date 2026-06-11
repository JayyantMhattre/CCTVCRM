# Tenant — Extending

## Add tenant admin endpoints

1. Create command/query in `Ashraak.Tenant.Application` (e.g. `SuspendTenant`, `ChangePlan`)
2. Map in `TenantEndpoints.cs` with authorization:

```csharp
group.MapPost("/{tenantId:guid}/suspend", SuspendTenant)
    .RequireAuthorization("TenantAdmin");
```

3. Aggregate methods already raise domain events — add `IPublisher.Publish` for contract events after save.

## Publish contract events

After `SaveChangesAsync` in handlers:

```csharp
await _publisher.Publish(new TenantProvisionedEvent(tenantId, name, plan), ct);
await _publisher.Publish(new TenantDeletedEvent(tenantId), ct);
```

This enables `TenantDeletedEventHandler` in Users module.

## Register ValidationPipelineBehavior

In `TenantModule.cs`:

```csharp
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ProvisionTenantCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
});
```

Returns `Result` failures instead of throwing `ValidationException`.

## Wire outbox

1. Change `TenantDbContext` to inherit `BaseDbContext`
2. Add EF migration for `outbox_messages` if not applied
3. Deploy outbox processor for `tenant` schema

## Extend TenantService cache

Add caching to `GetSeatLimitAsync` or new methods using `CacheKeyBuilder.ForEntity` / `ForFeatureFlags`.

Invalidate on plan change via `ICacheInvalidationService.InvalidateTenantConfigAsync` (registered but no callers yet).

## Add feature flags

Store in tenant settings or dedicated table; expose via `GetFeatureFlagAsync`. Cache key: `ForFeatureFlags(tenantId)`.

## Slug uniqueness

Enforced by unique index in `TenantConfiguration`. Handle `Conflict` error in provision handler.

## Module boundaries

- Export tenant data only via `ITenantService` and HTTP API
- Do not reference Auth or Users Domain projects
