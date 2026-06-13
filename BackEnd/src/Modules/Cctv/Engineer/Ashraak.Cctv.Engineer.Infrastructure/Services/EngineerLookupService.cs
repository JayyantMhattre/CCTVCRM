using Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer;
using Ashraak.Cctv.Engineer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

namespace Ashraak.Cctv.Engineer.Infrastructure.Services;

internal sealed class EngineerLookupService(IEngineerRepository repository) : IEngineerLookupService
{
    public async Task<bool> ExistsAsync(Guid engineerId, CancellationToken cancellationToken = default)
    {
        var engineer = await repository.GetByIdAsync(EngineerId.From(engineerId), cancellationToken);
        return engineer is not null && engineer.IsActive;
    }

    public async Task<EngineerSummaryDto?> GetAsync(Guid engineerId, CancellationToken cancellationToken = default)
    {
        var engineer = await repository.GetByIdAsync(EngineerId.From(engineerId), cancellationToken);
        return engineer is null ? null : ToSummary(engineer);
    }

    public async Task<EngineerSummaryDto?> GetForPlatformUserAsync(
        Guid platformUserId,
        CancellationToken cancellationToken = default)
    {
        var engineer = await repository.GetByPlatformUserIdAsync(platformUserId, cancellationToken);
        return engineer is null ? null : ToSummary(engineer);
    }

    private static EngineerSummaryDto ToSummary(Domain.Aggregates.Engineer.Engineer engineer) =>
        new(
            engineer.Id.Value,
            engineer.EngineerNumber,
            engineer.Name,
            engineer.Phone,
            engineer.Status.ToString(),
            engineer.PlatformUserId,
            engineer.CreatedAtUtc);
}
