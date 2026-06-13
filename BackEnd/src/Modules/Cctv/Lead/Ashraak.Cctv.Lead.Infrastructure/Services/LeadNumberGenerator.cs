using Ashraak.Cctv.Lead.Application.Abstractions;
using Ashraak.Cctv.Lead.Domain.Repositories;

namespace Ashraak.Cctv.Lead.Infrastructure.Services;

/// <summary>Generates business numbers in format <c>LD-YYYY-NNNN</c>.</summary>
internal sealed class LeadNumberGenerator(ILeadRepository repository) : ILeadNumberGenerator
{
    public async Task<string> GenerateNextAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var sequence = await repository.GetYearlySequenceAsync(year, cancellationToken);
        return $"LD-{year}-{(sequence + 1):D4}";
    }
}
