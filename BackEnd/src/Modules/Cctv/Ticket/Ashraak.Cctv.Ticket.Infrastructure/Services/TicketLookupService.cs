using Ashraak.Cctv.Ticket.Application.Mapping;
using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.Cctv.Ticket.Domain.Enums;
using Ashraak.Cctv.Ticket.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

namespace Ashraak.Cctv.Ticket.Infrastructure.Services;

internal sealed class TicketLookupService(ITicketRepository ticketRepository) : ITicketLookupService
{
    private static readonly HashSet<TicketStatus> OpenTicketStatuses =
    [
        TicketStatus.Open,
        TicketStatus.Assigned,
        TicketStatus.InProgress,
        TicketStatus.Reopened
    ];

    public async Task<TicketDetailDto?> GetTicketAsync(
        Guid ticketId,
        CancellationToken cancellationToken = default)
    {
        var ticket = await ticketRepository.GetByIdAsync(TicketId.From(ticketId), cancellationToken);
        return ticket is null ? null : TicketMapper.ToDetail(ticket);
    }

    public async Task<IReadOnlyList<TicketSummaryDto>> GetOpenTicketsForSiteAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        var result = await ticketRepository.GetPagedAsync(
            1, 100, TicketStatus.Open, null, null, siteId, null, null, cancellationToken);

        return result.Items.Select(TicketMapper.ToSummary).ToList();
    }

    public async Task<IReadOnlyList<TicketSummaryDto>> GetOpenTicketsForEngineerAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default)
    {
        var result = await ticketRepository.GetForEngineerAsync(engineerId, 1, 100, cancellationToken);

        return result.Items
            .Where(t => OpenTicketStatuses.Contains(t.Status))
            .Select(TicketMapper.ToSummary)
            .ToList();
    }
}
