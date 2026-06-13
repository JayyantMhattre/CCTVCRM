using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Queries.GetLeadAttachments;

public sealed record GetLeadAttachmentsQuery(
    Guid TenantId,
    Guid UserId,
    Guid LeadId) : IRequest<Result<IReadOnlyList<LeadAttachmentDto>>>;
