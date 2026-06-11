using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using Ashraak.Tenant.Domain.Repositories;
using MediatR;

using ContractsPlan = Ashraak.SharedKernel.Contracts.Tenant.Dtos.TenantPlan;
using DomainPlan = Ashraak.Tenant.Domain.Enums.TenantPlan;

namespace Ashraak.Tenant.Application.Commands.ProvisionTenant;

/// <summary>
/// Handles <see cref="ProvisionTenantCommand"/>.
/// Checks slug uniqueness, maps the contract plan enum to the domain enum,
/// creates the aggregate, and persists it.
/// </summary>
/// <remarks>
/// The domain–contract enum mapping uses an explicit integer cast rather than a
/// library mapper to keep a zero-dependency boundary. The two enums must remain
/// in the same ordinal order; a unit test validates this in <c>Ashraak.Architecture.Tests</c>.
/// </remarks>
internal sealed class ProvisionTenantCommandHandler : IRequestHandler<ProvisionTenantCommand, Result<Guid>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initialises the handler with its dependencies via constructor injection.
    /// </summary>
    public ProvisionTenantCommandHandler(ITenantRepository tenantRepository, IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<Result<Guid>> Handle(ProvisionTenantCommand request, CancellationToken cancellationToken)
    {
        if (await _tenantRepository.SlugExistsAsync(request.Slug, cancellationToken))
            return Error.Conflict("Tenant.SlugTaken", $"The slug '{request.Slug}' is already taken.");

        var domainPlan = (DomainPlan)(int)request.Plan;

        var tenant = Domain.Aggregates.Tenant.Tenant.Create(
            request.Name,
            request.Slug,
            domainPlan,
            request.OwnerUserId);

        _tenantRepository.Add(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tenant.Id.Value;
    }
}
