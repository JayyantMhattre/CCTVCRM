namespace Ashraak.ApiKeys.Application.Abstractions;

public interface IApiKeyGenerator
{
    /// <summary>Generates a plaintext key and its lookup prefix (ashk_{env}_...).</summary>
    (string PlaintextKey, string KeyPrefix) Generate(string environment);
}
