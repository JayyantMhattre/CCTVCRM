using Ashraak.SharedKernel.Contracts.Users.Dtos;
using Ashraak.SharedKernel.Contracts.Users.Interfaces;
using Ashraak.Users.Domain.Aggregates.UserProfile;
using Ashraak.Users.Domain.Repositories;

using ContractsStatus = Ashraak.SharedKernel.Contracts.Users.Dtos.UserStatus;

namespace Ashraak.Users.Infrastructure.Services;

/// <summary>
/// Implementation of <see cref="IUserService"/> — the cross-module read facade for the Users module.
/// Maps <c>UserProfile</c> aggregates to <see cref="UserDto"/> contracts.
/// </summary>
/// <remarks>
/// For high-throughput scenarios, add a Redis cache layer here (similar to <c>TenantService</c>)
/// to avoid repeated database queries from the Auth and Audit modules.
/// Invalidate the cache when a profile is updated or deactivated.
/// </remarks>
internal sealed class UserService : IUserService
{
    private readonly IUserProfileRepository _userProfileRepository;

    /// <summary>Initialises the service with its repository dependency.</summary>
    public UserService(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    /// <inheritdoc/>
    public async Task<UserDto?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _userProfileRepository.GetByIdAsync(UserId.From(userId), cancellationToken);
        return profile is null ? null : MapToDto(profile);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<UserDto>> GetUsersForTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var profiles = await _userProfileRepository.GetByTenantAsync(tenantId, cancellationToken);
        return profiles.Select(MapToDto).ToList().AsReadOnly();
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default) =>
        await _userProfileRepository.ExistsAsync(userId, tenantId, cancellationToken);

    /// <summary>
    /// Maps a <c>UserProfile</c> aggregate to the cross-module <see cref="UserDto"/> contract.
    /// The integer cast between domain <c>UserStatus</c> and contract <c>UserStatus</c> relies on
    /// both enums maintaining the same ordinal order.
    /// </summary>
    private static UserDto MapToDto(Domain.Aggregates.UserProfile.UserProfile profile) => new(
        profile.Id.Value,
        profile.TenantId,
        profile.Email,
        profile.DisplayName,
        profile.AvatarUrl,
        new UserPreferencesDto(
            profile.Preferences.Theme,
            profile.Preferences.Locale,
            profile.Preferences.Timezone,
            profile.Preferences.EmailNotificationsEnabled),
        (ContractsStatus)(int)profile.Status,
        profile.CreatedOnUtc);
}
