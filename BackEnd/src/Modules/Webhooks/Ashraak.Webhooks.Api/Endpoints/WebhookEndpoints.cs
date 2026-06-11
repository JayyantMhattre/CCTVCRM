using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Application.Commands.CreateWebhookSubscription;
using Ashraak.Webhooks.Application.Commands.DisableWebhookSubscription;
using Ashraak.Webhooks.Application.Commands.ReplayDeadLetter;
using Ashraak.Webhooks.Application.Commands.RetryDelivery;
using Ashraak.Webhooks.Application.Commands.RotateWebhookSecret;
using Ashraak.Webhooks.Application.Commands.UpdateWebhookSubscription;
using Ashraak.Webhooks.Application.Queries.GetDeadLetter;
using Ashraak.Webhooks.Application.Queries.GetDeadLetters;
using Ashraak.Webhooks.Application.Queries.GetDelivery;
using Ashraak.Webhooks.Application.Queries.GetDeliveryHistory;
using Ashraak.Webhooks.Application.Queries.GetSubscription;
using Ashraak.Webhooks.Application.Queries.GetSubscriptions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Webhooks.Api.Endpoints;

public static class WebhookEndpoints
{
    public static IEndpointRouteBuilder MapWebhooksEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/webhooks")
            .WithTags("Webhooks")
            .RequireAuthorization("TenantAdmin");

        group.MapGet("/health", () => Results.Ok(new { status = "ok", module = "Webhooks" }))
            .WithName("WebhooksHealth")
            .WithSummary("Webhook module health (foundation).")
            .AllowAnonymous();

        group.MapGet("/subscriptions", GetSubscriptions)
            .WithName("GetWebhookSubscriptions")
            .WithSummary("List webhook subscriptions for the current tenant.");

        group.MapGet("/subscriptions/{id:guid}", GetSubscription)
            .WithName("GetWebhookSubscription")
            .WithSummary("Get a webhook subscription by id.");

        group.MapPost("/subscriptions", CreateSubscription)
            .WithName("CreateWebhookSubscription")
            .WithSummary("Create a tenant webhook subscription.");

        group.MapPut("/subscriptions/{id:guid}", UpdateSubscription)
            .WithName("UpdateWebhookSubscription")
            .WithSummary("Update a webhook subscription.");

        group.MapPost("/subscriptions/{id:guid}/rotate-secret", RotateSecret)
            .WithName("RotateWebhookSecret")
            .WithSummary("Rotate the signing secret (returned once).");

        group.MapPost("/subscriptions/{id:guid}/disable", DisableSubscription)
            .WithName("DisableWebhookSubscription")
            .WithSummary("Disable a webhook subscription.");

        group.MapGet("/deliveries", GetDeliveryHistory)
            .WithName("GetWebhookDeliveryHistory")
            .WithSummary("Read-only delivery history for the current tenant.");

        group.MapGet("/deliveries/{id:guid}", GetDelivery)
            .WithName("GetWebhookDelivery")
            .WithSummary("Read-only delivery detail.");

        group.MapPost("/deliveries/{id:guid}/retry", RetryDelivery)
            .WithName("RetryWebhookDelivery")
            .WithSummary("Manually retry a failed or retrying delivery.");

        group.MapGet("/deadletters", GetDeadLetters)
            .WithName("GetWebhookDeadLetters")
            .WithSummary("Read-only dead letter queue for the current tenant.");

        group.MapGet("/deadletters/{id:guid}", GetDeadLetter)
            .WithName("GetWebhookDeadLetter")
            .WithSummary("Read-only dead letter detail.");

        group.MapPost("/deadletters/{id:guid}/replay", ReplayDeadLetter)
            .WithName("ReplayWebhookDeadLetter")
            .WithSummary("Replay a dead letter as a new delivery attempt.");

