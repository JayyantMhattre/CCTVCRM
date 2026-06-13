namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Ticket attachment metadata.</summary>
public sealed record TicketAttachmentDto(
    Guid Id,
    Guid FileId,
    string? Title,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
