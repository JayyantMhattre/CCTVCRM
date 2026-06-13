using Ashraak.SharedKernel.Results;
using CustomerAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Customer.Customer;

namespace Ashraak.Cctv.Customer.Application;

internal static class CustomerConcurrencyHelper
{
    public static Error? EnsureRowVersion(CustomerAggregate customer, uint expectedRowVersion)
    {
        if (customer.RowVersion != expectedRowVersion)
        {
            return Error.Conflict(
                "Customers.ConcurrencyConflict",
                "The customer was modified by another user. Refresh and try again.");
        }

        return null;
    }
}
