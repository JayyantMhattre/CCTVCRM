using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using Ashraak.Webhooks.Application.Mapping;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDelivery;
using Ashraak.Webhooks.Domain.Enums;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Domain.Repositories;
using MediatR;

namespace Ashraak.Webhooks.Application.Commands.RetryDelivery;

internal sealed class RetryDeliveryCommandHandler(
    IWebhookDeliveryStore deliveryStore,
    IWebhookDeliveryQueue deliveryQueue,
    IUnitOfWork unitOfWork,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<RetryDeliveryCommand, Result<WebhookDeliveryContract>>
{
    public async Task<Result<WebhookDeliveryContract>> Handle(
        RetryDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(WebhookFeatureFlags.Enabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Webhooks.Disabled", "Webhooks are not enabled for this tenant.");

        var authError = await WebhookAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var delivery = await deliveryStore.GetByIdForTenantAsync(
            WebhookDeliveryId.From(request.DeliveryId),
            request.TenantId,
            cancellationToken);

        if (delivery is null)
            return Error.NotFound("Webhooks.DeliveryNotFound", "Webhook delivery was not found.");

        if (delivery.Status is not (WebhookDeliveryStatus.Failed or WebhookDeliveryStatus.Retrying))
        {
            return Error.Conflict(
                "Webhooks.DeliveryNotRetryable",
                "Only failed or retrying deliveries can be manually retried.");
        }

        delivery.PrepareManualRetry();
        deliveryStore.Update(delivery);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await deliveryQueue.EnqueueAsync(delivery.Id.Value, cancellationToken);

        return WebhookDeliveryMapper.ToContract(delivery);
    }
}
