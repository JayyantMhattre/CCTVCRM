using Ashraak.Cctv.Reporting.Application.Queries.GetAdminDashboard;
using Ashraak.Cctv.Reporting.Application.Queries.GetCctvReport;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Text;

namespace Ashraak.Cctv.Reporting.Api.Endpoints;

/// <summary>Reporting endpoints (D1-11) under <c>/api/v1/cctv</c>.</summary>
public static class CctvReportingEndpoints
{
    public static IEndpointRouteBuilder MapCctvReportingEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/cctv/reports")
            .WithTags("CCTV — Reporting")
            .RequireAuthorization();

        group.MapGet("/leads", GetLeadReport).WithName("GetCctvLeadPipelineReport");
        group.MapGet("/customers", GetCustomerReport).WithName("GetCctvCustomerSummaryReport");
        group.MapGet("/amc", GetAmcReport).WithName("GetCctvAmcExpiryReport");
        group.MapGet("/visits", GetVisitReport).WithName("GetCctvVisitReport");
        group.MapGet("/engineers", GetEngineerReport).WithName("GetCctvEngineerPerformanceReport");
        group.MapGet("/tickets", GetTicketReport).WithName("GetCctvTicketReport");
        group.MapGet("/invoices", GetInvoiceReport).WithName("GetCctvInvoiceReport");
        group.MapGet("/revenue", GetRevenueReport).WithName("GetCctvRevenueReport");
        group.MapGet("/{reportKey}/export", ExportReportCsv).WithName("ExportCctvReportCsv");

        routeBuilder.MapGet("/cctv/admin/dashboard", GetAdminDashboard)
            .WithTags("CCTV — Reporting")
            .WithName("GetCctvAdminDashboard")
            .RequireAuthorization();

        return routeBuilder;
    }

    private static Task<IResult> GetLeadReport(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? status,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken) =>
        ExecuteReportQuery(currentUser, tenantContext, sender, CctvReportKey.Leads, page, pageSize, null, status, null, search, cancellationToken);

    private static Task<IResult> GetCustomerReport(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? status,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken) =>
        ExecuteReportQuery(currentUser, tenantContext, sender, CctvReportKey.Customers, page, pageSize, null, status, null, search, cancellationToken);

    private static Task<IResult> GetAmcReport(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] int? days,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken) =>
        ExecuteReportQuery(currentUser, tenantContext, sender, CctvReportKey.Amc, page, pageSize, days, null, null, search, cancellationToken);

    private static Task<IResult> GetVisitReport(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] int? days,
        [FromQuery] string? status,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken) =>
        ExecuteReportQuery(currentUser, tenantContext, sender, CctvReportKey.Visits, page, pageSize, days, status, null, search, cancellationToken);

    private static Task<IResult> GetEngineerReport(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? status,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken) =>
        ExecuteReportQuery(currentUser, tenantContext, sender, CctvReportKey.Engineers, page, pageSize, null, status, null, search, cancellationToken);

    private static Task<IResult> GetTicketReport(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken) =>
        ExecuteReportQuery(currentUser, tenantContext, sender, CctvReportKey.Tickets, page, pageSize, null, status, priority, search, cancellationToken);

    private static Task<IResult> GetInvoiceReport(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? status,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken) =>
        ExecuteReportQuery(currentUser, tenantContext, sender, CctvReportKey.Invoices, page, pageSize, null, status, null, search, cancellationToken);

    private static Task<IResult> GetRevenueReport(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken) =>
        ExecuteReportQuery(currentUser, tenantContext, sender, CctvReportKey.Revenue, page, pageSize, null, null, null, search, cancellationToken);

    private static async Task<IResult> ExportReportCsv(
        string reportKey,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] int? days,
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] string? search,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!TryParseReportKey(reportKey, out var key))
            return Results.BadRequest(new { message = "Unknown report key." });

        var query = BuildQueryContext(page, pageSize, days, status, priority, search);
        var result = await sender.Send(
            new GetCctvReportQuery(tenantContext.TenantId, currentUser.UserId, key, query),
            cancellationToken);

        if (!result.IsSuccess)
            return ToErrorResult(result.Error);

        var csv = BuildCsv(result.Value);
        var fileName = $"{result.Value.ReportKey}-{DateTime.UtcNow:yyyyMMdd}.csv";
        return Results.File(Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
    }

    private static async Task<IResult> GetAdminDashboard(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetAdminDashboardQuery(tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> ExecuteReportQuery(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CctvReportKey reportKey,
        int? page,
        int? pageSize,
        int? days,
        string? status,
        string? priority,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = BuildQueryContext(page, pageSize, days, status, priority, search);
        var result = await sender.Send(
            new GetCctvReportQuery(tenantContext.TenantId, currentUser.UserId, reportKey, query),
            cancellationToken);

        return ToResult(result, Results.Ok);
    }

    private static ReportQueryContext BuildQueryContext(
        int? page,
        int? pageSize,
        int? days,
        string? status,
        string? priority,
        string? search) =>
        new(
            Page: page is > 0 ? page.Value : 1,
            PageSize: pageSize is > 0 and <= 100 ? pageSize.Value : 20,
            Days: days is > 0 and <= 365 ? days.Value : 30,
            Status: string.IsNullOrWhiteSpace(status) ? null : status.Trim(),
            Priority: string.IsNullOrWhiteSpace(priority) ? null : priority.Trim(),
            Search: string.IsNullOrWhiteSpace(search) ? null : search.Trim());

    private static bool TryParseReportKey(string reportKey, out CctvReportKey key)
    {
        key = reportKey.ToLowerInvariant() switch
        {
            "leads" => CctvReportKey.Leads,
            "customers" => CctvReportKey.Customers,
            "amc" => CctvReportKey.Amc,
            "visits" => CctvReportKey.Visits,
            "engineers" => CctvReportKey.Engineers,
            "tickets" => CctvReportKey.Tickets,
            "invoices" => CctvReportKey.Invoices,
            "revenue" => CctvReportKey.Revenue,
            _ => default
        };

        return reportKey.ToLowerInvariant() is
            "leads" or "customers" or "amc" or "visits" or "engineers" or "tickets" or "invoices" or "revenue";
    }

    private static string BuildCsv(ReportTableDto table)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(',', table.Columns.Select(EscapeCsv)));

        foreach (var row in table.Rows)
        {
            var values = table.Columns.Select(column =>
                row.TryGetValue(column, out var value) ? EscapeCsv(value) : string.Empty);
            sb.AppendLine(string.Join(',', values));
        }

        return sb.ToString();
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";

        return value;
    }

    private static IResult ToResult<T>(Result<T> result, Func<T, IResult> onSuccess)
    {
        if (result.IsSuccess)
            return onSuccess(result.Value);

        return ToErrorResult(result.Error);
    }

    private static IResult ToErrorResult(Error error) =>
        error.Type switch
        {
            ErrorType.NotFound => Results.NotFound(),
            ErrorType.Forbidden or ErrorType.Unauthorized
                => Results.Problem(error.Description, statusCode: StatusCodes.Status403Forbidden),
            ErrorType.Conflict
                => Results.Problem(error.Description, statusCode: StatusCodes.Status409Conflict),
            _ => Results.Problem(error.Description, statusCode: StatusCodes.Status400BadRequest)
        };
}
