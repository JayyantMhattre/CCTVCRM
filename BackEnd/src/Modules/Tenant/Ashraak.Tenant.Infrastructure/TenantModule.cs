using Ashraak.SharedKernel.Contracts.Tenant.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Tenant.Application.Commands.ProvisionTenant;
using Ashraak.Tenant.Domain.Repositories;
using Ashraak.Tenant.Infrastructure.Persistence;
using Ashraak.Tenant.Infrastructure.Persistence.Repositories;
using Ashraak.Tenant.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Tenant.Infrastructure;

/// <summary>
/// DI composition root for the Tenant module.
/// Registers <c>TenantDbContext</c>, the <c>ITenantRepository</c>, the cross-module
/// <c>ITenantService</c>, MediatR handlers, and FluentValidation validators.
/// </summary>
/// <remarks>
/// Called once from <c>Ashraak.Api.Program</c> before any module that depends on
/// <see cref="ITenantService"/> (e.g. Auth, Users) is registered.
/// </remarks>
public static class TenantModule
{
    /// <summary>
    /// Registers all Tenant module services with the DI container.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="configuration">
    /// Application configuration used to resolve the <c>ConnectionStrings:Tenant</c>
    /// or <c>ConnectionStrings:DefaultConnection</c> value.
    /// </param>
    /// <returns>The same <paramref name="services"/> for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no database connection string is configured.</exception>
    public static IServiceCollection AddTenantModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Tenant")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Tenant database connection string is required.");

        services.AddDbContext<TenantDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "tenant");
                npgsql.EnableRetryOnFailure(3);
            });

            // Dynamically resolve interceptors so Tenant remains decoupled from Audit implementation.
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TenantDbContext>());
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITenantService, TenantService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ProvisionTenantCommand).Assembly));

        services.AddValidatorsFromAssembly(typeof(ProvisionTenantCommand).Assembly);

        return services;
    }
}
