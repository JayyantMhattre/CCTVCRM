using Ashraak.Cctv.Invoice.Application.Mapping;
using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.Cctv.Invoice.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

namespace Ashraak.Cctv.Invoice.Infrastructure.Services;

internal sealed class InvoiceLookupService(IInvoiceRepository invoiceRepository) : IInvoiceLookupService
{
    public async Task<InvoiceDetailDto?> GetInvoiceAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var invoice = await invoiceRepository.GetByIdAsync(InvoiceId.From(invoiceId), cancellationToken);
        return invoice is null ? null : InvoiceMapper.ToDetail(invoice);
    }

    public async Task<IReadOnlyList<InvoiceSummaryDto>> GetInvoicesForCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var result = await invoiceRepository.GetForCustomerAsync(customerId, 1, 100, cancellationToken);
        return result.Items.Select(InvoiceMapper.ToSummary).ToList();
    }
}
