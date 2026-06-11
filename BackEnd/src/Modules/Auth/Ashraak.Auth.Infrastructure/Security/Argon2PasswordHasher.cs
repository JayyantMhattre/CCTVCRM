using System.Security.Cryptography;
using Ashraak.Auth.Application.Abstractions;
using Konscious.Security.Cryptography;

namespace Ashraak.Auth.Infrastructure.Security;

/// <summary>
/// Password hasher backed by the Argon2id memory-hard key derivation function.
/// Argon2id provides resistance to both side-channel attacks (via memory access patterns)
/// and GPU-based brute force (via configurable memory cost).
/// </summary>
/// <remarks>
/// <para>
/// Configuration constants:
/// <list type="table">
///   <item><term>Salt</term><description>16 bytes of cryptographically random data (per hash).</description></item>
///   <item><term>Hash size</term><description>32 bytes (256-bit output).</description></item>
///   <item><term>Iterations</term><description>4 passes — OWASP minimum recommendation.</description></item>
///   <item><term>Memory</term><description>65,536 KB (64 MB) — significant GPU cost.</description></item>
///   <item><term>Parallelism</term><description>1 lane — single-threaded for consistent timing.</description></item>
/// </list>
/// </para>
/// <para>
/// Stored format: <c>{base64-salt}.{base64-hash}</c> — both components are required for verification.
/// </para>
/// </remarks>
internal sealed class Argon2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 4;
    private const int MemorySize = 65536;
    private const int DegreeOfParallelism = 1;

    /// <inheritdoc/>
    /// <remarks>Generates a fresh cryptographically random salt for every call.</remarks>
    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = ComputeHash(password, salt);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Uses <see cref="CryptographicOperations.FixedTimeEquals"/> to ensure constant-time
    /// comparison regardless of where the hashes diverge, preventing timing oracle attacks.
    /// </remarks>
    public bool Verify(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);
        var actualHash = ComputeHash(password, salt);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private static byte[] ComputeHash(string password, byte[] salt)
    {
        using var argon2 = new Argon2id(System.Text.Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            Iterations = Iterations,
            MemorySize = MemorySize,
            DegreeOfParallelism = DegreeOfParallelism
        };

        return argon2.GetBytes(HashSize);
    }
}
