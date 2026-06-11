namespace Ashraak.Auth.Application.Abstractions;

public interface ITotpService
{
    string GenerateSecret();
    string BuildAuthenticatorUri(string issuer, string accountName, string secret);
    bool ValidateCode(string secret, string code);
}
