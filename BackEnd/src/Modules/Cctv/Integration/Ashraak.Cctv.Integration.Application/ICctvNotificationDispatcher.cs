namespace Ashraak.Cctv.Integration.Application;

/// <summary>
/// Central CCTV notification dispatcher — reuses platform email and configured SMS.
/// Respects user email notification preferences; SMS gated by feature flag.
/// </summary>
public interface ICctvNotificationDispatcher
{
    Task NotifyAdminsAsync(
        Guid tenantId,
        string templateKey,
        IReadOnlyDictionary<string, string> data,
        bool sendSms = false,
        CancellationToken cancellationToken = default);

    Task NotifyCustomerAsync(
        Guid tenantId,
        Guid customerId,
        string templateKey,
        IReadOnlyDictionary<string, string> data,
        bool sendSms = false,
        CancellationToken cancellationToken = default);

    Task NotifyEngineerAsync(
        Guid tenantId,
        Guid engineerId,
        string templateKey,
        IReadOnlyDictionary<string, string> data,
        bool sendSms = false,
        CancellationToken cancellationToken = default);

    Task NotifyEmailAsync(
        Guid tenantId,
        string email,
        string? phone,
        string recipientName,
        string templateKey,
        IReadOnlyDictionary<string, string> data,
        bool sendSms = false,
        bool respectEmailPreference = true,
        CancellationToken cancellationToken = default);
}
