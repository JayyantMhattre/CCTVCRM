namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Pipeline status transition (POST /cctv/leads/{id}/status).</summary>
public sealed record ChangeLeadStatusRequest(
    string ToStatus,
    string? Notes,
    uint RowVersion);
