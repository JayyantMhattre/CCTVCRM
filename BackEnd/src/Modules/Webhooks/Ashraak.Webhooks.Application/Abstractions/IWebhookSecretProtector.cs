namespace Ashraak.Webhooks.Application.Abstractions;

/// <summary>Protects webhook signing secrets at rest (reversible for W2 signing).</summary>
public interface IWebhookSecretProtector
{
    string Protect(string plaintextSecret);

    string Unprotect(string protectedSecret);
}
