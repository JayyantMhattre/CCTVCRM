using Ashraak.Webhooks.Domain.Enums;

namespace Ashraak.Webhooks.Application.Abstractions;

public interface IWebhookFailureClassifier
{
    WebhookFailureType Classify(int? statusCode, string? errorMessage);
}
