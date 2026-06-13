namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Remark row.</summary>
public sealed record LeadRemarkDto(
    Guid Id,
    Guid LeadId,
    string Remark,
    DateTime CreatedAtUtc,
    Guid CreatedBy);
