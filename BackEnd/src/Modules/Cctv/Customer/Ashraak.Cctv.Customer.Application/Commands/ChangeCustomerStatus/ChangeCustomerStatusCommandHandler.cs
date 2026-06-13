using Ashraak.Cctv.Customer.Application.Mapping;
using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Enums;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.ChangeCustomerStatus;

internal sealed class ChangeCustomerStatusCommandHandler(
    ICustomerRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<ChangeCustomerStatusCommand, Result<CustomerDetailDto>>
{
    public async Task<Result<CustomerDetailDto>> Handle(
        ChangeCustomerStatusCommand request,
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

        CustomerStatus toStatus;
        try
        {
            toStatus = CustomerMapper.ParseStatus(request.Status);
        }
        catch (ArgumentException)
        {
            return Error.Validation("Customers.InvalidStatus", "Invalid customer status.");
        }

        try
        {
            customer.ChangeStatus(toStatus, request.UserId);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Conflict("Customers.InvalidTransition", ex.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CustomerMapper.ToDetail(customer);
    }
}
