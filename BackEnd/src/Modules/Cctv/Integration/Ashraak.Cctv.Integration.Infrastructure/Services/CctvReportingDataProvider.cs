using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.Cctv.Customer.Domain.Enums;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.Cctv.Engineer.Domain.Enums;
using Ashraak.Cctv.Engineer.Domain.Repositories;
using Ashraak.Cctv.Invoice.Domain.Enums;
using Ashraak.Cctv.Invoice.Domain.Repositories;
using Ashraak.Cctv.Lead.Domain.Enums;
using Ashraak.Cctv.Lead.Domain.Repositories;
using Ashraak.Cctv.Service.Domain.Enums;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.Cctv.Ticket.Domain.Enums;
using Ashraak.Cctv.Ticket.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

namespace Ashraak.Cctv.Integration.Infrastructure.Services;

internal sealed class CctvReportingDataProvider(
    ILeadRepository leadRepository,
    ICustomerRepository customerRepository,
    IAmcContractRepository amcContractRepository,
    IServiceScheduleRepository scheduleRepository,
    IServiceVisitRepository visitRepository,
    ITicketRepository ticketRepository,
    IInvoiceRepository invoiceRepository,
    IEngineerRepository engineerRepository) : ICctvReportingDataProvider
{
    public async Task<ReportTableDto> GetLeadPipelineReportAsync(
        ReportQueryContext query,
        CancellationToken cancellationToken = default)
    {
        LeadStatus? status = Enum.TryParse<LeadStatus>(query.Status, true, out var parsed) ? parsed : null;
        var result = await leadRepository.GetPagedAsync(query.Page, query.PageSize, status, query.Search, cancellationToken);
        var rows = result.Items.Select(lead => Row(
            ("LeadId", lead.Id.Value.ToString()),
            ("LeadNumber", lead.LeadNumber),
            ("ContactName", lead.ContactName),
            ("Status", lead.Status.ToString()),
            ("City", lead.City),
            ("CreatedAtUtc", lead.CreatedAtUtc.ToString("O")))).ToList();

        return BuildTable("leads", ["LeadId", "LeadNumber", "ContactName", "Status", "City", "CreatedAtUtc"], rows, result.TotalCount, query);
    }

    public async Task<ReportTableDto> GetCustomerSummaryReportAsync(
        ReportQueryContext query,
        CancellationToken cancellationToken = default)
    {
        CustomerStatus? status = Enum.TryParse<CustomerStatus>(query.Status, true, out var parsed) ? parsed : null;
        var result = await customerRepository.GetPagedAsync(query.Page, query.PageSize, status, query.Search, cancellationToken);
        var rows = result.Items.Select(c => Row(
            ("CustomerId", c.Id.Value.ToString()),
            ("CustomerNumber", c.CustomerNumber),
            ("Name", c.Name),
            ("City", c.City),
            ("Status", c.Status.ToString()),
            ("CreatedAtUtc", c.CreatedAtUtc.ToString("O")))).ToList();

        return BuildTable("customers", ["CustomerId", "CustomerNumber", "Name", "City", "Status", "CreatedAtUtc"], rows, result.TotalCount, query);
    }

    public async Task<ReportTableDto> GetAmcExpiryReportAsync(
        ReportQueryContext query,
        CancellationToken cancellationToken = default)
    {
        var contracts = await amcContractRepository.GetPagedAsync(query.Page, query.PageSize, null, null, null, query.Search, cancellationToken);
        var cutoff = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(query.Days));
        var rows = contracts.Items
            .SelectMany(c => c.Terms.Select(term => new { Contract = c, Term = term }))
            .Where(x => x.Term.EndDate <= cutoff)
            .Select(x => Row(
                ("ContractId", x.Contract.Id.Value.ToString()),
                ("ContractNumber", x.Contract.ContractNumber),
                ("SiteId", x.Contract.SiteId.ToString()),
                ("TermEndDate", x.Term.EndDate.ToString("O")),
                ("Status", x.Contract.Status.ToString()))).ToList();

        var summary = PaginationSummary(contracts.TotalCount, query);
        summary["ExpiringWithinDays"] = query.Days.ToString();
        summary["Count"] = rows.Count.ToString();
        return new ReportTableDto("amc", ["ContractId", "ContractNumber", "SiteId", "TermEndDate", "Status"], rows, summary);
    }

    public async Task<ReportTableDto> GetVisitReportAsync(
        ReportQueryContext query,
        CancellationToken cancellationToken = default)
    {
        ScheduleStatus? status = Enum.TryParse<ScheduleStatus>(query.Status, true, out var parsed) ? parsed : null;
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-query.Days));
        var schedules = await scheduleRepository.GetPagedAsync(
            query.Page, query.PageSize, fromDate, null, status, null, null, cancellationToken);
        var rows = new List<IReadOnlyDictionary<string, string>>();
        foreach (var schedule in schedules.Items)
        {
            var visit = await visitRepository.GetByScheduleIdAsync(schedule.Id, cancellationToken);
            rows.Add(Row(
                ("ScheduleId", schedule.Id.Value.ToString()),
                ("VisitId", visit?.Id.Value.ToString() ?? string.Empty),
                ("ScheduleNumber", schedule.ScheduleNumber),
                ("ScheduledDate", schedule.ScheduledDate.ToString("O")),
                ("ScheduleStatus", schedule.Status.ToString()),
                ("VisitSubmitted", visit is not null && visit.ReportStatus != VisitReportStatus.Draft ? "Yes" : "No"),
                ("VisitApproved", visit is not null && visit.IsCustomerVisible ? "Yes" : "No")));
        }

        return BuildTable(
            "visits",
            ["ScheduleId", "VisitId", "ScheduleNumber", "ScheduledDate", "ScheduleStatus", "VisitSubmitted", "VisitApproved"],
            rows,
            schedules.TotalCount,
            query);
    }

    public async Task<ReportTableDto> GetEngineerPerformanceReportAsync(
        ReportQueryContext query,
        CancellationToken cancellationToken = default)
    {
        EngineerStatus? status = Enum.TryParse<EngineerStatus>(query.Status, true, out var parsed) ? parsed : null;
        var engineers = await engineerRepository.GetPagedAsync(query.Page, query.PageSize, status, query.Search, cancellationToken);
        var rows = new List<IReadOnlyDictionary<string, string>>();
        foreach (var engineer in engineers.Items)
        {
            var tickets = await ticketRepository.GetForEngineerAsync(engineer.Id.Value, 1, 100, cancellationToken);
            var schedules = await scheduleRepository.GetForEngineerAsync(engineer.Id.Value, null, null, cancellationToken);
            rows.Add(Row(
                ("EngineerId", engineer.Id.Value.ToString()),
                ("EngineerNumber", engineer.EngineerNumber),
                ("Name", engineer.Name),
                ("Status", engineer.Status.ToString()),
                ("AssignedTickets", tickets.TotalCount.ToString()),
                ("AssignedSchedules", schedules.Count.ToString())));
        }

        return BuildTable(
            "engineers",
            ["EngineerId", "EngineerNumber", "Name", "Status", "AssignedTickets", "AssignedSchedules"],
            rows,
            engineers.TotalCount,
            query);
    }

    public async Task<ReportTableDto> GetTicketReportAsync(
        ReportQueryContext query,
        CancellationToken cancellationToken = default)
    {
        TicketStatus? status = Enum.TryParse<TicketStatus>(query.Status, true, out var statusParsed) ? statusParsed : null;
        TicketPriority? priority = Enum.TryParse<TicketPriority>(query.Priority, true, out var priorityParsed) ? priorityParsed : null;
        var result = await ticketRepository.GetPagedAsync(
            query.Page, query.PageSize, status, priority, null, null, null, query.Search, cancellationToken);
        var rows = result.Items.Select(t => Row(
            ("TicketId", t.Id.Value.ToString()),
            ("TicketNumber", t.TicketNumber),
            ("Subject", t.Subject),
            ("Priority", t.Priority.ToString()),
            ("Status", t.Status.ToString()),
            ("CreatedAtUtc", t.CreatedAtUtc.ToString("O")))).ToList();

        return BuildTable("tickets", ["TicketId", "TicketNumber", "Subject", "Priority", "Status", "CreatedAtUtc"], rows, result.TotalCount, query);
    }

    public async Task<ReportTableDto> GetInvoiceReportAsync(
        ReportQueryContext query,
        CancellationToken cancellationToken = default)
    {
        InvoiceStatus? status = Enum.TryParse<InvoiceStatus>(query.Status, true, out var parsed) ? parsed : null;
        var result = await invoiceRepository.GetPagedAsync(query.Page, query.PageSize, status, null, null, query.Search, cancellationToken);
        var rows = result.Items.Select(i => Row(
            ("InvoiceId", i.Id.Value.ToString()),
            ("InvoiceNumber", i.InvoiceNumber),
            ("InvoiceType", i.InvoiceType.ToString()),
            ("Status", i.Status.ToString()),
            ("TotalAmount", i.TotalAmount.ToString("F2")),
            ("InvoiceDate", i.InvoiceDate.ToString("O")))).ToList();

        return BuildTable(
            "invoices",
            ["InvoiceId", "InvoiceNumber", "InvoiceType", "Status", "TotalAmount", "InvoiceDate"],
            rows,
            result.TotalCount,
            query);
    }

    public async Task<ReportTableDto> GetRevenueReportAsync(
        ReportQueryContext query,
        CancellationToken cancellationToken = default)
    {
        var result = await invoiceRepository.GetPagedAsync(
            query.Page, query.PageSize, InvoiceStatus.Paid, null, null, query.Search, cancellationToken);
        var total = result.Items.Sum(i => i.TotalAmount);
        var rows = result.Items.Select(i => Row(
            ("InvoiceId", i.Id.Value.ToString()),
            ("InvoiceNumber", i.InvoiceNumber),
            ("CustomerId", i.CustomerId.ToString()),
            ("TotalAmount", i.TotalAmount.ToString("F2")),
            ("PaidAtUtc", i.PaidAtUtc?.ToString("O") ?? string.Empty))).ToList();

        var summary = PaginationSummary(result.TotalCount, query);
        summary["PaidTotal"] = total.ToString("F2");
        return new ReportTableDto(
            "revenue",
            ["InvoiceId", "InvoiceNumber", "CustomerId", "TotalAmount", "PaidAtUtc"],
            rows,
            summary);
    }

    public async Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken cancellationToken = default)
    {
        var customers = await customerRepository.GetPagedAsync(1, 1, CustomerStatus.Active, null, cancellationToken);
        var tickets = await ticketRepository.GetPagedAsync(1, 1, TicketStatus.Open, null, null, null, null, null, cancellationToken);
        var pendingVisits = await visitRepository.GetPagedAsync(1, 1, VisitReportStatus.Submitted, null, null, cancellationToken);
        var amc = await GetAmcExpiryReportAsync(new ReportQueryContext(Days: 30), cancellationToken);
        var drafts = await invoiceRepository.GetPagedAsync(1, 1, InvoiceStatus.Draft, null, null, null, cancellationToken);

        return new AdminDashboardDto(
            (int)customers.TotalCount,
            (int)tickets.TotalCount,
            (int)pendingVisits.TotalCount,
            int.Parse(amc.Summary["Count"]),
            (int)drafts.TotalCount);
    }

    private static IReadOnlyDictionary<string, string> Row(params (string Key, string Value)[] pairs)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in pairs)
            map[key] = value;
        return map;
    }

    private static ReportTableDto BuildTable(
        string reportKey,
        IReadOnlyList<string> columns,
        List<IReadOnlyDictionary<string, string>> rows,
        long totalCount,
        ReportQueryContext query) =>
        new(reportKey, columns, rows, PaginationSummary(totalCount, query));

    private static Dictionary<string, string> PaginationSummary(long totalCount, ReportQueryContext query)
    {
        var totalPages = query.PageSize > 0
            ? (int)Math.Ceiling(totalCount / (double)query.PageSize)
            : 1;
        return new Dictionary<string, string>
        {
            ["TotalCount"] = totalCount.ToString(),
            ["Page"] = query.Page.ToString(),
            ["PageSize"] = query.PageSize.ToString(),
            ["TotalPages"] = totalPages.ToString(),
        };
    }
}
