using System.Security.Cryptography;
using Ashraak.Auth.Application.Abstractions;
using Ashraak.Auth.Domain.Aggregates.AuthUser;
using Ashraak.Auth.Domain.Repositories;
using Ashraak.Caching.Abstractions;
using Ashraak.SharedKernel.Contracts.Notifications.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Auth.Application.Commands.PasswordReset;

public sealed record RequestPasswordResetOtpCommand(
    Guid TenantId,
    string Email,
    string? PhoneNumber) : IRequest<Result>;

public sealed record VerifyPasswordResetOtpCommand(
    Guid TenantId,
    string Email,
    string OtpCode) : IRequest<Result<PasswordResetChallengeResult>>;

public sealed record PasswordResetChallengeResult(string ChallengeId);

public sealed record ConfirmPasswordResetCommand(
    string ChallengeId,
    string NewPassword) : IRequest<Result>;

internal sealed record PasswordResetOtpPayload(
    Guid UserId,
    Guid TenantId,
    string Email,
    string OtpHash);

internal sealed record PasswordResetChallengePayload(
    Guid UserId,
    Guid TenantId,
    string Email);

internal sealed class RequestPasswordResetOtpCommandHandler(
    IAuthUserRepository users,
    ICacheService cache,
    INotificationService notifications,
    IUnitOfWork unitOfWork) : IRequestHandler<RequestPasswordResetOtpCommand, Result>
{
    private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(10);

    public async Task<Result> Handle(RequestPasswordResetOtpCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return Error.Validation("Auth.EmailRequired", "Email is required.");

        var email = request.Email.Trim().ToLowerInvariant();
        var user = await users.GetByEmailAndTenantAsync(email, request.TenantId, cancellationToken);

        // Do not reveal whether the account exists.
        if (user is null || !user.IsActive)
            return Result.Success();

        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var otpHash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(otp)));
        var key = BuildOtpKey(request.TenantId, email);

        await cache.SetAsync(
            key,
            new PasswordResetOtpPayload(user.Id.Value, request.TenantId, email, otpHash),
            OtpLifetime,
            cancellationToken);

        user.RequestPasswordReset(otp);
        users.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["OtpCode"] = otp,
            ["Email"] = email,
            ["RecipientName"] = email
        };

        try
        {
            await notifications.SendEmailAsync("password-reset-otp", email, data, cancellationToken);
        }
        catch
        {
            return Error.Unexpected("Auth.OtpSendFailed", "Could not send verification code. Try again later.");
        }

        return Result.Success();
    }

    private static string BuildOtpKey(Guid tenantId, string email) => $"auth:password-reset:otp:{tenantId}:{email}";
}

internal sealed class VerifyPasswordResetOtpCommandHandler(
    ICacheService cache) : IRequestHandler<VerifyPasswordResetOtpCommand, Result<PasswordResetChallengeResult>>
{
    private static readonly TimeSpan ChallengeLifetime = TimeSpan.FromMinutes(15);

    public async Task<Result<PasswordResetChallengeResult>> Handle(
        VerifyPasswordResetOtpCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.OtpCode))
            return Error.Validation("Auth.OtpRequired", "Verification code is required.");

        var email = request.Email.Trim().ToLowerInvariant();
        var key = $"auth:password-reset:otp:{request.TenantId}:{email}";
        var payload = await cache.GetAsync<PasswordResetOtpPayload>(key, cancellationToken);
        if (payload is null)
            return Error.Validation("Auth.OtpExpired", "Verification code expired. Request a new code.");

        var submittedHash = Convert.ToHexString(
            SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(request.OtpCode.Trim())));

        if (!string.Equals(submittedHash, payload.OtpHash, StringComparison.OrdinalIgnoreCase))
            return Error.Validation("Auth.OtpInvalid", "Invalid verification code.");

        await cache.RemoveAsync(key, cancellationToken);

        var challengeId = Guid.NewGuid().ToString("N");
        await cache.SetAsync(
            $"auth:password-reset:challenge:{challengeId}",
            new PasswordResetChallengePayload(payload.UserId, payload.TenantId, payload.Email),
            ChallengeLifetime,
            cancellationToken);

        return new PasswordResetChallengeResult(challengeId);
    }
}

internal sealed class ConfirmPasswordResetCommandHandler(
    IAuthUserRepository users,
    ICacheService cache,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<ConfirmPasswordResetCommand, Result>
{
    public async Task<Result> Handle(ConfirmPasswordResetCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
            return Error.Validation("Auth.WeakPassword", "Password must be at least 8 characters.");

        var key = $"auth:password-reset:challenge:{request.ChallengeId}";
        var payload = await cache.GetAsync<PasswordResetChallengePayload>(key, cancellationToken);
        if (payload is null)
            return Error.Validation("Auth.ChallengeExpired", "Reset session expired. Start again.");

        var user = await users.GetByIdAsync(AuthUserId.From(payload.UserId), cancellationToken);
        if (user is null || user.TenantId != payload.TenantId)
            return Error.NotFound("Auth.UserNotFound", "User not found.");

        var hash = passwordHasher.Hash(request.NewPassword);
        user.ChangePassword(hash);
        user.RevokeAllSessions();
        users.Update(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(key, cancellationToken);

        return Result.Success();
    }
}
