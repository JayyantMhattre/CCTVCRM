namespace Ashraak.Cctv.Customer.Application.Abstractions;

/// <summary>Generates business site numbers in format <c>ST-YYYY-NNNN</c>.</summary>
public interface ISiteNumberGenerator
{
    Task<string> GenerateNextAsync(CancellationToken cancellationToken);
}
