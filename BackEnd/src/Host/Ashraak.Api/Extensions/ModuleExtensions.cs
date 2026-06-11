using Ashraak.Audit.Api.Endpoints;
using Ashraak.Audit.Infrastructure;
using Ashraak.Auth.Api.Endpoints;
using Ashraak.Auth.Infrastructure;
using Ashraak.Caching.Redis;
using Ashraak.Tenant.Api.Endpoints;
using Ashraak.Tenant.Infrastructure;
using Ashraak.Notifications.Api.Endpoints;
using Ashraak.Notifications.Infrastructure;
using Ashraak.Users.Api.Endpoints;
using Ashraak.Users.Infrastructure;
using Ashraak.Files.Api.Endpoints;
using Ashraak.Files.Infrastructure;
using Ashraak.ApiKeys.Api.Endpoints;
using Ashraak.ApiKeys.Infrastructure;
using Ashraak.Webhooks.Api.Endpoints;
using Ashraak.Webhooks.Infrastructure;

namespace Ashraak.Api.Extensions;

/// <summary>
/// Plug-in orchestrator for all feature modules.
///
/// <para>
/// Pattern: each module is a vertical slice that exposes two extension-method
/// contracts used here:
/// <list type="number">
///   <item><c>Add{Module}(IServiceCollection, IConfiguration)</c> — DI registration.</item>
///   <item><c>Map{Module}Endpoints(IEndpointRouteBuilder)</c>       — route registration.</item>
/// </list>
/// Adding or removing a module requires a single-line change in each method below.
/// No other file in the host project needs to change.
/// </para>
/// </summary>
internal static class ModuleExtensions
{
    // ─── Service Registration ─────────────────────────────────────────────────

    /// <summary>
    /// Registers all feature modules with the DI container.
    /// Order matters: Caching must be first because Auth and subsequent modules
    /// resolve <c>ICacheService</c> during their own registration.
    /// </summary>
    /// <param name="services">The host service collection.</param>
    /// <param name="configuration">The merged application configuration.</param>
    public static IServiceCollection AddModules(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Layer 0 — Caching must be first: other modules depend on ICacheService.
        services.AddCachingModule(configuration);

        // Layer 1 — Identity + authorisation (no dependencies on other modules).
        services.AddAuthModule(configuration);

        // Layer 2 — Tenant and user data (use Auth contracts for permission checks).
        services.AddTenantModule(configuration);
        services.AddUsersModule(configuration);
        services.AddFilesModule(configuration);
        services.AddWebhooksModule(configuration);
        services.AddApiKeysModule(configuration);

        // Layer 3 — Cross-cutting observers (Audit, Notifications). Order after business modules.
        services.AddAuditModule(configuration);
        services.AddNotificationsModule(configuration);

        return services;
    }

    // ─── Versioned Endpoint Registration ─────────────────────────────────────

    /// <summary>
    /// Maps all versioned feature-module REST endpoints onto the provided
    /// <paramref name="routeBuilder"/>.
    ///
    /// <para>
    /// In <c>Program.cs</c> a versioned <see cref="RouteGroupBuilder"/> scoped to
    /// <c>/api/v{version:apiVersion}</c> is passed here so that every module's
    /// routes are automatically prefixed (e.g. <c>/api/v1/auth/register</c>)
    /// without the module knowing anything about the versioning strategy.
    /// </para>
    /// </summary>
    /// <param name="routeBuilder">
    /// The versioned route group provided by the host's composition root.
    /// </param>
    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder routeBuilder)
    {
        // Each mapper appends its own sub-group (e.g. /auth, /tenants, /users, /audit-logs).
        routeBuilder.MapAuthEndpoints();
        routeBuilder.MapTenantEndpoints();
        routeBuilder.MapUserEndpoints();
        routeBuilder.MapFileEndpoints();
        routeBuilder.MapWebhooksEndpoints();
        routeBuilder.MapApiKeysEndpoints();
        routeBuilder.MapAuditEndpoints();
        routeBuilder.MapNotificationsEndpoints();

        return routeBuilder;
    }

    // ─── Protocol (Unversioned) Endpoint Registration ─────────────────────────

    /// <summary>
    /// Maps all protocol-level endpoints that must remain at a fixed, non-versioned
    /// URL (OpenIddict token endpoint, OAuth2 SSO flows).
    ///
    /// <para>
    /// These are registered directly on the root <see cref="WebApplication"/> so they
    /// are never wrapped inside a versioned route group.
    /// </para>
    /// </summary>
    /// <param name="app">The built <see cref="WebApplication"/>.</param>
    public static WebApplication MapModuleProtocolEndpoints(this WebApplication app)
    {
        // OpenIddict /connect/token and SSO browser-redirect flows.
        app.MapAuthProtocolEndpoints();

        return app;
    }
}
