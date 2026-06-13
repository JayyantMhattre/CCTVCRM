using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.Cctv.Ticket.Domain.Enums;
using Ashraak.Cctv.Ticket.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using TicketAggregate = Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket.Ticket;

namespace Ashraak.Cctv.Ticket.Infrastructure.Persistence.Repositories;

internal sealed class TicketRepository(TicketDbContext db) : ITicketRepository
{
    private IQueryable<TicketAggregate> QueryWithChildren() =>
        db.Tickets
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .Include(t => t.Assignments)
            .Include(t => t.StatusHistory);

    public Task<TicketAggregate?> GetByIdAsync(TicketId id, CancellationToken cancellationToken) =>
        QueryWithChildren().FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);

    public async Task<TicketListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        TicketStatus? status,
        TicketPriority? priority,
        Guid? customerId,
        Guid? siteId,
        Guid? engineerId,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = QueryWithChildren().Where(t => !t.IsDeleted);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);
        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);
        if (customerId.HasValue)
            query = query.Where(t => t.CustomerId == customerId.Value);
        if (siteId.HasValue)
            query = query.Where(t => t.SiteId == siteId.Value);
        if (engineerId.HasValue)
            query = query.Where(t => t.Assignments.Any(a => a.IsActive && a.EngineerId == engineerId.Value));
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(t => t.TicketNumber.Contains(term) || t.Subject.Contains(term));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(t => t.CreatedAtUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new TicketListResult(items, totalCount);
    }

    public Task<TicketListResult> GetForCustomerAsync(
        Guid customerId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken) =>
        GetPagedAsync(pageNumber, pageSize, null, null, customerId, null, null, null, cancellationToken);

    public Task<TicketListResult> GetForEngineerAsync(
        Guid engineerId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken) =>
        GetPagedAsync(pageNumber, pageSize, null, null, null, null, engineerId, null, cancellationToken);

    public async Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken)
    {
        var prefix = $"TK-{year}-";
        var numbers = await db.Tickets
            .Where(t => t.TicketNumber.StartsWith(prefix))
            .Select(t => t.TicketNumber)
            .ToListAsync(cancellationToken);

        if (numbers.Count == 0)
            return 0;

        return numbers
            .Select(n => int.TryParse(n.AsSpan(prefix.Length), out var seq) ? seq : 0)
            .DefaultIfEmpty(0)
            .Max();
    }

    public void Add(TicketAggregate ticket) => db.Tickets.Add(ticket);
}
