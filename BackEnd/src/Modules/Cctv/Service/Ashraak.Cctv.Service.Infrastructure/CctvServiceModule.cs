using Ashraak.Cctv.Service.Application.Abstractions;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.Cctv.Service.Infrastructure.Persistence.Repositories;
using Ashraak.Cctv.Service.Infrastructure.Services;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Cctv.Service.Infrastructure;

/// <summary>DI composition root for the CCTV Service module.</summary>
public static class CctvServiceModule
{
    public static IServiceCollection AddCctvServiceModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Service")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Service database connection string is required.");

        services.AddDbContext<Persistence.ServiceDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_service");
                npgsql.EnableRetryOnFailure(3);
            });
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<Persistence.ServiceDbContext>());
        services.AddScoped<IServiceScheduleRepository, ServiceScheduleRepository>();
        services.AddScoped<IServiceVisitRepository, ServiceVisitRepository>();
        services.AddScoped<IServiceScheduleNumberGenerator, ServiceScheduleNumberGenerator>();
        services.AddScoped<IScheduleLookupService, ScheduleLookupService>();
        services.AddScoped<IVisitLookupService, VisitLookupService>();
        services.AddScoped<IScheduleGenerationService, ScheduleGenerationService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Application.ServiceApplicationAssemblyMarker).Assembly));

        services.AddValidatorsFromAssembly(typeof(Application.ServiceApplicationAssemblyMarker).Assembly);

        return services;
    }
}
