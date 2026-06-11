using Ashraak.Auth.Domain.Aggregates.AuthUser.Events;
using Ashraak.Auth.Domain.ValueObjects;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Auth.Domain.Aggregates.AuthUser;

/// <summary>
/// Aggregate root representing an authenticated identity in the system.
/// Owns credentials (password hash, MFA secret) and account state (active/locked).
/// </summary>
/// <remarks>
/// <para>
/// <see cref="AuthUser"/> is the Auth module's sole aggregate root. It is intentionally
/// kept thin — it does NOT hold profile data (display name, avatar). That belongs to
/// <c>UserProfile</c> in the Users module. The two aggregates share the same <c>Guid</c> Id
/// so they can be correlated across modules.
/// </para>
/// <para>
/// Brute-force protection: after 5 consecutive failed login attempts the account is
/// locked for 15 minutes. The lock lifts automatically when <see cref="IsLocked"/>
/// returns <see langword="false"/>.
/// </para>
/// </remarks>
public sealed class AuthUser : AggregateRoot<AuthUserId>
{
    private AuthUser(AuthUserId id) : base(id) { }

    /// <summary>Gets the tenant this user identity belongs to.</summary>
    public Guid TenantId { get; private set; }

    /// <summary>Gets the user's email address (always lowercased on creation).</summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the Argon2id-hashed password in the format <c>{base64-salt}.{base64-hash}</c>.
    /// Never expose this value to the API layer.
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>Gets a value indicating whether the user's email has been verified.</summary>
    public bool EmailVerified { get; private set; }

    /// <summary>Gets a value indicating whether TOTP multi-factor authentication is enabled.</summary>
    public bool MfaEnabled { get; private set; }

    /// <summary>
    /// Gets the base-32 encoded TOTP secret used to generate time-based codes.
    /// <see langword="null"/> when MFA is not configured.
    /// </summary>
    public string? MfaSecret { get; private set; }

    /// <summary>Gets a value indicating whether this user account is active (not deactivated or banned).</summary>
    public bool IsActive { get; private set; }

    /// <summary>Gets the number of consecutive failed login attempts since the last success.</summary>
    public int FailedLoginAttempts { get; private set; }

    /// <summary>
    /// Gets the UTC time until which this account is temporarily locked.
    /// <see langword="null"/> when the account is not locked.
    /// </summary>
    public DateTime? LockedUntilUtc { get; private set; }

    /// <summary>Gets the UTC timestamp when this identity record was first created.</summary>
    public DateTime CreatedOnUtc { get; private set; }

    /// <summary>Gets the UTC timestamp of the most recent state change.</summary>
    public DateTime UpdatedOnUtc { get; private set; }

    /// <summary>
    /// Factory method that creates a new <see cref="AuthUser"/> from registration data.
    /// The email is lowercased for case-insensitive matching.
    /// No domain events are raised here — the <c>RegisterUserCommandHandler</c> handles
    /// cross-module notification via <c>TenantProvisionedEvent</c>.
    /// </summary>
    /// <param name="userId">The shared identifier used across the Auth and Users modules.</param>
    /// <param name="tenantId">The tenant this user belongs to.</param>
    /// <param name="email">The user's email address (will be lowercased).</param>
    /// <param name="passwordHash">An Argon2id hash produced by <c>IPasswordHasher.Hash</c>.</param>
    public static AuthUser Create(
        Guid userId,
        Guid tenantId,
        string email,
        string passwordHash,
        string displayName)
    {
        var user = new AuthUser(AuthUserId.From(userId))
        {
            TenantId = tenantId,
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            IsActive = true,
            EmailVerified = false,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(userId, tenantId, user.Email, displayName));
        return user;
    }

    /// <summary>
    /// Gets a value indicating whether the account is currently locked due to too many failed login attempts.
    /// </summary>
    public bool IsLocked => LockedUntilUtc.HasValue && LockedUntilUtc.Value > DateTime.UtcNow;

