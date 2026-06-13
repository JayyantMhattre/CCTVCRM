namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/tickets/{id}/comments request body.</summary>
public sealed record CreateTicketCommentRequest(
    string Text);
