using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.Cctv.Invoice.Domain.Enums;
using InvoiceAggregate = Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice.Invoice;

namespace Ashraak.Cctv.Invoice.Domain.Repositories;

public sealed record InvoiceListResult(IReadOnlyList<InvoiceAggregate> Items, long TotalCount);

public interface IInvoiceRepository
{
    Task<InvoiceAggregate?> GetByIdAsync(InvoiceId id, CancellationToken cancellationToken);

    Task<InvoiceListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        InvoiceStatus? status,
        InvoiceType? invoiceType,
        Guid? customerId,
        string? search,
        CancellationToken cancellationToken);

    Task<InvoiceListResult> GetForCustomerAsync(
        Guid customerId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken);

    void Add(InvoiceAggregate invoice);
}
