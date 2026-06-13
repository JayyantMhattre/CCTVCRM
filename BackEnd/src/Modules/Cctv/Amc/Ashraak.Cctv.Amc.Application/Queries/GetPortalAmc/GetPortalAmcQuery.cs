using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.GetPortalAmc;

public sealed record GetPortalAmcQuery(
    Guid TenantId,
    Guid UserId) : IRequest<Result<PortalAmcDto>>;
