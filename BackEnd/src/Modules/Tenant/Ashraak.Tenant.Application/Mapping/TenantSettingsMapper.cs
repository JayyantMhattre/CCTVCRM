using Ashraak.SharedKernel.Contracts.Tenant.Dtos;
using Ashraak.Tenant.Domain.Aggregates.Tenant.ValueObjects;

namespace Ashraak.Tenant.Application.Mapping;

public static class TenantSettingsMapper
{
    public static TenantSettingsDto ToDto(TenantSettings settings) =>
        new(
            settings.Locale,
            settings.Timezone,
            settings.PasswordMinLength,
            settings.RequireMfa,
            settings.SessionTimeoutMinutes);
}
