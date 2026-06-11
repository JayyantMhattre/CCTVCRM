namespace Ashraak.Webhooks.Application.Abstractions;

public interface IWebhookSignatureService
{
    string ComputeSignature(string secret, string payloadJson);
}
