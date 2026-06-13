namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/tickets request body.</summary>
public sealed record CreateTicketRequest(
    Guid SiteId,
    string Subject,
    string Description,
    string Priority,
    Guid? ServiceVisitId = null,
    IReadOnlyList<Guid>? AttachmentFileIds = null);
