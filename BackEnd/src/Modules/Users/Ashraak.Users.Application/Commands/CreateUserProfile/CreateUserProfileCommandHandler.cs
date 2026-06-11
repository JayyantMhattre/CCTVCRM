using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using Ashraak.Users.Domain.Aggregates.UserProfile;
using Ashraak.Users.Domain.Repositories;
using MediatR;

namespace Ashraak.Users.Application.Commands.CreateUserProfile;

/// <summary>
/// Handles <see cref="CreateUserProfileCommand"/>.
/// Idempotent: if a profile already exists for the given user + tenant combination, it succeeds silently.
/// This is intentional to support at-least-once delivery from the outbox processor.
/// </summary>
internal sealed class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, Result>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises the handler with its dependencies via constructor injection.</summary>
    public CreateUserProfileCommandHandler(
        IUserProfileRepository userProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        if (await _userProfileRepository.ExistsAsync(request.UserId, request.TenantId, cancellationToken))
            return Result.Success();

        var profile = UserProfile.Create(
            request.UserId,
            request.TenantId,
            request.Email,
            request.DisplayName);

        _userProfileRepository.Add(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
