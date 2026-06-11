using System.Security.Cryptography;
using Ashraak.Webhooks.Application.Abstractions;

namespace Ashraak.Webhooks.Infrastructure.Security;

internal sealed class WebhookSecretGenerator : IWebhookSecretGenerator
{
    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }
}
