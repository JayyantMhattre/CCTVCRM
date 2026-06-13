using Ashraak.Cctv.Customer.Application.Abstractions;
using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Interfaces;

namespace Ashraak.Cctv.Customer.Infrastructure.Services;

/// <summary>Real customer provisioning from lead conversion (replaces Integration stub).</summary>
internal sealed class CustomerProvisioningService(
    ICustomerRepository repository,
    ICustomerNumberGenerator numberGenerator,
    IUnitOfWork unitOfWork) : ICustomerProvisioningService
{
    public async Task<CustomerProvisioningResultDto> ProvisionFromLeadAsync(
        CustomerProvisioningRequestDto request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var name = string.IsNullOrWhiteSpace(request.OrganizationName)
            ? request.ContactName.Trim()
            : request.OrganizationName.Trim();

        var billingAddress = string.IsNullOrWhiteSpace(request.Address)
            ? request.City.Trim()
            : request.Address.Trim();

        var customerNumber = await numberGenerator.GenerateNextAsync(cancellationToken);
        var customer = Domain.Aggregates.Customer.Customer.CreateFromLead(
            CustomerId.New(),
            customerNumber,
            name,
            request.Email.Trim(),
            request.Phone.Trim(),
            billingAddress,
            request.City.Trim(),
            request.LeadId,
            Guid.Empty);

        repository.Add(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CustomerProvisioningResultDto(customer.Id.Value);
    }
}
