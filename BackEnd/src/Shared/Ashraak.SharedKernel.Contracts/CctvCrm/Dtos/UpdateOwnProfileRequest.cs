namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Customer self-service profile update (PATCH /cctv/portal/profile).</summary>
public sealed record UpdateOwnProfileRequest(
    string Name,
    string Phone,
    string Email,
    uint RowVersion);
