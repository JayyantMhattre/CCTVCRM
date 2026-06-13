namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Pipeline list row (GET /cctv/leads).</summary>
public sealed record LeadSummaryDto(
    Guid Id,
    string LeadNumber,
    string Status,
    string Source,
    string ContactName,
    string? OrganizationName,
    string Email,
    string Phone,
    string City,
    Guid? OwnerUserId,
    DateTime CreatedAtUtc);
