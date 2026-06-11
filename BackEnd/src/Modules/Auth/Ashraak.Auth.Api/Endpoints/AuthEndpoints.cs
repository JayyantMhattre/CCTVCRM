using System.Security.Claims;
using Ashraak.Auth.Application.Abstractions;
using Ashraak.Auth.Application.Commands.Mfa;
using Ashraak.Auth.Application.Commands.RegisterUser;
using Ashraak.Auth.Domain.Aggregates.AuthUser;
using Ashraak.Auth.Domain.Entities;
using Ashraak.Auth.Domain.Repositories;
using Ashraak.Caching.Abstractions;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.Tenant.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Ashraak.Auth.Api.Endpoints;

/// <summary>
/// Minimal API endpoints for the Auth module.
///
/// <para>
/// Two registration methods are intentionally separate:
/// <list type="bullet">
///   <item>
///     <see cref="MapAuthEndpoints"/> — versioned REST endpoints mapped under the host's
///     <c>/api/v{version}</c> route group (e.g. <c>/api/v1/auth/register</c>).
///   </item>
///   <item>
///     <see cref="MapAuthProtocolEndpoints"/> — unversioned protocol endpoints
///     (<c>/connect/token</c>, SSO flows) that must keep a stable URL forever
///     because external OAuth providers and clients have them hardcoded.
///   </item>
/// </list>
/// </para>
/// </summary>
public static class AuthEndpoints
{
    // ─── Versioned REST Endpoints ─────────────────────────────────────────────

    /// <summary>
    /// Registers versioned Auth REST endpoints on the supplied <paramref name="routeBuilder"/>.
    /// The host passes its <c>/api/v{version:apiVersion}</c> route group here so that
    /// final routes resolve to e.g. <c>POST /api/v1/auth/register</c>.
    /// </summary>
    /// <param name="routeBuilder">
    /// The versioned route group provided by <c>Ashraak.Api.Program</c>.
    /// </param>
    /// <returns>The same <paramref name="routeBuilder"/> for chaining.</returns>
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        // No /api/ prefix — the host's versioned group provides it.
        var group = routeBuilder.MapGroup("/auth")
            .WithTags("Auth");

        group.MapPost("/register", Register)
            .WithName("Register")
            .WithSummary("Register a new user account within a tenant.")
            .AllowAnonymous();

        routeBuilder.MapAuthExtendedEndpoints();

