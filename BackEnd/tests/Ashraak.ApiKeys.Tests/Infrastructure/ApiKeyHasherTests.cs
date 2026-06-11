using Ashraak.ApiKeys.Infrastructure.Security;
using FluentAssertions;
using Xunit;

namespace Ashraak.ApiKeys.Tests.Infrastructure;

public sealed class ApiKeyHasherTests
{
    private readonly ApiKeyHasher _hasher = new();

    [Fact]
    public void Hash_and_verify_round_trip()
    {
        var plaintext = "ashk_prod_abc12345_secretvalue";
        var hash = _hasher.Hash(plaintext);

        hash.Should().StartWith("argon2id$");
        _hasher.Verify(plaintext, hash).Should().BeTrue();
        _hasher.Verify("wrong-key", hash).Should().BeFalse();
    }
}
