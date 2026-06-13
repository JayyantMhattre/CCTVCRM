using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Queries.GetEngineerVisitDetail;

public sealed record GetEngineerVisitDetailQuery(
    Guid TenantId,
    Guid UserId,
    Guid VisitId) : IRequest<Result<VisitDetailDto>>;
