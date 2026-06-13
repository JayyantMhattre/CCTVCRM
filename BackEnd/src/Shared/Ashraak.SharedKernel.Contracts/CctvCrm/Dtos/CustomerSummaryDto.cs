namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Customer list row (GET /cctv/customers).</summary>
public sealed record CustomerSummaryDto(
    Guid Id,
    string CustomerNumber,
    string Name,
    string Email,
    string Phone,
    string City,
    string Status,
    Guid? SourceLeadId,
    DateTime CreatedAtUtc);
