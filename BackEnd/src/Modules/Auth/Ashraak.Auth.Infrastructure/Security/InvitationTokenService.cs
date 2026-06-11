using System.Security.Cryptography;
using System.Text;
using Ashraak.Auth.Application.Abstractions;

namespace Ashraak.Auth.Infrastructure.Security;

internal sealed class InvitationTokenService : IInvitationTokenService
{
    public string GenerateToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

    public string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    public bool VerifyToken(string token, string hash) =>
        string.Equals(HashToken(token), hash, StringComparison.OrdinalIgnoreCase);
}
