using Ashraak.ApiKeys.Infrastructure.Persistence;
using Ashraak.Auth.Infrastructure.Persistence;
using Ashraak.BuildingBlocks.Infrastructure.Outbox;
using Ashraak.Files.Infrastructure.Persistence;
using Ashraak.Tenant.Infrastructure.Persistence;
using Ashraak.Users.Infrastructure.Persistence;
using Ashraak.Webhooks.Infrastructure.Persistence;

namespace Ashraak.Api.Extensions;

/// <summary>
/// Registers hosted outbox processors for SQL modules.
/// </summary>
internal static class OutboxExtensions
{
    public static IServiceCollection AddOutboxProcessors(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxProcessorOptions>(configuration.GetSection("Outbox"));

        services.AddHostedService<OutboxProcessorHostedService<AuthDbContext>>();
        services.AddHostedService<OutboxProcessorHostedService<TenantDbContext>>();
        services.AddHostedService<OutboxProcessorHostedService<UsersDbContext>>();
        services.AddHostedService<OutboxProcessorHostedService<FilesDbContext>>();
        services.AddHostedService<OutboxProcessorHostedService<WebhooksDbContext>>();
        services.AddHostedService<OutboxProcessorHostedService<ApiKeysDbContext>>();

        return services;
    }
}
