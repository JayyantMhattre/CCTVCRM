using Ashraak.SharedKernel.Domain.Primitives;
using Ashraak.Tenant.Domain.Aggregates.Tenant.Events;
using Ashraak.Tenant.Domain.Aggregates.Tenant.ValueObjects;
using Ashraak.Tenant.Domain.Enums;

namespace Ashraak.Tenant.Domain.Aggregates.Tenant;

/// <summary>
/// Core aggregate root of the Tenant module.
/// Represents a single organisational unit (company, team, or workspace) that uses the platform.
/// All user data, settings, and resources are scoped to a tenant.
/// </summary>
/// <remarks>
/// <para>
/// State transitions are guarded by domain methods (e.g. <see cref="Suspend"/>, <see cref="ChangePlan"/>)
/// that raise domain events. The events are serialised into the outbox table and dispatched to
/// the Users and Audit modules via <c>OutboxProcessorBase</c>.
/// </para>
/// <para>
/// The private parameterless constructor is required by EF Core for materialising entities
/// from the database; all business creation must go through <see cref="Create"/>.
/// </para>
/// </remarks>
public sealed class Tenant : AggregateRoot<TenantId>
{
    private Tenant() : base(TenantId.New()) { }

    /// <summary>Gets the human-readable name of the tenant (e.g. <c>"Acme Corporation"</c>).</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Gets the URL-safe slug used in subdomains and API paths (e.g. <c>"acme"</c>).</summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>Gets the tenant's current subscription tier.</summary>
    public TenantPlan Plan { get; private set; }

    /// <summary>Gets the tenant's current lifecycle status.</summary>
    public TenantStatus Status { get; private set; }

    /// <summary>Gets the optional CNAME mapping if the tenant has configured a custom domain.</summary>
    public string? CustomDomain { get; private set; }

    /// <summary>Gets the tenant's configurable settings (locale, timezone, MFA policy, etc.).</summary>
    public TenantSettings Settings { get; private set; } = TenantSettings.Default;

    /// <summary>
    /// Gets the subscription value object that determines seat limits and storage quotas
    /// for the tenant's current <see cref="Plan"/>.
    /// </summary>
    public Subscription Subscription { get; private set; } = null!;

    /// <summary>Gets the UTC timestamp when this tenant was provisioned.</summary>
    public DateTime CreatedOnUtc { get; private set; }

    /// <summary>Gets the UTC timestamp of the most recent state change.</summary>
    public DateTime UpdatedOnUtc { get; private set; }

    /// <summary>
    /// Factory method that provisions a new, active tenant and raises <see cref="TenantCreatedDomainEvent"/>.
    /// </summary>
    /// <param name="name">Display name (max 100 characters).</param>
    /// <param name="slug">URL slug (lowercase alphanumeric + hyphens, max 50 characters).</param>
    /// <param name="plan">Initial subscription tier.</param>
    /// <param name="ownerUserId">The <c>AuthUser</c> identifier of the account owner.</param>
    /// <returns>A new, unpersisted <see cref="Tenant"/> aggregate with a freshly generated <see cref="TenantId"/>.</returns>
    public static Tenant Create(string name, string slug, TenantPlan plan, Guid ownerUserId)
    {
        var tenant = new Tenant
        {
            Name = name,
            Slug = slug,
            Plan = plan,
            Status = TenantStatus.Active,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow,
            Subscription = Subscription.Create(plan)
        };

        tenant.RaiseDomainEvent(new TenantCreatedDomainEvent(
            tenant.Id.Value,
            name,
            slug,
            plan,
            ownerUserId));

        return tenant;
    }

    /// <summary>
    /// Suspends the tenant, preventing its users from logging in.
    /// Raises <see cref="TenantSuspendedDomainEvent"/>. Idempotent if already suspended.
    /// </summary>
    /// <param name="reason">Human-readable reason shown to administrators and logged in Audit.</param>
    public void Suspend(string reason)
    {
        if (Status == TenantStatus.Suspended)
            return;

        Status = TenantStatus.Suspended;
        UpdatedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new TenantSuspendedDomainEvent(Id.Value, reason));
    }

    /// <summary>
    /// Reactivates a suspended tenant, allowing its users to log in again.
    /// Idempotent if already active.
    /// </summary>
    public void Activate()
    {
        if (Status == TenantStatus.Active)
            return;

        Status = TenantStatus.Active;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft-deletes the tenant and raises <see cref="TenantDeletedDomainEvent"/> so that
    /// the Users module can deactivate all associated <c>UserProfile</c> records.
    /// Hard deletion with data purge is handled by a scheduled background job after the retention period.
    /// </summary>
    public void Delete()
    {
        Status = TenantStatus.Deleted;
        UpdatedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new TenantDeletedDomainEvent(Id.Value));
    }

    /// <summary>
    /// Upgrades or downgrades the tenant's subscription plan.
    /// Rebuilds the <see cref="Subscription"/> value object with new limits and raises
    /// <see cref="TenantPlanChangedDomainEvent"/>. Idempotent if the plan has not changed.
    /// </summary>
    /// <param name="newPlan">The target subscription tier.</param>
    public void ChangePlan(TenantPlan newPlan)
    {
        if (Plan == newPlan)
            return;

        var oldPlan = Plan;
        Plan = newPlan;
        Subscription = Subscription.Create(newPlan);
        UpdatedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new TenantPlanChangedDomainEvent(Id.Value, oldPlan, newPlan));
    }

    /// <summary>
    /// Sets or clears the tenant's custom domain (CNAME).
    /// Pass <see langword="null"/> to remove the custom domain mapping.
    /// </summary>
    /// <param name="customDomain">The new custom domain, or <see langword="null"/> to clear it.</param>
    public void SetCustomDomain(string? customDomain)
    {
        CustomDomain = customDomain;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Replaces the tenant's configuration settings with a new <see cref="TenantSettings"/> value object.
    /// </summary>
    /// <param name="settings">The new settings to apply.</param>
    public void UpdateSettings(TenantSettings settings)
    {
        Settings = settings;
        UpdatedOnUtc = DateTime.UtcNow;
    }
}
