using Ashraak.SharedKernel.Results;
using LeadAggregate = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Lead;

namespace Ashraak.Cctv.Lead.Application;

internal static class LeadConcurrencyHelper
{
    public static Error? EnsureRowVersion(LeadAggregate lead, uint expectedRowVersion)
    {
        if (lead.RowVersion != expectedRowVersion)
        {
            return Error.Conflict(
                "Leads.ConcurrencyConflict",
                "The lead was modified by another user. Refresh and try again.");
        }

        return null;
    }
}
