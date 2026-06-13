namespace Ashraak.Cctv.Invoice.Application.Abstractions;

/// <summary>Generates business invoice numbers in format <c>INV-YYYY-NNNN</c>.</summary>
public interface IInvoiceNumberGenerator
{
    Task<string> GenerateNextAsync(CancellationToken cancellationToken);
}
