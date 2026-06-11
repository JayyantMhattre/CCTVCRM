using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Ashraak.Infrastructure.Shared.Database;

/// <summary>
/// Creates open NpgsqlConnections from the configured connection string.
/// Registered as scoped so a single connection is reused per HTTP request.
/// PgBouncer is the external pool; NpgsqlConnection.Open() is fast.
/// </summary>
public sealed class NpgsqlConnectionFactory(IConfiguration configuration) : IDatabaseConnectionFactory
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
