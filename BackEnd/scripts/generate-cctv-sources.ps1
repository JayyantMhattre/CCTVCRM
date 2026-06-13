$ErrorActionPreference = 'Stop'
$root = Join-Path $PSScriptRoot '..'

$modules = @(
    @{ Name = 'Lead'; Schema = 'cctv_lead'; HasDb = $true; Route = 'leads'; Tag = 'CCTV — Lead' },
    @{ Name = 'Customer'; Schema = 'cctv_customer'; HasDb = $true; Route = 'customers'; Tag = 'CCTV — Customer' },
    @{ Name = 'Amc'; Schema = 'cctv_amc'; HasDb = $true; Route = 'amc'; Tag = 'CCTV — AMC' },
    @{ Name = 'Service'; Schema = 'cctv_service'; HasDb = $true; Route = 'service'; Tag = 'CCTV — Service' },
    @{ Name = 'Ticket'; Schema = 'cctv_ticket'; HasDb = $true; Route = 'tickets'; Tag = 'CCTV — Ticket' },
    @{ Name = 'Engineer'; Schema = 'cctv_engineer'; HasDb = $true; Route = 'engineers'; Tag = 'CCTV — Engineer' },
    @{ Name = 'Invoice'; Schema = 'cctv_invoice'; HasDb = $true; Route = 'invoices'; Tag = 'CCTV — Invoice' },
    @{ Name = 'Reporting'; Schema = ''; HasDb = $false; Route = 'reports'; Tag = 'CCTV — Reporting' }
)

foreach ($m in $modules) {
    $ns = "Ashraak.Cctv.$($m.Name)"
    $domainDir = Join-Path $root "src/Modules/Cctv/$($m.Name)/$ns.Domain"
    $appDir = Join-Path $root "src/Modules/Cctv/$($m.Name)/$ns.Application"
    $infraDir = Join-Path $root "src/Modules/Cctv/$($m.Name)/$ns.Infrastructure"
    $apiDir = Join-Path $root "src/Modules/Cctv/$($m.Name)/$ns.Api"

    @"
namespace $ns.Domain;

/// <summary>Assembly anchor for the $($m.Name) domain layer (Sprint 0 skeleton).</summary>
public static class $($m.Name)ModuleAnchor;
"@ | Set-Content (Join-Path $domainDir "$($m.Name)ModuleAnchor.cs") -Encoding UTF8

    @"
namespace $ns.Application;

/// <summary>Assembly anchor for MediatR and FluentValidation registration.</summary>
public static class $($m.Name)ApplicationAssemblyMarker;
"@ | Set-Content (Join-Path $appDir "$($m.Name)ApplicationAssemblyMarker.cs") -Encoding UTF8

    if ($m.HasDb) {
        $persistDir = Join-Path $infraDir 'Persistence'
        New-Item -ItemType Directory -Force -Path $persistDir | Out-Null
        @"
using Ashraak.BuildingBlocks.Infrastructure.Persistence;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace $ns.Infrastructure.Persistence;

/// <summary>EF Core context for schema <c>$($m.Schema)</c>. Sprint 0 — no entities.</summary>
public sealed class $($m.Name)DbContext : BaseDbContext, IUnitOfWork
{
    public $($m.Name)DbContext(DbContextOptions<$($m.Name)DbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("$($m.Schema)");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof($($m.Name)DbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
"@ | Set-Content (Join-Path $infraDir "Persistence\$($m.Name)DbContext.cs") -Encoding UTF8
    }

    $addDb = if ($m.HasDb) {
@"
        var connectionString = configuration.GetConnectionString("$($m.Name)")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("$($m.Name) database connection string is required.");

        services.AddDbContext<Persistence.$($m.Name)DbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "$($m.Schema)");
                npgsql.EnableRetryOnFailure(3);
            });
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<Persistence.$($m.Name)DbContext>());
"@
    } else { '' }

    @"
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
$(if ($m.HasDb) { 'using Microsoft.EntityFrameworkCore;' + "`nusing Microsoft.EntityFrameworkCore.Diagnostics;" + "`nusing Ashraak.SharedKernel.Interfaces;" })

namespace $ns.Infrastructure;

/// <summary>DI composition root for the CCTV $($m.Name) module (Sprint 0 skeleton).</summary>
public static class Cctv$($m.Name)Module
{
    public static IServiceCollection AddCctv$($m.Name)Module(
        this IServiceCollection services,
        IConfiguration configuration)
    {
$addDb
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Application.$($m.Name)ApplicationAssemblyMarker).Assembly));

        services.AddValidatorsFromAssembly(typeof(Application.$($m.Name)ApplicationAssemblyMarker).Assembly);

        return services;
    }
}
"@ | Set-Content (Join-Path $infraDir "Cctv$($m.Name)Module.cs") -Encoding UTF8

    $endpointsDir = Join-Path $apiDir 'Endpoints'
    New-Item -ItemType Directory -Force -Path $endpointsDir | Out-Null

    @"
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace $ns.Api.Endpoints;

/// <summary>Route group placeholder for $($m.Name) (Sprint 0 — no business endpoints).</summary>
public static class Cctv$($m.Name)Endpoints
{
    public static IEndpointRouteBuilder MapCctv$($m.Name)Endpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGroup("/cctv/$($m.Route)")
            .WithTags("$($m.Tag)")
            .RequireAuthorization();

        return routeBuilder;
    }
}
"@ | Set-Content (Join-Path $endpointsDir "Cctv$($m.Name)Endpoints.cs") -Encoding UTF8
}

Write-Host 'CCTV module source files generated.'
