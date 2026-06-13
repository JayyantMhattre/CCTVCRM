using Ashraak.Cctv.Amc.Application.Abstractions;
using Ashraak.Cctv.Amc.Domain.Repositories;

namespace Ashraak.Cctv.Amc.Infrastructure.Services;

/// <summary>Generates business numbers in format <c>AMC-YYYY-NNNN</c>.</summary>
internal sealed class AmcContractNumberGenerator(IAmcContractRepository repository) : IAmcContractNumberGenerator
{
    public async Task<string> GenerateNextAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var sequence = await repository.GetYearlySequenceAsync(year, cancellationToken);
        return $"AMC-{year}-{(sequence + 1):D4}";
    }
}
