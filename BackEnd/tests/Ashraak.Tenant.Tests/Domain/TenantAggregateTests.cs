using Ashraak.Tenant.Domain.Aggregates.Tenant.Events;
using Ashraak.Tenant.Domain.Enums;
using FluentAssertions;
using Xunit;

using TenantAggregate = Ashraak.Tenant.Domain.Aggregates.Tenant.Tenant;

namespace Ashraak.Tenant.Tests.Domain;

public sealed class TenantAggregateTests
{
    [Fact]
    public void Create_ShouldRaiseTenantCreatedDomainEvent()
    {
        var ownerUserId = Guid.NewGuid();

        var tenant = TenantAggregate.Create("Acme Corp", "acme-corp", TenantPlan.Free, ownerUserId);

        tenant.DomainEvents.Should().HaveCount(1);
        tenant.DomainEvents[0].Should().BeOfType<TenantCreatedDomainEvent>();

        var evt = (TenantCreatedDomainEvent)tenant.DomainEvents[0];
        evt.Name.Should().Be("Acme Corp");
        evt.Slug.Should().Be("acme-corp");
        evt.Plan.Should().Be(TenantPlan.Free);
        evt.OwnerUserId.Should().Be(ownerUserId);
    }

    [Fact]
    public void Suspend_ShouldChangeStatusAndRaiseEvent()
    {
        var tenant = TenantAggregate.Create("Acme", "acme", TenantPlan.Free, Guid.NewGuid());
        tenant.ClearDomainEvents();

        tenant.Suspend("Non-payment");

        tenant.Status.Should().Be(TenantStatus.Suspended);
        tenant.DomainEvents.Should().HaveCount(1);
        tenant.DomainEvents[0].Should().BeOfType<TenantSuspendedDomainEvent>();
    }

    [Fact]
    public void Suspend_WhenAlreadySuspended_ShouldNotRaiseEvent()
    {
        var tenant = TenantAggregate.Create("Acme", "acme", TenantPlan.Free, Guid.NewGuid());
        tenant.Suspend("Reason 1");
        tenant.ClearDomainEvents();

        tenant.Suspend("Reason 2");

        tenant.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void ChangePlan_ShouldRaisePlanChangedEvent()
    {
        var tenant = TenantAggregate.Create("Acme", "acme", TenantPlan.Free, Guid.NewGuid());
        tenant.ClearDomainEvents();

        tenant.ChangePlan(TenantPlan.Pro);

        tenant.Plan.Should().Be(TenantPlan.Pro);
        tenant.DomainEvents.Should().HaveCount(1);

        var evt = tenant.DomainEvents[0].Should().BeOfType<TenantPlanChangedDomainEvent>().Subject;
        evt.OldPlan.Should().Be(TenantPlan.Free);
        evt.NewPlan.Should().Be(TenantPlan.Pro);
    }

    [Fact]
    public void Subscription_Free_ShouldHave5SeatLimit()
    {
        var tenant = TenantAggregate.Create("Acme", "acme", TenantPlan.Free, Guid.NewGuid());
        tenant.Subscription.SeatLimit.Should().Be(5);
    }

    [Fact]
    public void Subscription_Enterprise_ShouldHaveUnlimitedSeats()
    {
        var tenant = TenantAggregate.Create("Acme", "acme", TenantPlan.Enterprise, Guid.NewGuid());
        tenant.Subscription.SeatLimit.Should().Be(int.MaxValue);
    }
}
