using Ashraak.SharedKernel.Contracts.Users.Dtos;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Users.Application.Commands.UpdateUserPreferences;

public sealed record UpdateUserPreferencesCommand(
    Guid UserId,
    Guid RequestingUserId,
    Guid TenantId,
    bool? EmailNotificationsEnabled,
    string? Theme,
    string? Locale,
    string? Timezone) : IRequest<Result<UserPreferencesDto>>;
