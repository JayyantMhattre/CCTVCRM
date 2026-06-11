using Ashraak.SharedKernel.Contracts.Tenant.Events;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.Users.Domain.Repositories;
using MediatR;

namespace Ashraak.Users.Application.EventHandlers;

/// <summary>
/// MediatR notification handler that reacts to <see cref="TenantDeletedEvent"/>.
/// Deactivates all <c>UserProfile</c> records belonging to the deleted tenant so that
/// their sessions are invalidated and they can no longer log in.
/// </summary>
/// <remarks>
/// This handler operates reactively via the outbox pattern — it is invoked by
/// <c>OutboxProcessorBase</c> after the Tenant module has persisted the deletion
/// and the outbox record has been committed to the database.
/// This guarantees that user deactivation happens even if the process crashes between
/// the Tenant commit and this handler's execution.
/// </remarks>
internal sealed class TenantDeletedEventHandler : INotificationHandler<TenantDeletedEvent>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initialises the handler with its dependencies via constructor injection.</summary>
    public TenantDeletedEventHandler(
        IUserProfileRepository userProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task Handle(TenantDeletedEvent notification, CancellationToken cancellationToken)
    {
        var users = await _userProfileRepository.GetByTenantAsync(
            notification.TenantId, cancellationToken);

        foreach (var user in users)
            user.Deactivate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
