using Ashraak.Auth.Application.Abstractions;
using Ashraak.Auth.Infrastructure.Persistence.Repositories;

namespace Ashraak.Auth.Infrastructure.Services;

internal sealed class RoleAssignmentService(AuthAuthorizationRepository repository) : IRoleAssignmentService
{
    public Task AssignRoleAsync(Guid userId, Guid tenantId, string roleName, CancellationToken cancellationToken = default) =>
        repository.AssignRoleAsync(userId, tenantId, roleName, cancellationToken);
}
