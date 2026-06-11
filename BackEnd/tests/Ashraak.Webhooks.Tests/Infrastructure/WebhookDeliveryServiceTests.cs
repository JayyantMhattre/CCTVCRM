using System.Net;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Application;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;
using Ashraak.Webhooks.Domain.Enums;
using Ashraak.Webhooks.Domain.Repositories;
using Ashraak.Webhooks.Infrastructure.Delivery;
using Ashraak.Webhooks.Infrastructure.Retry;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Ashraak.Webhooks.Tests.Infrastructure;

public sealed class WebhookDeliveryServiceTests
{
    [Fact]
    public async Task ExecuteAsync_SuccessfulResponse_MarksDeliverySucceeded()
    {
        var handler = new StubHandler(HttpStatusCode.OK, "accepted");
        var services = BuildServices(handler, out var deliveryStore, out var unitOfWork, out _);

        var delivery = WebhookDelivery.CreatePending(
            WebhookDeliveryId.New(),
            services.SubscriptionId,
            services.TenantId,
            "user.created",
            "v1",
            "corr-1",
            """{"eventName":"user.created"}""");

        await services.DeliveryService.ExecuteAsync(delivery, CancellationToken.None);

        delivery.Status.Should().Be(WebhookDeliveryStatus.Succeeded);
        delivery.ResponseCode.Should().Be(200);
        deliveryStore.Received(1).Update(delivery);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        handler.LastRequest!.Headers.GetValues("X-Webhook-Signature").Should().NotBeEmpty();
        handler.LastRequest.Headers.GetValues("X-Correlation-Id").Should().Contain("corr-1");
    }

    [Fact]
    public async Task ExecuteAsync_FailedResponse_InvokesOutcomeHandler()
    {
        var handler = new StubHandler(HttpStatusCode.InternalServerError, "boom");
        var services = BuildServices(handler, out _, out _, out var outcomeHandler);

        var delivery = WebhookDelivery.CreatePending(
            WebhookDeliveryId.New(),
            services.SubscriptionId,
            services.TenantId,
            "user.created",
            "v1",
            null,
            "{}");

        await services.DeliveryService.ExecuteAsync(delivery, CancellationToken.None);

        await outcomeHandler.Received(1).HandleFailureAsync(
            delivery,
            500,
            "boom",
            "HTTP 500",
            Arg.Any<CancellationToken>());
    }

    private static (
        WebhookDeliveryService DeliveryService,
        Guid TenantId,
        Guid SubscriptionId) BuildServices(
        StubHandler handler,
        out IWebhookDeliveryStore deliveryStore,
        out IUnitOfWork unitOfWork,
        out IWebhookDeliveryOutcomeHandler outcomeHandler)
    {
        var tenantId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var secretProtector = Substitute.For<IWebhookSecretProtector>();
        secretProtector.Unprotect(Arg.Any<string>()).Returns("plain-secret");

        var subscription = WebhookSubscription.Create(
            WebhookSubscriptionId.From(subscriptionId),
            tenantId,
            "Hook",
            "https://example.com/hook",
            "protected-secret",
            Guid.NewGuid());

        var subscriptionStore = Substitute.For<IWebhookSubscriptionStore>();
        subscriptionStore.GetByIdAsync(Arg.Any<WebhookSubscriptionId>(), Arg.Any<CancellationToken>())
            .Returns(subscription);

        deliveryStore = Substitute.For<IWebhookDeliveryStore>();
        unitOfWork = Substitute.For<IUnitOfWork>();
        outcomeHandler = Substitute.For<IWebhookDeliveryOutcomeHandler>();

        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient(WebhookDeliveryService.HttpClientName)
            .Returns(new HttpClient(handler));

        var service = new WebhookDeliveryService(
            factory,
            subscriptionStore,
            deliveryStore,
            new WebhookSignatureService(),
            secretProtector,
            outcomeHandler,
            unitOfWork,
            Options.Create(new WebhookDeliveryOptions { TimeoutSeconds = 5 }),
            new WebhookDeliveryMetrics(),
            NullLogger<WebhookDeliveryService>.Instance);

        return (service, tenantId, subscriptionId);
    }

    private sealed class StubHandler(HttpStatusCode statusCode, string body) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body)
            });
        }
    }
}
