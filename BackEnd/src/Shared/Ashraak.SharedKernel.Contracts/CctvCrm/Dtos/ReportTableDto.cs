namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Tabular report payload for admin reporting endpoints.</summary>
public sealed record ReportTableDto(
    string ReportKey,
    IReadOnlyList<string> Columns,
    IReadOnlyList<IReadOnlyDictionary<string, string>> Rows,
    IReadOnlyDictionary<string, string> Summary);

/// <summary>Admin dashboard KPI widgets.</summary>
public sealed record AdminDashboardDto(
    int ActiveCustomers,
    int OpenTickets,
    int PendingVisitApprovals,
    int ExpiringAmcTerms30Days,
    int DraftInvoices);
