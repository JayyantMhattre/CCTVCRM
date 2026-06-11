using Microsoft.Extensions.Logging;

namespace Ashraak.Notifications.Infrastructure.Providers;

/// <summary>Development provider — logs email to console and structured logs.</summary>
internal sealed class ConsoleEmailProvider(ILogger<ConsoleEmailProvider> logger) : IEmailProvider
{
    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "EMAIL to={To} subject={Subject} body={Body}",
            to,
            subject,
            body);

        Console.WriteLine($"[NOTIFICATION] To: {to}{Environment.NewLine}Subject: {subject}{Environment.NewLine}{body}");
        return Task.CompletedTask;
    }
}
