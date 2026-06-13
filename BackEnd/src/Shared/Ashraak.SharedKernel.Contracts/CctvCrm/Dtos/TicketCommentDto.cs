namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Ticket comment row.</summary>
public sealed record TicketCommentDto(
    Guid Id,
    string Comment,
    string AuthorRole,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
