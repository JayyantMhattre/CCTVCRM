using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.ListRenewalRequests;

internal sealed class ListRenewalRequestsQueryHandler(
    IAmcContractRepository contractRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ListRenewalRequestsQuery, Result<PagedResult<AmcRenewalRequestSummaryDto>>>
{
    public async Task<Result<PagedResult<AmcRenewalRequestSummaryDto>>> Handle(
        ListRenewalRequestsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcContractAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var pageNumber = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize;

        var result = await contractRepository.GetRenewalRequestsAsync(pageNumber, pageSize, cancellationToken);
        var items = result.Items.Select(x => AmcMapper.ToRenewalRequest(x.Contract, x.Term)).ToList();

        return new PagedResult<AmcRenewalRequestSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
