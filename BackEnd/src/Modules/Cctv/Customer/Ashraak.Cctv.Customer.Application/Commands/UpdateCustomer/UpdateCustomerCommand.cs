using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(
    Guid TenantId,
    Guid UserId,
    Guid CustomerId,
    string Name,
    string Email,
    string Phone,
    string BillingAddress,
    string City,
    uint RowVersion) : IRequest<Result<CustomerDetailDto>>;
