using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Customer.Application.Queries.GetPortalProfile;

public sealed record GetPortalProfileQuery(
    Guid TenantId,
    Guid UserId) : IRequest<Result<CustomerDetailDto>>;
