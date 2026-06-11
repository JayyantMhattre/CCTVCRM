using Ashraak.ApiKeys.Application.Commands.CreateApiKey;
using Ashraak.ApiKeys.Application.Commands.RevokeApiKey;
using Ashraak.ApiKeys.Application.Commands.RotateApiKey;
using Ashraak.ApiKeys.Application.Commands.UpdateApiKeyScopes;
using Ashraak.ApiKeys.Application.Queries.GetApiKey;
using Ashraak.ApiKeys.Application.Queries.ListApiKeys;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.ApiKeys.Api.Endpoints;

public static class ApiKeyEndpoints
{
    public static IEndpointRouteBuilder MapApiKeysEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/api-keys")
            .WithTags("ApiKeys")
            .RequireAuthorization();

        group.MapGet("/health", () => Results.Ok(new { status = "ok", module = "ApiKeys" }))
            .WithName("ApiKeysHealth")
            .WithSummary("API Keys module health.")
            .AllowAnonymous();

        group.MapGet("/", ListApiKeys)
            .WithName("ListApiKeys")
            .WithSummary("List API keys for the current tenant.");

        group.MapGet("/{id:guid}", GetApiKey)
            .WithName("GetApiKey")
            .WithSummary("Get API key metadata and usage summary.");

        group.MapPost("/", CreateApiKey)
            .WithName("CreateApiKey")
            .WithSummary("Create an API key (plaintext returned once).");

        group.MapPost("/{id:guid}/rotate", RotateApiKey)
            .WithName("RotateApiKey")
            .WithSummary("Rotate an API key (old secret invalidated).");

        group.MapPost("/{id:guid}/revoke", RevokeApiKey)
            .WithName("RevokeApiKey")
            .WithSummary("Immediately revoke an API key.");

        group.MapPut("/{id:guid}/scopes", UpdateScopes)
            .WithName("UpdateApiKeyScopes")
            .WithSummary("Update API key scopes.");

        return routeBuilder;
    }

    private static async Task<IResult> ListApiKeys(
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ListApiKeysQuery(tenantContext.TenantId, currentUser.UserId),
            cancellationToken);
        return ToResult(result, value => Results.Ok(value));
    }

    private static async Task<IResult> GetApiKey(
        Guid id,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetApiKeyQuery(tenantContext.TenantId, currentUser.UserId, id),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static async Task<IResult> CreateApiKey(
        [FromBody] CreateApiKeyRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateApiKeyCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.Name,
                request.Description ?? string.Empty,
                request.Scopes ?? [],
                request.ExpiresOnUtc),
            cancellationToken);

        return ToResult(
            result,
            value => Results.Created($"/api/v1/api-keys/{value.Id}", value));
    }

    private static async Task<IResult> RotateApiKey(
        Guid id,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RotateApiKeyCommand(tenantContext.TenantId, currentUser.UserId, id),
            cancellationToken);
        return ToResult(result, value => Results.Ok(value));
    }

    private static async Task<IResult> RevokeApiKey(
        Guid id,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RevokeApiKeyCommand(tenantContext.TenantId, currentUser.UserId, id),
            cancellationToken);
        return ToVoidResult(result, () => Results.NoContent());
    }

    private static async Task<IResult> UpdateScopes(
        Guid id,
        [FromBody] UpdateApiKeyScopesRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateApiKeyScopesCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                id,
                request.Scopes ?? []),
            cancellationToken);
        return ToResult(result, Results.Ok);
    }

    private static IResult ToResult<T>(Result<T> result, Func<T, IResult> onSuccess)
    {
        if (result.IsSuccess)
            return onSuccess(result.Value);

        return ToErrorResult(result.Error);
    }

    private static IResult ToVoidResult(Result result, Func<IResult> onSuccess)
    {
        if (result.IsSuccess)
            return onSuccess();

        return ToErrorResult(result.Error);
    }

    private static IResult ToErrorResult(Error error) =>
        error.Type switch
        {
            ErrorType.NotFound => Results.NotFound(),
            ErrorType.Forbidden or ErrorType.Unauthorized
                => Results.Problem(error.Description, statusCode: StatusCodes.Status403Forbidden),
            ErrorType.Conflict
                => Results.Problem(error.Description, statusCode: StatusCodes.Status409Conflict),
            _ => Results.Problem(error.Description, statusCode: StatusCodes.Status400BadRequest)
        };
}

public sealed record CreateApiKeyRequest(
    string Name,
    string? Description,
    IReadOnlyList<string>? Scopes,
    DateTime? ExpiresOnUtc);

public sealed record UpdateApiKeyScopesRequest(IReadOnlyList<string>? Scopes);
