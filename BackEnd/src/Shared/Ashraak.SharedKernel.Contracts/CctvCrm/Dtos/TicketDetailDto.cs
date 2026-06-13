namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Ticket detail with children.</summary>
public sealed record TicketDetailDto(
    Guid Id,
    string TicketNumber,
    Guid CustomerId,
    Guid SiteId,
    Guid? AmcContractId,
    Guid? OriginServiceVisitId,
    string Source,
    string Subject,
    string Description,
    string Priority,
    string Status,
    DateTime? ResolvedAtUtc,
    DateTime? ClosedAtUtc,
    int ReopenCount,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime? UpdatedAtUtc,
    Guid? UpdatedBy,
    uint RowVersion,
    Guid? AssignedEngineerId,
    IReadOnlyList<TicketCommentDto> Comments,
    IReadOnlyList<TicketAttachmentDto> Attachments,
    IReadOnlyList<TicketStatusHistoryDto> StatusHistory);
