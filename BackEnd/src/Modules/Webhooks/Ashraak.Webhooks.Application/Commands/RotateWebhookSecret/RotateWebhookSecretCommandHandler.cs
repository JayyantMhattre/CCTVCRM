using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Application.Commands.CreateWebhookSubscription;
using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;
using Ashraak.Webhooks.Domain.Repositories;
using MediatR;

namespace Ashraak.Webhooks.Application.Commands.RotateWebhookSecret;

internal sealed class RotateWebhookSecretCommandHandler(
    IWebhookSubscriptionStore store,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IWebhookSecretGenerator secretGenerator,
    IWebhookSecretProtector secretProtector,
    IUnitOfWork unitOfWork) : IRequestHandler<RotateWebhookSecretCommand, Result<WebhookSubscriptionSecretContract>>
{
    public async Task<Result<WebhookSubscriptionSecretContract>> Handle(
        RotateWebhookSecretCommand request,
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

        var plaintextSecret = secretGenerator.Generate();
        subscription.RotateSecret(secretProtector.Protect(plaintextSecret), request.UserId);
        store.Update(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CreateWebhookSubscriptionCommandHandler.MapWithSecret(subscription, plaintextSecret);
    }
}
