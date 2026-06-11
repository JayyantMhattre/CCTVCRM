using System.Data;
using Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Ashraak.BuildingBlocks.Data.Sql.Providers;

/// <summary>
/// Opens <see cref="MySqlConnection"/> connections for MySQL / MariaDB.
/// Registered automatically when <c>Database:Provider = MySql</c>.
/// Uses the high-performance <c>MySqlConnector</c> driver (async-native).
/// </summary>
public sealed class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration["Database:ConnectionString"]
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "MySQL connection string not found.  " +
                "Set 'Database:ConnectionString' or 'ConnectionStrings:DefaultConnection'.");
    }

    /// <inheritdoc/>
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
