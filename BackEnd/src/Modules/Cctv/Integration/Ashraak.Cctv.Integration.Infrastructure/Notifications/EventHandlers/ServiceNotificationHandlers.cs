using Ashraak.Cctv.Integration.Application;
using Ashraak.Cctv.Service.Domain.Aggregates.Schedule.Events;
using Ashraak.Cctv.Service.Domain.Aggregates.Visit.Events;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;

namespace Ashraak.Cctv.Integration.Infrastructure.Notifications.EventHandlers;

internal sealed class VisitScheduledNotificationHandler(
    ICctvNotificationDispatcher dispatcher,
    IScheduleLookupService schedules,
    ISiteLookupService sites,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    IFeatureFlagService featureFlags) : INotificationHandler<VisitScheduledDomainEvent>
{
    public async Task Handle(VisitScheduledDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, cancellationToken: cancellationToken))
            return;

        var tenantId = CctvNotificationHandlerSupport.ResolveTenant(tenantContext, options);
        var schedule = await schedules.GetScheduleAsync(notification.ScheduleId, cancellationToken);
        var site = await sites.GetSiteAsync(notification.SiteId, cancellationToken);

        var adminData = CctvNotificationHandlerSupport.Data(
            ("ScheduleNumber", notification.ScheduleNumber),
            ("VisitDate", notification.ScheduledDate.ToString("yyyy-MM-dd")),
            ("SiteName", site?.Name));

        var customerData = CctvNotificationHandlerSupport.Data(
            ("ScheduleNumber", notification.ScheduleNumber),
            ("VisitDate", notification.ScheduledDate.ToString("yyyy-MM-dd")),
            ("SiteName", site?.Name),
            (CctvNotificationHandlerSupport.DeepLinkKey, CctvNotificationHandlerSupport.CustomerVisitsDeepLink()));

        await dispatcher.NotifyAdminsAsync(tenantId, CctvNotificationTemplateKeys.VisitScheduled, adminData, cancellationToken: cancellationToken);

        if (site is not null)
        {
            await dispatcher.NotifyCustomerAsync(
                tenantId,
                site.CustomerId,
                CctvNotificationTemplateKeys.VisitScheduled,
                customerData,
                sendSms: true,
                cancellationToken: cancellationToken);
        }
    }
}

internal sealed class EngineerAssignedNotificationHandler(
    ICctvNotificationDispatcher dispatcher,
    IScheduleLookupService schedules,
    IEngineerLookupService engineers,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    IFeatureFlagService featureFlags) : INotificationHandler<EngineerAssignedDomainEvent>
{
    public async Task Handle(EngineerAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, cancellationToken: cancellationToken))
            return;

        var tenantId = CctvNotificationHandlerSupport.ResolveTenant(tenantContext, options);
        var schedule = await schedules.GetScheduleAsync(notification.ScheduleId, cancellationToken);
        if (schedule is null)
            return;

        var engineer = await engineers.GetAsync(notification.EngineerId, cancellationToken);
        var visitId = schedule.Visit?.Id;
        var engineerDeepLink = visitId is { } id && id != Guid.Empty
            ? CctvNotificationHandlerSupport.EngineerVisitReportDeepLink(id)
            : CctvNotificationHandlerSupport.EngineerVisitsDeepLink();

        var data = CctvNotificationHandlerSupport.Data(
            ("ScheduleNumber", schedule.ScheduleNumber),
            ("VisitDate", schedule.ScheduledDate.ToString("yyyy-MM-dd")),
            ("EngineerName", engineer?.Name),
            (CctvNotificationHandlerSupport.DeepLinkKey, engineerDeepLink));

        await dispatcher.NotifyEngineerAsync(
            tenantId,
            notification.EngineerId,
            CctvNotificationTemplateKeys.VisitScheduled,
            data,
            sendSms: true,
            cancellationToken: cancellationToken);
    }
}

internal sealed class VisitCompletedNotificationHandler(
    ICctvNotificationDispatcher dispatcher,
    IScheduleLookupService schedules,
    ISiteLookupService sites,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    IFeatureFlagService featureFlags) : INotificationHandler<VisitCompletedDomainEvent>
{
    public async Task Handle(VisitCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, cancellationToken: cancellationToken))
            return;

        var tenantId = CctvNotificationHandlerSupport.ResolveTenant(tenantContext, options);
        var schedule = await schedules.GetScheduleAsync(notification.ScheduleId, cancellationToken);
        var site = await sites.GetSiteAsync(notification.SiteId, cancellationToken);

        var adminData = CctvNotificationHandlerSupport.Data(
            ("ScheduleNumber", schedule?.ScheduleNumber),
            ("VisitDate", schedule?.ScheduledDate.ToString("yyyy-MM-dd")),
            ("SiteName", site?.Name));

        var customerData = CctvNotificationHandlerSupport.Data(
            ("ScheduleNumber", schedule?.ScheduleNumber),
            ("VisitDate", schedule?.ScheduledDate.ToString("yyyy-MM-dd")),
            ("SiteName", site?.Name),
            (CctvNotificationHandlerSupport.DeepLinkKey, CctvNotificationHandlerSupport.CustomerServiceHistoryDeepLink()));

        await dispatcher.NotifyAdminsAsync(tenantId, CctvNotificationTemplateKeys.VisitCompleted, adminData, cancellationToken: cancellationToken);

        if (site is not null)
        {
            await dispatcher.NotifyCustomerAsync(
                tenantId,
                site.CustomerId,
                CctvNotificationTemplateKeys.VisitCompleted,
                customerData,
                cancellationToken: cancellationToken);
        }
    }
}

internal sealed class VisitApprovedNotificationHandler(
    ICctvNotificationDispatcher dispatcher,
    IScheduleLookupService schedules,
    ISiteLookupService sites,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    IFeatureFlagService featureFlags) : INotificationHandler<VisitReportApprovedDomainEvent>
{
    public async Task Handle(VisitReportApprovedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, cancellationToken: cancellationToken))
            return;

        var tenantId = CctvNotificationHandlerSupport.ResolveTenant(tenantContext, options);
        var schedule = await schedules.GetScheduleAsync(notification.ScheduleId, cancellationToken);
        if (schedule is null)
            return;

        var site = await sites.GetSiteAsync(schedule.SiteId, cancellationToken);
        var adminData = CctvNotificationHandlerSupport.Data(
            ("ScheduleNumber", schedule.ScheduleNumber),
            ("VisitDate", schedule.ScheduledDate.ToString("yyyy-MM-dd")),
            ("SiteName", site?.Name),
            ("ApprovalStatus", "Approved"));

        var customerData = CctvNotificationHandlerSupport.Data(
            ("ScheduleNumber", schedule.ScheduleNumber),
            ("VisitDate", schedule.ScheduledDate.ToString("yyyy-MM-dd")),
            ("SiteName", site?.Name),
            ("ApprovalStatus", "Approved"),
            (CctvNotificationHandlerSupport.DeepLinkKey, CctvNotificationHandlerSupport.CustomerServiceHistoryDeepLink()));

        await dispatcher.NotifyAdminsAsync(tenantId, CctvNotificationTemplateKeys.VisitApproved, adminData, cancellationToken: cancellationToken);

        if (site is not null)
        {
            await dispatcher.NotifyCustomerAsync(
                tenantId,
                site.CustomerId,
                CctvNotificationTemplateKeys.VisitApproved,
                customerData,
                cancellationToken: cancellationToken);
        }
    }
}
