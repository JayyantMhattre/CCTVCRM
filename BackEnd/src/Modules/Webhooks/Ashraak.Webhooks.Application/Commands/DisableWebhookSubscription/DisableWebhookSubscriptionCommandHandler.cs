using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using Ashraak.Webhooks.Application.Mapping;
using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;
using Ashraak.Webhooks.Domain.Repositories;
using MediatR;

namespace Ashraak.Webhooks.Application.Commands.DisableWebhookSubscription;

internal sealed class DisableWebhookSubscriptionCommandHandler(
    IWebhookSubscriptionStore store,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<DisableWebhookSubscriptionCommand, Result<WebhookSubscriptionContract>>
{
    public async Task<Result<WebhookSubscriptionContract>> Handle(
        DisableWebhookSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(WebhookFeatureFlags.Enabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Webhooks.Disabled", "Webhooks are not enabled for this tenant.");

        var authError = await WebhookAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var subscription = await store.GetByIdAsync(WebhookSubscriptionId.From(request.SubscriptionId), cancellationToken);
        if (subscription is null || subscription.TenantId != request.TenantId)
            return Error.NotFound("Webhooks.NotFound", "Webhook subscription was not found.");

        subscription.Disable(request.UserId);
        store.Update(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return WebhookSubscriptionMapper.ToContract(subscription);
    }
}
