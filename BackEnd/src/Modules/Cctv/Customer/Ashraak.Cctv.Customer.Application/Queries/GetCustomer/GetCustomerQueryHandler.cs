using Ashraak.Cctv.Customer.Application.Mapping;
using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.GetCustomer;

internal sealed class GetCustomerQueryHandler(
    ICustomerRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetCustomerQuery, Result<CustomerDetailDto>>
{
    public async Task<Result<CustomerDetailDto>> Handle(
        GetCustomerQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.CustomersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Customers.Disabled", "Customer management is not enabled for this tenant.");

        var authError = await CustomerAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var customer = await repository.GetByIdAsync(CustomerId.From(request.CustomerId), cancellationToken);
        if (customer is null)
            return Error.NotFound("Customers.NotFound", "Customer not found.");

        return CustomerMapper.ToDetail(customer);
    }
}
