using Ashraak.Cctv.Lead.Domain.Enums;
using LeadAggregate = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Lead;
using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;

namespace Ashraak.Cctv.Lead.Domain.Repositories;

public sealed record LeadListResult(IReadOnlyList<LeadAggregate> Items, long TotalCount);

public interface ILeadRepository
{
    Task<LeadAggregate?> GetByIdAsync(LeadId id, CancellationToken cancellationToken);

    Task<LeadAggregate?> GetByNumberAsync(string leadNumber, CancellationToken cancellationToken);

    Task<LeadListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        LeadStatus? status,
        string? search,
        CancellationToken cancellationToken);

    Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken);

    void Add(LeadAggregate lead);
}
