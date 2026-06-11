namespace Ashraak.SharedKernel.Contracts.Audit.Dtos;

/// <summary>
/// Data transfer object representing a single auditable action recorded in the system.
/// Written to MongoDB by the Audit module's <c>DomainEventAuditHandler</c>.
/// Provides an immutable, tamper-evident log entry with hash chaining.
/// </summary>
/// <param name="TenantId">The tenant in whose context the action occurred.</param>
/// <param name="UserId">
/// The user who performed the action, or <see langword="null"/> for system-initiated actions
/// (e.g. scheduled jobs, background processors).
/// </param>
/// <param name="Module">The source module name (e.g. <c>"Tenant"</c>, <c>"Auth"</c>).</param>
/// <param name="Action">
/// A short verb describing what happened (e.g. <c>"Provisioned"</c>, <c>"Deleted"</c>,
/// <c>"PlanChanged"</c>).
/// </param>
/// <param name="EntityType">The aggregate or entity type affected (e.g. <c>"Tenant"</c>, <c>"UserProfile"</c>).</param>
/// <param name="EntityId">The string representation of the affected entity's identifier. May be <see langword="null"/> for non-entity actions.</param>
/// <param name="OldValues">JSON snapshot of the entity state before the change. <see langword="null"/> for create operations.</param>
/// <param name="NewValues">JSON snapshot of the entity state after the change. <see langword="null"/> for delete operations.</param>
/// <param name="IpAddress">The client IP address, if available from the HTTP context.</param>
/// <param name="UserAgent">The client User-Agent header, if available.</param>
/// <param name="OccurredOnUtc">UTC timestamp of when the domain event was raised.</param>
public sealed record AuditEntryDto(
    Guid TenantId,
    Guid? UserId,
    string Module,
    string Action,
    string EntityType,
    string? EntityId,
    string? OldValues,
    string? NewValues,
    string? IpAddress,
    string? UserAgent,
    DateTime OccurredOnUtc);
