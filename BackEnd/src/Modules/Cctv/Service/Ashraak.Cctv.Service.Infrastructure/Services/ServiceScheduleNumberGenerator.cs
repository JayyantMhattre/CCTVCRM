using Ashraak.Cctv.Service.Application.Abstractions;
using Ashraak.Cctv.Service.Domain.Repositories;

namespace Ashraak.Cctv.Service.Infrastructure.Services;

internal sealed class ServiceScheduleNumberGenerator(IServiceScheduleRepository repository) : IServiceScheduleNumberGenerator
{
    public async Task<string> GenerateNextAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var sequence = await repository.GetYearlySequenceAsync(year, cancellationToken);
        return $"VS-{year}-{(sequence + 1):D4}";
    }
}
