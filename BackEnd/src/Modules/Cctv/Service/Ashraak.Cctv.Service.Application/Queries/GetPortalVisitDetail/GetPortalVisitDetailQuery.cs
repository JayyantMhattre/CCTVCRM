using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetPortalVisitDetail;

public sealed record GetPortalVisitDetailQuery(
    Guid TenantId,
    Guid UserId,
    Guid VisitId) : IRequest<Result<VisitDetailDto>>;
