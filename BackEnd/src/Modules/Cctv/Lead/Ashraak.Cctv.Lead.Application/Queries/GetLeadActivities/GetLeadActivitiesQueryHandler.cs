using Ashraak.Cctv.Lead.Application.Mapping;
using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;
using Ashraak.Cctv.Lead.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Queries.GetLeadActivities;

internal sealed class GetLeadActivitiesQueryHandler(
    ILeadRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetLeadActivitiesQuery, Result<IReadOnlyList<LeadActivityDto>>>
{
    public async Task<Result<IReadOnlyList<LeadActivityDto>>> Handle(
        GetLeadActivitiesQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.LeadsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Leads.Disabled", "Lead management is not enabled for this tenant.");

        var authError = await LeadAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var lead = await repository.GetByIdAsync(LeadId.From(request.LeadId), cancellationToken);
        if (lead is null)
            return Error.NotFound("Leads.NotFound", "Lead not found.");

        return lead.Activities
            .OrderByDescending(a => a.OccurredAtUtc)
            .Select(LeadMapper.ToActivity)
            .ToList();
    }
}
