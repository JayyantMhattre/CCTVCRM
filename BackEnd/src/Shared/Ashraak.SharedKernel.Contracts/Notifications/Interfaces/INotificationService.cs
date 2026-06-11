namespace Ashraak.SharedKernel.Contracts.Notifications.Interfaces;

/// <summary>
/// Cross-module contract for sending templated email notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends an email using a named template and tokenised data map.
    /// </summary>
    /// <param name="template">Template key (see <see cref="EmailTemplates"/>).</param>
    /// <param name="to">Recipient email address.</param>
    /// <param name="data">Placeholder values for the template body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendEmailAsync(
        string template,
        string to,
        IReadOnlyDictionary<string, string> data,
        CancellationToken cancellationToken = default);
}
