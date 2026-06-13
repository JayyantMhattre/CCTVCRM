using Ashraak.Cctv.Integration.Application;
using Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Events;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace Ashraak.Cctv.Integration.Infrastructure.Notifications.EventHandlers;

internal sealed class LeadCreatedNotificationHandler(
    ICctvNotificationDispatcher dispatcher,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    IFeatureFlagService featureFlags) : INotificationHandler<LeadCreatedDomainEvent>
{
    public async Task Handle(LeadCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.LeadsEnabled, cancellationToken: cancellationToken))
            return;

        var tenantId = CctvNotificationHandlerSupport.ResolveTenant(tenantContext, options);
        var data = CctvNotificationHandlerSupport.Data(
            ("LeadNumber", notification.LeadNumber),
            ("ContactName", notification.ContactName),
            ("Email", notification.Email),
            ("Source", notification.Source.ToString()));

        await dispatcher.NotifyAdminsAsync(tenantId, CctvNotificationTemplateKeys.LeadCreated, data, cancellationToken: cancellationToken);
    }
}

internal sealed class LeadConvertedNotificationHandler(
    ICctvNotificationDispatcher dispatcher,
    ICustomerLookupService customers,
    IAmcContractLookupService contracts,
    ISiteLookupService sites,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    IFeatureFlagService featureFlags) : INotificationHandler<LeadConvertedDomainEvent>
{
    public async Task Handle(LeadConvertedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.LeadsEnabled, cancellationToken: cancellationToken))
            return;

        var tenantId = CctvNotificationHandlerSupport.ResolveTenant(tenantContext, options);
        var customer = await customers.GetCustomerAsync(notification.CustomerId, cancellationToken);
        var site = await sites.GetSiteAsync(notification.SiteId, cancellationToken);
        var contract = await contracts.GetByIdAsync(notification.ContractId, cancellationToken);

        var data = CctvNotificationHandlerSupport.Data(
            ("LeadNumber", notification.LeadNumber),
            ("CustomerName", customer?.Name),
            ("SiteName", site?.Name),
            ("ContractNumber", contract?.ContractNumber));

        await dispatcher.NotifyAdminsAsync(tenantId, CctvNotificationTemplateKeys.LeadConverted, data, cancellationToken: cancellationToken);
        await dispatcher.NotifyCustomerAsync(
            tenantId,
            notification.CustomerId,
            CctvNotificationTemplateKeys.CustomerWelcome,
            data,
            cancellationToken: cancellationToken);
    }
}
