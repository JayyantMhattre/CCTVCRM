namespace Ashraak.Auth.Application.Abstractions;

/// <summary>
/// Abstraction over the password hashing and verification algorithm.
/// Implemented by <c>Argon2PasswordHasher</c> in the Auth Infrastructure layer,
/// which uses Argon2id with a 16-byte random salt, 65,536 KB memory, and 4 iterations.
/// </summary>
/// <remarks>
/// Keeping this in the Application layer (not Infrastructure) allows command handlers
/// to hash passwords without depending on the concrete cryptographic library.
/// It also simplifies unit testing by permitting a fake hasher that returns deterministic values.
/// </remarks>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes <paramref name="password"/> using Argon2id with a random salt.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>
    /// A string in the format <c>{base64-salt}.{base64-hash}</c> that can be stored safely.
    /// </returns>
    string Hash(string password);

    /// <summary>
    /// Verifies <paramref name="password"/> against the <paramref name="hash"/> produced by <see cref="Hash"/>.
    /// Uses constant-time comparison to prevent timing attacks.
    /// </summary>
    /// <param name="password">The plain-text password provided by the user.</param>
    /// <param name="hash">The stored hash in <c>{base64-salt}.{base64-hash}</c> format.</param>
    /// <returns><see langword="true"/> when the password matches the hash; <see langword="false"/> otherwise.</returns>
    bool Verify(string password, string hash);
}
