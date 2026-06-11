using Ashraak.BuildingBlocks.Data.Abstractions.Configuration;
using Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;
using Ashraak.BuildingBlocks.Data.Sql.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Ashraak.BuildingBlocks.Data.Sql;

/// <summary>
/// DI registration helpers for the SQL data layer.
///
/// <para>
/// Call <see cref="AddSqlDataLayer{TContext}"/> once per module in your module's
/// <c>ServiceCollectionExtensions.AddXxxModule</c> method.  It reads
/// <c>Database:Provider</c> from configuration and registers the appropriate
/// EF Core provider and Dapper connection factory.
/// </para>
///
/// <para>
/// <b>Multi-module usage</b>: Each module passes its own <typeparamref name="TContext"/>
/// (e.g. <c>AuthDbContext</c>, <c>TenantDbContext</c>).  The generic repository and
/// unit-of-work resolve the correct context because they are registered with
/// <typeparamref name="TContext"/> as the service key.
/// </para>
///
/// <example>
/// <code>
/// // In BillingModule ServiceCollectionExtensions:
/// services.AddSqlDataLayer&lt;BillingDbContext&gt;(configuration, "billing");
/// services.AddScoped&lt;IDataRepository&lt;Invoice&gt;&gt;(sp =>
///     new GenericSqlRepository&lt;Invoice&gt;(
///         sp.GetRequiredService&lt;BillingDbContext&gt;(),
///         sp.GetRequiredService&lt;IDbConnectionFactory&gt;(),
///         sp.GetService&lt;ITenantContext&gt;()));
/// </code>
/// </example>
/// </summary>
public static class SqlDataModule
{
    /// <summary>
    /// Registers an EF Core <typeparamref name="TContext"/> with the provider selected
    /// in <c>Database:Provider</c>, plus the matching Dapper connection factory and
    /// the <see cref="IDataUnitOfWork"/> bound to that context.
    /// </summary>
    /// <typeparam name="TContext">The module's concrete <see cref="DbContext"/>.</typeparam>
    /// <param name="services">The DI container.</param>
    /// <param name="configuration">Application configuration root.</param>
    /// <param name="schemaOrPrefix">
    /// Optional schema / table-prefix used by the module (passed to the context's
    /// <c>OnModelCreating</c>).  Not applied by this method directly — the context
    /// uses it internally.
    /// </param>
    public static IServiceCollection AddSqlDataLayer<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string? schemaOrPrefix = null)
        where TContext : DbContext
    {
        var options = new DatabaseOptions();
        configuration.GetSection(DatabaseOptions.SectionName).Bind(options);

        var connectionString = options.ConnectionString.Length > 0
            ? options.ConnectionString
            : configuration.GetConnectionString("DefaultConnection")
              ?? throw new InvalidOperationException(
                  $"No connection string found.  Set 'Database:ConnectionString' " +
                  $"or 'ConnectionStrings:DefaultConnection' in configuration.");

        // ── Register EF Core with the selected provider ───────────────────────
        services.AddDbContext<TContext>(dbOptions =>
        {
            ConfigureProvider(dbOptions, options.Provider, connectionString, options);
        });

        // ── Dapper connection factory ────────────────────────────────────────
        // Register as the abstract IDbConnectionFactory so repositories only depend
        // on the interface, not the concrete provider class.
        services.AddScoped<IDbConnectionFactory>(sp =>
            CreateConnectionFactory(options.Provider, configuration));

        // ── Unit of work bound to this module's context ──────────────────────
        // The factory delegate resolves TContext so each module gets its own UoW.
        services.AddScoped<IDataUnitOfWork>(sp =>
            new SqlDataUnitOfWork(sp.GetRequiredService<TContext>()));

        return services;
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>Configures the <see cref="DbContextOptionsBuilder"/> for the selected provider.</summary>
    private static void ConfigureProvider(
        DbContextOptionsBuilder dbOptions,
        DatabaseProviderType provider,
        string connectionString,
        DatabaseOptions options)
    {
        _ = provider switch
        {
            DatabaseProviderType.SqlServer =>
                dbOptions.UseSqlServer(connectionString, sql =>
                {
                    sql.CommandTimeout(options.CommandTimeout);
                    sql.EnableRetryOnFailure(maxRetryCount: 3);
                }),

            DatabaseProviderType.PostgreSql =>
                dbOptions.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.CommandTimeout(options.CommandTimeout);
                    npgsql.EnableRetryOnFailure(maxRetryCount: 3);
                }),

            DatabaseProviderType.MySql =>
                // Pomelo requires the server version to configure query hints correctly.
                // AutoDetect adds a lightweight version-probe on first connection.
                dbOptions.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mysql => mysql.CommandTimeout(options.CommandTimeout)),

            DatabaseProviderType.Oracle =>
                dbOptions.UseOracle(connectionString, oracle =>
                {
                    oracle.CommandTimeout(options.CommandTimeout);
                }),

            DatabaseProviderType.MongoDB =>
                throw new InvalidOperationException(
                    "MongoDB is not supported by SqlDataModule.  " +
                    "Use MongoDataModule.AddMongoDataLayer() instead."),

            _ => throw new NotSupportedException(
                $"Database provider '{provider}' is not supported by SqlDataModule.")
        };

        // Development-only diagnostics — never enable in production.
        if (options.EnableDetailedErrors)
            dbOptions.EnableDetailedErrors();

        if (options.EnableSensitiveDataLogging)
            dbOptions.EnableSensitiveDataLogging();
    }

    /// <summary>
    /// Factory method that returns the correct <see cref="IDbConnectionFactory"/>
    /// for Dapper queries based on the configured provider.
    /// </summary>
    private static IDbConnectionFactory CreateConnectionFactory(
        DatabaseProviderType provider,
        IConfiguration configuration)
        => provider switch
        {
            DatabaseProviderType.SqlServer  => new SqlServerConnectionFactory(configuration),
            DatabaseProviderType.PostgreSql => new PostgreSqlConnectionFactory(configuration),
            DatabaseProviderType.MySql      => new MySqlConnectionFactory(configuration),
            DatabaseProviderType.Oracle     => new OracleConnectionFactory(configuration),
            _ => throw new NotSupportedException(
                $"No Dapper connection factory for provider '{provider}'.")
        };
}
