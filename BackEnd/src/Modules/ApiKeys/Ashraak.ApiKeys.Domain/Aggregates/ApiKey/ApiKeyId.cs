namespace Ashraak.ApiKeys.Domain.Aggregates.ApiKey;

public readonly record struct ApiKeyId(Guid Value)
{
    public static ApiKeyId New() => new(Guid.NewGuid());
    public static ApiKeyId From(Guid value) => new(value);
}
