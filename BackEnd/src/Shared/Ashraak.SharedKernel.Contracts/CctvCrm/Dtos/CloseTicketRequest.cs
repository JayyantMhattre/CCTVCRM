namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/tickets/{id}/close request body.</summary>
public sealed record CloseTicketRequest(
    uint RowVersion);
