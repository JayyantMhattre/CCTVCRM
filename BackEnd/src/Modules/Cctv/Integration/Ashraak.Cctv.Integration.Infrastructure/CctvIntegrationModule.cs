using Ashraak.BuildingBlocks.Infrastructure.Outbox;
using Ashraak.Cctv.Amc.Infrastructure.Persistence;
using Ashraak.Cctv.Customer.Infrastructure.Persistence;
using Ashraak.Cctv.Engineer.Infrastructure.Persistence;
using Ashraak.Cctv.Integration.Application;
using Ashraak.Cctv.Integration.Application.Abstractions;
using Ashraak.Cctv.Integration.Infrastructure.Notifications;
using Ashraak.Cctv.Integration.Infrastructure.Seeding;
using Ashraak.Cctv.Integration.Infrastructure.Services;
using Ashraak.Cctv.Invoice.Infrastructure.Persistence;
using Ashraak.Cctv.Lead.Infrastructure.Persistence;
using Ashraak.Cctv.Service.Infrastructure.Persistence;
using Ashraak.Cctv.Ticket.Infrastructure.Persistence;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Cctv.Integration.Infrastructure;

/// <summary>DI registration for CCTV integration services (SMS, PDF, notifications, RBAC seed).</summary>
public static class CctvIntegrationModule
{
    public static IServiceCollection AddCctvIntegrationModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CctvNotificationOptions>(configuration.GetSection("Cctv:Notifications"));
        services.Configure<SmsOptions>(configuration.GetSection(SmsOptions.SectionName));

        services.AddHttpClient("CctvSms");

        services.AddSingleton<ISmsProvider, ConfiguredSmsProvider>();
        services.AddSingleton<IPdfGenerationService, PdfGenerationService>();
        services.AddScoped<ICctvNotificationDispatcher, CctvNotificationDispatcher>();
        services.AddScoped<ICctvFileStore, CctvFileStore>();
        services.AddScoped<ICctvReportingDataProvider, CctvReportingDataProvider>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(CctvIntegrationModule).Assembly));

        services.AddHostedService<OutboxProcessorHostedService<LeadDbContext>>();
        services.AddHostedService<OutboxProcessorHostedService<CustomerDbContext>>();
        services.AddHostedService<OutboxProcessorHostedService<AmcDbContext>>();
        services.AddHostedService<OutboxProcessorHostedService<ServiceDbContext>>();
        services.AddHostedService<OutboxProcessorHostedService<TicketDbContext>>();
        services.AddHostedService<OutboxProcessorHostedService<EngineerDbContext>>();
        services.AddHostedService<OutboxProcessorHostedService<InvoiceDbContext>>();

        services.AddHostedService<CctvRbacSeedHostedService>();
        services.AddHostedService<CctvAmcExpiryReminderHostedService>();

        return services;
    }
}
