using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Cross-module read access to invoices.</summary>
public interface IInvoiceLookupService
{
    Task<InvoiceDetailDto?> GetInvoiceAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InvoiceSummaryDto>> GetInvoicesForCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);
}
