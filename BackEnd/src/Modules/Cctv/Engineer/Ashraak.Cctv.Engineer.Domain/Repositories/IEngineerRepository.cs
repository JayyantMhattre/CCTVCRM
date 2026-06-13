using Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer;
using Ashraak.Cctv.Engineer.Domain.Enums;
using EngineerAggregate = Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer.Engineer;

namespace Ashraak.Cctv.Engineer.Domain.Repositories;

public sealed record EngineerListResult(IReadOnlyList<EngineerAggregate> Items, long TotalCount);

public interface IEngineerRepository
{
    Task<EngineerAggregate?> GetByIdAsync(EngineerId id, CancellationToken cancellationToken);

    Task<EngineerAggregate?> GetByPlatformUserIdAsync(Guid platformUserId, CancellationToken cancellationToken);

    Task<IReadOnlyList<EngineerAggregate>> GetAllActiveAsync(CancellationToken cancellationToken);

    Task<EngineerListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        EngineerStatus? status,
        string? search,
        CancellationToken cancellationToken);

    Task<bool> ExistsByPlatformUserIdAsync(
        Guid platformUserId,
        EngineerId? excludeId,
        CancellationToken cancellationToken);

    Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken);

    void Add(EngineerAggregate engineer);
}
