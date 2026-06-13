namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Lead detail with child counts (GET /cctv/leads/{id}).</summary>
public sealed record LeadDetailDto(
    Guid Id,
    string LeadNumber,
    string Status,
    string Source,
    string ContactName,
    string? OrganizationName,
    string Email,
    string Phone,
    string City,
    string? Address,
    string? RequirementSummary,
    Guid? OwnerUserId,
    Guid? ConvertedCustomerId,
    Guid? ConvertedSiteId,
    Guid? ConvertedContractId,
    DateTime? ConvertedAtUtc,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime? UpdatedAtUtc,
    Guid? UpdatedBy,
    uint RowVersion,
    int ActivityCount,
    int RemarkCount,
    int AttachmentCount);