    /// <summary>
    /// Records a successful login, resets the failed-attempt counter, clears any lock,
    /// and raises <see cref="UserLoggedInDomainEvent"/> for the Audit module.
    /// </summary>
    /// <param name="ipAddress">The client's IP address from the HTTP request.</param>
    /// <param name="userAgent">The client's <c>User-Agent</c> header value.</param>
    public void RecordSuccessfulLogin(string ipAddress, string userAgent)
    {
        var wasLocked = IsLocked;
        FailedLoginAttempts = 0;
        LockedUntilUtc = null;
        UpdatedOnUtc = DateTime.UtcNow;

        if (wasLocked)
            RaiseDomainEvent(new AccountUnlockedDomainEvent(Id.Value, TenantId, Email));

        RaiseDomainEvent(new UserLoggedInDomainEvent(
            Id.Value, TenantId, Email, ipAddress, userAgent));
    }

    /// <summary>
    /// Records a failed login attempt and raises security audit domain events.
    /// </summary>
    public void RecordFailedLogin(string ipAddress, string userAgent, bool invalidPassword)
    {
        FailedLoginAttempts++;
        UpdatedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(new FailedLoginDomainEvent(Id.Value, TenantId, Email, ipAddress, userAgent));

        if (invalidPassword)
        {
            RaiseDomainEvent(new InvalidPasswordDomainEvent(
                Id.Value, TenantId, Email, ipAddress, userAgent));
        }

        if (FailedLoginAttempts >= 5)
        {
            LockedUntilUtc = DateTime.UtcNow.AddMinutes(15);
            RaiseDomainEvent(new AccountLockedDomainEvent(
                Id.Value, TenantId, Email, LockedUntilUtc.Value));
        }
    }

    /// <summary>
    /// Replaces the stored password hash and raises <see cref="UserPasswordChangedDomainEvent"/>
    /// so that the Auth module revokes all existing tokens for this user.
    /// </summary>
    /// <param name="newPasswordHash">The new Argon2id hash produced by <c>IPasswordHasher.Hash</c>.</param>
    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new UserPasswordChangedDomainEvent(Id.Value, TenantId));
    }

    /// <summary>
    /// Configures TOTP-based MFA for this user by storing the shared <paramref name="secret"/>.
    /// The caller is responsible for verifying a valid TOTP code before calling this method.
    /// </summary>
    /// <param name="secret">Base-32 encoded TOTP secret generated for this user.</param>
    public void EnableMfa(string secret)
    {
        MfaSecret = secret;
        MfaEnabled = true;
        UpdatedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new MfaEnabledDomainEvent(Id.Value, TenantId, Email));
    }

    /// <summary>Disables MFA for this user.</summary>
    public void DisableMfa()
    {
        MfaSecret = null;
        MfaEnabled = false;
        UpdatedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new MfaDisabledDomainEvent(Id.Value, TenantId, Email));
    }

    /// <summary>Records a failed MFA verification attempt.</summary>
    public void RecordMfaChallengeFailed(string ipAddress)
    {
        UpdatedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new MfaChallengeFailedDomainEvent(Id.Value, TenantId, Email, ipAddress));
    }

    /// <summary>Records a successful MFA verification at login.</summary>
    public void RecordMfaVerified()
    {
        UpdatedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new MfaVerifiedDomainEvent(Id.Value, TenantId, Email));
    }

    /// <summary>Records a password reset request with a short-lived token reference.</summary>
    public void RequestPasswordReset(string resetToken)
    {
        UpdatedOnUtc = DateTime.UtcNow;
        RaiseDomainEvent(new PasswordResetRequestedDomainEvent(Id.Value, TenantId, Email, resetToken));
    }

    /// <summary>Revokes a single session/token identifier.</summary>
    public void RevokeToken(string tokenId) =>
        RaiseDomainEvent(new TokenRevokedDomainEvent(Id.Value, TenantId, tokenId));

    /// <summary>Revokes all active sessions for this user.</summary>
    public void RevokeAllSessions() =>
        RaiseDomainEvent(new RevokeAllSessionsDomainEvent(Id.Value, TenantId));

    /// <summary>
    /// Marks this user's email address as verified.
    /// Typically called after the user clicks a confirmation link containing a signed token.
    /// </summary>
    public void VerifyEmail()
    {
        EmailVerified = true;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the account. A deactivated user cannot log in.
    /// The record is retained for audit purposes; use a hard-delete flow for GDPR erasure.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedOnUtc = DateTime.UtcNow;
    }
}
