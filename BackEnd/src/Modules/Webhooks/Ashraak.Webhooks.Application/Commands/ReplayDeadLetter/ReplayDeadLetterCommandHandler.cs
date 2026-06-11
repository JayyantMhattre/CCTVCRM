using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Contracts.Webhooks;
using Ashraak.SharedKernel.Contracts.Webhooks.Dtos;
using Ashraak.SharedKernel.Results;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Application.Mapping;
using Ashraak.Webhooks.Domain.Aggregates.WebhookDeadLetter;
using MediatR;

namespace Ashraak.Webhooks.Application.Commands.ReplayDeadLetter;

internal sealed class ReplayDeadLetterCommandHandler(
    IDeadLetterService deadLetterService,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<ReplayDeadLetterCommand, Result<WebhookDeliveryContract>>
{
    public async Task<Result<WebhookDeliveryContract>> Handle(
        ReplayDeadLetterCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(WebhookFeatureFlags.Enabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Webhooks.Disabled", "Webhooks are not enabled for this tenant.");

        var authError = await WebhookAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        try
        {
            var delivery = await deadLetterService.ReplayAsync(
                WebhookDeadLetterId.From(request.DeadLetterId),
                request.TenantId,
                cancellationToken);

            return WebhookDeliveryMapper.ToContract(delivery);
        }
        catch (InvalidOperationException ex)
        {
            return Error.NotFound("Webhooks.DeadLetterNotFound", ex.Message);
        }
    }
}
