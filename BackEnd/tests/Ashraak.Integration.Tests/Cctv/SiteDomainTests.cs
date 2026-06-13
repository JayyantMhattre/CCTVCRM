using Ashraak.Cctv.Customer.Domain.Aggregates.Customer;
using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Ashraak.Cctv.Customer.Domain.Enums;
using Ashraak.Cctv.Customer.Domain.Repositories;
using Ashraak.Cctv.Customer.Infrastructure.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;
using SiteAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Site.Site;

namespace Ashraak.Integration.Tests.Cctv;

/// <summary>D1-3 domain tests for site contacts, number format, and primary contact rule.</summary>
public sealed class SiteDomainTests
{
    private static readonly Guid TestUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly CustomerId TestCustomerId = CustomerId.New();

    [Fact]
    public void ReplaceContacts_MoreThanThree_Throws()
    {
        var site = CreateSite();
        var contacts = Enumerable.Range(1, 4).Select(i => new SiteContactInput(
            $"Contact {i}", null, "+919999999999", null, i == 1)).ToList();

        var act = () => site.ReplaceContacts(contacts, TestUserId);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*at most 3*");
    }

    [Fact]
    public void ReplaceContacts_TwoPrimaries_Throws()
    {
        var site = CreateSite();
        var contacts = new[]
        {
            new SiteContactInput("A", null, "+911111111111", null, true),
            new SiteContactInput("B", null, "+912222222222", null, true)
        };

        var act = () => site.ReplaceContacts(contacts, TestUserId);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Exactly one contact must be primary*");
    }

    [Fact]
    public void ReplaceContacts_NoPrimaryWhenContactsExist_Throws()
    {
        var site = CreateSite();
        var contacts = new[]
        {
            new SiteContactInput("A", null, "+911111111111", null, false)
        };

        var act = () => site.ReplaceContacts(contacts, TestUserId);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Exactly one contact must be primary*");
    }

    [Fact]
    public void ReplaceContacts_ValidSet_Succeeds()
    {
        var site = CreateSite();
        var contacts = new[]
        {
            new SiteContactInput("Primary", "Manager", "+911111111111", "a@example.com", true),
            new SiteContactInput("Secondary", null, "+912222222222", null, false)
        };

        site.ReplaceContacts(contacts, TestUserId);

        site.Contacts.Should().HaveCount(2);
        site.Contacts.Single(c => c.IsPrimary).Name.Should().Be("Primary");
    }

    [Fact]
    public async Task SiteNumberGenerator_ProducesExpectedFormat()
    {
        var repository = Substitute.For<ISiteRepository>();
        repository.GetYearlySequenceAsync(2026, Arg.Any<CancellationToken>()).Returns(12);

        var generator = new SiteNumberGenerator(repository);
        var number = await generator.GenerateNextAsync(CancellationToken.None);

        number.Should().Be("ST-2026-0013");
    }

    [Fact]
    public async Task SiteNumberGenerator_FirstOfYear_UsesSequenceOne()
    {
        var repository = Substitute.For<ISiteRepository>();
        repository.GetYearlySequenceAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(0);

        var generator = new SiteNumberGenerator(repository);
        var number = await generator.GenerateNextAsync(CancellationToken.None);

        number.Should().MatchRegex(@"^ST-\d{4}-0001$");
    }

    private static SiteAggregate CreateSite() =>
        SiteAggregate.CreateManual(
            SiteId.New(),
            TestCustomerId,
            "ST-2026-0001",
            "Main Office",
            "123 Site St",
            "Mumbai",
            TestUserId);
}
