using Ashraak.SharedKernel.Results;
using SiteAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Site.Site;

namespace Ashraak.Cctv.Customer.Application;

internal static class SiteConcurrencyHelper
{
    public static Error? EnsureRowVersion(SiteAggregate site, uint expectedRowVersion)
    {
        if (site.RowVersion != expectedRowVersion)
        {
            return Error.Conflict(
                "Sites.ConcurrencyConflict",
                "The site was modified by another user. Refresh and try again.");
        }

        return null;
    }
}
