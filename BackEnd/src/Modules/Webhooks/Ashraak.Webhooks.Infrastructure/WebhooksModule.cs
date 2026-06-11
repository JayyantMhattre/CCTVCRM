using Ashraak.BuildingBlocks.Infrastructure.Outbox;
using Ashraak.SharedKernel.Contracts.Webhooks.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Webhooks.Application;
using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Domain.Repositories;
using Ashraak.Webhooks.Infrastructure.Delivery;
using Ashraak.Webhooks.Infrastructure.Outbox;
using Ashraak.Webhooks.Infrastructure.Persistence;
using Ashraak.Webhooks.Infrastructure.Persistence.Repositories;
using Ashraak.Webhooks.Infrastructure.Security;
using Ashraak.Webhooks.Infrastructure.Retry;
using Ashraak.Webhooks.Infrastructure.Seeding;
using Ashraak.Webhooks.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Webhooks.Infrastructure;

public static class WebhooksModule
{
    public const string WebhookHttpClientName = WebhookDeliveryService.HttpClientName;

    public static IServiceCollection AddWebhooksModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<WebhookOptions>(configuration.GetSection(WebhookOptions.SectionName));
        services.Configure<WebhookDeliveryOptions>(configuration.GetSection(WebhookDeliveryOptions.SectionName));
        services.Configure<WebhookRetryOptions>(configuration.GetSection(WebhookRetryOptions.SectionName));
        services.Configure<WebhookDLQOptions>(configuration.GetSection(WebhookDLQOptions.SectionName));

        var connectionString = configuration.GetConnectionString("Webhooks")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Webhooks database connection string is required.");

        services.AddDataProtection();

        services.AddDbContext<WebhooksDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "webhooks");
                npgsql.EnableRetryOnFailure(3);
            });
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WebhooksDbContext>());
        services.AddScoped<IOutboxWriter, WebhooksOutboxWriter>();

        services.AddScoped<IWebhookSubscriptionStore, WebhookSubscriptionRepository>();
        services.AddScoped<IWebhookSubscriptionRepository, WebhookSubscriptionRepository>();
        services.AddScoped<IWebhookEventDefinitionStore, WebhookEventDefinitionRepository>();
        services.AddScoped<IWebhookDeliveryStore, WebhookDeliveryRepository>();
        services.AddScoped<IWebhookDeliveryRepository, WebhookDeliveryRepository>();
        services.AddScoped<IWebhookDeadLetterStore, WebhookDeadLetterRepository>();
        services.AddScoped<IWebhookDeadLetterRepository, WebhookDeadLetterRepository>();

        services.AddSingleton<IWebhookSecretGenerator, WebhookSecretGenerator>();
        services.AddScoped<IWebhookSecretProtector, WebhookSecretProtector>();
        services.AddScoped<IWebhookPublisher, WebhookPublisher>();

        services.AddSingleton<WebhookDeliveryQueue>();
        services.AddSingleton<IWebhookDeliveryQueue>(sp => sp.GetRequiredService<WebhookDeliveryQueue>());
        services.AddSingleton<WebhookDeliveryMetrics>();
        services.AddSingleton<IWebhookSignatureService, WebhookSignatureService>();
        services.AddSingleton<IWebhookFailureClassifier, WebhookFailureClassifier>();
        services.AddSingleton<IWebhookRetryBackoffCalculator, WebhookRetryBackoffCalculator>();
        services.AddScoped<IWebhookPayloadBuilder, WebhookPayloadBuilder>();
        services.AddScoped<IWebhookDispatcher, WebhookDispatcher>();
        services.AddScoped<IWebhookDeliveryService, WebhookDeliveryService>();
        services.AddScoped<IWebhookDeliveryOutcomeHandler, WebhookDeliveryOutcomeHandler>();
        services.AddScoped<IDeadLetterService, DeadLetterService>();

        services.AddHttpClient(WebhookHttpClientName);

        services.AddHostedService<WebhookEventDefinitionSeeder>();
        services.AddHostedService<WebhookDeliveryHostedService>();
        services.AddHostedService<WebhookRetryHostedService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(WebhooksApplicationAnchor).Assembly));

        return services;
    }
}
