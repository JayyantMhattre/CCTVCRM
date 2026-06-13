using Ashraak.Cctv.Integration.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Ashraak.Cctv.Integration.Infrastructure.Services;

/// <summary>Sprint 0 stub — logs SMS requests until a provider is configured (ADR-CCTV-0001).</summary>
internal sealed class StubSmsProvider(ILogger<StubSmsProvider> logger) : ISmsProvider
{
    public Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        logger.LogInformation(
            "SMS stub: would send to {PhoneNumber} (length {MessageLength})",
            phoneNumber,
            message.Length);
        return Task.CompletedTask;
    }
}
