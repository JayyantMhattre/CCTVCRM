using System.Security.Cryptography;
using System.Text;
using Ashraak.Audit.Domain.Entities;
using Ashraak.SharedKernel.Contracts.Audit.Dtos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Ashraak.Audit.Infrastructure.Services;

/// <summary>
/// Background worker that drains queued audit entries and persists them to MongoDB.
/// This keeps capture points (middleware/interceptors/domain handlers) non-blocking.
/// </summary>
internal sealed class AuditMongoWriterHostedService : BackgroundService
{
    private readonly IAuditWriteQueue _writeQueue;
    private readonly IMongoCollection<AuditEntry> _collection;
    private readonly ILogger<AuditMongoWriterHostedService> _logger;

    public AuditMongoWriterHostedService(
        IAuditWriteQueue writeQueue,
        IMongoDatabase database,
        ILogger<AuditMongoWriterHostedService> logger)
    {
        _writeQueue = writeQueue;
        _collection = database.GetCollection<AuditEntry>("audit_entries");
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in _writeQueue.DequeueAllAsync(stoppingToken))
        {
            try
            {
                await PersistAsync(item, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Normal shutdown path.
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist audit entry for tenant {TenantId}", item.TenantId);
            }
        }
    }

    private async Task PersistAsync(AuditEntryDto entry, CancellationToken cancellationToken)
    {
        var previousEntry = await _collection
            .Find(e => e.TenantId == entry.TenantId)
            .SortByDescending(e => e.OccurredOnUtc)
            .Limit(1)
            .FirstOrDefaultAsync(cancellationToken);

        var auditEntry = AuditEntry.Create(
            entry.TenantId,
            entry.UserId,
            entry.Module,
            entry.Action,
            entry.EntityType,
            entry.EntityId,
            entry.OldValues,
            entry.NewValues,
            entry.IpAddress,
            entry.UserAgent,
            previousEntry?.Hash);

        var hash = ComputeHash(auditEntry, previousEntry?.Hash);
        auditEntry.SetHash(hash);

        await _collection.InsertOneAsync(auditEntry, cancellationToken: cancellationToken);
    }

    private static string ComputeHash(AuditEntry entry, string? previousHash)
    {
        var payload = $"{entry.Id}{entry.TenantId}{entry.OccurredOnUtc:O}{entry.Action}{previousHash ?? string.Empty}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexStringLower(bytes);
    }
}
