namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Input for AMC contract provisioning from a converted lead.</summary>
public sealed record AmcContractProvisioningRequestDto(
    Guid LeadId,
    Guid CustomerId,
    Guid SiteId,
    Guid PlanVersionId,
    DateOnly InitialTermStartDate,
    DateOnly InitialTermEndDate);
