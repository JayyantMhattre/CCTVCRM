using Ashraak.Audit.Application.Abstractions;
using Ashraak.Audit.Domain.Entities;
using Ashraak.SharedKernel.Contracts.Audit.Dtos;
using MongoDB.Driver;

namespace Ashraak.Audit.Infrastructure.Repositories;

internal sealed class AuditReadService(IMongoDatabase database) : IAuditReadService
{
    private const string CollectionName = "audit_entries";

    public async Task<AuditLogPageDto> QueryAsync(
        Guid tenantId,
        string? module,
        string? search,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var collection = database.GetCollection<AuditEntry>(CollectionName);
        var filter = Builders<AuditEntry>.Filter.Eq(e => e.TenantId, tenantId);

        if (!string.IsNullOrWhiteSpace(module))
        {
            filter &= Builders<AuditEntry>.Filter.Regex(
                e => e.Module,
                new MongoDB.Bson.BsonRegularExpression(module, "i"));
        }

        if (from.HasValue)
            filter &= Builders<AuditEntry>.Filter.Gte(e => e.OccurredOnUtc, from.Value);

        if (to.HasValue)
            filter &= Builders<AuditEntry>.Filter.Lte(e => e.OccurredOnUtc, to.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var regex = new MongoDB.Bson.BsonRegularExpression(search, "i");
            filter &= Builders<AuditEntry>.Filter.Or(
                Builders<AuditEntry>.Filter.Regex(e => e.Action, regex),
                Builders<AuditEntry>.Filter.Regex(e => e.Module, regex),
                Builders<AuditEntry>.Filter.Regex(e => e.EntityType, regex));
        }

        var total = await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var items = await collection
            .Find(filter)
            .SortByDescending(e => e.OccurredOnUtc)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = items.Select(Map).ToList();
        return new AuditLogPageDto(dtos, page, pageSize, total);
    }

    private static AuditLogItemDto Map(AuditEntry entry) =>
        new(
            entry.Id,
            entry.TenantId,
            entry.UserId,
            entry.Module,
            entry.Action,
            ClassifyEventType(entry),
            entry.IpAddress,
            entry.UserAgent,
            entry.OccurredOnUtc);

    private static string ClassifyEventType(AuditEntry entry)
    {
        if (string.Equals(entry.Module, "Audit", StringComparison.OrdinalIgnoreCase)
            && entry.Action.Contains("Api", StringComparison.OrdinalIgnoreCase))
            return "ApiCall";

        if (entry.EntityType.Contains("DomainEvent", StringComparison.OrdinalIgnoreCase)
            || entry.EntityType.EndsWith("Event", StringComparison.OrdinalIgnoreCase))
            return "DomainEvent";

        if (entry.Action.Contains("User", StringComparison.OrdinalIgnoreCase))
            return "UserAction";

        return "EntityChange";
    }
}
