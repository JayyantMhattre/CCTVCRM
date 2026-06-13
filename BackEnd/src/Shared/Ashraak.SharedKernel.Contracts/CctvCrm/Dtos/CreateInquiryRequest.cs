namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Public website inquiry submission (POST /cctv/inquiries).</summary>
public sealed record CreateInquiryRequest(
    string InquiryType,
    string Name,
    string? Organization,
    string Email,
    string Phone,
    string City,
    string? Address,
    string? RequirementSummary,
    string? PreferredPlanCode,
    string? SourcePage);
