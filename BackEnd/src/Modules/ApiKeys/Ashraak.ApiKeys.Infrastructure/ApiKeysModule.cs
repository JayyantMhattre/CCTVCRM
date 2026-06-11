using Ashraak.ApiKeys.Application;
using Ashraak.ApiKeys.Application.Abstractions;
using Ashraak.ApiKeys.Domain.Repositories;
using Ashraak.ApiKeys.Infrastructure.Observability;
using Ashraak.ApiKeys.Infrastructure.Outbox;
using Ashraak.ApiKeys.Infrastructure.Persistence;
using Ashraak.ApiKeys.Infrastructure.Persistence.Repositories;
using Ashraak.ApiKeys.Infrastructure.Security;
using Ashraak.ApiKeys.Infrastructure.Services;
using Ashraak.BuildingBlocks.Infrastructure.Outbox;
using Ashraak.SharedKernel.Contracts.ApiKeys.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.ApiKeys.Infrastructure;

public static class ApiKeysModule
{
    public static IServiceCollection AddApiKeysModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ApiKeysOptions>(configuration.GetSection(ApiKeysOptions.SectionName));

        var connectionString = configuration.GetConnectionString("ApiKeys")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ApiKeys database connection string is required.");

        services.AddDbContext<ApiKeysDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "apikeys");
                npgsql.EnableRetryOnFailure(3);
            });
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApiKeysDbContext>());
        services.AddScoped<IOutboxWriter, ApiKeysOutboxWriter>();

        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddSingleton<IApiKeyGenerator, ApiKeyGenerator>();
        services.AddSingleton<IApiKeyHasher, ApiKeyHasher>();
        services.AddScoped<IApiKeyValidator, ApiKeyValidator>();
        services.AddSingleton<ApiKeyMetrics>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApiKeysApplicationAnchor).Assembly));

        return services;
    }
}
