namespace Ashraak.SharedKernel.Contracts.Webhooks.Dtos;

public sealed record WebhookDeliveryContract(
    Guid Id,
    Guid SubscriptionId,
    Guid TenantId,
    string EventName,
    string EventVersion,
    string? CorrelationId,
    int AttemptNumber,
    int RetryCount,
    string Status,
    int? ResponseCode,
    string? ResponseBody,
    string? LastFailureReason,
    int? LastFailureCode,
    DateTime? NextRetryOnUtc,
    DateTime StartedOnUtc,
    DateTime? CompletedOnUtc);
