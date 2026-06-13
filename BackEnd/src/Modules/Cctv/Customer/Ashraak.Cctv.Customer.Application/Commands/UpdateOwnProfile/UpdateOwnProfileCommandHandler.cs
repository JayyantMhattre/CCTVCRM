using Ashraak.Cctv.Customer.Application.Mapping;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.UpdateOwnProfile;

internal sealed class UpdateOwnProfileCommandHandler(
    ICustomerRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateOwnProfileCommand, Result<CustomerDetailDto>>
{
    public async Task<Result<CustomerDetailDto>> Handle(
        UpdateOwnProfileCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.CustomersEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Customers.Disabled", "Customer management is not enabled for this tenant.");

        var authError = await CustomerAuthorization.EnsureCanAccessPortalProfileAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var customer = await repository.GetByPortalUserIdAsync(request.UserId, cancellationToken);
        if (customer is null)
            return Error.NotFound("Customers.PortalProfileNotFound", "No customer profile linked to this user.");

        var concurrencyError = CustomerConcurrencyHelper.EnsureRowVersion(customer, request.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        try
        {
            customer.UpdateOwnProfile(request.Name, request.Phone, request.Email, request.UserId);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Conflict("Customers.InvalidState", ex.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CustomerMapper.ToDetail(customer);
    }
}
