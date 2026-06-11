namespace Ashraak.SharedKernel.Contracts.Audit.Dtos;

/// <summary>
/// Audit log row returned by the read API (includes id and event classification for UI).
/// </summary>
public sealed record AuditLogItemDto(
    string Id,
    Guid TenantId,
    Guid? UserId,
    string Module,
    string Action,
    string EventType,
    string? IpAddress,
    string? UserAgent,
    DateTime OccurredOnUtc);

/// <summary>
/// Paginated audit log response.
/// </summary>
public sealed record AuditLogPageDto(
    IReadOnlyList<AuditLogItemDto> Items,
    int Page,
    int PageSize,
    long TotalCount);
