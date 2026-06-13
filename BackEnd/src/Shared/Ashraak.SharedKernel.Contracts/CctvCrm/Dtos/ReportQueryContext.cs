namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Pagination and filter parameters for admin report queries.</summary>
public sealed record ReportQueryContext(
    int Page = 1,
    int PageSize = 20,
    int Days = 30,
    string? Status = null,
    string? Priority = null,
    string? Search = null);
