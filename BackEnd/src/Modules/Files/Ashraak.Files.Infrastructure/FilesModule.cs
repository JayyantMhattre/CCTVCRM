using Ashraak.Files.Application;
using Ashraak.Files.Application.Commands.UploadFile;
using Ashraak.Files.Domain.Repositories;
using Ashraak.Files.Infrastructure.Persistence;
using Ashraak.Files.Infrastructure.Persistence.Repositories;
using Ashraak.Files.Infrastructure.Services;
using Ashraak.Files.Infrastructure.Storage;
using Ashraak.SharedKernel.Contracts.Storage.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ashraak.Files.Infrastructure;

public static class FilesModule
{
    public static IServiceCollection AddFilesModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));
        services.Configure<StorageValidationOptions>(options =>
        {
            var storage = configuration.GetSection(StorageOptions.SectionName).Get<StorageOptions>() ?? new StorageOptions();
            options.MaxUploadBytes = storage.MaxUploadBytes;
            options.AllowedContentTypes = storage.AllowedContentTypes;
        });

        var connectionString = configuration.GetConnectionString("Files")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Files database connection string is required.");

        services.AddDbContext<FilesDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__ef_migrations_history", "files");
                npgsql.EnableRetryOnFailure(3);
            });
            options.AddInterceptors(sp.GetServices<IInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<FilesDbContext>());
        services.AddScoped<IFileRecordRepository, FileRecordRepository>();

        RegisterStorageProvider(services, configuration);
        services.AddScoped<IStorageHealthIndicator, StorageHealthIndicator>();

        services.AddScoped<IFileStorage, FileStorageService>();
        services.AddSingleton<IFileScanService, StubFileScanService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(FilesApplicationAnchor).Assembly));

        return services;
    }

    private static void RegisterStorageProvider(IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration.GetSection(StorageOptions.SectionName)["Provider"] ?? "Local";

        services.AddHttpClient("S3");
        services.AddHttpClient("Azure");

        switch (provider.Trim().ToUpperInvariant())
        {
            case "S3":
                services.AddScoped<IStorageProvider, S3CompatibleStorageProvider>();
                break;
            case "AZURE":
                services.AddScoped<IStorageProvider, AzureBlobStorageProvider>();
                break;
            default:
                services.AddScoped<IStorageProvider, LocalDiskStorageProvider>();
                break;
        }
    }
}
