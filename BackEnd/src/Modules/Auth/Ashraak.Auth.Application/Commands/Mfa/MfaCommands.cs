using Ashraak.Auth.Application.Abstractions;
using Ashraak.Auth.Domain.Aggregates.AuthUser;
using Ashraak.Auth.Domain.Repositories;
using Ashraak.Caching.Abstractions;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Auth.Application.Commands.Mfa;

public sealed record BeginMfaEnrollCommand(Guid UserId, Guid TenantId, string Email)
    : IRequest<Result<BeginMfaEnrollResult>>;

public sealed record BeginMfaEnrollResult(string Secret, string AuthenticatorUri);

internal sealed class BeginMfaEnrollCommandHandler(ITotpService totp) : IRequestHandler<BeginMfaEnrollCommand, Result<BeginMfaEnrollResult>>
{
    public Task<Result<BeginMfaEnrollResult>> Handle(BeginMfaEnrollCommand request, CancellationToken cancellationToken)
    {
        var secret = totp.GenerateSecret();
        var uri = totp.BuildAuthenticatorUri("Ashraak", request.Email, secret);
        return Task.FromResult<Result<BeginMfaEnrollResult>>(new BeginMfaEnrollResult(secret, uri));
    }
}

public sealed record ConfirmMfaEnrollCommand(Guid UserId, Guid TenantId, string Secret, string Code)
    : IRequest<Result>;

internal sealed class ConfirmMfaEnrollCommandHandler(
    IAuthUserRepository users,
    ITotpService totp,
    IUnitOfWork unitOfWork) : IRequestHandler<ConfirmMfaEnrollCommand, Result>
{
    public async Task<Result> Handle(ConfirmMfaEnrollCommand request, CancellationToken cancellationToken)
    {
        if (!totp.ValidateCode(request.Secret, request.Code))
            return Error.Validation("Auth.MfaInvalid", "Invalid verification code.");

        var user = await users.GetByIdAsync(AuthUserId.From(request.UserId), cancellationToken);
        if (user is null || user.TenantId != request.TenantId)
            return Error.NotFound("Auth.UserNotFound", "User not found.");

        user.EnableMfa(request.Secret);
        users.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed record DisableMfaCommand(Guid UserId, Guid TenantId, string Code) : IRequest<Result>;

internal sealed class DisableMfaCommandHandler(
    IAuthUserRepository users,
    ITotpService totp,
    IUnitOfWork unitOfWork) : IRequestHandler<DisableMfaCommand, Result>
{
    public async Task<Result> Handle(DisableMfaCommand request, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(AuthUserId.From(request.UserId), cancellationToken);
        if (user is null || user.TenantId != request.TenantId)
            return Error.NotFound("Auth.UserNotFound", "User not found.");

        if (user.MfaEnabled && user.MfaSecret is not null && !totp.ValidateCode(user.MfaSecret, request.Code))
            return Error.Validation("Auth.MfaInvalid", "Invalid verification code.");

        user.DisableMfa();
        users.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public sealed record CreateMfaChallengeCommand(Guid UserId, Guid TenantId) : IRequest<Result<string>>;

internal sealed class CreateMfaChallengeCommandHandler(ICacheService cache) : IRequestHandler<CreateMfaChallengeCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateMfaChallengeCommand request, CancellationToken cancellationToken)
    {
        var challengeId = Guid.NewGuid().ToString("N");
        var key = $"mfa:challenge:{challengeId}";
        await cache.SetAsync(key, new MfaChallengePayload(request.UserId, request.TenantId), TimeSpan.FromMinutes(5), cancellationToken);
        return challengeId;
    }
}

internal sealed record MfaChallengePayload(Guid UserId, Guid TenantId);

public sealed record VerifyMfaChallengeCommand(string ChallengeId, string Code, string IpAddress)
    : IRequest<Result<MfaChallengeVerifyResult>>;

public sealed record MfaChallengeVerifyResult(Guid UserId, Guid TenantId);

internal sealed class VerifyMfaChallengeCommandHandler(
    ICacheService cache,
    IAuthUserRepository users,
    ITotpService totp,
    IUnitOfWork unitOfWork) : IRequestHandler<VerifyMfaChallengeCommand, Result<MfaChallengeVerifyResult>>
{
    public async Task<Result<MfaChallengeVerifyResult>> Handle(
        VerifyMfaChallengeCommand request,
        CancellationToken cancellationToken)
    {
        var key = $"mfa:challenge:{request.ChallengeId}";
        var payload = await cache.GetAsync<MfaChallengePayload>(key, cancellationToken);
        if (payload is null)
            return Error.Validation("Auth.MfaChallengeExpired", "MFA challenge expired. Sign in again.");

        var user = await users.GetByIdAsync(AuthUserId.From(payload.UserId), cancellationToken);
        if (user is null || user.MfaSecret is null)
            return Error.NotFound("Auth.UserNotFound", "User not found.");

        if (!totp.ValidateCode(user.MfaSecret, request.Code))
        {
            user.RecordMfaChallengeFailed(request.IpAddress);
            users.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Error.Validation("Auth.MfaInvalid", "Invalid MFA code.");
        }

        user.RecordMfaVerified();
        users.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(key, cancellationToken);

        return new MfaChallengeVerifyResult(user.Id.Value, user.TenantId);
    }
}
