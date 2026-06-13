using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Ashraak.Cctv.Reporting.Infrastructure;

/// <summary>DI composition root for the CCTV Reporting module (Sprint 0 skeleton).</summary>
public static class CctvReportingModule
{
    public static IServiceCollection AddCctvReportingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Application.ReportingApplicationAssemblyMarker).Assembly));

        services.AddValidatorsFromAssembly(typeof(Application.ReportingApplicationAssemblyMarker).Assembly);

        return services;
    }
}
