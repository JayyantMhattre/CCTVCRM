using Ashraak.SharedKernel.Contracts.Audit.Dtos;

namespace Ashraak.Audit.Infrastructure.Services;

/// <summary>
/// Internal async queue used by the Audit module to decouple audit capture
/// from MongoDB persistence. Producers enqueue quickly; a background worker
/// performs the actual writes.
/// </summary>
internal interface IAuditWriteQueue
{
    /// <summary>
    /// Enqueues an audit entry for asynchronous persistence.
    /// This call is intentionally lightweight and non-blocking.
    /// </summary>
    ValueTask EnqueueAsync(AuditEntryDto entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads all queued entries as an async stream consumed by the background writer.
    /// </summary>
    IAsyncEnumerable<AuditEntryDto> DequeueAllAsync(CancellationToken cancellationToken);
}
