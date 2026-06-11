using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;
using Ashraak.Webhooks.Infrastructure.Persistence;
using Ashraak.Webhooks.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Ashraak.Webhooks.Tests.Infrastructure;

public sealed class TenantIsolationTests
{
    [Fact]
    public async Task Repository_QueryFilter_ReturnsOnlyCurrentTenant()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();

        var tenantContext = Substitute.For<ITenantContext>();
        tenantContext.TenantId.Returns(tenantA);

        var options = new DbContextOptionsBuilder<WebhooksDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new WebhooksDbContext(options, tenantContext);

        context.WebhookSubscriptions.Add(WebhookSubscription.Create(
            WebhookSubscriptionId.New(), tenantA, "A", "https://a.test", "s", Guid.NewGuid()));
        context.WebhookSubscriptions.Add(WebhookSubscription.Create(
            WebhookSubscriptionId.New(), tenantB, "B", "https://b.test", "s", Guid.NewGuid()));
        await context.SaveChangesAsync();

        var repo = new WebhookSubscriptionRepository(context);
        var results = await repo.GetByTenantAsync(tenantA);

        results.Should().HaveCount(1);
        results[0].Name.Should().Be("A");
    }
}
