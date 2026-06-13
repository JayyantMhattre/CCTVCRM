using Ashraak.Cctv.Customer.Application.Mapping;
using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.UpdateCustomer;

internal sealed class UpdateCustomerCommandHandler(
    ICustomerRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCustomerCommand, Result<CustomerDetailDto>>
{
    public async Task<Result<CustomerDetailDto>> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.CustomersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Customers.Disabled", "Customer management is not enabled for this tenant.");

        var authError = await CustomerAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var customer = await repository.GetByIdAsync(CustomerId.From(request.CustomerId), cancellationToken);
        if (customer is null)
            return Error.NotFound("Customers.NotFound", "Customer not found.");

        var concurrencyError = CustomerConcurrencyHelper.EnsureRowVersion(customer, request.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        try
        {
            customer.UpdateDetails(
                request.Name,
                request.Email,
                request.Phone,
                request.BillingAddress,
                request.City,
                request.UserId);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Conflict("Customers.InvalidState", ex.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CustomerMapper.ToDetail(customer);
    }
}
