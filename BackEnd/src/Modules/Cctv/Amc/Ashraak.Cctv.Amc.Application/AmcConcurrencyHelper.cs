using Ashraak.SharedKernel.Results;

namespace Ashraak.Cctv.Amc.Application;

internal static class AmcConcurrencyHelper
{
    public static Error? EnsureRowVersion(uint expected, uint actual)
    {
        if (expected == actual)
            return null;

        return Error.Conflict(
            "Amc.ConcurrencyConflict",
            "The record was modified by another user. Refresh and try again.");
    }
}
