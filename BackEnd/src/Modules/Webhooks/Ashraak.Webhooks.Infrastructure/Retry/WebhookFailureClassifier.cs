using Ashraak.Webhooks.Application.Abstractions;
using Ashraak.Webhooks.Domain.Enums;

namespace Ashraak.Webhooks.Infrastructure.Retry;

internal sealed class WebhookFailureClassifier : IWebhookFailureClassifier
{
    private static readonly HashSet<int> TransientStatusCodes =
    [
        408, 429, 500, 502, 503, 504
    ];

    private static readonly HashSet<int> PermanentStatusCodes =
    [
        400, 401, 403, 404, 410, 422
    ];

    public WebhookFailureType Classify(int? statusCode, string? errorMessage)
    {
        if (statusCode.HasValue)
        {
            if (TransientStatusCodes.Contains(statusCode.Value))
                return WebhookFailureType.Transient;

            if (PermanentStatusCodes.Contains(statusCode.Value))
                return WebhookFailureType.Permanent;
        }

        if (IsTransportError(errorMessage))
            return WebhookFailureType.Transient;

        if (IsPermanentMessage(errorMessage))
            return WebhookFailureType.Permanent;

        return statusCode is null or >= 500
            ? WebhookFailureType.Transient
            : WebhookFailureType.Unknown;
    }

    private static bool IsTransportError(string? message) =>
        !string.IsNullOrWhiteSpace(message)
        && (message.Contains("timeout", StringComparison.OrdinalIgnoreCase)
            || message.Contains("connection", StringComparison.OrdinalIgnoreCase)
            || message.Contains("network", StringComparison.OrdinalIgnoreCase)
            || message.Contains("TaskCanceledException", StringComparison.OrdinalIgnoreCase)
            || message.Contains("HttpRequestException", StringComparison.OrdinalIgnoreCase));

    private static bool IsPermanentMessage(string? message) =>
        !string.IsNullOrWhiteSpace(message)
        && (message.Contains("tenant mismatch", StringComparison.OrdinalIgnoreCase)
            || message.Contains("Subscription not found", StringComparison.OrdinalIgnoreCase)
            || message.Contains("disabled", StringComparison.OrdinalIgnoreCase)
            || message.Contains("Invalid URL", StringComparison.OrdinalIgnoreCase)
            || message.Contains("Invalid Payload", StringComparison.OrdinalIgnoreCase));
}
