using Ashraak.Cctv.Ticket.Application.Abstractions;
using Ashraak.Cctv.Ticket.Domain.Repositories;
using Ashraak.Cctv.Ticket.Infrastructure.Persistence.Repositories;
using Ashraak.Cctv.Ticket.Infrastructure.Services;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Cctv.Ticket.Infrastructure;

/// <summary>DI composition root for the CCTV Ticket module.</summary>
public static class CctvTicketModule
{
    public static IServiceCollection AddCctvTicketModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Ticket")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Ticket database connection string is required.");

        services.AddDbContext<Persistence.TicketDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_ticket");
                npgsql.EnableRetryOnFailure(3);
            });
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<Persistence.TicketDbContext>());
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ITicketNumberGenerator, TicketNumberGenerator>();
        services.AddScoped<ITicketLookupService, TicketLookupService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Application.TicketApplicationAssemblyMarker).Assembly));

        services.AddValidatorsFromAssembly(typeof(Application.TicketApplicationAssemblyMarker).Assembly);

        return services;
    }
}
