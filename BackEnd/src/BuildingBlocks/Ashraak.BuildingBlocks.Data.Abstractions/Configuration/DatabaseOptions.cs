namespace Ashraak.BuildingBlocks.Data.Abstractions.Configuration;

/// <summary>
/// Strongly-typed options bound from the <c>"Database"</c> section of
/// <c>appsettings.json</c> (or environment variables).
///
/// <para>
/// Example configuration:
/// <code>
/// "Database": {
///   "Provider": "PostgreSql",
///   "ConnectionString": "Host=localhost;Database=ashraak;Username=app;Password=secret",
///   "DatabaseName": "",
///   "CommandTimeout": 30,
///   "EnableDetailedErrors": false,
///   "EnableSensitiveDataLogging": false
/// }
/// </code>
/// </para>
/// </summary>
public sealed class DatabaseOptions
{
    /// <summary>The configuration section key.</summary>
    public const string SectionName = "Database";

    /// <summary>
    /// The database provider to use.
    /// Valid values: <c>SqlServer</c>, <c>PostgreSql</c>, <c>MySql</c>, <c>Oracle</c>, <c>MongoDB</c>.
    /// </summary>
    public DatabaseProviderType Provider { get; set; } = DatabaseProviderType.PostgreSql;

    /// <summary>
    /// The ADO.NET / MongoDB connection string for the selected provider.
    /// Must match the provider's expected format.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Database (schema) name — required when <see cref="Provider"/> is <c>MongoDB</c>.
    /// Ignored for SQL providers (database name is part of the connection string).
    /// </summary>
    public string DatabaseName { get; set; } = string.Empty;

    /// <summary>
    /// Seconds before a command is cancelled.  Defaults to 30.
    /// Override for long-running reporting queries.
    /// </summary>
    public int CommandTimeout { get; set; } = 30;

    /// <summary>
    /// Enables EF Core detailed exception messages.
    /// Enable only in <b>Development</b> — messages may leak schema information.
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Enables EF Core sensitive data logging (logs parameter values).
    /// Enable only in <b>Development</b> — <b>never</b> in production.
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;
}
