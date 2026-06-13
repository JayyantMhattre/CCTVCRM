using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.GetCustomer;

public sealed record GetCustomerQuery(
    Guid TenantId,
    Guid UserId,
    Guid CustomerId) : IRequest<Result<CustomerDetailDto>>;
