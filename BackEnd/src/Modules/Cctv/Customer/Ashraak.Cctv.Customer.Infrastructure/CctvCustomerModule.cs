using Ashraak.Cctv.Customer.Application.Abstractions;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.Cctv.Customer.Infrastructure.Persistence.Repositories;
using Ashraak.Cctv.Customer.Infrastructure.Services;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Cctv.Customer.Infrastructure;

/// <summary>DI composition root for the CCTV Customer module.</summary>
public static class CctvCustomerModule
{
    public static IServiceCollection AddCctvCustomerModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Customer")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Customer database connection string is required.");

        services.AddDbContext<Persistence.CustomerDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_customer");
                npgsql.EnableRetryOnFailure(3);
            });
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<Persistence.CustomerDbContext>());
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<ICustomerNumberGenerator, CustomerNumberGenerator>();
        services.AddScoped<ISiteNumberGenerator, SiteNumberGenerator>();
        services.AddScoped<ICustomerLookupService, CustomerLookupService>();
        services.AddScoped<ISiteLookupService, SiteLookupService>();
        services.AddScoped<ICustomerProvisioningService, CustomerProvisioningService>();
        services.AddScoped<ISiteProvisioningService, SiteProvisioningService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Application.CustomerApplicationAssemblyMarker).Assembly));

        services.AddValidatorsFromAssembly(typeof(Application.CustomerApplicationAssemblyMarker).Assembly);

        return services;
    }
}
