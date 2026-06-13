using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Cross-module read-only reporting data provider (implemented in Integration.Infrastructure).</summary>
public interface ICctvReportingDataProvider
{
    Task<ReportTableDto> GetLeadPipelineReportAsync(ReportQueryContext query, CancellationToken cancellationToken = default);

    Task<ReportTableDto> GetCustomerSummaryReportAsync(ReportQueryContext query, CancellationToken cancellationToken = default);

    Task<ReportTableDto> GetAmcExpiryReportAsync(ReportQueryContext query, CancellationToken cancellationToken = default);

    Task<ReportTableDto> GetVisitReportAsync(ReportQueryContext query, CancellationToken cancellationToken = default);

    Task<ReportTableDto> GetEngineerPerformanceReportAsync(ReportQueryContext query, CancellationToken cancellationToken = default);

    Task<ReportTableDto> GetTicketReportAsync(ReportQueryContext query, CancellationToken cancellationToken = default);

    Task<ReportTableDto> GetInvoiceReportAsync(ReportQueryContext query, CancellationToken cancellationToken = default);

    Task<ReportTableDto> GetRevenueReportAsync(ReportQueryContext query, CancellationToken cancellationToken = default);

    Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken cancellationToken = default);
}
