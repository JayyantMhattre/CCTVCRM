namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Input for customer provisioning from a converted lead.</summary>
public sealed record CustomerProvisioningRequestDto(
    Guid LeadId,
    string ContactName,
    string? OrganizationName,
    string Email,
    string Phone,
    string City,
    string? Address);
