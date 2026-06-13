using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.StartVisit;

public sealed record StartVisitCommand(
    Guid TenantId,
    Guid UserId,
    Guid VisitId,
    DateTime? StartedAt,
    uint RowVersion) : IRequest<Result<VisitDetailDto>>;
