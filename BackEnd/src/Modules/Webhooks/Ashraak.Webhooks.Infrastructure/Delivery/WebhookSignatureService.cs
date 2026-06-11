using System.Security.Cryptography;
using System.Text;
using Ashraak.Webhooks.Application.Abstractions;

namespace Ashraak.Webhooks.Infrastructure.Delivery;

internal sealed class WebhookSignatureService : IWebhookSignatureService
{
    public string ComputeSignature(string secret, string payloadJson)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var data = Encoding.UTF8.GetBytes(payloadJson);
        var hash = HMACSHA256.HashData(key, data);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
