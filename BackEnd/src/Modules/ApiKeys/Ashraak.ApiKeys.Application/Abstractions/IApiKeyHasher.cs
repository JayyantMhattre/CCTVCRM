namespace Ashraak.ApiKeys.Application.Abstractions;

public interface IApiKeyHasher
{
    string Hash(string plaintextKey);
    bool Verify(string plaintextKey, string hashedSecret);
}
