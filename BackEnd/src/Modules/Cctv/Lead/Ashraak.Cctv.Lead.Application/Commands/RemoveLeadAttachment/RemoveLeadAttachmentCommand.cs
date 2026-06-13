using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.RemoveLeadAttachment;

public sealed record RemoveLeadAttachmentCommand(
    Guid TenantId,
    Guid UserId,
    Guid LeadId,
    Guid AttachmentId) : IRequest<Result>;
