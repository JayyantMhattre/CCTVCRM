using Ashraak.Cctv.Invoice.Application.Abstractions;
using Ashraak.Cctv.Invoice.Domain.Repositories;
using Ashraak.Cctv.Invoice.Infrastructure.Persistence.Repositories;
using Ashraak.Cctv.Invoice.Infrastructure.Services;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Cctv.Invoice.Infrastructure;

/// <summary>DI composition root for the CCTV Invoice module.</summary>
public static class CctvInvoiceModule
{
    public static IServiceCollection AddCctvInvoiceModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Invoice")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Invoice database connection string is required.");

        services.AddDbContext<Persistence.InvoiceDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "cctv_invoice");
                npgsql.EnableRetryOnFailure(3);
            });
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<Persistence.InvoiceDbContext>());
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IInvoiceNumberGenerator, InvoiceNumberGenerator>();
        services.AddScoped<IInvoiceLookupService, InvoiceLookupService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Application.InvoiceApplicationAssemblyMarker).Assembly));

        services.AddValidatorsFromAssembly(typeof(Application.InvoiceApplicationAssemblyMarker).Assembly);

        return services;
    }
}
