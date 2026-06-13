namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Mutable lead fields (PUT /cctv/leads/{id}).</summary>
public sealed record UpdateLeadRequest(
    string ContactName,
    string? OrganizationName,
    string Email,
    string Phone,
    string City,
    string? Address,
    string? RequirementSummary,
    Guid? OwnerUserId,
    uint RowVersion);
