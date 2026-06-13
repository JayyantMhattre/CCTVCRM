namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Customer detail (GET /cctv/customers/{id}).</summary>
public sealed record CustomerDetailDto(
    Guid Id,
    string CustomerNumber,
    string Name,
    string Email,
    string Phone,
    string BillingAddress,
    string City,
    string Status,
    Guid? PortalUserId,
    Guid? SourceLeadId,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime? UpdatedAtUtc,
    Guid? UpdatedBy,
    uint RowVersion);
