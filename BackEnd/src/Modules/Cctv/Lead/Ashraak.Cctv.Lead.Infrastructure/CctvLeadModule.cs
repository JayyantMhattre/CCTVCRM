using Ashraak.Cctv.Lead.Application.Abstractions;
using Ashraak.Cctv.Lead.Domain.Repositories;
using Ashraak.Cctv.Lead.Infrastructure.Persistence.Repositories;
using Ashraak.Cctv.Lead.Infrastructure.Services;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Cctv.Lead.Infrastructure;

/// <summary>DI composition root for the CCTV Lead module.</summary>
public static class CctvLeadModule
{
    public static IServiceCollection AddCctvLeadModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Lead")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Lead database connection string is required.");

        services.AddDbContext<Persistence.LeadDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_lead");
                npgsql.EnableRetryOnFailure(3);
            });
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<Persistence.LeadDbContext>());
        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<ILeadNumberGenerator, LeadNumberGenerator>();
        services.AddScoped<ILeadConversionOrchestrator, LeadConversionOrchestrator>();
        services.AddScoped<ILeadLookupService, LeadLookupService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Application.LeadApplicationAssemblyMarker).Assembly));

        services.AddValidatorsFromAssembly(typeof(Application.LeadApplicationAssemblyMarker).Assembly);

        return services;
    }
}
