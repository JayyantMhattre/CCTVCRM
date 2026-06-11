using Ashraak.SharedKernel.Domain.Primitives;
using Ashraak.Tenant.Domain.Enums;

namespace Ashraak.Tenant.Domain.Aggregates.Tenant.ValueObjects;

/// <summary>
/// Value object encapsulating the resource limits and billing metadata
/// associated with a tenant's subscription plan.
/// Recreated atomically whenever the plan changes via <c>Tenant.ChangePlan</c>.
/// </summary>
/// <remarks>
/// Limits are deliberately baked into the domain model rather than fetched from an
/// external service to keep plan evaluation fast and offline-capable.
/// When plans are updated in production, a migration of existing subscriptions
/// is required via a domain event handler.
/// </remarks>
public sealed class Subscription : ValueObject
{
    private Subscription() { }

    /// <summary>Gets the subscription tier this object represents.</summary>
    public TenantPlan Plan { get; private init; }

    /// <summary>
    /// Gets the maximum number of user seats allowed under this plan.
    /// <see cref="int.MaxValue"/> represents unlimited seats (Enterprise).
    /// </summary>
    public int SeatLimit { get; private init; }

    /// <summary>Gets the storage quota in gigabytes allocated to this plan.</summary>
    public int StorageLimitGb { get; private init; }

    /// <summary>
    /// Gets the optional UTC expiry date of the subscription.
    /// <see langword="null"/> for perpetual or self-serve subscriptions.
    /// </summary>
    public DateTime? ExpiresOnUtc { get; private init; }

    /// <summary>
    /// Creates a <see cref="Subscription"/> with the default limits for the given <paramref name="plan"/>.
    /// </summary>
    /// <param name="plan">The subscription tier to create limits for.</param>
    /// <returns>A new, immutable <see cref="Subscription"/> value object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for unknown plan values.</exception>
    public static Subscription Create(TenantPlan plan) => plan switch
    {
        TenantPlan.Free => new Subscription { Plan = plan, SeatLimit = 5, StorageLimitGb = 1 },
        TenantPlan.Pro => new Subscription { Plan = plan, SeatLimit = 50, StorageLimitGb = 50 },
        TenantPlan.Enterprise => new Subscription { Plan = plan, SeatLimit = int.MaxValue, StorageLimitGb = 1000 },
        _ => throw new ArgumentOutOfRangeException(nameof(plan))
    };

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="currentCount"/> is below the seat limit.
    /// Used by the Users module to enforce plan limits during user invitation.
    /// </summary>
    /// <param name="currentCount">The current number of active seats used by the tenant.</param>
    public bool HasAvailableSeats(int currentCount) => currentCount < SeatLimit;

    /// <inheritdoc/>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Plan;
        yield return SeatLimit;
        yield return StorageLimitGb;
        yield return ExpiresOnUtc;
    }
}
