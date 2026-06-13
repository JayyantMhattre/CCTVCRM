using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.ChangeCustomerStatus;

public sealed record ChangeCustomerStatusCommand(
    Guid TenantId,
    Guid UserId,
    Guid CustomerId,
    string Status,
    uint RowVersion) : IRequest<Result<CustomerDetailDto>>;
