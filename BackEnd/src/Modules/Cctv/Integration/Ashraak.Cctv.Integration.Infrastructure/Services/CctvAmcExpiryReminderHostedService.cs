using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.Cctv.Integration.Application;
using Ashraak.Cctv.Integration.Infrastructure.Notifications;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ashraak.Cctv.Integration.Infrastructure.Services;

/// <summary>Daily job that sends AMC expiry reminders 30 days before term end.</summary>
internal sealed class CctvAmcExpiryReminderHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<CctvAmcExpiryReminderHostedService> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromHours(24);
    private const int ReminderDaysBeforeExpiry = 30;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var featureFlags = scope.ServiceProvider.GetRequiredService<IFeatureFlagService>();
                if (await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, cancellationToken: stoppingToken))
                    await ProcessRemindersAsync(scope.ServiceProvider, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AMC expiry reminder cycle failed");
            }

            try
            {
                await Task.Delay(PollInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task ProcessRemindersAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        var repository = services.GetRequiredService<IAmcContractRepository>();
        var dispatcher = services.GetRequiredService<ICctvNotificationDispatcher>();
        var customers = services.GetRequiredService<ICustomerLookupService>();
        var sites = services.GetRequiredService<ISiteLookupService>();
        var tenantContext = services.GetRequiredService<ITenantContext>();
        var notificationOptions = services.GetRequiredService<IOptions<CctvNotificationOptions>>();

        var targetDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(ReminderDaysBeforeExpiry));
        var expiring = await repository.GetActiveTermsExpiringOnAsync(targetDate, cancellationToken);
        if (expiring.Count == 0)
            return;

        var tenantId = CctvTenantHelper.ResolveTenantId(tenantContext, notificationOptions.Value);
        foreach (var (contract, term) in expiring)
        {
            var customer = await customers.GetCustomerAsync(contract.CustomerId, cancellationToken);
            var site = await sites.GetSiteAsync(contract.SiteId, cancellationToken);
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ContractNumber"] = contract.ContractNumber,
                ["CustomerName"] = customer?.Name ?? string.Empty,
                ["SiteName"] = site?.Name ?? string.Empty,
                ["ContractExpiryDate"] = term.EndDate.ToString("yyyy-MM-dd"),
                ["Days"] = ReminderDaysBeforeExpiry.ToString(),
                [CctvNotificationHandlerSupport.DeepLinkKey] = CctvNotificationHandlerSupport.CustomerAmcDeepLink()
            };

            await dispatcher.NotifyAdminsAsync(
                tenantId,
                CctvNotificationTemplateKeys.AmcExpiryReminder,
                data,
                sendSms: true,
                cancellationToken: cancellationToken);

            await dispatcher.NotifyCustomerAsync(
                tenantId,
                contract.CustomerId,
                CctvNotificationTemplateKeys.AmcExpiryReminder,
                data,
                sendSms: true,
                cancellationToken: cancellationToken);
        }

        logger.LogInformation("Sent AMC expiry reminders for {Count} term(s) expiring on {Date}", expiring.Count, targetDate);
    }
}
