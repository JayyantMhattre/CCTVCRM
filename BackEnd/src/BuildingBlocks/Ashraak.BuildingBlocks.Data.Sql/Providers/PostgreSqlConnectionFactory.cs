using System.Data;
using Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Ashraak.BuildingBlocks.Data.Sql.Providers;

/// <summary>
/// Opens Npgsql connections for PostgreSQL.
/// Registered automatically when <c>Database:Provider = PostgreSql</c>.
///
/// <para>
/// Scoped registration: PgBouncer / Npgsql connection pool handles the actual
/// pooling; the factory simply opens a logical connection from the pool.
/// </para>
/// </summary>
public sealed class PostgreSqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public PostgreSqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration["Database:ConnectionString"]
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "PostgreSQL connection string not found.  " +
                "Set 'Database:ConnectionString' or 'ConnectionStrings:DefaultConnection'.");
    }

    /// <inheritdoc/>
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
