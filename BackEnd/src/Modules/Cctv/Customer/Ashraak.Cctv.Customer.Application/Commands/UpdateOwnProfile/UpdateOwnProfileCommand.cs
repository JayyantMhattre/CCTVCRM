using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Commands.UpdateOwnProfile;

public sealed record UpdateOwnProfileCommand(
    Guid TenantId,
    Guid UserId,
    string Name,
    string Phone,
    string Email,
    uint RowVersion) : IRequest<Result<CustomerDetailDto>>;
