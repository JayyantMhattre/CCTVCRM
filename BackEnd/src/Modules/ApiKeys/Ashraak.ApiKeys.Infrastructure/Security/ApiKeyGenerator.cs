using System.Security.Cryptography;
using System.Text;
using Ashraak.ApiKeys.Application.Abstractions;

namespace Ashraak.ApiKeys.Infrastructure.Security;

internal sealed class ApiKeyGenerator : IApiKeyGenerator
{
    public (string PlaintextKey, string KeyPrefix) Generate(string environment)
    {
        var env = string.IsNullOrWhiteSpace(environment) ? "prod" : environment.Trim().ToLowerInvariant();
        var secretBytes = RandomNumberGenerator.GetBytes(24);
        var secret = Convert.ToBase64String(secretBytes)
            .TrimEnd('=')
            .Replace('+', 'x')
            .Replace('/', 'y');

        var prefix = $"ashk_{env}_{secret[..8]}";
        var plaintextKey = $"{prefix}_{secret}";
        return (plaintextKey, prefix);
    }
}
