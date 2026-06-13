namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>AMC contract detail with full term history (GET /cctv/contracts/{id}).</summary>
public sealed record AmcContractDetailDto(
    Guid Id,
    string ContractNumber,
    Guid SiteId,
    Guid CustomerId,
    Guid? SourceLeadId,
    string Status,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    DateTime? UpdatedAtUtc,
    Guid? UpdatedBy,
    uint RowVersion,
    IReadOnlyList<AmcContractTermSummaryDto> Terms);
