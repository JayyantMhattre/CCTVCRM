using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Reporting.Application.Queries.GetCctvReport;

internal sealed class GetCctvReportQueryHandler(
    ICctvReportingDataProvider dataProvider,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetCctvReportQuery, Result<ReportTableDto>>
{
    public async Task<Result<ReportTableDto>> Handle(
        GetCctvReportQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ReportingEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Reports.Disabled", "Reporting is not enabled for this tenant.");

        var authError = await ReportingAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var query = NormalizeQuery(request.Query);

        ReportTableDto report = request.ReportKey switch
        {
            CctvReportKey.Leads => await dataProvider.GetLeadPipelineReportAsync(query, cancellationToken),
            CctvReportKey.Customers => await dataProvider.GetCustomerSummaryReportAsync(query, cancellationToken),
            CctvReportKey.Amc => await dataProvider.GetAmcExpiryReportAsync(query, cancellationToken),
            CctvReportKey.Visits => await dataProvider.GetVisitReportAsync(query, cancellationToken),
            CctvReportKey.Engineers => await dataProvider.GetEngineerPerformanceReportAsync(query, cancellationToken),
            CctvReportKey.Tickets => await dataProvider.GetTicketReportAsync(query, cancellationToken),
            CctvReportKey.Invoices => await dataProvider.GetInvoiceReportAsync(query, cancellationToken),
            CctvReportKey.Revenue => await dataProvider.GetRevenueReportAsync(query, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(request.ReportKey), request.ReportKey, "Unknown report key.")
        };

        return report;
    }

    private static ReportQueryContext NormalizeQuery(ReportQueryContext query)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 20 : Math.Min(query.PageSize, 100);
        var days = query.Days < 1 ? 30 : Math.Min(query.Days, 365);
        return query with { Page = page, PageSize = pageSize, Days = days };
    }
}
