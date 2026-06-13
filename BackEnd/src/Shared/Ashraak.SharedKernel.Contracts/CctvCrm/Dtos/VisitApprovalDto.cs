namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Admin review round history.</summary>
public sealed record VisitApprovalDto(
    Guid Id,
    string Decision,
    Guid? ReviewedBy,
    DateTime? ReviewedAtUtc,
    string? ReviewRemarks,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
