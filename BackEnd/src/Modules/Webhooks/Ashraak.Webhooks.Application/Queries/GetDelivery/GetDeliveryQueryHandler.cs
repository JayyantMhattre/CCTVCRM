using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Queries.GetDelivery;

internal sealed class GetDeliveryQueryHandler(
    IWebhookDeliveryRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetDeliveryQuery, Result<WebhookDeliveryContract>>
{
    public async Task<Result<WebhookDeliveryContract>> Handle(
        GetDeliveryQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(WebhookFeatureFlags.Enabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Webhooks.Disabled", "Webhooks are not enabled for this tenant.");

        var authError = await WebhookAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var delivery = await repository.GetByIdAsync(request.TenantId, request.DeliveryId, cancellationToken);
        return delivery is null
            ? Error.NotFound("Webhooks.DeliveryNotFound", "Webhook delivery was not found.")
            : delivery;
    }
}
