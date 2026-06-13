namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Convert a Won lead to Customer + Site + AMC contract (POST /cctv/leads/{id}/convert).</summary>
public sealed record ConvertLeadRequest(
    Guid PlanVersionId,
    string SiteName,
    string SiteAddress,
    DateOnly InitialTermStartDate,
    DateOnly InitialTermEndDate,
    uint RowVersion);
