using Ashraak.Cctv.Integration.Application.Rbac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Ashraak.Cctv.Integration.Infrastructure.Seeding;

/// <summary>
/// Idempotently seeds CCTV role permission grants into the platform Auth schema.
/// Does not modify Auth module code — uses direct SQL against auth.permission_grants.
/// </summary>
internal sealed class CctvRbacSeedHostedService(
    IServiceProvider serviceProvider,
    IConfiguration configuration,
    ILogger<CctvRbacSeedHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var enabled = configuration.GetValue("Cctv:RbacSeed:Enabled", true);
        if (!enabled)
        {
            logger.LogInformation("CCTV RBAC seed skipped (Cctv:RbacSeed:Enabled=false).");
            return;
        }

        var connectionString = configuration.GetConnectionString("Auth")
            ?? configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            logger.LogWarning("CCTV RBAC seed skipped — no database connection string.");
            return;
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        await SeedAsync(connectionString, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task SeedAsync(string connectionString, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var tenantIds = await ResolveTenantIdsAsync(connection, cancellationToken);
        if (tenantIds.Count == 0)
        {
            logger.LogInformation("CCTV RBAC seed: no tenants found — skipping.");
            return;
        }

        var inserted = 0;
        foreach (var tenantId in tenantIds)
        {
            foreach (var (roleName, permissions) in CctvPermissions.RolePermissionMap)
            {
                foreach (var permission in permissions)
                {
                    inserted += await InsertGrantIfMissingAsync(
                        connection,
                        tenantId,
                        roleName,
                        permission,
                        cancellationToken);
                }
            }
        }

        logger.LogInformation("CCTV RBAC seed complete — {Inserted} new permission grants.", inserted);
    }

    private static async Task<IReadOnlyList<Guid>> ResolveTenantIdsAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id FROM tenant.tenants
            UNION
            SELECT DISTINCT tenant_id FROM auth.permission_grants WHERE tenant_id IS NOT NULL
            """;

        await using var cmd = new NpgsqlCommand(sql, connection);
        var ids = new List<Guid>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            if (!reader.IsDBNull(0))
            {
                ids.Add(reader.GetGuid(0));
            }
        }

        return ids.Distinct().ToList();
    }

    private static async Task<int> InsertGrantIfMissingAsync(
        NpgsqlConnection connection,
        Guid tenantId,
        string roleName,
        string permission,
        CancellationToken cancellationToken)
    {
        const string existsSql = """
            SELECT 1 FROM auth.permission_grants
            WHERE tenant_id = @tenant_id AND role_name = @role_name AND permission = @permission
            LIMIT 1
            """;

        await using (var existsCmd = new NpgsqlCommand(existsSql, connection))
        {
            existsCmd.Parameters.AddWithValue("tenant_id", tenantId);
            existsCmd.Parameters.AddWithValue("role_name", roleName);
            existsCmd.Parameters.AddWithValue("permission", permission);
            var exists = await existsCmd.ExecuteScalarAsync(cancellationToken);
            if (exists is not null)
            {
                return 0;
            }
        }

        const string insertSql = """
            INSERT INTO auth.permission_grants (id, tenant_id, role_name, user_id, permission, condition_expression, created_on_utc)
            VALUES (@id, @tenant_id, @role_name, NULL, @permission, NULL, @created_on_utc)
            """;

        await using var insertCmd = new NpgsqlCommand(insertSql, connection);
        insertCmd.Parameters.AddWithValue("id", Guid.NewGuid());
        insertCmd.Parameters.AddWithValue("tenant_id", tenantId);
        insertCmd.Parameters.AddWithValue("role_name", roleName);
        insertCmd.Parameters.AddWithValue("permission", permission);
        insertCmd.Parameters.AddWithValue("created_on_utc", DateTime.UtcNow);
        await insertCmd.ExecuteNonQueryAsync(cancellationToken);
        return 1;
    }
}