        return routeBuilder;
    }

    // ─── Unversioned Protocol Endpoints ──────────────────────────────────────

    /// <summary>
    /// Registers OpenIddict and SSO browser-redirect endpoints directly on the
    /// <paramref name="app"/> at fixed, non-versioned paths.
    ///
    /// <para>
    /// These paths MUST NOT change between API versions because:
    /// <list type="bullet">
    ///   <item>OAuth2 providers (Google, Microsoft) have the callback URLs pre-registered.</item>
    ///   <item>OpenIddict validates requests against the configured endpoint URIs.</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="app">The root <see cref="WebApplication"/>.</param>
    /// <returns>The same <paramref name="app"/> for chaining.</returns>
    public static WebApplication MapAuthProtocolEndpoints(this WebApplication app)
    {
        // OpenIddict resource-owner password grant — must match SetTokenEndpointUris().
        app.MapPost("/connect/token", IssueToken)
            .WithTags("OpenIddict")
            .WithName("IssueToken")
            .WithSummary("Issues a JWT via the resource-owner password grant.")
            .AllowAnonymous();

        // SSO flows — browser redirects; OAuth providers have these URLs registered.
        var sso = app.MapGroup("/api/auth/sso")
            .WithTags("Auth SSO")
            .AllowAnonymous();

        sso.MapGet("/google", StartGoogleSso)
            .WithName("StartGoogleSso")
            .WithSummary("Starts Google OAuth2 login challenge.");

        sso.MapGet("/microsoft", StartMicrosoftSso)
            .WithName("StartMicrosoftSso")
            .WithSummary("Starts Microsoft OAuth2 login challenge.");

        sso.MapGet("/callback", CompleteSso)
            .WithName("CompleteSso")
            .WithSummary("Returns external identity claims after OAuth2 callback.");

        return app;
    }

    /// <summary>
    /// <c>POST /api/auth/register</c> — Registers a new user identity.
    /// </summary>
    /// <param name="request">The registration payload from the request body.</param>
    /// <param name="sender">MediatR sender injected from the DI container.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <returns>
    /// <c>201 Created</c> with <c>Location: /api/users/{userId}</c> on success.
    /// <c>400 Bad Request</c> with a ProblemDetails body on failure.
    /// </returns>
    private static async Task<IResult> Register(
        [FromBody] RegisterRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.TenantId,
            request.Email,
            request.Password,
            request.DisplayName);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            // Location points to the versioned Users profile endpoint (v1 is the current release).
            : Results.Created($"/api/v1/users/{result.Value}", new { UserId = result.Value });
    }

    /// <summary>
    /// OpenIddict token endpoint pass-through implementation.
    /// Supports resource-owner password flow with tenant-aware validation.
    /// </summary>
    private static async Task<IResult> IssueToken(
        HttpContext context,
        IAuthUserRepository authUserRepository,
        IPasswordHasher passwordHasher,
        ITenantService tenantService,
        IAuthPermissionChecker permissionChecker,
        ISessionCacheService sessionCacheService,
        IUserSessionRepository userSessionRepository,
        ISender sender,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        if (!context.Request.HasFormContentType)
            return Results.BadRequest(new { error = "invalid_request", error_description = "Form-encoded token request is required." });

        var form = await context.Request.ReadFormAsync(cancellationToken);
        var grantType = form["grant_type"].FirstOrDefault() ?? string.Empty;

        if (string.Equals(grantType, "mfa", StringComparison.OrdinalIgnoreCase))
        {
            return await CompleteMfaTokenAsync(
                context, form, authUserRepository, permissionChecker, sessionCacheService,
                userSessionRepository, sender, unitOfWork, cancellationToken);
        }

        if (!string.Equals(grantType, "password", StringComparison.OrdinalIgnoreCase))
            return Results.BadRequest(new { error = "unsupported_grant_type", error_description = "Supported grants: password, mfa." });

        var tenantRaw = form["tenant_id"].FirstOrDefault()
            ?? form["tenantId"].FirstOrDefault()
            ?? context.Request.Headers["X-Tenant-ID"].FirstOrDefault();

        if (!Guid.TryParse(tenantRaw, out var tenantId))
            return Results.BadRequest(new { error = "invalid_tenant", error_description = "A valid tenant_id is required." });

        if (!await tenantService.IsActiveAsync(tenantId, cancellationToken))
            return Results.BadRequest(new { error = "tenant_inactive", error_description = "The tenant is inactive." });

        var email = form["username"].FirstOrDefault() ?? string.Empty;
        var password = form["password"].FirstOrDefault() ?? string.Empty;
        var user = await authUserRepository.GetByEmailAndTenantAsync(email, tenantId, cancellationToken);
        if (user is null)
            return Results.BadRequest(new { error = "invalid_grant", error_description = "Invalid credentials." });

        if (!passwordHasher.Verify(password, user.PasswordHash))
        {
            user.RecordFailedLogin(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                context.Request.Headers.UserAgent.FirstOrDefault() ?? "unknown",
                invalidPassword: true);
            authUserRepository.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Results.BadRequest(new { error = "invalid_grant", error_description = "Invalid credentials." });
        }

        if (!user.IsActive || user.IsLocked)
            return Results.BadRequest(new { error = "invalid_grant", error_description = "User account is inactive or locked." });

        var settings = await tenantService.GetSettingsAsync(tenantId, cancellationToken);
        var mfaRequired = user.MfaEnabled || (settings?.RequireMfa ?? false);
        if (mfaRequired)
        {
            var challengeResult = await sender.Send(
                new CreateMfaChallengeCommand(user.Id.Value, tenantId), cancellationToken);
            if (challengeResult.IsFailure)
                return Results.BadRequest(new { error = "mfa_error", error_description = challengeResult.Error.Description });

            return Results.BadRequest(new
            {
                error = "mfa_required",
                error_description = "Multi-factor authentication is required.",
                mfa_challenge_id = challengeResult.Value
            });
        }

        return await IssueTokenForUserAsync(
            context,
            user,
            tenantId,
            authUserRepository,
            permissionChecker,
            sessionCacheService,
            userSessionRepository,
            unitOfWork,
            cancellationToken);
    }

    private static async Task<IResult> CompleteMfaTokenAsync(
        HttpContext context,
        IFormCollection form,
        IAuthUserRepository authUserRepository,
        IAuthPermissionChecker permissionChecker,
        ISessionCacheService sessionCacheService,
        IUserSessionRepository userSessionRepository,
        ISender sender,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var challengeId = form["mfa_challenge_id"].FirstOrDefault() ?? string.Empty;
        var code = form["mfa_code"].FirstOrDefault() ?? form["code"].FirstOrDefault() ?? string.Empty;

        var verify = await sender.Send(
            new VerifyMfaChallengeCommand(
                challengeId,
                code,
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown"),
            cancellationToken);

        if (verify.IsFailure)
            return Results.BadRequest(new { error = "invalid_grant", error_description = verify.Error.Description });

        var user = await authUserRepository.GetByIdAsync(
            AuthUserId.From(verify.Value.UserId), cancellationToken);

        if (user is null)
            return Results.BadRequest(new { error = "invalid_grant", error_description = "User not found." });

        user.RecordSuccessfulLogin(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            context.Request.Headers.UserAgent.FirstOrDefault() ?? "unknown");
        authUserRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await IssueTokenForUserAsync(
            context,
            user,
            verify.Value.TenantId,
            authUserRepository,
            permissionChecker,
            sessionCacheService,
            userSessionRepository,
            unitOfWork,
            cancellationToken);
    }

    private static async Task<IResult> IssueTokenForUserAsync(
        HttpContext context,
        Domain.Aggregates.AuthUser.AuthUser user,
        Guid tenantId,
        IAuthUserRepository authUserRepository,
        IAuthPermissionChecker permissionChecker,
        ISessionCacheService sessionCacheService,
        IUserSessionRepository userSessionRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var roles = await permissionChecker.GetRolesAsync(user.Id.Value, tenantId, cancellationToken);
        var permissions = await permissionChecker.GetPermissionsAsync(user.Id.Value, tenantId, cancellationToken);

        var sessionId = Guid.NewGuid();
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var ua = context.Request.Headers.UserAgent.FirstOrDefault() ?? "unknown";
        var now = DateTime.UtcNow;

        userSessionRepository.Add(new UserSession
        {
            Id = sessionId,
            UserId = user.Id.Value,
            TenantId = tenantId,
            CreatedOnUtc = now,
            LastUsedOnUtc = now,
            IpAddress = ip,
            UserAgent = ua,
            IsRevoked = false
        });
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await sessionCacheService.SetAsync(
            tenantId,
            user.Id.Value,
            new SessionCacheEntry(tenantId, user.Id.Value, roles, permissions, now),
            TimeSpan.FromHours(8),
            cancellationToken);

        var identity = new ClaimsIdentity(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            OpenIddictConstants.Claims.Name,
            OpenIddictConstants.Claims.Role);

        identity.AddClaim(OpenIddictConstants.Claims.Subject, user.Id.Value.ToString("D"));
        identity.AddClaim(OpenIddictConstants.Claims.Email, user.Email);
        identity.AddClaim("tenant_id", tenantId.ToString("D"));
        identity.AddClaim("tenantId", tenantId.ToString("D"));
        identity.AddClaim("session_id", sessionId.ToString("D"));
        foreach (var role in roles)
            identity.AddClaim(OpenIddictConstants.Claims.Role, role);
        foreach (var permission in permissions)
            identity.AddClaim("permission", permission);

        var principal = new ClaimsPrincipal(identity);
        principal.SetScopes(
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.OfflineAccess,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Roles);

        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(
                OpenIddictConstants.Destinations.AccessToken,
                OpenIddictConstants.Destinations.IdentityToken);
        }

        return Results.SignIn(principal, authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Starts Google external login flow.
    /// </summary>
    private static IResult StartGoogleSso(HttpContext context)
    {
        // Redirect back to the fixed SSO callback path after Google authenticates.
        var properties = new AuthenticationProperties { RedirectUri = "/api/auth/sso/callback" };
        return Results.Challenge(properties, authenticationSchemes: ["Google"]);
    }

    /// <summary>
    /// Starts Microsoft external login flow.
    /// </summary>
    private static IResult StartMicrosoftSso(HttpContext context)
    {
        // Redirect back to the fixed SSO callback path after Microsoft authenticates.
        var properties = new AuthenticationProperties { RedirectUri = "/api/auth/sso/callback" };
        return Results.Challenge(properties, authenticationSchemes: ["Microsoft"]);
    }

    /// <summary>
    /// Returns the external principal details after successful SSO callback.
    /// </summary>
    private static IResult CompleteSso(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return Results.Unauthorized();

        var claims = context.User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Results.Ok(new
        {
            message = "External SSO authenticated. Exchange into local tenant account in next implementation phase.",
            claims
        });
    }
}

/// <summary>
/// HTTP request body for the <c>POST /api/auth/register</c> endpoint.
/// </summary>
/// <param name="TenantId">The tenant identifier the user is registering under.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The plain-text password (transmitted over HTTPS only).</param>
/// <param name="DisplayName">The user's display name.</param>
public sealed record RegisterRequest(
    Guid TenantId,
    string Email,
    string Password,
    string DisplayName);
