using Ashraak.Cctv.Amc.Domain.Aggregates.Contract.Events;
using Ashraak.Cctv.Integration.Application;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace Ashraak.Cctv.Integration.Infrastructure.Notifications.EventHandlers;

internal sealed class RenewalRequestedNotificationHandler(
    ICctvNotificationDispatcher dispatcher,
    IAmcContractLookupService contracts,
    ICustomerLookupService customers,
    ISiteLookupService sites,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    IFeatureFlagService featureFlags) : INotificationHandler<RenewalRequestedDomainEvent>
{
    public async Task Handle(RenewalRequestedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, cancellationToken: cancellationToken))
            return;

        var tenantId = CctvNotificationHandlerSupport.ResolveTenant(tenantContext, options);
        var contract = await contracts.GetByIdAsync(notification.ContractId, cancellationToken);
        var term = await contracts.GetTermByIdAsync(notification.ContractId, notification.TermId, cancellationToken);
        var customer = await customers.GetCustomerAsync(notification.CustomerId, cancellationToken);
        var site = contract is not null ? await sites.GetSiteAsync(contract.SiteId, cancellationToken) : null;

        var adminData = CctvNotificationHandlerSupport.Data(
            ("ContractNumber", contract?.ContractNumber),
            ("CustomerName", customer?.Name),
            ("SiteName", site?.Name),
            ("ContractExpiryDate", term?.EndDate.ToString("yyyy-MM-dd")),
            ("RequestedAtUtc", notification.RequestedAtUtc.ToString("u")));

        var customerData = CctvNotificationHandlerSupport.Data(
            ("ContractNumber", contract?.ContractNumber),
            ("CustomerName", customer?.Name),
            ("SiteName", site?.Name),
            ("ContractExpiryDate", term?.EndDate.ToString("yyyy-MM-dd")),
            ("RequestedAtUtc", notification.RequestedAtUtc.ToString("u")),
            (CctvNotificationHandlerSupport.DeepLinkKey, CctvNotificationHandlerSupport.CustomerAmcDeepLink()));

        await dispatcher.NotifyAdminsAsync(
            tenantId,
            CctvNotificationTemplateKeys.AmcRenewalRequested,
            adminData,
            cancellationToken: cancellationToken);

        await dispatcher.NotifyCustomerAsync(
            tenantId,
            notification.CustomerId,
            CctvNotificationTemplateKeys.AmcRenewalRequested,
            customerData,
            cancellationToken: cancellationToken);
    }
}
