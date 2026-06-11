using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ashraak.Notifications.Infrastructure.Providers;

/// <summary>SMTP transport for production (configuration-driven).</summary>
internal sealed class SmtpEmailProvider(
    IOptions<NotificationOptions> options,
    ILogger<SmtpEmailProvider> logger) : IEmailProvider
{
    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        var smtp = options.Value.Smtp;
        if (string.IsNullOrWhiteSpace(smtp.Host))
            throw new InvalidOperationException("SMTP host is not configured.");

        using var message = new MailMessage(smtp.FromAddress, to, subject, body) { IsBodyHtml = false };
        using var client = new SmtpClient(smtp.Host, smtp.Port)
        {
            EnableSsl = smtp.UseSsl,
            Credentials = string.IsNullOrWhiteSpace(smtp.Username)
                ? CredentialCache.DefaultNetworkCredentials
                : new NetworkCredential(smtp.Username, smtp.Password)
        };

        await client.SendMailAsync(message, cancellationToken);
        logger.LogInformation("SMTP email sent to {To}", to);
    }
}
