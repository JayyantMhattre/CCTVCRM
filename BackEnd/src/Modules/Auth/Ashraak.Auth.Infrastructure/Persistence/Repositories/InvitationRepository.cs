using Ashraak.Auth.Domain.Aggregates.Invitation;
using Ashraak.Auth.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Auth.Infrastructure.Persistence.Repositories;

internal sealed class InvitationRepository(AuthDbContext context) : IInvitationRepository
{
    public Task<Invitation?> GetByIdAsync(InvitationId id, CancellationToken cancellationToken = default) =>
        context.Invitations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Invitation?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        context.Invitations.FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

    public async Task<IReadOnlyList<Invitation>> GetPendingByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default) =>
        await context.Invitations
            .Where(x => x.TenantId == tenantId && x.Status == InvitationStatus.Pending)
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToListAsync(cancellationToken);

    public void Add(Invitation invitation) => context.Invitations.Add(invitation);

    public void Update(Invitation invitation) => context.Invitations.Update(invitation);
}
