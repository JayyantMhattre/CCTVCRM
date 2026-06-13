using Ashraak.Cctv.Ticket.Application.Abstractions;
using Ashraak.Cctv.Ticket.Domain.Repositories;

namespace Ashraak.Cctv.Ticket.Infrastructure.Services;

/// <summary>Generates business numbers in format <c>TK-YYYY-NNNN</c>.</summary>
internal sealed class TicketNumberGenerator(ITicketRepository repository) : ITicketNumberGenerator
{
    public async Task<string> GenerateNextAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var sequence = await repository.GetYearlySequenceAsync(year, cancellationToken);
        return $"TK-{year}-{(sequence + 1):D4}";
    }
}
