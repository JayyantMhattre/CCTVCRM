namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Log a pipeline activity (POST /cctv/leads/{id}/activities).</summary>
public sealed record CreateLeadActivityRequest(
    string ActivityType,
    string Description,
    string? FromStatus,
    string? ToStatus);
