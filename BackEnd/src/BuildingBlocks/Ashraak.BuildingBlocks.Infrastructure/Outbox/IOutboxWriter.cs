using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Enqueues integration/domain events into the module outbox table within the current unit of work.
/// </summary>
public interface IOutboxWriter
{
    /// <summary>
    /// Adds an outbox message that will be dispatched after <see cref="Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync"/>.
    /// </summary>
    void Enqueue(IDomainEvent domainEvent);
}
