using Ashraak.SharedKernel.Contracts.Audit.Dtos;

namespace Ashraak.Audit.Application.Abstractions;

/// <summary>
/// Reads audit entries from the MongoDB audit store.
/// </summary>
public interface IAuditReadService
{
    Task<AuditLogPageDto> QueryAsync(
        Guid tenantId,
        string? module,
        string? search,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
