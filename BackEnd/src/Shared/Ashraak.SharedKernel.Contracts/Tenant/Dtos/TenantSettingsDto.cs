namespace Ashraak.SharedKernel.Contracts.Tenant.Dtos;

/// <summary>
/// Tenant workspace configuration exposed via API.
/// </summary>
public sealed record TenantSettingsDto(
    string Locale,
    string Timezone,
    int PasswordMinLength,
    bool RequireMfa,
    int SessionTimeoutMinutes);
