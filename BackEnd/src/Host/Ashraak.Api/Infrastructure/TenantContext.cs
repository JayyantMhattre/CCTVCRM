using Ashraak.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Ashraak.Api.Infrastructure;

/// <summary>
/// HTTP-context-backed implementation of <see cref="ITenantContext"/>.
/// Resolves the active tenant from the following sources in priority order:
/// <list type="number">
///   <item><description>The <c>tenant_id</c> JWT claim (set by OpenIddict at login).</description></item>
///   <item><description>The <c>X-Tenant-ID</c> HTTP header (for machine-to-machine flows).</description></item>
/// </list>
/// </summary>
/// <remarks>
/// Registered as a scoped service so that a fresh resolution happens per HTTP request.
/// Module infrastructure layers (EF Core global query filters, repositories) read from
/// this context to scope data access to the current tenant automatically.
/// </remarks>
internal sealed class TenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>Initialises the context with the HTTP context accessor.</summary>
    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext? Context => _httpContextAccessor.HttpContext;

    /// <inheritdoc/>
    /// <remarks>
    /// Resolution order: <c>tenant_id</c>/<c>tenantId</c> JWT claim → <c>X-Tenant-ID</c> header → <see cref="Guid.Empty"/>.
    /// </remarks>
    public Guid TenantId => Guid.TryParse(
        Context?.User.FindFirst("tenant_id")?.Value
        ?? Context?.User.FindFirst("tenantId")?.Value
        ?? Context?.Request.Headers["X-Tenant-ID"].FirstOrDefault(),
        out var id) ? id : Guid.Empty;

    /// <inheritdoc/>
    /// <remarks>Extracted from the <c>X-Tenant-Slug</c> request header.</remarks>
    public string TenantSlug =>
        Context?.Request.Headers["X-Tenant-Slug"].FirstOrDefault() ?? string.Empty;

    /// <inheritdoc/>
    /// <remarks>Extracted from the <c>tenant_plan</c> JWT claim. Defaults to <c>"Free"</c>.</remarks>
    public string Plan =>
        Context?.User.FindFirst("tenant_plan")?.Value ?? "Free";

    /// <inheritdoc/>
    /// <remarks>
    /// Always returns <see langword="true"/> here; active status enforcement is done
    /// at the middleware level via <c>ITenantService.IsActiveAsync</c>.
    /// </remarks>
    public bool IsActive => true;
}
