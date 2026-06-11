using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Queries.GetDeadLetters;

internal sealed class GetDeadLettersQueryHandler(
    IWebhookDeadLetterRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetDeadLettersQuery, Result<IReadOnlyList<WebhookDeadLetterContract>>>
{
    public async Task<Result<IReadOnlyList<WebhookDeadLetterContract>>> Handle(
        GetDeadLettersQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(WebhookFeatureFlags.Enabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Webhooks.Disabled", "Webhooks are not enabled for this tenant.");

        var authError = await WebhookAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var limit = Math.Clamp(request.Limit, 1, 500);
        var items = await repository.GetByTenantAsync(
            request.TenantId,
            request.SubscriptionId,
            request.EventName,
            request.FromUtc,
            request.ToUtc,
            limit,
            cancellationToken);

        return items.ToList();
    }
}
