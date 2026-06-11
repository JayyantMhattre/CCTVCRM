namespace Ashraak.Auth.Application.Abstractions;

public interface IInvitationTokenService
{
    string GenerateToken();
    string HashToken(string token);
    bool VerifyToken(string token, string hash);
}
