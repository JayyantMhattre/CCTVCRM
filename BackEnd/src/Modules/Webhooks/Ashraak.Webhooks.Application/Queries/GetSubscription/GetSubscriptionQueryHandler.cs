using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Queries.GetSubscription;

internal sealed class GetSubscriptionQueryHandler(
    IWebhookSubscriptionRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetSubscriptionQuery, Result<WebhookSubscriptionContract>>
{
    public async Task<Result<WebhookSubscriptionContract>> Handle(
        GetSubscriptionQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(WebhookFeatureFlags.Enabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Webhooks.Disabled", "Webhooks are not enabled for this tenant.");

        var authError = await WebhookAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var subscription = await repository.GetByIdAsync(request.TenantId, request.SubscriptionId, cancellationToken);
        return subscription is null
            ? Error.NotFound("Webhooks.NotFound", "Webhook subscription was not found.")
            : subscription;
    }
}
