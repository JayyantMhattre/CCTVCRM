namespace Ashraak.Auth.Domain.Entities;

/// <summary>
/// Tracks an active refresh/login session for device management.
/// </summary>
public sealed class UserSession
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime LastUsedOnUtc { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public bool IsRevoked { get; set; }
    public DateTime? RevokedOnUtc { get; set; }
}
