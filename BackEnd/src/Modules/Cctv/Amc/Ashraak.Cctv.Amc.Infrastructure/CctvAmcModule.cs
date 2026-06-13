using Ashraak.Cctv.Amc.Application.Abstractions;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.Cctv.Amc.Infrastructure.Persistence.Repositories;
using Ashraak.Cctv.Amc.Infrastructure.Services;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Cctv.Amc.Infrastructure;

/// <summary>DI composition root for the CCTV AMC module.</summary>
public static class CctvAmcModule
{
    public static IServiceCollection AddCctvAmcModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Amc")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Amc database connection string is required.");

        services.AddDbContext<Persistence.AmcDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_amc");
                npgsql.EnableRetryOnFailure(3);
            });
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<Persistence.AmcDbContext>());
        services.AddScoped<IAmcPlanRepository, AmcPlanRepository>();
        services.AddScoped<IAmcContractRepository, AmcContractRepository>();
        services.AddScoped<IAmcContractNumberGenerator, AmcContractNumberGenerator>();
        services.AddScoped<IAmcPlanLookupService, AmcPlanLookupService>();
        services.AddScoped<IAmcContractLookupService, AmcContractLookupService>();
        services.AddScoped<IAmcContractProvisioningService, AmcContractProvisioningService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Application.AmcApplicationAssemblyMarker).Assembly));

        services.AddValidatorsFromAssembly(typeof(Application.AmcApplicationAssemblyMarker).Assembly);

        return services;
    }
}
