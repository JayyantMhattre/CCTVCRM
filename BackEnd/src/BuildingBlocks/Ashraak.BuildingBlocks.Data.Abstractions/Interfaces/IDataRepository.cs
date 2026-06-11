using Ashraak.BuildingBlocks.Data.Abstractions.Common;

namespace Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;

/// <summary>
/// Full data-access contract combining read, write, and advanced operations.
///
/// <para>
/// This is the primary interface to depend on in application-layer services when
/// you need the full repository surface.  For read-only query handlers, prefer
/// <see cref="IReadRepository{T}"/> to signal intent.
/// </para>
///
/// <code>
/// // Example: inject the full repository in a command handler
/// public class CreateInvoiceHandler(IDataRepository&lt;Invoice&gt; invoices, IDataUnitOfWork uow)
/// {
///     public async Task Handle(CreateInvoiceCommand cmd, CancellationToken ct)
///     {
///         var invoice = new Invoice(cmd.Amount, cmd.TenantId);
///         await invoices.InsertAsync(invoice, ct);
///         await uow.SaveChangesAsync(ct);
///     }
/// }
/// </code>
/// </summary>
public interface IDataRepository<T> : IReadRepository<T>, IWriteRepository<T>, IAdvancedRepository
    where T : BaseDataEntity
{
}
