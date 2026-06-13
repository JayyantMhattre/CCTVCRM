using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Cross-module read access to tickets.</summary>
public interface ITicketLookupService
{
    Task<TicketDetailDto?> GetTicketAsync(
        Guid ticketId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TicketSummaryDto>> GetOpenTicketsForSiteAsync(
        Guid siteId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TicketSummaryDto>> GetOpenTicketsForEngineerAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default);
}
