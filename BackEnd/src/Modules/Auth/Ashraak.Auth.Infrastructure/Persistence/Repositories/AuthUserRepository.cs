using Ashraak.Auth.Domain.Aggregates.AuthUser;
using Ashraak.Auth.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Auth.Infrastructure.Persistence.Repositories;

internal sealed class AuthUserRepository : IAuthUserRepository
{
    private readonly AuthDbContext _context;

    public AuthUserRepository(AuthDbContext context) => _context = context;

    public async Task<AuthUser?> GetByIdAsync(AuthUserId id, CancellationToken cancellationToken = default) =>
        await _context.AuthUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<AuthUser?> GetByEmailAndTenantAsync(string email, Guid tenantId, CancellationToken cancellationToken = default) =>
        await _context.AuthUsers
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant() && u.TenantId == tenantId, cancellationToken);

    public async Task<bool> ExistsAsync(string email, Guid tenantId, CancellationToken cancellationToken = default) =>
        await _context.AuthUsers
            .AnyAsync(u => u.Email == email.ToLowerInvariant() && u.TenantId == tenantId, cancellationToken);

    public void Add(AuthUser user) => _context.AuthUsers.Add(user);
    public void Update(AuthUser user) => _context.AuthUsers.Update(user);
}
