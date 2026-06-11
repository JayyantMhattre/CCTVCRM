using System.Data;

namespace Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;

/// <summary>
/// Abstraction over raw ADO.NET connections used by Dapper in the data layer.
///
/// <para>
/// Each SQL provider (SQL Server, PostgreSQL, MySQL, Oracle) registers its
/// own implementation.  Query handlers that execute raw SQL via Dapper
/// depend on this interface rather than on a concrete connection type.
/// </para>
///
/// <para>
/// The returned connection is <b>open</b> and caller-owned.
/// Always dispose it (use a <see langword="using"/> block):
/// <code>
/// await using var conn = await _factory.CreateConnectionAsync(ct);
/// var results = await conn.QueryAsync&lt;MyDto&gt;("SELECT ...");
/// </code>
/// </para>
///
/// <para>
/// <b>Relationship to <c>IDatabaseConnectionFactory</c></b>:
/// This interface mirrors the existing <c>Ashraak.Infrastructure.Shared.Database.IDatabaseConnectionFactory</c>
/// but lives in the abstraction layer so modules can depend on it without
/// taking an infrastructure project reference.  The two interfaces are
/// intentionally compatible — the same implementation class can satisfy both.
/// </para>
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates and opens a new <see cref="IDbConnection"/>.
    /// </summary>
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
