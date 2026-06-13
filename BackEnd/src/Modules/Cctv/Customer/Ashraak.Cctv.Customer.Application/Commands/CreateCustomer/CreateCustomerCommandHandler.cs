using Ashraak.Cctv.Customer.Application.Abstractions;
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
using CustomerAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Customer.Customer;

namespace Ashraak.Cctv.Customer.Application.Commands.CreateCustomer;

internal sealed class CreateCustomerCommandHandler(
    ICustomerRepository repository,
    ICustomerNumberGenerator numberGenerator,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCustomerCommand, Result<CustomerDetailDto>>
{
    public async Task<Result<CustomerDetailDto>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.CustomersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Customers.Disabled", "Customer management is not enabled for this tenant.");

        var authError = await CustomerAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var customerNumber = await numberGenerator.GenerateNextAsync(cancellationToken);
        var customer = CustomerAggregate.CreateManual(
            CustomerId.New(),
            customerNumber,
            request.Name,
            request.Email,
            request.Phone,
            request.BillingAddress,
            request.City,
            request.UserId);

        repository.Add(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CustomerMapper.ToDetail(customer);
    }
}
