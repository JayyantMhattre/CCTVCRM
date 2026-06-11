using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Queries.GetDeliveryHistory;

internal sealed class GetDeliveryHistoryQueryHandler(
    IWebhookDeliveryRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetDeliveryHistoryQuery, Result<IReadOnlyList<WebhookDeliveryContract>>>
{
    public async Task<Result<IReadOnlyList<WebhookDeliveryContract>>> Handle(
        GetDeliveryHistoryQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(WebhookFeatureFlags.Enabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Webhooks.Disabled", "Webhooks are not enabled for this tenant.");

        var authError = await WebhookAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var limit = Math.Clamp(request.Limit, 1, 500);
        var items = await repository.GetHistoryAsync(
            request.TenantId,
            request.SubscriptionId,
            request.EventName,
            request.Status,
            request.FromUtc,
            request.ToUtc,
            limit,
            cancellationToken);

        return items.ToList();
    }
}
