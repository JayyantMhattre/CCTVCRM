using Ashraak.SharedKernel.Contracts.Users.Dtos;
using Ashraak.SharedKernel.Results;
using Ashraak.Users.Domain.Aggregates.UserProfile;
using Ashraak.Users.Domain.Aggregates.UserProfile.ValueObjects;
using Ashraak.Users.Domain.Repositories;
using Ashraak.SharedKernel.Interfaces;
using MediatR;

namespace Ashraak.Users.Application.Commands.UpdateUserPreferences;

internal sealed class UpdateUserPreferencesCommandHandler(
    IUserProfileRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserPreferencesCommand, Result<UserPreferencesDto>>
{
    public async Task<Result<UserPreferencesDto>> Handle(
        UpdateUserPreferencesCommand request,
        CancellationToken cancellationToken)
    {
        var profile = await repository.GetByIdAsync(UserId.From(request.UserId), cancellationToken);
        if (profile is null)
            return Error.NotFound("Users.NotFound", "User profile not found.");

        if (profile.TenantId != request.TenantId && !profile.Memberships.Any(m => m.TenantId == request.TenantId))
            return Error.Forbidden("Users.TenantMismatch", "User is not a member of this tenant.");

        if (request.UserId != request.RequestingUserId)
            return Error.Forbidden("Users.PreferencesForbidden", "You can only update your own preferences.");

        var current = profile.Preferences;
        var updated = UserPreferences.Create(
            request.Theme ?? current.Theme,
            request.Locale ?? current.Locale,
            request.Timezone ?? current.Timezone,
            request.EmailNotificationsEnabled ?? current.EmailNotificationsEnabled);

        profile.UpdatePreferences(updated);
        repository.Update(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserPreferencesDto(
            updated.Theme,
            updated.Locale,
            updated.Timezone,
            updated.EmailNotificationsEnabled);
    }
}
