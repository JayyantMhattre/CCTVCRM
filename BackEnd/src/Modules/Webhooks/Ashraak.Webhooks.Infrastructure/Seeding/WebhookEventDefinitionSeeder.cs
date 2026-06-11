using Ashraak.Webhooks.Domain.Aggregates.WebhookEventDefinition;
using Ashraak.Webhooks.Domain.Repositories;
using Ashraak.Webhooks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ashraak.Webhooks.Infrastructure.Seeding;

/// <summary>Seeds foundation webhook event catalog entries (W1).</summary>
internal sealed class WebhookEventDefinitionSeeder(
    IServiceScopeFactory scopeFactory,
    ILogger<WebhookEventDefinitionSeeder> logger) : IHostedService
{
    private static readonly (string EventName, string Description)[] FoundationEvents =
    [
        ("user.created", "A user account was created."),
        ("user.updated", "A user profile was updated."),
        ("user.invited", "A user was invited to the tenant."),
        ("tenant.created", "A tenant workspace was provisioned."),
        ("tenant.updated", "Tenant settings were updated."),
        ("tenant.suspended", "A tenant was suspended."),
        ("file.uploaded", "A file was uploaded."),
        ("file.deleted", "A file was deleted."),
        ("notification.sent", "A notification was sent."),
        ("notification.failed", "A notification delivery failed."),
        ("auth.password.changed", "A user password was changed.")
    ];

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WebhooksDbContext>();
        var store = scope.ServiceProvider.GetRequiredService<IWebhookEventDefinitionStore>();

        await context.Database.EnsureCreatedAsync(cancellationToken);

        foreach (var (eventName, description) in FoundationEvents)
        {
            var existing = await store.GetByEventNameAsync(eventName, cancellationToken);
            if (existing is not null)
                continue;

            store.Add(WebhookEventDefinition.Seed(eventName, description));
            logger.LogInformation("Seeded webhook event definition: {EventName}", eventName);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
