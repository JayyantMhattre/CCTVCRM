using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.Cctv.Customer.Application.Mapping;
using Ashraak.Cctv.Customer.Domain.Enums;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.ListCustomers;

internal sealed class ListCustomersQueryHandler(
    ICustomerRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ListCustomersQuery, Result<PagedResult<CustomerSummaryDto>>>
{
    public async Task<Result<PagedResult<CustomerSummaryDto>>> Handle(
        ListCustomersQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.CustomersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Customers.Disabled", "Customer management is not enabled for this tenant.");

        var authError = await CustomerAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        CustomerStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!CustomerMapper.TryParseStatus(request.Status, out var parsedStatus))
                return Error.Validation("Customers.InvalidStatus", "Invalid customer status filter.");

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

        var items = result.Items.Select(CustomerMapper.ToSummary).ToList();
        return new PagedResult<CustomerSummaryDto>(items, pageNumber, pageSize, result.TotalCount);
    }
}
