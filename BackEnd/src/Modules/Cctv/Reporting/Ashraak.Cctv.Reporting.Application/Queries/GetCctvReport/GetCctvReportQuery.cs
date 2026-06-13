using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Reporting.Application.Queries.GetCctvReport;

public enum CctvReportKey
{
    Leads,
    Customers,
    Amc,
    Visits,
    Engineers,
    Tickets,
    Invoices,
    Revenue
}

public sealed record GetCctvReportQuery(
    Guid TenantId,
    Guid UserId,
    CctvReportKey ReportKey,
    ReportQueryContext Query) : IRequest<Result<ReportTableDto>>;
