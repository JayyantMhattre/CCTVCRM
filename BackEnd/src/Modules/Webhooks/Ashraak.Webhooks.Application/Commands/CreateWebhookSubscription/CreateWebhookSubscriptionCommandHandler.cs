using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Domain.Aggregates.WebhookSubscription;
using Ashraak.Webhooks.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace Ashraak.Webhooks.Application.Commands.CreateWebhookSubscription;

internal sealed class CreateWebhookSubscriptionCommandHandler(
    IWebhookSubscriptionStore store,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IWebhookSecretGenerator secretGenerator,
    IWebhookSecretProtector secretProtector,
    IOptions<WebhookOptions> webhookOptions,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateWebhookSubscriptionCommand, Result<WebhookSubscriptionSecretContract>>
{
    public async Task<Result<WebhookSubscriptionSecretContract>> Handle(
        CreateWebhookSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(WebhookFeatureFlags.Enabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Webhooks.Disabled", "Webhooks are not enabled for this tenant.");

        var authError = await WebhookAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        if (string.IsNullOrWhiteSpace(request.Name))
            return Error.Validation("Webhooks.NameRequired", "Subscription name is required.");

        var endpointError = WebhookEndpointValidator.ValidateUrl(
            request.EndpointUrl,
            requireHttps: webhookOptions.Value.RequireHttpsEndpoints);
        if (endpointError is not null)
            return endpointError;

        if (await store.ExistsByNameAsync(request.TenantId, request.Name.Trim(), cancellationToken))
            return Error.Conflict("Webhooks.NameExists", "A subscription with this name already exists for the tenant.");

        var plaintextSecret = secretGenerator.Generate();
        var subscription = WebhookSubscription.Create(
            WebhookSubscriptionId.New(),
            request.TenantId,
            request.Name,
            request.EndpointUrl,
            secretProtector.Protect(plaintextSecret),
            request.UserId,
            request.SubscribedEventNames);

        store.Add(subscription);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapWithSecret(subscription, plaintextSecret);
    }

    internal static WebhookSubscriptionSecretContract MapWithSecret(
        WebhookSubscription subscription,
        string plaintextSecret) =>
        new(
            subscription.Id.Value,
            subscription.TenantId,
            subscription.Name,
            subscription.EndpointUrl,
            subscription.Enabled,
            plaintextSecret,
            subscription.CreatedBy,
            subscription.CreatedOnUtc,
            subscription.UpdatedOnUtc);
}
