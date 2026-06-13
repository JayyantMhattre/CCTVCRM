using Ashraak.Auth.Application.Commands.Invitations;
using Ashraak.Auth.Application.Commands.Mfa;
using Ashraak.Auth.Application.Commands.PasswordReset;
using Ashraak.Auth.Domain.Entities;
using Ashraak.Auth.Domain.Repositories;
using Ashraak.SharedKernel.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Auth.Api.Endpoints;

internal static class AuthExtendedEndpoints
{
    public static IEndpointRouteBuilder MapAuthExtendedEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/auth")
            .WithTags("Auth")
            .RequireAuthorization();

        group.MapPost("/invitations", CreateInvitation);
        group.MapPost("/invitations/{invitationId:guid}/resend", ResendInvitation);
        group.MapPost("/invitations/{invitationId:guid}/revoke", RevokeInvitation);

        group.MapPost("/mfa/enroll", BeginMfaEnroll);
        group.MapPost("/mfa/confirm", ConfirmMfaEnroll);
        group.MapPost("/mfa/disable", DisableMfa);

        group.MapGet("/sessions", ListSessions);
        group.MapPost("/sessions/{sessionId:guid}/revoke", RevokeSession);
        group.MapPost("/sessions/revoke-all", RevokeAllSessions);

        var publicGroup = routeBuilder.MapGroup("/auth")
            .WithTags("Auth");

        publicGroup.MapPost("/invitations/accept", AcceptInvitation)
            .AllowAnonymous();

        publicGroup.MapPost("/mfa/verify", VerifyMfaChallenge)
            .AllowAnonymous();

        publicGroup.MapPost("/password-reset/request", RequestPasswordResetOtp)
            .AllowAnonymous();

        publicGroup.MapPost("/password-reset/verify", VerifyPasswordResetOtp)
            .AllowAnonymous();

        publicGroup.MapPost("/password-reset/confirm", ConfirmPasswordReset)
            .AllowAnonymous();

        return routeBuilder;
    }

    private static async Task<IResult> CreateInvitation(
        [FromBody] CreateInvitationRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateInvitationCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                request.Email,
                request.Role ?? "Member",
                request.ExpiryDays),
            cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> ResendInvitation(
        Guid invitationId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ResendInvitationCommand(invitationId, tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> RevokeInvitation(
        Guid invitationId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RevokeInvitationCommand(invitationId, tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok();
    }

    private static async Task<IResult> AcceptInvitation(
        [FromBody] AcceptInvitationRequest request,
        ICurrentUser currentUser,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var linkingUserId = currentUser.IsAuthenticated ? currentUser.UserId : (Guid?)null;
        var result = await sender.Send(
            new AcceptInvitationCommand(request.Token, request.Password, request.DisplayName, linkingUserId),
            cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok(new { UserId = result.Value });
    }

    private static async Task<IResult> BeginMfaEnroll(
        ICurrentUser currentUser,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new BeginMfaEnrollCommand(currentUser.UserId, currentUser.TenantId, currentUser.Email),
            cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> ConfirmMfaEnroll(
        [FromBody] ConfirmMfaEnrollRequest request,
        ICurrentUser currentUser,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ConfirmMfaEnrollCommand(
                currentUser.UserId,
                currentUser.TenantId,
                request.Secret,
                request.Code),
            cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok();
    }

    private static async Task<IResult> DisableMfa(
        [FromBody] DisableMfaRequest request,
        ICurrentUser currentUser,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new DisableMfaCommand(currentUser.UserId, currentUser.TenantId, request.Code),
            cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok();
    }

    private static async Task<IResult> VerifyMfaChallenge(
        [FromBody] VerifyMfaChallengeRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new VerifyMfaChallengeCommand(
                request.ChallengeId,
                request.Code,
                "unknown"),
            cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> RequestPasswordResetOtp(
        [FromBody] RequestPasswordResetOtpRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RequestPasswordResetOtpCommand(request.TenantId, request.Email, request.PhoneNumber),
            cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok();
    }

    private static async Task<IResult> VerifyPasswordResetOtp(
        [FromBody] VerifyPasswordResetOtpRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new VerifyPasswordResetOtpCommand(request.TenantId, request.Email, request.OtpCode),
            cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> ConfirmPasswordReset(
        [FromBody] ConfirmPasswordResetRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new ConfirmPasswordResetCommand(request.ChallengeId, request.NewPassword),
            cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Ok();
    }

    private static async Task<IResult> ListSessions(
        ICurrentUser currentUser,
        IUserSessionRepository sessions,
        CancellationToken cancellationToken)
    {
        var list = await sessions.GetActiveByUserAsync(
            currentUser.UserId,
            currentUser.TenantId,
            cancellationToken);

        return Results.Ok(list.Select(s => new SessionDto(
            s.Id,
            s.CreatedOnUtc,
            s.LastUsedOnUtc,
            s.IpAddress,
            s.UserAgent,
            s.IsRevoked)));
    }

    private static async Task<IResult> RevokeSession(
        Guid sessionId,
        ICurrentUser currentUser,
        IUserSessionRepository sessions,
        IAuthUserRepository users,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var session = await sessions.GetByIdAsync(sessionId, cancellationToken);
        if (session is null || session.UserId != currentUser.UserId)
            return Results.NotFound();

        session.IsRevoked = true;
        session.RevokedOnUtc = DateTime.UtcNow;
        sessions.Update(session);

        var user = await users.GetByIdAsync(Domain.Aggregates.AuthUser.AuthUserId.From(currentUser.UserId), cancellationToken);
        user?.RevokeToken(sessionId.ToString("D"));
        if (user is not null)
            users.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }

    private static async Task<IResult> RevokeAllSessions(
        ICurrentUser currentUser,
        IUserSessionRepository sessionRepository,
        IAuthUserRepository users,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var active = await sessionRepository.GetActiveByUserAsync(
            currentUser.UserId,
            currentUser.TenantId,
            cancellationToken);

        foreach (var session in active)
        {
            session.IsRevoked = true;
            session.RevokedOnUtc = DateTime.UtcNow;
            sessionRepository.Update(session);
        }

        var user = await users.GetByIdAsync(Domain.Aggregates.AuthUser.AuthUserId.From(currentUser.UserId), cancellationToken);
        user?.RevokeAllSessions();
        if (user is not null)
            users.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
}

public sealed record CreateInvitationRequest(string Email, string? Role, int ExpiryDays = 7);
public sealed record AcceptInvitationRequest(string Token, string? Password, string? DisplayName);
public sealed record ConfirmMfaEnrollRequest(string Secret, string Code);
public sealed record DisableMfaRequest(string Code);
public sealed record VerifyMfaChallengeRequest(string ChallengeId, string Code);
public sealed record RequestPasswordResetOtpRequest(Guid TenantId, string Email, string? PhoneNumber);
public sealed record VerifyPasswordResetOtpRequest(Guid TenantId, string Email, string OtpCode);
public sealed record ConfirmPasswordResetRequest(string ChallengeId, string NewPassword);
public sealed record SessionDto(
    Guid Id,
    DateTime CreatedOnUtc,
    DateTime LastUsedOnUtc,
    string IpAddress,
    string UserAgent,
    bool IsRevoked);
