using Ashraak.Audit.Application.Abstractions;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Audit.Api.Endpoints;

public static class AuditEndpoints
{
    public static IEndpointRouteBuilder MapAuditEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/audit-logs")
            .WithTags("Audit")
            .RequireAuthorization("AdminOnly");

        group.MapGet("/", GetAuditLogs)
            .WithName("GetAuditLogs")
            .WithSummary("Queries the tamper-evident audit log with optional filters.");

        return routeBuilder;
    }

    private static async Task<IResult> GetAuditLogs(
        Guid? tenantId,
        string? module,
        string? search,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        ITenantContext tenantContext,
        IAuditReadService auditReadService,
        CancellationToken cancellationToken)
    {
        var resolvedTenant = tenantId ?? tenantContext.TenantId;
        if (resolvedTenant == Guid.Empty)
            return Results.BadRequest("Tenant context is required.");

        if (tenantContext.TenantId != Guid.Empty && tenantId.HasValue && tenantId != tenantContext.TenantId)
            return Results.Forbid();

        var result = await auditReadService.QueryAsync(
            resolvedTenant,
            module,
            search,
            from,
            to,
            page <= 0 ? 1 : page,
            pageSize <= 0 ? 50 : pageSize,
            cancellationToken);

        return Results.Ok(result);
    }
}
