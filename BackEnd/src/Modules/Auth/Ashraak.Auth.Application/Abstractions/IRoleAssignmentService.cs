namespace Ashraak.Auth.Application.Abstractions;

public interface IRoleAssignmentService
{
    Task AssignRoleAsync(Guid userId, Guid tenantId, string roleName, CancellationToken cancellationToken = default);
}
