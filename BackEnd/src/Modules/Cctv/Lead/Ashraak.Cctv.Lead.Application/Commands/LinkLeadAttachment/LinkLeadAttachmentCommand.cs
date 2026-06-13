using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.LinkLeadAttachment;

public sealed record LinkLeadAttachmentCommand(
    Guid TenantId,
    Guid UserId,
    Guid LeadId,
    Guid FileId,
    string Title) : IRequest<Result<LeadAttachmentDto>>;
