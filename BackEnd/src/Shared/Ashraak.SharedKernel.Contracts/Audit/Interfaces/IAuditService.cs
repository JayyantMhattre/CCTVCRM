using Ashraak.SharedKernel.Contracts.Audit.Dtos;

namespace Ashraak.SharedKernel.Contracts.Audit.Interfaces;

/// <summary>
/// Cross-module facade for writing audit log entries.
/// Implemented by the Audit module's infrastructure and consumed by
/// <c>DomainEventAuditHandler</c> in the Audit Application layer.
/// </summary>
/// <remarks>
/// The implementation writes entries to MongoDB with SHA-256 hash chaining
/// to provide tamper evidence. Each document includes a <c>PreviousHash</c>
/// field that chains it to the previous audit entry for the same tenant.
/// </remarks>
public interface IAuditService
{
    /// <summary>
    /// Persists a single audit entry to the audit store (MongoDB).
    /// </summary>
    /// <param name="entry">The audit entry to record.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be cancelled.</param>
    Task LogAsync(AuditEntryDto entry, CancellationToken cancellationToken = default);
}
