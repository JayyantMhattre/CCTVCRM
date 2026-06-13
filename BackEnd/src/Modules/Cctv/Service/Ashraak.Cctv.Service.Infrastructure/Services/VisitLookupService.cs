using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Aggregates.Visit;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

namespace Ashraak.Cctv.Service.Infrastructure.Services;

internal sealed class VisitLookupService(
    IServiceVisitRepository visitRepository,
    IServiceScheduleRepository scheduleRepository) : IVisitLookupService
{
    public async Task<VisitDetailDto?> GetVisitAsync(
        Guid visitId,
        CancellationToken cancellationToken = default)
    {
        var visit = await visitRepository.GetByIdAsync(ServiceVisitId.From(visitId), cancellationToken);
        if (visit is null)
            return null;

        var schedule = await scheduleRepository.GetByIdAsync(visit.ServiceScheduleId, cancellationToken);
        return schedule is null ? null : ServiceMapper.ToVisitDetail(visit, schedule);
    }

    public async Task<IReadOnlyList<VisitSummaryDto>> GetApprovedVisitsForSiteAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        var visits = await visitRepository.GetApprovedForSitesAsync([siteId], cancellationToken);
        var results = new List<VisitSummaryDto>();

        foreach (var visit in visits)
        {
            var schedule = await scheduleRepository.GetByIdAsync(visit.ServiceScheduleId, cancellationToken);
            if (schedule is not null)
                results.Add(ServiceMapper.ToVisitSummary(visit, schedule));
        }

        return results;
    }
}
