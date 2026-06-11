using Ashraak.SharedKernel.Results;
using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using MediatR;

namespace Ashraak.Tenant.Application.Commands.ProvisionTenant;

/// <summary>
/// Command to provision a new tenant in the system.
/// On success, creates the <c>Tenant</c> aggregate, persists it, and raises
/// <c>TenantCreatedDomainEvent</c> (which flows to the Users and Audit modules via the outbox).
/// Returns the new tenant's <see cref="Guid"/> identifier.
/// </summary>
/// <param name="Name">Human-readable tenant name (max 100 characters).</param>
/// <param name="Slug">
/// URL-safe slug (lowercase alphanumeric + hyphens, max 50 characters).
/// Must be globally unique. Returns <c>409 Conflict</c> if already taken.
/// </param>
/// <param name="Plan">The initial subscription tier.</param>
/// <param name="OwnerUserId">The <c>AuthUser</c> identifier of the account owner.</param>
public sealed record ProvisionTenantCommand(
    string Name,
    string Slug,
    TenantPlan Plan,
    Guid OwnerUserId) : IRequest<Result<Guid>>;
