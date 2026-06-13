using Ashraak.Cctv.Integration.Application;
using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket.Events;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace Ashraak.Cctv.Integration.Infrastructure.Notifications.EventHandlers;

internal sealed class TicketCreatedNotificationHandler(
    ICctvNotificationDispatcher dispatcher,
    ITicketLookupService tickets,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    IFeatureFlagService featureFlags) : INotificationHandler<TicketCreatedDomainEvent>
{
    public async Task Handle(TicketCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.TicketsEnabled, cancellationToken: cancellationToken))
            return;

        var tenantId = CctvNotificationHandlerSupport.ResolveTenant(tenantContext, options);
        var ticket = await tickets.GetTicketAsync(notification.TicketId, cancellationToken);

        var adminData = CctvNotificationHandlerSupport.Data(
            ("TicketNumber", notification.TicketNumber),
            ("TicketSubject", ticket?.Subject),
            ("Source", notification.Source));

        var customerData = CctvNotificationHandlerSupport.Data(
            ("TicketNumber", notification.TicketNumber),
            ("TicketSubject", ticket?.Subject),
            ("Source", notification.Source),
            (CctvNotificationHandlerSupport.DeepLinkKey, CctvNotificationHandlerSupport.CustomerTicketDeepLink(notification.TicketId)));

        await dispatcher.NotifyAdminsAsync(tenantId, CctvNotificationTemplateKeys.TicketCreated, adminData, cancellationToken: cancellationToken);
        await dispatcher.NotifyCustomerAsync(
            tenantId,
            notification.CustomerId,
            CctvNotificationTemplateKeys.TicketCreated,
            customerData,
            cancellationToken: cancellationToken);
    }
}

internal sealed class TicketAssignedNotificationHandler(
    ICctvNotificationDispatcher dispatcher,
    ITicketLookupService tickets,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    IFeatureFlagService featureFlags) : INotificationHandler<TicketAssignedDomainEvent>
{
    public async Task Handle(TicketAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.TicketsEnabled, cancellationToken: cancellationToken))
            return;

        var tenantId = CctvNotificationHandlerSupport.ResolveTenant(tenantContext, options);
        var ticket = await tickets.GetTicketAsync(notification.TicketId, cancellationToken);
        if (ticket is null)
            return;

        var data = CctvNotificationHandlerSupport.Data(
            ("TicketNumber", ticket.TicketNumber),
            ("TicketSubject", ticket.Subject),
            ("Priority", ticket.Priority));

        await dispatcher.NotifyAdminsAsync(tenantId, CctvNotificationTemplateKeys.TicketAssigned, data, cancellationToken: cancellationToken);
        await dispatcher.NotifyEngineerAsync(
            tenantId,
            notification.EngineerId,
            CctvNotificationTemplateKeys.TicketAssigned,
            data,
            sendSms: true,
            cancellationToken: cancellationToken);
        await dispatcher.NotifyCustomerAsync(
            tenantId,
            ticket.CustomerId,
            CctvNotificationTemplateKeys.TicketAssigned,
            data,
            cancellationToken: cancellationToken);
    }
}

internal sealed class TicketClosedNotificationHandler(
    ICctvNotificationDispatcher dispatcher,
    ITicketLookupService tickets,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    IFeatureFlagService featureFlags) : INotificationHandler<TicketClosedDomainEvent>
{
    public async Task Handle(TicketClosedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.TicketsEnabled, cancellationToken: cancellationToken))
            return;

        var tenantId = CctvNotificationHandlerSupport.ResolveTenant(tenantContext, options);
        var ticket = await tickets.GetTicketAsync(notification.TicketId, cancellationToken);
        if (ticket is null)
            return;

        var adminData = CctvNotificationHandlerSupport.Data(
            ("TicketNumber", ticket.TicketNumber),
            ("TicketSubject", ticket.Subject));

        var customerData = CctvNotificationHandlerSupport.Data(
            ("TicketNumber", ticket.TicketNumber),
            ("TicketSubject", ticket.Subject),
            (CctvNotificationHandlerSupport.DeepLinkKey, CctvNotificationHandlerSupport.CustomerTicketDeepLink(notification.TicketId)));

        await dispatcher.NotifyAdminsAsync(tenantId, CctvNotificationTemplateKeys.TicketClosed, adminData, cancellationToken: cancellationToken);
        await dispatcher.NotifyCustomerAsync(
            tenantId,
            ticket.CustomerId,
            CctvNotificationTemplateKeys.TicketClosed,
            customerData,
            cancellationToken: cancellationToken);
    }
}
