using Ashraak.Auth.Domain.Entities;
using Ashraak.Auth.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Auth.Infrastructure.Persistence.Repositories;

internal sealed class UserSessionRepository(AuthDbContext context) : IUserSessionRepository
{
    public async Task<IReadOnlyList<UserSession>> GetActiveByUserAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        await context.UserSessions
            .Where(x => x.UserId == userId && x.TenantId == tenantId && !x.IsRevoked)
            .OrderByDescending(x => x.LastUsedOnUtc)
            .ToListAsync(cancellationToken);

    public Task<UserSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
        context.UserSessions.FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);

    public void Add(UserSession session) => context.UserSessions.Add(session);

    public void Update(UserSession session) => context.UserSessions.Update(session);
}
