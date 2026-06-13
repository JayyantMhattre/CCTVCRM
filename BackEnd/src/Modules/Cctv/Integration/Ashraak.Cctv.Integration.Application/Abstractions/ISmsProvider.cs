namespace Ashraak.Cctv.Integration.Application.Abstractions;

/// <summary>
/// SMS delivery abstraction for CCTV notification flows (OTP, alerts).
/// Sprint 0: stub implementation logs only — provider selected via ADR.
/// </summary>
public interface ISmsProvider
{
    /// <summary>Sends an SMS message to the given phone number.</summary>
    Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}
