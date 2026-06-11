using Ashraak.Webhooks.Application.Abstractions;
using Microsoft.AspNetCore.DataProtection;

namespace Ashraak.Webhooks.Infrastructure.Security;

internal sealed class WebhookSecretProtector(IDataProtectionProvider dataProtectionProvider) : IWebhookSecretProtector
{
    private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector("Ashraak.Webhooks.SubscriptionSecret.v1");

    public string Protect(string plaintextSecret) => _protector.Protect(plaintextSecret);

    public string Unprotect(string protectedSecret) => _protector.Unprotect(protectedSecret);
}
