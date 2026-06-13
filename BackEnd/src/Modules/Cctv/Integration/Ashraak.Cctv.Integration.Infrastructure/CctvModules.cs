using Ashraak.Cctv.Amc.Infrastructure;
using Ashraak.Cctv.Customer.Infrastructure;
using Ashraak.Cctv.Engineer.Infrastructure;
using Ashraak.Cctv.Invoice.Infrastructure;
using Ashraak.Cctv.Lead.Infrastructure;
using Ashraak.Cctv.Reporting.Infrastructure;
using Ashraak.Cctv.Service.Infrastructure;
using Ashraak.Cctv.Ticket.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Cctv.Integration.Infrastructure;

/// <summary>Registers all CCTV business modules with the host (Layer 2 — Sprint 0).</summary>
public static class CctvModules
{
    public static IServiceCollection AddCctvModules(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCctvIntegrationModule(configuration);
        services.AddCctvLeadModule(configuration);
        services.AddCctvCustomerModule(configuration);
        services.AddCctvAmcModule(configuration);
        services.AddCctvServiceModule(configuration);
        services.AddCctvTicketModule(configuration);
        services.AddCctvEngineerModule(configuration);
        services.AddCctvInvoiceModule(configuration);
        services.AddCctvReportingModule(configuration);
        return services;
    }
}
