using Ashraak.Auth.Application.Abstractions;
using Ashraak.Auth.Domain.Aggregates.AuthUser;
using Ashraak.Auth.Domain.Aggregates.Invitation;
using Ashraak.Auth.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.Users.Events;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Auth.Application.Commands.Invitations;

public sealed record CreateInvitationCommand(
    Guid TenantId,
    Guid InvitedByUserId,
    string Email,
    string Role,
    int ExpiryDays = 7) : IRequest<Result<CreateInvitationResult>>;

public sealed record CreateInvitationResult(Guid InvitationId, string Token);

internal sealed class CreateInvitationCommandHandler(
    IInvitationRepository invitationRepository,
    IInvitationTokenService tokenService,
    IAuthPermissionChecker permissionChecker,
    IAuthUserRepository authUserRepository,
    IPublisher publisher,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateInvitationCommand, Result<CreateInvitationResult>>
{
    public async Task<Result<CreateInvitationResult>> Handle(
        CreateInvitationCommand request,
        CancellationToken cancellationToken)
    {
        if (!await permissionChecker.HasPermissionAsync(
                request.InvitedByUserId, request.TenantId, "user:invite", cancellationToken)
            && !await permissionChecker.IsInRoleAsync(request.InvitedByUserId, request.TenantId, "Admin", cancellationToken))
            return Error.Forbidden("Auth.InviteForbidden", "You do not have permission to invite users.");

        if (await authUserRepository.ExistsAsync(request.Email, request.TenantId, cancellationToken))
            return Error.Conflict("Auth.EmailRegistered", "User is already registered in this tenant.");

        var token = tokenService.GenerateToken();
        var hash = tokenService.HashToken(token);
        var expires = DateTime.UtcNow.AddDays(Math.Clamp(request.ExpiryDays, 1, 30));

        var invitation = Invitation.Create(
            request.TenantId,
            request.Email,
            request.Role,
            hash,
            expires,
            request.InvitedByUserId);

        invitationRepository.Add(invitation);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new UserInvitedEvent(
            invitation.Id.Value,
            token,
            invitation.Email,
            invitation.TenantId,
            request.InvitedByUserId,
            invitation.ExpiresOnUtc), cancellationToken);

        return new CreateInvitationResult(invitation.Id.Value, token);
    }
}

public sealed record ResendInvitationCommand(Guid InvitationId, Guid TenantId, Guid RequestedByUserId)
    : IRequest<Result<CreateInvitationResult>>;

internal sealed class ResendInvitationCommandHandler(
    IInvitationRepository invitationRepository,
    IInvitationTokenService tokenService,
    IAuthPermissionChecker permissionChecker,
    IPublisher publisher,
    IUnitOfWork unitOfWork) : IRequestHandler<ResendInvitationCommand, Result<CreateInvitationResult>>
{
    public async Task<Result<CreateInvitationResult>> Handle(
        ResendInvitationCommand request,
        CancellationToken cancellationToken)
    {
        if (!await permissionChecker.HasPermissionAsync(
                request.RequestedByUserId, request.TenantId, "user:invite", cancellationToken)
            && !await permissionChecker.IsInRoleAsync(request.RequestedByUserId, request.TenantId, "Admin", cancellationToken))
            return Error.Forbidden("Auth.InviteForbidden", "You do not have permission to resend invitations.");

        var invitation = await invitationRepository.GetByIdAsync(InvitationId.From(request.InvitationId), cancellationToken);
        if (invitation is null || invitation.TenantId != request.TenantId)
            return Error.NotFound("Auth.InvitationNotFound", "Invitation not found.");

        var token = tokenService.GenerateToken();
        var hash = tokenService.HashToken(token);
        var expires = DateTime.UtcNow.AddDays(7);

        invitation.Resend(hash, expires, request.RequestedByUserId);
        invitationRepository.Update(invitation);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new UserInvitedEvent(
            invitation.Id.Value,
            token,
            invitation.Email,
            invitation.TenantId,
            request.RequestedByUserId,
            invitation.ExpiresOnUtc), cancellationToken);

        return new CreateInvitationResult(invitation.Id.Value, token);
    }
}

