using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.LinkVisitSelfie;

public sealed record LinkVisitSelfieCommand(
    Guid TenantId,
    Guid UserId,
    Guid VisitId,
    Guid FileId,
    DateTime? CapturedAt) : IRequest<Result<VisitDetailDto>>;
