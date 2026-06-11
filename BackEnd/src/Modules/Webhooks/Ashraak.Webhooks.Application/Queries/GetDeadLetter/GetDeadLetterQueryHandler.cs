using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Webhooks.Application.Queries.GetDeadLetter;

internal sealed class GetDeadLetterQueryHandler(
    IWebhookDeadLetterRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetDeadLetterQuery, Result<WebhookDeadLetterContract>>
{
    public async Task<Result<WebhookDeadLetterContract>> Handle(
        GetDeadLetterQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(WebhookFeatureFlags.Enabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Webhooks.Disabled", "Webhooks are not enabled for this tenant.");

        var authError = await WebhookAuthorization.EnsureCanReadAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var deadLetter = await repository.GetByIdAsync(request.TenantId, request.DeadLetterId, cancellationToken);
        return deadLetter is null
            ? Error.NotFound("Webhooks.DeadLetterNotFound", "Webhook dead letter was not found.")
            : deadLetter;
    }
}
