using Ashraak.SharedKernel.Contracts.Users.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Users.Application.Commands.UpdateUserPreferences;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Users.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/users")
            .WithTags("Users")
            .RequireAuthorization();

        group.MapGet("/{userId:guid}", GetUser)
            .WithName("GetUser")
            .WithSummary("Returns a user profile by its identifier.");

        group.MapGet("/tenant/{tenantId:guid}", GetUsersForTenant)
            .WithName("GetUsersForTenant")
            .WithSummary("Returns all active user profiles for a tenant.");

        group.MapGet("/tenant/current", GetUsersForCurrentTenant)
            .WithName("GetUsersForCurrentTenant")
            .WithSummary("Returns all user profiles for the tenant resolved from request context.");

        group.MapPatch("/{userId:guid}/preferences", UpdatePreferences)
            .WithName("UpdateUserPreferences")
            .WithSummary("Updates notification and UI preferences for a user.");

        return routeBuilder;
    }

    private static async Task<IResult> GetUser(
        Guid userId,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        var user = await userService.GetUserAsync(userId, cancellationToken);
        return user is null ? Results.NotFound() : Results.Ok(user);
    }

    private static async Task<IResult> GetUsersForTenant(
        Guid tenantId,
        ITenantContext tenantContext,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        if (tenantContext.TenantId != Guid.Empty && tenantContext.TenantId != tenantId)
            return Results.Forbid();

        var users = await userService.GetUsersForTenantAsync(tenantId, cancellationToken);
        return Results.Ok(users);
    }

    private static async Task<IResult> GetUsersForCurrentTenant(
        ITenantContext tenantContext,
        IUserService userService,
        CancellationToken cancellationToken)
    {
        if (tenantContext.TenantId == Guid.Empty)
            return Results.BadRequest("No tenant context was resolved for the current request.");

        var users = await userService.GetUsersForTenantAsync(tenantContext.TenantId, cancellationToken);
        return Results.Ok(users);
    }

    private static async Task<IResult> UpdatePreferences(
        Guid userId,
        [FromBody] UpdatePreferencesRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated)
            return Results.Unauthorized();

        var command = new UpdateUserPreferencesCommand(
            userId,
            currentUser.UserId,
            tenantContext.TenantId,
            request.EmailNotificationsEnabled,
            request.Theme,
            request.Locale,
            request.Timezone);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok(result.Value);
    }
}

public sealed record UpdatePreferencesRequest(
    bool? EmailNotificationsEnabled,
    string? Theme,
    string? Locale,
    string? Timezone);
