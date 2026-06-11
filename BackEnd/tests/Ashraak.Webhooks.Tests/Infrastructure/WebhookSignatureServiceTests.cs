using Ashraak.Webhooks.Infrastructure.Delivery;
using FluentAssertions;
using Xunit;

namespace Ashraak.Webhooks.Tests.Infrastructure;

public sealed class WebhookSignatureServiceTests
{
    [Fact]
    public void ComputeSignature_IsDeterministicHex()
    {
        var service = new WebhookSignatureService();
        var sig1 = service.ComputeSignature("secret", """{"a":1}""");
        var sig2 = service.ComputeSignature("secret", """{"a":1}""");

        sig1.Should().Be(sig2);
        sig1.Should().MatchRegex("^[0-9a-f]{64}$");
    }

    [Fact]
    public void ComputeSignature_DiffersForDifferentSecrets()
    {
        var service = new WebhookSignatureService();
        var sig1 = service.ComputeSignature("secret-a", "{}");
        var sig2 = service.ComputeSignature("secret-b", "{}");

        sig1.Should().NotBe(sig2);
    }
}
