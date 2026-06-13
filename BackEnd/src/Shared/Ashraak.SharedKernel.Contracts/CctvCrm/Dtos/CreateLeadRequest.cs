namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Manual lead creation by admin (POST /cctv/leads).</summary>
public sealed record CreateLeadRequest(
    string ContactName,
    string? OrganizationName,
    string Email,
    string Phone,
    string City,
    string? Address,
    string? RequirementSummary,
    string Source,
    Guid? OwnerUserId);
