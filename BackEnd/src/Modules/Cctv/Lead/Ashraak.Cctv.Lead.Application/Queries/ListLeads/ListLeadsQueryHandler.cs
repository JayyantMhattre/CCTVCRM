using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.Cctv.Lead.Application.Mapping;
using Ashraak.Cctv.Lead.Domain.Enums;
using Ashraak.Cctv.Lead.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Queries.ListLeads;

internal sealed class ListLeadsQueryHandler(
    ILeadRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ListLeadsQuery, Result<PagedResult<LeadSummaryDto>>>
{
    public async Task<Result<PagedResult<LeadSummaryDto>>> Handle(
        ListLeadsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.LeadsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Leads.Disabled", "Lead management is not enabled for this tenant.");

        var authError = await LeadAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        LeadStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!LeadMapper.TryParseStatus(request.Status, out var parsedStatus))
                return Error.Validation("Leads.InvalidStatus", "Invalid lead status filter.");

            status = parsedStatus;
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, 100);

        var result = await repository.GetPagedAsync(
            pageNumber,
            pageSize,
            status,
            request.Search,
            cancellationToken);

        var items = result.Items.Select(LeadMapper.ToSummary).ToList();
        return new PagedResult<LeadSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
