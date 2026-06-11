using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Tenant.Application.Commands.UpdateTenantSettings;

public sealed record UpdateTenantSettingsCommand(
    Guid TenantId,
    string Locale,
    string Timezone,
    int PasswordMinLength,
    bool RequireMfa,
    int SessionTimeoutMinutes) : IRequest<Result<TenantSettingsDto>>;
