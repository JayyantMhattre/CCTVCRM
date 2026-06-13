namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Site contact person.</summary>
public sealed record SiteContactDto(
    Guid Id,
    short ContactSlot,
    string Name,
    string? Designation,
    string Phone,
    string? Email,
    bool IsPrimary);
