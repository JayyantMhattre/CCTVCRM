namespace Ashraak.BuildingBlocks.Data.Abstractions.Configuration;

/// <summary>
/// Enumeration of supported database providers in the multi-provider data layer.
/// Set via <c>appsettings.json → Database:Provider</c> or the
/// <c>DATABASE__PROVIDER</c> environment variable.
/// </summary>
public enum DatabaseProviderType
{
    /// <summary>Microsoft SQL Server (EF Core + Dapper via <c>Microsoft.Data.SqlClient</c>).</summary>
    SqlServer,

    /// <summary>PostgreSQL (EF Core + Dapper via <c>Npgsql</c> — the existing project default).</summary>
    PostgreSql,

    /// <summary>MySQL / MariaDB (EF Core via Pomelo + Dapper via <c>MySqlConnector</c>).</summary>
    MySql,

    /// <summary>Oracle Database (EF Core + Dapper via <c>Oracle.ManagedDataAccess.Core</c>).</summary>
    Oracle,

    /// <summary>MongoDB (native <c>MongoDB.Driver</c> — no EF Core, no Dapper).</summary>
    MongoDB
}
