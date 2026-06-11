using Ashraak.Users.Domain.Aggregates.UserProfile;
using Ashraak.Users.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Users.Infrastructure.Persistence.Repositories;

internal sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly UsersDbContext _context;

    public UserProfileRepository(UsersDbContext context) => _context = context;

    public async Task<UserProfile?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default) =>
        await _context.UserProfiles
            .Include(u => u.Memberships)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<UserProfile?> GetByEmailAndTenantAsync(string email, Guid tenantId, CancellationToken cancellationToken = default) =>
        await _context.UserProfiles
            .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId, cancellationToken);

    public async Task<IReadOnlyList<UserProfile>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default) =>
        await _context.UserProfiles
            .Where(u => u.TenantId == tenantId)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default) =>
        await _context.UserProfiles
            .AnyAsync(u => u.Id == UserId.From(userId) && u.TenantId == tenantId, cancellationToken);

    public void Add(UserProfile profile) => _context.UserProfiles.Add(profile);
    public void Update(UserProfile profile) => _context.UserProfiles.Update(profile);
}
