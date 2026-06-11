namespace Ashraak.BuildingBlocks.Data.Abstractions.Interfaces;

/// <summary>
/// Advanced data access operations that go beyond standard CRUD.
///
/// <para>
/// These methods bypass the ORM abstraction and execute raw SQL / stored
/// procedures directly through an ADO.NET connection (Dapper).
/// They are available <b>only for SQL providers</b> (SQL Server, PostgreSQL,
/// MySQL, Oracle).  MongoDB implementations throw <see cref="NotSupportedException"/>.
/// </para>
///
/// <para>
/// Use these methods for:
/// <list type="bullet">
///   <item>High-performance read projections (flat DTOs, aggregates, reporting).</item>
///   <item>Complex batch updates that are too slow with the load-then-save pattern.</item>
///   <item>Stored procedure calls required by legacy or compliance scenarios.</item>
/// </list>
/// </para>
/// </summary>
public interface IAdvancedRepository
{
    // ── Raw SQL (Dapper) ─────────────────────────────────────────────────────

    /// <summary>
    /// Executes a raw SQL query and maps the results to <typeparamref name="TResult"/>.
    /// Internally uses Dapper's <c>QueryAsync&lt;TResult&gt;</c>.
    /// </summary>
    /// <typeparam name="TResult">The POCO/DTO to map results into.</typeparam>
    /// <param name="sql">Parameterised SQL string (e.g. <c>SELECT * FROM t WHERE id = @id</c>).</param>
    /// <param name="parameters">Anonymous object or Dapper <c>DynamicParameters</c>.</param>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    Task<IReadOnlyList<TResult>> ExecuteRawSqlAsync<TResult>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);

    // ── Stored procedures ────────────────────────────────────────────────────

    /// <summary>
    /// Executes a stored procedure and returns the number of rows affected.
    /// Internally uses Dapper's <c>ExecuteAsync</c> with <c>CommandType.StoredProcedure</c>.
    /// </summary>
    /// <param name="procedureName">The stored procedure name (schema-qualified if needed).</param>
    /// <param name="parameters">Anonymous object or Dapper <c>DynamicParameters</c>.</param>
    /// <param name="cancellationToken">Propagates cancellation.</param>
    Task<int> ExecuteStoredProcedureAsync(
        string procedureName,
        object? parameters = null,
        CancellationToken cancellationToken = default);
}
