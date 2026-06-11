using Ashraak.Notifications.Infrastructure.Providers;
using Ashraak.Notifications.Infrastructure.Templates;
using Ashraak.SharedKernel.Contracts.Notifications.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ashraak.Notifications.Infrastructure.Services;

internal sealed class NotificationService(
    IServiceProvider serviceProvider,
    IOptions<NotificationOptions> options,
    FileEmailTemplateRenderer templateRenderer) : INotificationService
{
    public async Task SendEmailAsync(
        string template,
        string to,
        IReadOnlyDictionary<string, string> data,
        CancellationToken cancellationToken = default)
    {
        var (subject, body) = await templateRenderer.RenderAsync(template, data, cancellationToken);
        var provider = ResolveProvider(options.Value.Provider);
        await provider.SendAsync(to, subject, body, cancellationToken);
    }

    private IEmailProvider ResolveProvider(string name) =>
        (name.ToLowerInvariant()) switch
        {
            "smtp" => serviceProvider.GetRequiredService<SmtpEmailProvider>(),
            "sendgrid" or "ses" => serviceProvider.GetRequiredService<ConsoleEmailProvider>(),
            _ => serviceProvider.GetRequiredService<ConsoleEmailProvider>()
        };
}
