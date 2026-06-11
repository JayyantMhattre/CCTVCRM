using Ashraak.Auth.Domain.Entities;

namespace Ashraak.Auth.Domain.Repositories;

public interface IUserSessionRepository
{
    Task<IReadOnlyList<UserSession>> GetActiveByUserAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<UserSession?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    void Add(UserSession session);
    void Update(UserSession session);
}
