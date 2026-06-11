using Ashraak.Audit.Application.EventHandlers;
using Ashraak.Audit.Infrastructure.Interceptors;
using Ashraak.Audit.Infrastructure.Repositories;
using Ashraak.Audit.Infrastructure.Services;
using Ashraak.SharedKernel.Contracts.Audit.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Ashraak.Audit.Infrastructure;

/// <summary>
/// DI composition root for the Audit module.
/// Registers the MongoDB client/database, async audit queue, background writer,
/// audit capture services, EF interceptors, and MediatR handlers.
/// </summary>
/// <remarks>
/// Called once from <c>Ashraak.Api.Program</c>. The <see cref="IMongoClient"/> and
/// <see cref="IMongoDatabase"/> are registered as singletons because MongoDB's driver
/// is fully thread-safe and designed for long-lived connections.
/// </remarks>
public static class AuditModule
{
    /// <summary>
    /// Registers all Audit module services with the DI container.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="configuration">
    /// Application configuration used to read <c>ConnectionStrings:MongoDB</c>.
    /// The database name is derived from the connection string; defaults to <c>"ashraak_audit"</c>.
    /// </param>
    /// <returns>The same <paramref name="services"/> for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the MongoDB connection string is absent.</exception>
    public static IServiceCollection AddAuditModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB")
            ?? throw new InvalidOperationException("MongoDB connection string 'MongoDB' is required.");

        var mongoUrl = new MongoUrl(connectionString);
        var mongoClient = new MongoClient(mongoUrl);
        var database = mongoClient.GetDatabase(mongoUrl.DatabaseName ?? "ashraak_audit");

        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddSingleton(database);

        EnsureIndexes(database);

        // Queue + background worker provide async/non-blocking audit persistence.
        services.AddSingleton<IAuditWriteQueue, AuditWriteQueue>();
        services.AddSingleton<IAuditService, AuditRepository>();
        services.AddScoped<Application.Abstractions.IAuditReadService, AuditReadService>();
        services.AddHostedService<AuditMongoWriterHostedService>();

        // Register as generic EF interceptor so feature modules can attach it
        // without directly referencing Audit infrastructure types.
        services.AddSingleton<IInterceptor, AuditEntityChangeInterceptor>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DomainEventAuditHandler).Assembly));

        return services;
    }

    /// <summary>
    /// Creates MongoDB indexes required for efficient audit log queries.
    /// Safe to call on startup — MongoDB skips creation if indexes already exist.
    /// </summary>
    /// <param name="database">The target MongoDB database.</param>
    private static void EnsureIndexes(IMongoDatabase database)
    {
        var collection = database.GetCollection<Ashraak.Audit.Domain.Entities.AuditEntry>("audit_entries");

        var indexModels = new[]
        {
            new CreateIndexModel<Ashraak.Audit.Domain.Entities.AuditEntry>(
                Builders<Ashraak.Audit.Domain.Entities.AuditEntry>.IndexKeys.Ascending(e => e.TenantId),
                new CreateIndexOptions { Name = "ix_tenant_id" }),
            new CreateIndexModel<Ashraak.Audit.Domain.Entities.AuditEntry>(
                Builders<Ashraak.Audit.Domain.Entities.AuditEntry>.IndexKeys
                    .Ascending(e => e.TenantId)
                    .Descending(e => e.OccurredOnUtc),
                new CreateIndexOptions { Name = "ix_tenant_occurred" }),
        };

        collection.Indexes.CreateMany(indexModels);
    }
}
