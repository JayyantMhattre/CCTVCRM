namespace Ashraak.Cctv.Customer.Application.Abstractions;

/// <summary>Generates business numbers in format <c>CU-YYYY-NNNN</c>.</summary>
public interface ICustomerNumberGenerator
{
    Task<string> GenerateNextAsync(CancellationToken cancellationToken);
}
