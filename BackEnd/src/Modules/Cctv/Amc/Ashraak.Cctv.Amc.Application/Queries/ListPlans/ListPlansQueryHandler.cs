using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.ListPlans;

internal sealed class ListPlansQueryHandler(
    IAmcPlanRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ListPlansQuery, Result<PagedResult<AmcPlanSummaryDto>>>
{
    public async Task<Result<PagedResult<AmcPlanSummaryDto>>> Handle(
        ListPlansQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcPlanAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var pageNumber = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize;
        var status = AmcMapper.ParsePlanStatus(request.Status);

        var result = await repository.GetPagedAsync(pageNumber, pageSize, status, request.Search, cancellationToken);
        var items = result.Items.Select(AmcMapper.ToPlanSummary).ToList();

        return new PagedResult<AmcPlanSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
