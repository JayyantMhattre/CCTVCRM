using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using Ashraak.SharedKernel.Results;
using Ashraak.Tenant.Domain.Repositories;
using MediatR;

using ContractsPlan = Ashraak.SharedKernel.Contracts.Tenant.Dtos.TenantPlan;
using ContractsStatus = Ashraak.SharedKernel.Contracts.Tenant.Dtos.TenantStatus;

namespace Ashraak.Tenant.Application.Queries.GetTenant;

/// <summary>
/// Handles <see cref="GetTenantQuery"/>.
/// Loads the tenant aggregate from the repository, maps it to the <see cref="TenantDto"/>
/// contract, and returns it.
/// </summary>
/// <remarks>
/// In high-throughput scenarios, consider adding a Redis cache lookup here (using <c>ICacheService</c>)
/// before hitting the database. The <c>TenantService</c> implementation already does this for
/// cross-module reads; queries initiated by the tenant's own management API bypass that cache
/// to always return the freshest data.
/// </remarks>
internal sealed class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, Result<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;

    /// <summary>Initialises the handler with its repository dependency.</summary>
    public GetTenantQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    /// <inheritdoc/>
    public async Task<Result<TenantDto>> Handle(GetTenantQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(
            new Domain.Aggregates.Tenant.TenantId(request.TenantId),
            cancellationToken);

        if (tenant is null)
            return Error.NotFound("Tenant.NotFound", $"Tenant '{request.TenantId}' was not found.");

        return new TenantDto(
            tenant.Id.Value,
            tenant.Name,
            tenant.Slug,
            (ContractsPlan)(int)tenant.Plan,
            (ContractsStatus)(int)tenant.Status,
            tenant.CustomDomain,
            tenant.CreatedOnUtc);
    }
}
