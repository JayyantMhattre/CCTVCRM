using Ashraak.Cctv.Integration.Application;
using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice.Events;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace Ashraak.Cctv.Integration.Infrastructure.Notifications.EventHandlers;

internal sealed class InvoiceGeneratedNotificationHandler(
    ICctvNotificationDispatcher dispatcher,
    IInvoiceLookupService invoices,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    IFeatureFlagService featureFlags) : INotificationHandler<InvoiceGeneratedDomainEvent>
{
    public async Task Handle(InvoiceGeneratedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.InvoicesEnabled, cancellationToken: cancellationToken))
            return;

        var tenantId = CctvNotificationHandlerSupport.ResolveTenant(tenantContext, options);
        var invoice = await invoices.GetInvoiceAsync(notification.InvoiceId, cancellationToken);
        if (invoice is null)
            return;

        var adminData = CctvNotificationHandlerSupport.Data(
            ("InvoiceNumber", notification.InvoiceNumber),
            ("InvoiceAmount", notification.TotalAmount.ToString("F2")),
            ("InvoiceDate", invoice.InvoiceDate.ToString("yyyy-MM-dd")),
            ("Status", invoice.Status));

        var customerData = CctvNotificationHandlerSupport.Data(
            ("InvoiceNumber", notification.InvoiceNumber),
            ("InvoiceAmount", notification.TotalAmount.ToString("F2")),
            ("InvoiceDate", invoice.InvoiceDate.ToString("yyyy-MM-dd")),
            ("Status", invoice.Status),
            (CctvNotificationHandlerSupport.DeepLinkKey, CctvNotificationHandlerSupport.CustomerInvoiceDeepLink(notification.InvoiceId)));

        await dispatcher.NotifyAdminsAsync(tenantId, CctvNotificationTemplateKeys.InvoiceGenerated, adminData, cancellationToken: cancellationToken);
        await dispatcher.NotifyCustomerAsync(
            tenantId,
            invoice.CustomerId,
            CctvNotificationTemplateKeys.InvoiceGenerated,
            customerData,
            cancellationToken: cancellationToken);
    }
}
