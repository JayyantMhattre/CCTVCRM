using Ashraak.Cctv.Engineer.Application.Abstractions;
using Ashraak.Cctv.Engineer.Domain.Repositories;

namespace Ashraak.Cctv.Engineer.Infrastructure.Services;

/// <summary>Generates business numbers in format <c>EN-YYYY-NNNN</c>.</summary>
internal sealed class EngineerNumberGenerator(IEngineerRepository repository) : IEngineerNumberGenerator
{
    public async Task<string> GenerateNextAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var sequence = await repository.GetYearlySequenceAsync(year, cancellationToken);
        return $"EN-{year}-{(sequence + 1):D4}";
    }
}
