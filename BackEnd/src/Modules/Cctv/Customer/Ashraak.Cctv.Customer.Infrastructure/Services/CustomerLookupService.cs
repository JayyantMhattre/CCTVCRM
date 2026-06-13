using Ashraak.Cctv.Customer.Application.Mapping;
using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

namespace Ashraak.Cctv.Customer.Infrastructure.Services;

internal sealed class CustomerLookupService(ICustomerRepository repository) : ICustomerLookupService
{
    public async Task<CustomerSummaryDto?> GetCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(CustomerId.From(customerId), cancellationToken);
        return customer is null ? null : CustomerMapper.ToSummary(customer);
    }

    public async Task<CustomerSummaryDto?> GetCustomerForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByPortalUserIdAsync(userId, cancellationToken);
        return customer is null ? null : CustomerMapper.ToSummary(customer);
    }
}
