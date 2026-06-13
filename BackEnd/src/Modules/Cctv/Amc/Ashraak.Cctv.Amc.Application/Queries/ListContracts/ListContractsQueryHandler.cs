using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.ListContracts;

internal sealed class ListContractsQueryHandler(
    IAmcContractRepository contractRepository,
    IAmcPlanRepository planRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ListContractsQuery, Result<PagedResult<AmcContractSummaryDto>>>
{
    public async Task<Result<PagedResult<AmcContractSummaryDto>>> Handle(
        ListContractsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcContractAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var pageNumber = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize;
        var status = AmcMapper.ParseContractStatus(request.Status);

        var result = await contractRepository.GetPagedAsync(
            pageNumber, pageSize, status, request.SiteId, request.CustomerId, request.Search, cancellationToken);

        var items = new List<AmcContractSummaryDto>();
        foreach (var contract in result.Items)
        {
            string? planCode = null;
            var activeTerm = contract.GetActiveTerm();
            if (activeTerm is not null)
            {
                var version = await planRepository.GetVersionByIdAsync(activeTerm.PlanVersionId, cancellationToken);
                if (version is not null)
                {
                    var plan = await planRepository.GetByIdAsync(version.PlanId, cancellationToken);
                    planCode = plan?.PlanCode;
                }
            }

            items.Add(AmcMapper.ToContractSummary(contract, planCode));
        }

        return new PagedResult<AmcContractSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
