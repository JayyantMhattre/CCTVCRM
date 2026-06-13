using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    Guid TenantId,
    Guid UserId,
    string Name,
    string Email,
    string Phone,
    string BillingAddress,
    string City) : IRequest<Result<CustomerDetailDto>>;
