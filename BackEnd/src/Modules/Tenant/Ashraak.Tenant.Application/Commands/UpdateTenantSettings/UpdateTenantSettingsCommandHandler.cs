using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using Ashraak.SharedKernel.Results;
using Ashraak.Tenant.Application.Mapping;
using Ashraak.Tenant.Domain.Aggregates.Tenant.ValueObjects;
using Ashraak.Tenant.Domain.Repositories;
using Ashraak.SharedKernel.Interfaces;
using MediatR;

namespace Ashraak.Tenant.Application.Commands.UpdateTenantSettings;

internal sealed class UpdateTenantSettingsCommandHandler(
    ITenantRepository tenantRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTenantSettingsCommand, Result<TenantSettingsDto>>
{
    public async Task<Result<TenantSettingsDto>> Handle(
        UpdateTenantSettingsCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetByIdAsync(
            new Domain.Aggregates.Tenant.TenantId(request.TenantId),
            cancellationToken);

        if (tenant is null)
            return Error.NotFound("Tenant.NotFound", "Tenant not found.");

        var settings = TenantSettings.Create(
            request.Locale,
            request.Timezone,
            request.PasswordMinLength,
            request.RequireMfa,
            request.SessionTimeoutMinutes);

        tenant.UpdateSettings(settings);
        tenantRepository.Update(tenant);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return TenantSettingsMapper.ToDto(settings);
    }
}
