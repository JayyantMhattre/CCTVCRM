namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>PATCH /cctv/tickets/{id}/status request body.</summary>
public sealed record UpdateTicketStatusRequest(
    string ToStatus,
    string? Comment,
    uint RowVersion);
