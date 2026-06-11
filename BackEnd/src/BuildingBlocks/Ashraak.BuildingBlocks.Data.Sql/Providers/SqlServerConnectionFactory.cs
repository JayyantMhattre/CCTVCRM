using System.Data;
using Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ashraak.BuildingBlocks.Data.Sql.Providers;

/// <summary>
/// Opens <see cref="SqlConnection"/> connections for Microsoft SQL Server.
/// Registered automatically when <c>Database:Provider = SqlServer</c>.
/// </summary>
public sealed class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration["Database:ConnectionString"]
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "SQL Server connection string not found.  " +
                "Set 'Database:ConnectionString' or 'ConnectionStrings:DefaultConnection'.");
    }

    /// <inheritdoc/>
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
