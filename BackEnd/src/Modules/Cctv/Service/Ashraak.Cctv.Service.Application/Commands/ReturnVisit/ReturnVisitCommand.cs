using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.ReturnVisit;

public sealed record ReturnVisitCommand(
    Guid TenantId,
    Guid UserId,
    Guid VisitId,
    string ReturnReason) : IRequest<Result<VisitDetailDto>>;