        return routeBuilder;
    }

    private static async Task<IResult> GetSubscriptions(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetSubscriptionsQuery(tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, success: Results.Ok);
    }

    private static async Task<IResult> GetSubscription(
        Guid id,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetSubscriptionQuery(id, tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, success: Results.Ok);
    }

    private static async Task<IResult> CreateSubscription(
        [FromBody] CreateWebhookSubscriptionRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateWebhookSubscriptionCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.Name,
                request.EndpointUrl,
                request.SubscribedEventNames),
            cancellationToken);

        return ToResult(
            result,
            success: value => Results.Created($"/api/v1/webhooks/subscriptions/{value.Id}", value));
    }

    private static async Task<IResult> UpdateSubscription(
        Guid id,
        [FromBody] UpdateWebhookSubscriptionRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateWebhookSubscriptionCommand(
                id,
                tenantContext.TenantId,
                currentUser.UserId,
                request.Name,
                request.EndpointUrl,
                request.Enabled),
            cancellationToken);

        return ToResult(result, success: Results.Ok);
    }

    private static async Task<IResult> RotateSecret(
        Guid id,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RotateWebhookSecretCommand(id, tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, success: Results.Ok);
    }

    private static async Task<IResult> GetDeliveryHistory(
        [FromQuery] Guid? subscriptionId,
        [FromQuery] string? eventName,
        [FromQuery] string? status,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] int? limit,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetDeliveryHistoryQuery(
                tenantContext.TenantId,
                currentUser.UserId,
                subscriptionId,
                eventName,
                status,
                fromUtc,
                toUtc,
                limit ?? 100),
            cancellationToken);

        return ToResult(result, success: Results.Ok);
    }

    private static async Task<IResult> GetDelivery(
        Guid id,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetDeliveryQuery(id, tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, success: Results.Ok);
    }

    private static async Task<IResult> RetryDelivery(
        Guid id,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RetryDeliveryCommand(id, tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, success: Results.Ok);
    }

    private static async Task<IResult> GetDeadLetters(
        [FromQuery] Guid? subscriptionId,
        [FromQuery] string? eventName,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] int? limit,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetDeadLettersQuery(
                tenantContext.TenantId,
                currentUser.UserId,
                subscriptionId,
                eventName,
                fromUtc,
                toUtc,
                limit ?? 100),
            cancellationToken);

        return ToResult(result, success: Results.Ok);
    }

    private static async Task<IResult> GetDeadLetter(
        Guid id,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetDeadLetterQuery(id, tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, success: Results.Ok);
    }

    private static async Task<IResult> ReplayDeadLetter(
        Guid id,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ReplayDeadLetterCommand(id, tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(
            result,
            success: value => Results.Created($"/api/v1/webhooks/deliveries/{value.Id}", value));
    }

    private static async Task<IResult> DisableSubscription(
        Guid id,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new DisableWebhookSubscriptionCommand(id, tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return ToResult(result, success: Results.Ok);
    }

    private static IResult ToResult<T>(
        SharedKernel.Results.Result<T> result,
        Func<T, IResult> success)
    {
        if (result.IsSuccess)
            return success(result.Value);

        return result.Error.Type switch
        {
            SharedKernel.Results.ErrorType.NotFound => Results.NotFound(),
            SharedKernel.Results.ErrorType.Forbidden or SharedKernel.Results.ErrorType.Unauthorized
                => Results.Problem(result.Error.Description, statusCode: StatusCodes.Status403Forbidden),
            SharedKernel.Results.ErrorType.Conflict
                => Results.Problem(result.Error.Description, statusCode: StatusCodes.Status409Conflict),
            _ => Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
        };
    }
}

public sealed record CreateWebhookSubscriptionRequest(
    string Name,
    string EndpointUrl,
    IReadOnlyList<string>? SubscribedEventNames = null);

public sealed record UpdateWebhookSubscriptionRequest(
    string Name,
    string EndpointUrl,
    bool Enabled,
    IReadOnlyList<string>? SubscribedEventNames = null);