public sealed record RevokeInvitationCommand(Guid InvitationId, Guid TenantId, Guid RequestedByUserId)
    : IRequest<Result>;

internal sealed class RevokeInvitationCommandHandler(
    IInvitationRepository invitationRepository,
    IAuthPermissionChecker permissionChecker,
    IUnitOfWork unitOfWork) : IRequestHandler<RevokeInvitationCommand, Result>
{
    public async Task<Result> Handle(RevokeInvitationCommand request, CancellationToken cancellationToken)
    {
        if (!await permissionChecker.HasPermissionAsync(
                request.RequestedByUserId, request.TenantId, "user:invite", cancellationToken)
            && !await permissionChecker.IsInRoleAsync(request.RequestedByUserId, request.TenantId, "Admin", cancellationToken))
            return Error.Forbidden("Auth.InviteForbidden", "You do not have permission to revoke invitations.");

        var invitation = await invitationRepository.GetByIdAsync(InvitationId.From(request.InvitationId), cancellationToken);
        if (invitation is null || invitation.TenantId != request.TenantId)
            return Error.NotFound("Auth.InvitationNotFound", "Invitation not found.");

        invitation.Revoke(request.RequestedByUserId);
        invitationRepository.Update(invitation);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed record AcceptInvitationCommand(
    string Token,
    string? Password,
    string? DisplayName,
    Guid? LinkingUserId = null) : IRequest<Result<Guid>>;

internal sealed class AcceptInvitationCommandHandler(
    IInvitationRepository invitationRepository,
    IInvitationTokenService tokenService,
    IAuthUserRepository authUserRepository,
    IRoleAssignmentService roleAssignmentService,
    ISender sender,
    IUnitOfWork unitOfWork) : IRequestHandler<AcceptInvitationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var hash = tokenService.HashToken(request.Token);
        var invitation = await invitationRepository.GetByTokenHashAsync(hash, cancellationToken);
        if (invitation is null)
            return Error.NotFound("Auth.InvitationNotFound", "Invalid or expired invitation token.");

        if (invitation.IsExpired)
            return Error.Validation("Auth.InvitationExpired", "Invitation has expired.");

        if (await authUserRepository.ExistsAsync(invitation.Email, invitation.TenantId, cancellationToken))
            return Error.Conflict("Auth.EmailRegistered", "User already exists in this tenant.");

        if (request.LinkingUserId is Guid linkId)
        {
            var source = await authUserRepository.GetByIdAsync(AuthUserId.From(linkId), cancellationToken);
            if (source is null)
                return Error.NotFound("Auth.UserNotFound", "Linking user was not found.");

            if (!string.Equals(source.Email, invitation.Email, StringComparison.OrdinalIgnoreCase))
                return Error.Validation("Auth.InviteEmailMismatch", "Signed-in user email does not match the invitation.");

            var linkedUserId = Guid.NewGuid();
            var linked = AuthUser.Create(
                linkedUserId,
                invitation.TenantId,
                source.Email,
                source.PasswordHash,
                request.DisplayName ?? invitation.Email);

            authUserRepository.Add(linked);
            await roleAssignmentService.AssignRoleAsync(linkedUserId, invitation.TenantId, invitation.Role, cancellationToken);

            invitation.Accept(linkedUserId);
            invitationRepository.Update(invitation);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return linkedUserId;
        }

        if (string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.DisplayName))
            return Error.Validation("Auth.InviteAccept", "Password and display name are required for new users.");

        var register = await sender.Send(
            new Commands.RegisterUser.RegisterUserCommand(
                invitation.TenantId,
                invitation.Email,
                request.Password!,
                request.DisplayName!),
            cancellationToken);

        if (register.IsFailure)
            return register.Error;

        var userId = register.Value;

        await roleAssignmentService.AssignRoleAsync(userId, invitation.TenantId, invitation.Role, cancellationToken);

        invitation.Accept(userId);
        invitationRepository.Update(invitation);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return userId;
    }
}
