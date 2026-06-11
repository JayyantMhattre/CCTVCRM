namespace Ashraak.Notifications.Infrastructure.Providers;

/// <summary>Module-internal email transport abstraction.</summary>
internal interface IEmailProvider
{
    Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken);
}
