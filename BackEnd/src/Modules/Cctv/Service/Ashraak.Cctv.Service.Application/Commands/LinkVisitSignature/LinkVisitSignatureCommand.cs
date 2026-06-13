using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.LinkVisitSignature;

public sealed record LinkVisitSignatureCommand(
    Guid TenantId,
    Guid UserId,
    Guid VisitId,
    Guid FileId,
    string SignerName,
    DateTime? CapturedAt) : IRequest<Result<VisitDetailDto>>;
