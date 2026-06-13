using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.Cctv.Invoice.Domain.Enums;
using Ashraak.Cctv.Invoice.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using InvoiceAggregate = Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice.Invoice;

namespace Ashraak.Cctv.Invoice.Infrastructure.Persistence.Repositories;

internal sealed class InvoiceRepository(InvoiceDbContext db) : IInvoiceRepository
{
    private IQueryable<InvoiceAggregate> QueryWithChildren() =>
        db.Invoices
            .Include(i => i.Lines)
            .Include(i => i.Attachments)
            .Include(i => i.StatusHistory);

    public Task<InvoiceAggregate?> GetByIdAsync(InvoiceId id, CancellationToken cancellationToken) =>
        QueryWithChildren().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<InvoiceListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        InvoiceStatus? status,
        InvoiceType? invoiceType,
        Guid? customerId,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = QueryWithChildren().AsQueryable();

        if (status.HasValue)
            query = query.Where(i => i.Status == status.Value);
        if (invoiceType.HasValue)
            query = query.Where(i => i.InvoiceType == invoiceType.Value);
        if (customerId.HasValue)
            query = query.Where(i => i.CustomerId == customerId.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(i => i.InvoiceNumber.Contains(term));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(i => i.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new InvoiceListResult(items, totalCount);
    }

    public Task<InvoiceListResult> GetForCustomerAsync(
        Guid customerId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken) =>
        GetPagedAsync(pageNumber, pageSize, null, null, customerId, null, cancellationToken);

    public async Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken)
    {
        var prefix = $"INV-{year}-";
        var numbers = await db.Invoices
            .Where(i => i.InvoiceNumber.StartsWith(prefix))
            .Select(i => i.InvoiceNumber)
            .ToListAsync(cancellationToken);

        if (numbers.Count == 0)
            return 0;

        return numbers
            .Select(n => int.TryParse(n.AsSpan(prefix.Length), out var seq) ? seq : 0)
            .DefaultIfEmpty(0)
            .Max();
    }

    public void Add(InvoiceAggregate invoice) => db.Invoices.Add(invoice);
}
