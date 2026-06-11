namespace Ashraak.SharedKernel.Contracts.Webhooks.Dtos;

public sealed record WebhookDeadLetterContract(
    Guid Id,
    Guid DeliveryId,
    Guid SubscriptionId,
    Guid TenantId,
    string EventName,
    string Payload,
    string? FailureReason,
    int? FailureCode,
    int RetryCount,
    string? CorrelationId,
    DateTime CreatedOnUtc);
