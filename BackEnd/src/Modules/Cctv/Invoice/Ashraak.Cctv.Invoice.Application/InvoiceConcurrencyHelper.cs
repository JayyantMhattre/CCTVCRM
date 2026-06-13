using Ashraak.SharedKernel.Results;

namespace Ashraak.Cctv.Invoice.Application;

internal static class InvoiceConcurrencyHelper
{
    public static Error? EnsureRowVersion(uint expected, uint actual)
    {
        if (expected == actual)
            return null;

        return Error.Conflict(
            "Invoices.ConcurrencyConflict",
            "The record was modified by another user. Refresh and try again.");
    }
}
