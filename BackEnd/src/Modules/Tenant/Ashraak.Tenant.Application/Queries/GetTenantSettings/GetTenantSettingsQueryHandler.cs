using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using Ashraak.SharedKernel.Results;
using Ashraak.Tenant.Application.Mapping;
using Ashraak.Tenant.Domain.Repositories;
using MediatR;

namespace Ashraak.Tenant.Application.Queries.GetTenantSettings;

internal sealed class GetTenantSettingsQueryHandler(
    ITenantRepository tenantRepository) : IRequestHandler<GetTenantSettingsQuery, Result<TenantSettingsDto>>
{
    public async Task<Result<TenantSettingsDto>> Handle(
        GetTenantSettingsQuery request,
        CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetByIdAsync(
            new Domain.Aggregates.Tenant.TenantId(request.TenantId),
            cancellationToken);

        if (tenant is null)
            return Error.NotFound("Tenant.NotFound", "Tenant not found.");

        return TenantSettingsMapper.ToDto(tenant.Settings);
    }
}
