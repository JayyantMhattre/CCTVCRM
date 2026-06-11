using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Queries.GetSubscriptions;

internal sealed class GetSubscriptionsQueryHandler(
    IWebhookSubscriptionRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetSubscriptionsQuery, Result<IReadOnlyList<WebhookSubscriptionContract>>>
{
    public async Task<Result<IReadOnlyList<WebhookSubscriptionContract>>> Handle(
        GetSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(WebhookFeatureFlags.Enabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Webhooks.Disabled", "Webhooks are not enabled for this tenant.");

        var authError = await WebhookAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var subscriptions = await repository.GetByTenantAsync(request.TenantId, cancellationToken);
        return subscriptions.ToList();
    }
}
