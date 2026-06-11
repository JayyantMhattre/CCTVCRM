using System.Text.Json;
using Ashraak.SharedKernel.Contracts.Audit.Dtos;
using Ashraak.SharedKernel.Contracts.Audit.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ashraak.Audit.Infrastructure.Interceptors;

/// <summary>
/// EF Core interceptor that captures entity create/update/delete operations
/// and emits audit entries containing before/after snapshots.
/// </summary>
internal sealed class AuditEntityChangeInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditEntityChangeInterceptor(
        IAuditService auditService,
        IHttpContextAccessor httpContextAccessor)
    {
        _auditService = auditService;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var context = eventData.Context;
        var entries = context.ChangeTracker.Entries()
            .Where(IsAuditableState)
            .ToList();

        if (entries.Count == 0)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (var entry in entries)
        {
            var tenantId = ResolveTenantId(entry);
            var userId = ResolveUserId();

            // Skip entries without tenant scope to preserve strict tenant-partitioned audit chains.
            if (tenantId == Guid.Empty)
                continue;

            var auditEntry = new AuditEntryDto(
                TenantId: tenantId,
                UserId: userId,
                Module: ResolveModuleName(context),
                Action: ResolveAction(entry.State),
                EntityType: entry.Metadata.ClrType.Name,
                EntityId: ResolveEntityId(entry),
                OldValues: SerializeOldValues(entry),
                NewValues: SerializeNewValues(entry),
                IpAddress: _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                UserAgent: _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.FirstOrDefault(),
                OccurredOnUtc: DateTime.UtcNow);

            await _auditService.LogAsync(auditEntry, cancellationToken);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static bool IsAuditableState(EntityEntry entry) =>
        entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted
        && !entry.Metadata.ClrType.Name.Contains("OutboxMessage", StringComparison.OrdinalIgnoreCase);

    private static string ResolveModuleName(DbContext context)
    {
        var ns = context.GetType().Namespace ?? string.Empty;
        var split = ns.Split('.');
        return split.Length > 1 ? split[1] : "Unknown";
    }

    private static string ResolveAction(EntityState state) => state switch
    {
        EntityState.Added => "Created",
        EntityState.Modified => "Updated",
        EntityState.Deleted => "Deleted",
        _ => "Unknown"
    };

    private Guid ResolveTenantId(EntityEntry entry)
    {
        var tenantProperty = entry.Properties.FirstOrDefault(p =>
            p.Metadata.Name.Equals("TenantId", StringComparison.OrdinalIgnoreCase));

        if (tenantProperty?.CurrentValue is Guid currentTenant && currentTenant != Guid.Empty)
            return currentTenant;
        if (tenantProperty?.OriginalValue is Guid originalTenant && originalTenant != Guid.Empty)
            return originalTenant;

        var httpTenant = _httpContextAccessor.HttpContext?.User.FindFirst("tenant_id")?.Value
            ?? _httpContextAccessor.HttpContext?.User.FindFirst("tenantId")?.Value
            ?? _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-ID"].FirstOrDefault();

        return Guid.TryParse(httpTenant, out var tenantFromRequest) ? tenantFromRequest : Guid.Empty;
    }

    private Guid? ResolveUserId()
    {
        var raw = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value
            ?? _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(raw, out var userId) ? userId : null;
    }

    private static string? ResolveEntityId(EntityEntry entry)
    {
        var idProperty = entry.Properties.FirstOrDefault(p =>
            p.Metadata.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));

        return (idProperty?.CurrentValue ?? idProperty?.OriginalValue)?.ToString();
    }

    private static string? SerializeOldValues(EntityEntry entry)
    {
        if (entry.State == EntityState.Added)
            return null;

        var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var property in entry.Properties)
        {
            values[property.Metadata.Name] = property.OriginalValue;
        }

        return JsonSerializer.Serialize(values, JsonOptions);
    }

    private static string? SerializeNewValues(EntityEntry entry)
    {
        if (entry.State == EntityState.Deleted)
            return null;

        var values = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var property in entry.Properties)
        {
            values[property.Metadata.Name] = property.CurrentValue;
        }

        return JsonSerializer.Serialize(values, JsonOptions);
    }
}
