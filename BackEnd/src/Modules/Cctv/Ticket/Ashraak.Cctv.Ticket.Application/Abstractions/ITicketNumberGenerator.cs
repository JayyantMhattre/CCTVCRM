namespace Ashraak.Cctv.Ticket.Application.Abstractions;

/// <summary>Generates business numbers in format <c>TK-YYYY-NNNN</c>.</summary>
public interface ITicketNumberGenerator
{
    Task<string> GenerateNextAsync(CancellationToken cancellationToken);
}
