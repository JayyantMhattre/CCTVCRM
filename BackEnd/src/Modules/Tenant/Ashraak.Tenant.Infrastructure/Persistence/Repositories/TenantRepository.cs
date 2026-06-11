using Ashraak.Tenant.Domain.Aggregates.Tenant;
using Ashraak.Tenant.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Tenant.Infrastructure.Persistence.Repositories;

internal sealed class TenantRepository : ITenantRepository
{
    private readonly TenantDbContext _context;

    public TenantRepository(TenantDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Aggregates.Tenant.Tenant?> GetByIdAsync(TenantId id, CancellationToken cancellationToken = default) =>
        await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<Domain.Aggregates.Tenant.Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        await _context.Tenants.FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default) =>
        await _context.Tenants.AnyAsync(t => t.Slug == slug, cancellationToken);

    public void Add(Domain.Aggregates.Tenant.Tenant tenant) =>
        _context.Tenants.Add(tenant);

    public void Update(Domain.Aggregates.Tenant.Tenant tenant) =>
        _context.Tenants.Update(tenant);
}
