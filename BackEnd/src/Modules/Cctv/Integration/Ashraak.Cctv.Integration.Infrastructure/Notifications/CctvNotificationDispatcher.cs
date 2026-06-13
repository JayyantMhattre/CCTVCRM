using Ashraak.Cctv.Integration.Application.Abstractions;

using Ashraak.Cctv.Integration.Application;

using Ashraak.SharedKernel.Contracts.Auth.Interfaces;

using Ashraak.SharedKernel.Contracts.CctvCrm;

using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;

using Ashraak.SharedKernel.Contracts.Notifications.Interfaces;

using Ashraak.SharedKernel.Contracts.Users.Interfaces;

using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Options;



namespace Ashraak.Cctv.Integration.Infrastructure.Notifications;



internal sealed class CctvNotificationDispatcher(

    INotificationService notificationService,

    ISmsProvider smsProvider,

    IUserService userService,

    IAuthPermissionChecker permissionChecker,

    ICustomerLookupService customerLookup,

    IEngineerLookupService engineerLookup,

    IFeatureFlagService featureFlags,

    IOptions<CctvNotificationOptions> notificationOptions,

    ILogger<CctvNotificationDispatcher> logger) : ICctvNotificationDispatcher

{

    public async Task NotifyAdminsAsync(

        Guid tenantId,

        string templateKey,

        IReadOnlyDictionary<string, string> data,

        bool sendSms = false,

        CancellationToken cancellationToken = default)

    {

        var users = await userService.GetUsersForTenantAsync(tenantId, cancellationToken);

        foreach (var user in users)

        {

            if (!await permissionChecker.IsInRoleAsync(user.UserId, tenantId, "Admin", cancellationToken))

                continue;



            if (!user.Preferences.EmailNotificationsEnabled)

                continue;



            await NotifyEmailAsync(

                tenantId,

                user.Email,

                phone: null,

                user.DisplayName,

                templateKey,

                data,

                sendSms,

                respectEmailPreference: false,

                cancellationToken);

        }

    }



    public async Task NotifyCustomerAsync(

        Guid tenantId,

        Guid customerId,

        string templateKey,

        IReadOnlyDictionary<string, string> data,

        bool sendSms = false,

        CancellationToken cancellationToken = default)

    {

        var customer = await customerLookup.GetCustomerAsync(customerId, cancellationToken);

        if (customer is null)

        {

            logger.LogWarning("Customer {CustomerId} not found for notification {Template}", customerId, templateKey);

            return;

        }



        await NotifyEmailAsync(

            tenantId,

            customer.Email,

            customer.Phone,

            customer.Name,

            templateKey,

            data,

            sendSms,

            respectEmailPreference: true,

            cancellationToken);

    }



    public async Task NotifyEngineerAsync(

        Guid tenantId,

        Guid engineerId,

        string templateKey,

        IReadOnlyDictionary<string, string> data,

        bool sendSms = false,

        CancellationToken cancellationToken = default)

    {

        var engineer = await engineerLookup.GetAsync(engineerId, cancellationToken);

        if (engineer is null)

        {

            logger.LogWarning("Engineer {EngineerId} not found for notification {Template}", engineerId, templateKey);

            return;

        }



        string? email = null;

        if (engineer.PlatformUserId.HasValue)

        {

            var user = await userService.GetUserAsync(engineer.PlatformUserId.Value, cancellationToken);

            if (user is not null && !user.Preferences.EmailNotificationsEnabled)

                return;



            email = user?.Email;

        }



        if (string.IsNullOrWhiteSpace(email))

        {

            logger.LogWarning("No email for engineer {EngineerId}; skipping {Template}", engineerId, templateKey);

            return;

        }



        await NotifyEmailAsync(

            tenantId,

            email,

            engineer.Phone,

            engineer.Name,

            templateKey,

            data,

            sendSms,

            respectEmailPreference: false,

            cancellationToken);

    }



    public async Task NotifyEmailAsync(

        Guid tenantId,

        string email,

        string? phone,

        string recipientName,

        string templateKey,

        IReadOnlyDictionary<string, string> data,

        bool sendSms = false,

        bool respectEmailPreference = true,

        CancellationToken cancellationToken = default)

    {

        if (string.IsNullOrWhiteSpace(email))

            return;



        var enriched = EnrichData(recipientName, data);



        if (!respectEmailPreference || await ShouldSendEmailAsync(email, tenantId, cancellationToken))

        {

            try

            {

                await notificationService.SendEmailAsync(templateKey, email, enriched, cancellationToken);

            }

            catch (Exception ex)

            {

                logger.LogError(ex, "Failed to send email template {Template} to {Email}", templateKey, email);

            }

        }



        if (sendSms && !string.IsNullOrWhiteSpace(phone)

            && await featureFlags.IsEnabledAsync(CctvFeatureFlags.SmsEnabled, cancellationToken: cancellationToken))

        {

            try

            {

                var smsBody = BuildSmsBody(templateKey, enriched);

                await smsProvider.SendAsync(phone, smsBody, cancellationToken);

            }

            catch (Exception ex)

            {

                logger.LogError(ex, "Failed to send SMS for template {Template} to {Phone}", templateKey, phone);

            }

        }

    }



    private Dictionary<string, string> EnrichData(string recipientName, IReadOnlyDictionary<string, string> data)

    {

        var enriched = new Dictionary<string, string>(data, StringComparer.OrdinalIgnoreCase)

        {

            ["RecipientName"] = recipientName,

            ["PortalUrl"] = notificationOptions.Value.PortalUrl

        };

        return enriched;

    }



    private async Task<bool> ShouldSendEmailAsync(string email, Guid tenantId, CancellationToken cancellationToken)

    {

        if (tenantId == Guid.Empty)

            return true;



        var users = await userService.GetUsersForTenantAsync(tenantId, cancellationToken);

        var match = users.FirstOrDefault(u =>

            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));



        return match is null || match.Preferences.EmailNotificationsEnabled;

    }



    private static string BuildSmsBody(string templateKey, IReadOnlyDictionary<string, string> data)

    {

        var summary = templateKey switch

        {

            CctvNotificationTemplateKeys.TicketAssigned =>

                $"Ticket assigned: {Get(data, "TicketNumber")} — {Get(data, "TicketSubject")}",

            CctvNotificationTemplateKeys.VisitScheduled =>

                $"Visit scheduled on {Get(data, "VisitDate")} — {Get(data, "ScheduleNumber")}",

            CctvNotificationTemplateKeys.AmcExpiryReminder =>

                $"AMC expires {Get(data, "ContractExpiryDate")}. Renew via portal.",

            _ => $"Aarvii notification: {templateKey}"

        };

        return summary;

    }



    private static string Get(IReadOnlyDictionary<string, string> data, string key) =>

        data.TryGetValue(key, out var value) ? value : string.Empty;

}

