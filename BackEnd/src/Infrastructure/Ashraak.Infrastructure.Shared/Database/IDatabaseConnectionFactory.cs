using System.Data;

namespace Ashraak.Infrastructure.Shared.Database;

/// <summary>
/// Abstraction over the raw ADO.NET connection used by Dapper read models.
/// Decouples module query handlers from Npgsql/SQL Server specifics.
/// </summary>
public interface IDatabaseConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
