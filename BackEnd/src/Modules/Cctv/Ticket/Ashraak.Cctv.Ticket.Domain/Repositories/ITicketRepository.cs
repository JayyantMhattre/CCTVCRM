using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.Cctv.Ticket.Domain.Enums;
using TicketAggregate = Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket.Ticket;

namespace Ashraak.Cctv.Ticket.Domain.Repositories;

public sealed record TicketListResult(IReadOnlyList<TicketAggregate> Items, long TotalCount);

public interface ITicketRepository
{
    Task<TicketAggregate?> GetByIdAsync(TicketId id, CancellationToken cancellationToken);

    Task<TicketListResult> GetPagedAsync(
        int pageNumber,
        int pageSize,
        TicketStatus? status,
        TicketPriority? priority,
        Guid? customerId,
        Guid? siteId,
        Guid? engineerId,
        string? search,
        CancellationToken cancellationToken);

    Task<TicketListResult> GetForCustomerAsync(
        Guid customerId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<TicketListResult> GetForEngineerAsync(
        Guid engineerId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<int> GetYearlySequenceAsync(int year, CancellationToken cancellationToken);

    void Add(TicketAggregate ticket);
}
