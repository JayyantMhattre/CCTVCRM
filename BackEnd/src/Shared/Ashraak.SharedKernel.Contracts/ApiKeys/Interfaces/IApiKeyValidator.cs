namespace Ashraak.SharedKernel.Contracts.ApiKeys.Interfaces;

/// <summary>Validates inbound API keys for machine-to-machine authentication.</summary>
public interface IApiKeyValidator
{
    /// <summary>
    /// Validates the plaintext key and returns principal claims data, or null if invalid.
    /// </summary>
    Task<ApiKeyValidationResult?> ValidateAsync(string plaintextKey, CancellationToken cancellationToken);
}

/// <summary>Result of a successful API key validation.</summary>
public sealed record ApiKeyValidationResult(
    Guid ApiKeyId,
    Guid TenantId,
    string KeyPrefix,
    IReadOnlyList<string> Scopes);
