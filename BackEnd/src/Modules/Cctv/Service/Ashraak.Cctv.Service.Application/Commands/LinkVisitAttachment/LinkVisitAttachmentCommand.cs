using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.LinkVisitAttachment;

public sealed record LinkVisitAttachmentCommand(
    Guid TenantId,
    Guid UserId,
    Guid VisitId,
    Guid FileId,
    string AttachmentType,
    string? Title) : IRequest<Result<VisitDetailDto>>;
