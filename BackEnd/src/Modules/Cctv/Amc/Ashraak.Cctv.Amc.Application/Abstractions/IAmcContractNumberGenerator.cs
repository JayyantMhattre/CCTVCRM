namespace Ashraak.Cctv.Amc.Application.Abstractions;

/// <summary>Generates AMC contract numbers in format <c>AMC-YYYY-NNNN</c>.</summary>
public interface IAmcContractNumberGenerator
{
    Task<string> GenerateNextAsync(CancellationToken cancellationToken);
}
