using Ashraak.ApiKeys.Domain.Aggregates.ApiKey;
using FluentAssertions;
using Xunit;

namespace Ashraak.ApiKeys.Tests.Domain;

public sealed class ApiKeyTests
{
    [Fact]
    public void Create_raises_created_event_and_is_active()
    {
        var key = ApiKey.Create(
            ApiKeyId.New(),
            Guid.NewGuid(),
            "Integration",
            "CRM connector",
            "ashk_prod_abc12345",
            "hash",
            "prod",
            ["users:read", "files:read"],
            Guid.NewGuid(),
            null);

        key.IsActive.Should().BeTrue();
        key.Scopes.Should().Contain("users:read");
        key.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void Revoke_disables_key()
    {
        var key = CreateSample();
        key.Revoke(Guid.NewGuid());

        key.IsActive.Should().BeFalse();
        key.RevokedOnUtc.Should().NotBeNull();
    }

    [Fact]
    public void RecordUsage_increments_counters()
    {
        var key = CreateSample();
        key.RecordUsage("corr-1", success: true);
        key.RecordUsage("corr-2", success: false);

        key.RequestCount.Should().Be(2);
        key.SuccessCount.Should().Be(1);
        key.FailureCount.Should().Be(1);
        key.LastCorrelationId.Should().Be("corr-2");
    }

    private static ApiKey CreateSample() =>
        ApiKey.Create(
            ApiKeyId.New(),
            Guid.NewGuid(),
            "Test",
            string.Empty,
            "ashk_prod_test1234",
            "hash",
            "prod",
            ["users:read"],
            Guid.NewGuid(),
            null);
}
