using Ashraak.Cctv.Invoice.Application.Abstractions;
using Ashraak.Cctv.Invoice.Domain.Repositories;

namespace Ashraak.Cctv.Invoice.Infrastructure.Services;

/// <summary>Generates business numbers in format <c>INV-YYYY-NNNN</c>.</summary>
internal sealed class InvoiceNumberGenerator(IInvoiceRepository repository) : IInvoiceNumberGenerator
{
    public async Task<string> GenerateNextAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var sequence = await repository.GetYearlySequenceAsync(year, cancellationToken);
        return $"INV-{year}-{(sequence + 1):D4}";
    }
}
