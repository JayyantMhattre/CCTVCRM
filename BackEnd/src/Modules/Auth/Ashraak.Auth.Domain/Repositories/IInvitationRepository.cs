using Ashraak.Auth.Domain.Aggregates.Invitation;

namespace Ashraak.Auth.Domain.Repositories;

public interface IInvitationRepository
{
    Task<Invitation?> GetByIdAsync(InvitationId id, CancellationToken cancellationToken = default);
    Task<Invitation?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invitation>> GetPendingByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    void Add(Invitation invitation);
    void Update(Invitation invitation);
}
