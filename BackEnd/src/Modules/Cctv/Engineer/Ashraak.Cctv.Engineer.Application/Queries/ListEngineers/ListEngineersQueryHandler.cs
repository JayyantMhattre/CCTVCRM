using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.Cctv.Engineer.Application.Mapping;
using Ashraak.Cctv.Engineer.Domain.Enums;
using Ashraak.Cctv.Engineer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Engineer.Application.Queries.ListEngineers;

internal sealed class ListEngineersQueryHandler(
    IEngineerRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ListEngineersQuery, Result<PagedResult<EngineerSummaryDto>>>
{
    public async Task<Result<PagedResult<EngineerSummaryDto>>> Handle(
        ListEngineersQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.EngineersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Engineers.Disabled", "Engineer management is not enabled for this tenant.");

        var authError = await EngineerAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        EngineerStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!EngineerMapper.TryParseStatus(request.Status, out var parsedStatus))
                return Error.Validation("Engineers.InvalidStatus", "Invalid engineer status filter.");

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

        var items = result.Items.Select(EngineerMapper.ToSummary).ToList();
        return new PagedResult<EngineerSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
