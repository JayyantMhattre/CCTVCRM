using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Tenant.Application.Commands.ProvisionTenant;
using Ashraak.Tenant.Application.Commands.UpdateTenantSettings;
using Ashraak.Tenant.Application.Queries.GetTenant;
using Ashraak.Tenant.Application.Queries.GetTenantSettings;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Tenant.Api.Endpoints;

/// <summary>
/// Minimal API endpoints for the Tenant module.
/// Grouped under <c>/tenants</c> (relative to the host's versioned prefix,
/// resolving to e.g. <c>/api/v1/tenants</c>).
/// Provisioning is anonymous; all other operations require authentication.
/// </summary>
public static class TenantEndpoints
{
    /// <summary>
    /// Registers all Tenant module endpoints on the supplied <paramref name="routeBuilder"/>.
    /// The host passes its <c>/api/v{version:apiVersion}</c> group here so that
    /// final routes resolve to e.g. <c>GET /api/v1/tenants/{tenantId}</c>.
    /// </summary>
    /// <param name="routeBuilder">
    /// The versioned route group provided by <c>Ashraak.Api.Program</c>.
    /// </param>
    /// <returns>The same <paramref name="routeBuilder"/> for chaining.</returns>
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        // No /api/ prefix — the host's versioned group provides it.
        var group = routeBuilder.MapGroup("/tenants")
            .WithTags("Tenants")
            .RequireAuthorization();

        group.MapPost("/", ProvisionTenant)
            .WithName("ProvisionTenant")
            .WithSummary("Provisions a new tenant workspace.")
            .AllowAnonymous();

        group.MapGet("/{tenantId:guid}", GetTenant)
            .WithName("GetTenant")
            .WithSummary("Returns the public profile of a tenant by its identifier.");

        group.MapGet("/current", GetCurrentTenant)
            .WithName("GetCurrentTenant")
            .WithSummary("Returns the tenant resolved from the current request context.");

        group.MapGet("/current/settings", GetCurrentTenantSettings)
            .WithName("GetCurrentTenantSettings")
            .WithSummary("Returns workspace settings for the current tenant.");

        group.MapPatch("/current/settings", UpdateCurrentTenantSettings)
            .WithName("UpdateCurrentTenantSettings")
            .WithSummary("Updates workspace settings for the current tenant.")
            .RequireAuthorization("TenantAdmin");

        return routeBuilder;
    }

    /// <summary>
    /// <c>POST /api/v1/tenants</c> — Provisions a new tenant workspace.
    /// </summary>
    /// <returns>
    /// <c>201 Created</c> with <c>Location: /api/tenants/{tenantId}</c> on success.
    /// <c>400 Bad Request</c> with ProblemDetails on validation or conflict.
    /// </returns>
    private static async Task<IResult> ProvisionTenant(
        [FromBody] ProvisionTenantRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ProvisionTenantCommand(
            request.Name,
            request.Slug,
            request.Plan,
            request.OwnerUserId);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            // Location points to the versioned GET endpoint (v1 is the current release).
            : Results.Created($"/api/v1/tenants/{result.Value}", new { TenantId = result.Value });
    }

    /// <summary>
    /// <c>GET /api/v1/tenants/{tenantId}</c> — Returns a tenant's public profile.
    /// </summary>
    /// <returns>
    /// <c>200 OK</c> with <see cref="TenantDto"/> on success.
    /// <c>404 Not Found</c> when the tenant does not exist.
    /// </returns>
    private static async Task<IResult> GetTenant(
        Guid tenantId,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        // Enforce resolver-based tenant scoping for authenticated flows.
        if (tenantContext.TenantId != Guid.Empty && tenantContext.TenantId != tenantId)
            return Results.Forbid();

        var result = await sender.Send(new GetTenantQuery(tenantId), cancellationToken);

        return result.IsFailure
            ? Results.NotFound(result.Error.Description)
            : Results.Ok(result.Value);
    }

    /// <summary>
    /// <c>GET /api/v1/tenants/current</c> — Returns the tenant resolved by tenant middleware/context.
    /// </summary>
    private static async Task<IResult> GetCurrentTenant(
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (tenantContext.TenantId == Guid.Empty)
            return Results.BadRequest("No tenant context was resolved for the current request.");

        var result = await sender.Send(new GetTenantQuery(tenantContext.TenantId), cancellationToken);

        return result.IsFailure
            ? Results.NotFound(result.Error.Description)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetCurrentTenantSettings(
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (tenantContext.TenantId == Guid.Empty)
            return Results.BadRequest("No tenant context was resolved for the current request.");

        var result = await sender.Send(new GetTenantSettingsQuery(tenantContext.TenantId), cancellationToken);
        return result.IsFailure
            ? Results.NotFound(result.Error.Description)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateCurrentTenantSettings(
        [FromBody] UpdateTenantSettingsRequest request,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (tenantContext.TenantId == Guid.Empty)
            return Results.BadRequest("No tenant context was resolved for the current request.");

        var command = new UpdateTenantSettingsCommand(
            tenantContext.TenantId,
            request.Locale,
            request.Timezone,
            request.PasswordMinLength,
            request.RequireMfa,
            request.SessionTimeoutMinutes);

        var result = await sender.Send(command, cancellationToken);
        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok(result.Value);
    }
}

public sealed record UpdateTenantSettingsRequest(
    string Locale,
    string Timezone,
    int PasswordMinLength,
    bool RequireMfa,
    int SessionTimeoutMinutes);

/// <summary>
/// HTTP request body for the <c>POST /api/tenants</c> endpoint.
/// </summary>
/// <param name="Name">Human-readable tenant name.</param>
/// <param name="Slug">URL-safe slug (lowercase alphanumeric + hyphens).</param>
/// <param name="Plan">The initial subscription tier.</param>
/// <param name="OwnerUserId">The account owner's <c>AuthUser</c> identifier.</param>
public sealed record ProvisionTenantRequest(
    string Name,
    string Slug,
    TenantPlan Plan,
    Guid OwnerUserId);
