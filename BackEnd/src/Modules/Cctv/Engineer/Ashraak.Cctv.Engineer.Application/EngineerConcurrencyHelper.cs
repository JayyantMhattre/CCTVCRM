using Ashraak.SharedKernel.Results;
using EngineerAggregate = Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer.Engineer;

namespace Ashraak.Cctv.Engineer.Application;

internal static class EngineerConcurrencyHelper
{
    public static Error? EnsureRowVersion(EngineerAggregate engineer, uint expectedRowVersion)
    {
        if (engineer.RowVersion != expectedRowVersion)
        {
            return Error.Conflict(
                "Engineers.ConcurrencyConflict",
                "The engineer was modified by another user. Refresh and try again.");
        }

        return null;
    }
}
