using System.Security.Cryptography;
using System.Text;
using Ashraak.ApiKeys.Application.Abstractions;
using Konscious.Security.Cryptography;

namespace Ashraak.ApiKeys.Infrastructure.Security;

/// <summary>Argon2id hashing for API key secrets — never store plaintext.</summary>
internal sealed class ApiKeyHasher : IApiKeyHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 4;
    private const int MemoryKb = 65536;
    private const int Parallelism = 1;

    public string Hash(string plaintextKey)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = HashWithSalt(plaintextKey, salt);
        return $"argon2id${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string plaintextKey, string hashedSecret)
    {
        var parts = hashedSecret.Split('$', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3 || parts[0] != "argon2id")
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var expected = Convert.FromBase64String(parts[2]);
        var actual = HashWithSalt(plaintextKey, salt);
        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }

    private static byte[] HashWithSalt(string plaintextKey, byte[] salt)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(plaintextKey))
        {
            Salt = salt,
            DegreeOfParallelism = Parallelism,
            MemorySize = MemoryKb,
            Iterations = Iterations
        };
        return argon2.GetBytes(HashSize);
    }
}
