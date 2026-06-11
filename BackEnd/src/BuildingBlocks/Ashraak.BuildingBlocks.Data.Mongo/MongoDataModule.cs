using Ashraak.BuildingBlocks.Data.Abstractions.Common;
using Ashraak.BuildingBlocks.Data.Abstractions.Configuration;
using Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Ashraak.BuildingBlocks.Data.Mongo;

/// <summary>
/// DI registration helpers for the MongoDB data layer.
///
/// <para>
/// Call <see cref="AddMongoDataLayer"/> once at application startup (typically in
/// the API host's <c>Program.cs</c> or a cross-cutting infrastructure module).
/// Subsequent calls for different entity types only need to register their
/// concrete <see cref="MongoDataRepository{T}"/> as <see cref="IDataRepository{T}"/>.
/// </para>
///
/// <example>
/// <code>
/// // In Program.cs or a module registration:
/// services.AddMongoDataLayer(configuration);
///
/// // Per-entity registration (add once per entity type that uses MongoDB):
/// services.AddScoped&lt;IDataRepository&lt;AuditLog&gt;&gt;(sp =>
///     new MongoDataRepository&lt;AuditLog&gt;(
///         sp.GetRequiredService&lt;IMongoDatabase&gt;(),
///         sp.GetRequiredService&lt;MongoDataUnitOfWork&gt;(),
///         tenantContext: sp.GetService&lt;ITenantContext&gt;()));
/// </code>
/// </example>
/// </summary>
public static class MongoDataModule
{
    /// <summary>
    /// Registers the MongoDB <see cref="IMongoClient"/>, <see cref="IMongoDatabase"/>,
    /// <see cref="MongoDataUnitOfWork"/> (<see cref="IDataUnitOfWork"/>), and
    /// <see cref="MongoIndexService"/> in the DI container.
    ///
    /// <para>
    /// Also configures global MongoDB serialisation conventions:
    /// <list type="bullet">
    ///   <item>camelCase element names.</item>
    ///   <item>Enum values stored as strings.</item>
    ///   <item><see cref="Guid"/> stored as standard GUID format (not legacy binary).</item>
    ///   <item>Ignores extra BSON elements on deserialisation (forward compatibility).</item>
    /// </list>
    /// </para>
    /// </summary>
    public static IServiceCollection AddMongoDataLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = new DatabaseOptions();
        configuration.GetSection(DatabaseOptions.SectionName).Bind(options);

        // Fall back to the dedicated MongoDB connection string key when the generic
        // Database:ConnectionString is not set (common when the project uses both SQL
        // and MongoDB simultaneously for different modules).
        var connectionString = options.ConnectionString.Length > 0
            ? options.ConnectionString
            : configuration.GetConnectionString("MongoDb")
              ?? throw new InvalidOperationException(
                  "MongoDB connection string not found.  " +
                  "Set 'Database:ConnectionString' or 'ConnectionStrings:MongoDb'.");

        var databaseName = options.DatabaseName.Length > 0
            ? options.DatabaseName
            : configuration["MongoDb:DatabaseName"]
              ?? "ashraak";

        // ── Register serialisation conventions (idempotent guard via ConventionRegistry) ──
        RegisterConventions();

        // ── IMongoClient — singleton, thread-safe, holds the connection pool ──
        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

        // ── IMongoDatabase — resolved from the singleton client ────────────────
        services.AddSingleton(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));

        // ── MongoDataUnitOfWork — scoped per request ───────────────────────────
        services.AddScoped<MongoDataUnitOfWork>();
        services.AddScoped<IDataUnitOfWork>(sp => sp.GetRequiredService<MongoDataUnitOfWork>());

        // ── MongoIndexService — singleton because it only creates, never queries ─
        services.AddSingleton<MongoIndexService>();

        return services;
    }

    /// <summary>
    /// Shorthand to register a <see cref="MongoDataRepository{T}"/> as
    /// <see cref="IDataRepository{T}"/> for the given entity type.
    /// </summary>
    public static IServiceCollection AddMongoRepository<T>(
        this IServiceCollection services,
        string? collectionName = null)
        where T : BaseDataEntity
    {
        services.AddScoped<IDataRepository<T>>(sp =>
            new MongoDataRepository<T>(
                sp.GetRequiredService<IMongoDatabase>(),
                sp.GetRequiredService<MongoDataUnitOfWork>(),
                collectionName,
                sp.GetService<ITenantContext>()));

        return services;
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private static bool _conventionsRegistered;
    private static readonly Lock _lock = new();

    private static void RegisterConventions()
    {
        lock (_lock)
        {
            if (_conventionsRegistered) return;

            // Convention pack applied to every MongoDB serialisation in this process.
            var conventions = new ConventionPack
            {
                // Serialize property names as camelCase in BSON documents.
                new CamelCaseElementNameConvention(),

                // Serialize enums as strings ("Active") instead of integers (1).
                new EnumRepresentationConvention(BsonType.String),

                // Ignore BSON fields that don't map to a C# property —
                // allows schema evolution without crashing on old documents.
                new IgnoreExtraElementsConvention(ignoreExtraElements: true),

                // Exclude null values from BSON serialisation to keep documents lean.
                new IgnoreIfNullConvention(ignoreIfNull: true),
            };

            ConventionRegistry.Register("Ashraak.DataLayer", conventions, _ => true);

            // Serialize Guid as standard UUID string format, not the legacy binary subtype 3.
            // This makes GUIDs human-readable in MongoDB Compass and consistent with REST APIs.
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            _conventionsRegistered = true;
        }
    }
}
