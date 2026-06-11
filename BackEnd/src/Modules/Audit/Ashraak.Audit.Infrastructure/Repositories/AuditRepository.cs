using Ashraak.SharedKernel.Contracts.Audit.Dtos;
using Ashraak.SharedKernel.Contracts.Audit.Interfaces;
using Ashraak.Audit.Infrastructure.Services;

namespace Ashraak.Audit.Infrastructure.Repositories;

/// <summary>
/// MongoDB-backed implementation of <see cref="IAuditService"/>.
/// This class is the async capture facade: it enqueues entries to the internal
/// audit queue, and the background writer performs MongoDB persistence.
/// </summary>
internal sealed class AuditRepository : IAuditService
{
    private readonly IAuditWriteQueue _queue;

    /// <summary>Initialises the repository with the internal async write queue.</summary>
    public AuditRepository(IAuditWriteQueue queue)
    {
        _queue = queue;
    }

    /// <inheritdoc/>
    public async Task LogAsync(AuditEntryDto entry, CancellationToken cancellationToken = default)
    {
        await _queue.EnqueueAsync(entry, cancellationToken);
    }
}
