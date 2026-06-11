using System.Data;
using Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace Ashraak.BuildingBlocks.Data.Sql.Providers;

/// <summary>
/// Opens <see cref="OracleConnection"/> connections for Oracle Database.
/// Registered automatically when <c>Database:Provider = Oracle</c>.
/// Uses Oracle's official managed driver (<c>Oracle.ManagedDataAccess.Core</c>).
///
/// <para>
/// <b>Prerequisites</b>: Ensure the Oracle wallet / TNS names file is configured
/// on the host when using connection names instead of full connection descriptors.
/// </para>
/// </summary>
public sealed class OracleConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public OracleConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration["Database:ConnectionString"]
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Oracle connection string not found.  " +
                "Set 'Database:ConnectionString' or 'ConnectionStrings:DefaultConnection'.");
    }

    /// <inheritdoc/>
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new OracleConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
