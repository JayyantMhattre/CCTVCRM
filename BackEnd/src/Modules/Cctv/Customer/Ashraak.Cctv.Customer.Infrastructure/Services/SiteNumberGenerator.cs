using Ashraak.Cctv.Customer.Application.Abstractions;
using Ashraak.Cctv.Customer.Domain.Repositories;

namespace Ashraak.Cctv.Customer.Infrastructure.Services;

/// <summary>Generates business numbers in format <c>ST-YYYY-NNNN</c>.</summary>
internal sealed class SiteNumberGenerator(ISiteRepository repository) : ISiteNumberGenerator
{
    public async Task<string> GenerateNextAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var sequence = await repository.GetYearlySequenceAsync(year, cancellationToken);
        return $"ST-{year}-{(sequence + 1):D4}";
    }
}
