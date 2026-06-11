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
using Microsoft.Extensions.Options;

namespace Ashraak.Webhooks.Application.Commands.UpdateWebhookSubscription;

internal sealed class UpdateWebhookSubscriptionCommandHandler(
    IWebhookSubscriptionStore store,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IOptions<WebhookOptions> webhookOptions,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateWebhookSubscriptionCommand, Result<WebhookSubscriptionContract>>
{
    public async Task<Result<WebhookSubscriptionContract>> Handle(
        UpdateWebhookSubscriptionCommand request,
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

        if (string.IsNullOrWhiteSpace(request.Name))
            return Error.Validation("Webhooks.NameRequired", "Subscription name is required.");

        var endpointError = WebhookEndpointValidator.ValidateUrl(
            request.EndpointUrl,
            requireHttps: webhookOptions.Value.RequireHttpsEndpoints);
        if (endpointError is not null)
            return endpointError;

        var subscriptionId = WebhookSubscriptionId.From(request.SubscriptionId);
        if (await store.ExistsByNameExcludingIdAsync(request.TenantId, request.Name.Trim(), subscriptionId, cancellationToken))
            return Error.Conflict("Webhooks.NameExists", "A subscription with this name already exists for the tenant.");

        subscription.Update(
            request.Name,
            request.EndpointUrl,
            request.Enabled,
            request.UserId,
            request.SubscribedEventNames);
        store.Update(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return WebhookSubscriptionMapper.ToContract(subscription);
    }
}
