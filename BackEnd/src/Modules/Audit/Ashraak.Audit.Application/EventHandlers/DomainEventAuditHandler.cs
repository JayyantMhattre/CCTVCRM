using System.Text.Json;
using Ashraak.SharedKernel.Contracts.Audit.Dtos;
using Ashraak.SharedKernel.Contracts.Audit.Interfaces;
using Ashraak.SharedKernel.Domain.Events;
using MediatR;

namespace Ashraak.Audit.Application.EventHandlers;

/// <summary>
/// Generic MediatR notification handler that intercepts every <see cref="IDomainEvent"/>
/// and records it as an <see cref="AuditEntryDto"/> in the audit store.
/// </summary>
/// <remarks>
/// <para>
/// This handler subscribes to the base <see cref="IDomainEvent"/> interface, so it will fire
/// for every domain event published by any module. The source module is inferred from the
/// event type's namespace (second segment, e.g. <c>"Tenant"</c> from
/// <c>"Ashraak.Tenant.Domain.Aggregates.Tenant.Events"</c>).
/// </para>
/// <para>
/// <c>TenantId</c> and <c>UserId</c> are extracted via reflection using a convention that
/// all domain events carrying these values expose them as properties of the same name.
/// Events without these properties will log <c>Guid.Empty</c> / <see langword="null"/> respectively.
/// </para>
/// <para>
/// The serialised <c>NewValues</c> field contains the full event JSON, providing a complete
/// record of what data was present at the time of the action.
/// </para>
/// <para>
/// The handler calls <see cref="IAuditService.LogAsync"/> which is implemented as an enqueue
/// operation in infrastructure; actual Mongo persistence is handled by a background worker.
/// </para>
/// </remarks>
public sealed class DomainEventAuditHandler : INotificationHandler<IDomainEvent>
{
    private readonly IAuditService _auditService;

    /// <summary>Initialises the handler with the audit write service.</summary>
    public DomainEventAuditHandler(IAuditService auditService)
    {
        _auditService = auditService;
    }

    /// <inheritdoc/>
    public async Task Handle(IDomainEvent notification, CancellationToken cancellationToken)
    {
        var eventType = notification.GetType();
        var moduleName = eventType.Namespace?.Split('.').ElementAtOrDefault(1) ?? "Unknown";
        var eventName = eventType.Name.Replace("DomainEvent", "").Replace("Event", "");

        var tenantId = ExtractTenantId(notification);
        var userId = ExtractUserId(notification);

        var entry = new AuditEntryDto(
            TenantId: tenantId,
            UserId: userId,
            Module: moduleName,
            Action: eventName,
            EntityType: eventType.Name,
            EntityId: null,
            OldValues: null,
            NewValues: JsonSerializer.Serialize(notification),
            IpAddress: null,
            UserAgent: null,
            OccurredOnUtc: notification.OccurredOnUtc);

        await _auditService.LogAsync(entry, cancellationToken);
    }

    /// <summary>
    /// Extracts the <c>TenantId</c> value from the event using reflection.
    /// Returns <see cref="Guid.Empty"/> if the event does not expose a <c>TenantId</c> property.
    /// </summary>
    private static Guid ExtractTenantId(IDomainEvent domainEvent)
    {
        var tenantIdProp = domainEvent.GetType().GetProperty("TenantId");
        return tenantIdProp?.GetValue(domainEvent) is Guid tenantId ? tenantId : Guid.Empty;
    }

    /// <summary>
    /// Extracts the <c>UserId</c> value from the event using reflection.
    /// Returns <see langword="null"/> for system events that do not carry a user identifier.
    /// </summary>
    private static Guid? ExtractUserId(IDomainEvent domainEvent)
    {
        var userIdProp = domainEvent.GetType().GetProperty("UserId");
        return userIdProp?.GetValue(domainEvent) is Guid userId ? userId : null;
    }
}
