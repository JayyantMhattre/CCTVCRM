namespace Ashraak.Webhooks.Application.Abstractions;

public interface IWebhookSecretGenerator
{
    string Generate();
}
