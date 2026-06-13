using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.Cctv.Customer.Application.Mapping;
using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Enums;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.ListSites;

internal sealed class ListSitesQueryHandler(
    ISiteRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ListSitesQuery, Result<PagedResult<SiteSummaryDto>>>
{
    public async Task<Result<PagedResult<SiteSummaryDto>>> Handle(
        ListSitesQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.CustomersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Sites.Disabled", "Site management is not enabled for this tenant.");

        var authError = await SiteAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        SiteStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!SiteMapper.TryParseStatus(request.Status, out var parsedStatus))
                return Error.Validation("Sites.InvalidStatus", "Invalid site status filter.");

            status = parsedStatus;
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, 100);

        CustomerId? customerId = request.CustomerId.HasValue
            ? CustomerId.From(request.CustomerId.Value)
            : null;

        var result = await repository.GetPagedAsync(
            pageNumber,
            pageSize,
            customerId,
            status,
            request.Search,
            cancellationToken);

        var items = result.Items.Select(SiteMapper.ToSummary).ToList();
        return new PagedResult<SiteSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
