namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/tickets/{id}/assign request body.</summary>
public sealed record AssignTicketRequest(
    Guid EngineerId,
    uint RowVersion);
