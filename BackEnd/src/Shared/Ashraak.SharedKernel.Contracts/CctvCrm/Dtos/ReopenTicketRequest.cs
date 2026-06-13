namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/tickets/{id}/reopen request body.</summary>
public sealed record ReopenTicketRequest(
    string Reason,
    uint RowVersion);
