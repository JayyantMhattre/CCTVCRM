using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Application;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Enums;
using Ashraak.Webhooks.Domain.Repositories;
using Ashraak.Webhooks.Infrastructure.Retry;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Ashraak.Webhooks.Tests.Infrastructure;

public sealed class WebhookDeliveryOutcomeHandlerTests
{
    [Fact]
    public async Task HandleFailureAsync_TransientFailure_SchedulesRetry()
    {
        var delivery = CreateDelivery();
        var (handler, deliveryStore, _) = BuildHandler(retryEnabled: true);

        await handler.HandleFailureAsync(delivery, 503, "error", "HTTP 503");

        delivery.Status.Should().Be(WebhookDeliveryStatus.Retrying);
        delivery.RetryCount.Should().Be(1);
        deliveryStore.Received(1).Update(delivery);
    }

    [Fact]
    public async Task HandleFailureAsync_PermanentFailure_MarksFailed()
    {
        var delivery = CreateDelivery();
        var (handler, deliveryStore, _) = BuildHandler(retryEnabled: true);

        await handler.HandleFailureAsync(delivery, 401, null, "HTTP 401");

        delivery.Status.Should().Be(WebhookDeliveryStatus.Failed);
        deliveryStore.Received(1).Update(delivery);
    }

    [Fact]
    public async Task HandleFailureAsync_ExhaustedRetries_MovesToDeadLetter()
    {
        var delivery = CreateDelivery();
        var deadLetterService = Substitute.For<IDeadLetterService>();
        var (handler, _, _) = BuildHandler(
            retryEnabled: true,
            maxRetries: 1,
            deadLetterService: deadLetterService);

        await handler.HandleFailureAsync(delivery, 503, null, "HTTP 503");

        await deadLetterService.Received(1).MoveToDeadLetterAsync(
            delivery,
            503,
            "HTTP 503",
            Arg.Any<CancellationToken>());
    }

    private static WebhookDelivery CreateDelivery() =>
        WebhookDelivery.CreatePending(
            WebhookDeliveryId.New(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "user.created",
            "v1",
            "corr-1",
            "{}");

    private static (
        WebhookDeliveryOutcomeHandler Handler,
        IWebhookDeliveryStore DeliveryStore,
        IUnitOfWork UnitOfWork) BuildHandler(
        bool retryEnabled,
        int maxRetries = 5,
        IDeadLetterService? deadLetterService = null)
    {
        var deliveryStore = Substitute.For<IWebhookDeliveryStore>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        deadLetterService ??= Substitute.For<IDeadLetterService>();
        var retryOptions = new WebhookRetryOptions
        {
            Enabled = retryEnabled,
            MaxRetries = maxRetries,
            RetryDelaysMinutes = [1, 5, 15, 60]
        };

        var handler = new WebhookDeliveryOutcomeHandler(
            new WebhookFailureClassifier(),
            new WebhookRetryBackoffCalculator(Options.Create(retryOptions)),
            deadLetterService,
            deliveryStore,
            unitOfWork,
            Options.Create(retryOptions),
            new WebhookDeliveryMetrics(),
            NullLogger<WebhookDeliveryOutcomeHandler>.Instance);

        return (handler, deliveryStore, unitOfWork);
    }
}
