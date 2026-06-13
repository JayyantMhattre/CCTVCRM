using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetVisit;

public sealed record GetVisitQuery(
    Guid TenantId,
    Guid UserId,
    Guid VisitId) : IRequest<Result<VisitDetailDto>>;
