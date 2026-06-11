# Shared Kernel — API Reference

Public types consumed by module code. Paths relative to `BackEnd/src/Shared/`.

## Domain primitives

| Type | Path | Usage |
|------|------|-------|
| `Entity<TId>` | `Ashraak.SharedKernel/Domain/Primitives/Entity.cs` | Base for entities with identity |
| `AggregateRoot<TId>` | `Domain/Primitives/AggregateRoot.cs` | DDD aggregate with domain events |
| `ValueObject` | `Domain/Primitives/ValueObject.cs` | Immutable value types |

## Events

| Type | Path | Usage |
|------|------|-------|
| `IDomainEvent` | `Domain/Events/IDomainEvent.cs` | Event marker + `EventId`, `OccurredOnUtc` |
| `DomainEvent` | `Domain/Events/DomainEvent.cs` | Base record for all events |
| `IHasDomainEvents` | `Domain/Events/IHasDomainEvents.cs` | Change-tracker integration |
| `IDomainEventPublisher` | `Domain/Events/IDomainEventPublisher.cs` | Immediate publish abstraction |

## Outbox

| Type | Path | Usage |
|------|------|-------|
| `IOutboxMessage` | `Outbox/IOutboxMessage.cs` | Outbox message contract |
| `OutboxMessage` | `Outbox/OutboxMessage.cs` | EF-mapped outbox row |

## Interfaces

| Type | Path | Usage |
|------|------|-------|
| `IUnitOfWork` | `Interfaces/IUnitOfWork.cs` | `SaveChangesAsync()` |
| `ICurrentUser` | `Interfaces/ICurrentUser.cs` | Current user ID, email, roles |
| `ITenantContext` | `Interfaces/ITenantContext.cs` | Resolved tenant ID |
| `IDateTimeProvider` | `Interfaces/IDateTimeProvider.cs` | Testable UTC clock |

## Results

| Type | Path | Usage |
|------|------|-------|
| `Result` | `Results/Result.cs` | Success/failure without value |
| `Result<TValue>` | `Results/Result.cs` | Success/failure with value |
| `Error` | `Results/Error.cs` | Error code, message, type |
| `ErrorType` | `Results/Error.cs` | Enum: Validation, NotFound, Conflict, etc. |

## Pagination

| Type | Path | Usage |
|------|------|-------|
| `PagedList<T>` | `Pagination/PagedList.cs` | Paginated result wrapper |
| `PaginationRequest` | `Pagination/PaginationRequest.cs` | Page/pageSize input |

## Guards

| Type | Path | Usage |
|------|------|-------|
| `Guard` | `Guards/Guard.cs` | Argument validation with `[CallerArgumentExpression]` |

## Contract interfaces (`Ashraak.SharedKernel.Contracts`)

| Interface | Path | Methods (summary) |
|-----------|------|-------------------|
| `ITenantService` | `Tenant/Interfaces/ITenantService.cs` | `GetTenantAsync`, `IsActiveAsync`, `GetFeatureFlagAsync`, `GetSeatLimitAsync` |
| `IUserService` | `Users/Interfaces/IUserService.cs` | `GetUserAsync`, `GetUsersForTenantAsync` |
| `IAuthPermissionChecker` | `Auth/Interfaces/IAuthPermissionChecker.cs` | `GetPermissionsAsync`, role/permission checks |
| `ITokenService` | `Auth/Interfaces/ITokenService.cs` | Contract only — no implementation yet |
| `IAuditService` | `Audit/Interfaces/IAuditService.cs` | `LogAsync(AuditEntryDto)` |

## Contract DTOs

| Type | Path |
|------|------|
| `TenantDto` | `Tenant/Dtos/TenantDto.cs` |
| `TenantPlan` | `Tenant/Dtos/TenantPlan.cs` |
| `UserDto` | `Users/Dtos/UserDto.cs` |
| `AuditEntryDto` | `Audit/Dtos/AuditEntryDto.cs` |

## Contract events

See [Events](./events.md) for the full list of cross-module MediatR notification types.
