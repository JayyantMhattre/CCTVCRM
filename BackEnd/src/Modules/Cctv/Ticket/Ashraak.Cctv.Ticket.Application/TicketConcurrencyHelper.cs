using Ashraak.SharedKernel.Results;

namespace Ashraak.Cctv.Ticket.Application;

internal static class TicketConcurrencyHelper
{
    public static Error? EnsureRowVersion(uint expected, uint actual)
    {
        if (expected == actual)
            return null;

        return Error.Conflict(
            "Tickets.ConcurrencyConflict",
            "The record was modified by another user. Refresh and try again.");
    }
}
