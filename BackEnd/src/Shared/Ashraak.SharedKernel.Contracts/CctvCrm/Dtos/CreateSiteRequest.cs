namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Request body for POST /cctv/sites.</summary>
public sealed record CreateSiteRequest(
    Guid CustomerId,
    string Name,
    string Address,
    string City);
