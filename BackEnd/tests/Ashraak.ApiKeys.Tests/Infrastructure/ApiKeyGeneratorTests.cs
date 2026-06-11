using Ashraak.ApiKeys.Infrastructure.Security;
using FluentAssertions;
using Xunit;

namespace Ashraak.ApiKeys.Tests.Infrastructure;

public sealed class ApiKeyGeneratorTests
{
    [Fact]
    public void Generate_produces_environment_aware_key()
    {
        var generator = new ApiKeyGenerator();
        var (plaintext, prefix) = generator.Generate("prod");

        plaintext.Should().StartWith("ashk_prod_");
        prefix.Should().StartWith("ashk_prod_");
        plaintext.Should().Contain(prefix);
    }
}
