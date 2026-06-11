using Ashraak.BuildingBlocks.Infrastructure.Outbox;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Domain.Aggregates.WebhookEventDefinition;
using Ashraak.Webhooks.Domain.Repositories;
using Ashraak.Webhooks.Infrastructure.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ashraak.Webhooks.Tests.Infrastructure;

public sealed class WebhookPublisherTests
{
    [Fact]
    public async Task PublishAsync_EnqueuesOutboxEvents()
    {
        var definitions = Substitute.For<IWebhookEventDefinitionStore>();
        var featureFlags = Substitute.For<IFeatureFlagService>();
        var outbox = Substitute.For<IOutboxWriter>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        featureFlags.IsEnabledAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(true);
        definitions.GetByEventNameAsync("user.created", Arg.Any<CancellationToken>())
            .Returns(WebhookEventDefinition.Seed("user.created", "test"));

        var publisher = new WebhookPublisher(definitions, featureFlags, outbox, unitOfWork);
        var tenantId = Guid.NewGuid();

        await publisher.PublishAsync(
            new WebhookEventContract(tenantId, "user.created", "v1", """{"id":"1"}"""),
            CancellationToken.None);

        outbox.Received(2).Enqueue(Arg.Any<SharedKernel.Domain.Events.IDomainEvent>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PublishAsync_WhenEventNotRegistered_Throws()
    {
        var definitions = Substitute.For<IWebhookEventDefinitionStore>();
        var featureFlags = Substitute.For<IFeatureFlagService>();
        var outbox = Substitute.For<IOutboxWriter>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        featureFlags.IsEnabledAsync(Arg.Any<string>(), Arg.Any<Guid?>(), Arg.Any<CancellationToken>())
            .Returns(true);
        definitions.GetByEventNameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((WebhookEventDefinition?)null);

        var publisher = new WebhookPublisher(definitions, featureFlags, outbox, unitOfWork);

        var act = () => publisher.PublishAsync(
            new WebhookEventContract(Guid.NewGuid(), "unknown.event", "v1", "{}"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
