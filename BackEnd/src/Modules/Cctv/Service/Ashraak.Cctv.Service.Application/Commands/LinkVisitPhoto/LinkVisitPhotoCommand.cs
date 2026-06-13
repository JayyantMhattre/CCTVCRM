using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.LinkVisitPhoto;

public sealed record LinkVisitPhotoCommand(
    Guid TenantId,
    Guid UserId,
    Guid VisitId,
    Guid FileId,
    string Category,
    string? Caption,
    DateTime? CapturedAt) : IRequest<Result<VisitDetailDto>>;
