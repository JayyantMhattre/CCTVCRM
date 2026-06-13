using Ashraak.Cctv.Engineer.Application.Abstractions;
using Ashraak.Cctv.Engineer.Domain.Repositories;
using Ashraak.Cctv.Engineer.Infrastructure.Persistence.Repositories;
using Ashraak.Cctv.Engineer.Infrastructure.Services;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Cctv.Engineer.Infrastructure;

/// <summary>DI composition root for the CCTV Engineer module (D1-8).</summary>
public static class CctvEngineerModule
{
    public static IServiceCollection AddCctvEngineerModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Engineer")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Engineer database connection string is required.");

        services.AddDbContext<Persistence.EngineerDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_engineer");
                npgsql.EnableRetryOnFailure(3);
            });
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<Persistence.EngineerDbContext>());
        services.AddScoped<IEngineerRepository, EngineerRepository>();
        services.AddScoped<IEngineerNumberGenerator, EngineerNumberGenerator>();
        services.AddScoped<IEngineerLookupService, EngineerLookupService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Application.EngineerApplicationAssemblyMarker).Assembly));

        services.AddValidatorsFromAssembly(typeof(Application.EngineerApplicationAssemblyMarker).Assembly);

        return services;
    }
}
