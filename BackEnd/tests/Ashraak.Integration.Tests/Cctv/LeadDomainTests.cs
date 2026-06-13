using Ashraak.Cctv.Lead.Domain.Aggregates.Lead;
using Ashraak.Cctv.Lead.Domain.Enums;
using Ashraak.Cctv.Lead.Domain.Repositories;
using Ashraak.Cctv.Lead.Infrastructure.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ashraak.Integration.Tests.Cctv;

/// <summary>B1 domain tests for lead status transitions and lead number generation.</summary>
public sealed class LeadDomainTests
{
    private static readonly Guid TestUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public void ChangeStatus_NewToContacted_SucceedsAndIncrementsRowVersion()
    {
        var lead = CreateLead();

        var activity = lead.ChangeStatus(LeadStatus.Contacted, TestUserId, "Called prospect");

        lead.Status.Should().Be(LeadStatus.Contacted);
        lead.RowVersion.Should().Be(2);
        activity.FromStatus.Should().Be(LeadStatus.New);
        activity.ToStatus.Should().Be(LeadStatus.Contacted);
        lead.Activities.Should().ContainSingle();
    }

    [Theory]
    [InlineData(LeadStatus.New, LeadStatus.Won)]
    [InlineData(LeadStatus.Contacted, LeadStatus.Won)]
    [InlineData(LeadStatus.Won, LeadStatus.Contacted)]
    [InlineData(LeadStatus.Lost, LeadStatus.Contacted)]
    [InlineData(LeadStatus.Converted, LeadStatus.Contacted)]
    public void ChangeStatus_InvalidTransition_Throws(LeadStatus from, LeadStatus to)
    {
        var lead = CreateLead();
        AdvanceTo(lead, from);

        var act = () => lead.ChangeStatus(to, TestUserId, null);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ChangeStatus_FullPipelineToWon_Succeeds()
    {
        var lead = CreateLead();

        lead.ChangeStatus(LeadStatus.Contacted, TestUserId, null);
        lead.ChangeStatus(LeadStatus.Qualified, TestUserId, null);
        lead.ChangeStatus(LeadStatus.QuotationSent, TestUserId, null);
        lead.ChangeStatus(LeadStatus.Negotiation, TestUserId, null);
        lead.ChangeStatus(LeadStatus.Won, TestUserId, null);

        lead.Status.Should().Be(LeadStatus.Won);
        lead.Activities.Should().HaveCount(5);
    }

    [Fact]
    public void Convert_WonLead_SetsConvertedStatusAndReferences()
    {
        var lead = CreateLead();
        AdvanceTo(lead, LeadStatus.Won);

        var customerId = Guid.NewGuid();
        var siteId = Guid.NewGuid();
        var contractId = Guid.NewGuid();

        lead.Convert(customerId, siteId, contractId, TestUserId);

        lead.Status.Should().Be(LeadStatus.Converted);
        lead.ConvertedCustomerId.Should().Be(customerId);
        lead.ConvertedSiteId.Should().Be(siteId);
        lead.ConvertedContractId.Should().Be(contractId);
        lead.ConvertedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task LeadNumberGenerator_ProducesExpectedFormat()
    {
        var repository = Substitute.For<ILeadRepository>();
        repository.GetYearlySequenceAsync(2026, Arg.Any<CancellationToken>()).Returns(5);

        var generator = new LeadNumberGenerator(repository);
        var number = await generator.GenerateNextAsync(CancellationToken.None);

        number.Should().Be("LD-2026-0006");
        await repository.Received(1).GetYearlySequenceAsync(DateTime.UtcNow.Year, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LeadNumberGenerator_FirstOfYear_UsesSequenceOne()
    {
        var repository = Substitute.For<ILeadRepository>();
        repository.GetYearlySequenceAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(0);

        var generator = new LeadNumberGenerator(repository);
        var number = await generator.GenerateNextAsync(CancellationToken.None);

        number.Should().MatchRegex(@"^LD-\d{4}-0001$");
    }

    private static Lead CreateLead() =>
        Lead.CreateFromInquiry(
            LeadId.New(),
            "LD-2026-0001",
            LeadSource.WebsiteContact,
            "Jane Doe",
            "Acme Corp",
            "jane@example.com",
            "+919999999999",
            "Mumbai",
            null,
            "CCTV installation",
            TestUserId);

    private static void AdvanceTo(Lead lead, LeadStatus target)
    {
        if (target == LeadStatus.New)
            return;

        if (target == LeadStatus.Lost)
        {
            lead.ChangeStatus(LeadStatus.Lost, TestUserId, null);
            return;
        }

        if (target == LeadStatus.Converted)
        {
            AdvanceTo(lead, LeadStatus.Won);
            lead.Convert(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), TestUserId);
            return;
        }

        var path = new[]
        {
            LeadStatus.Contacted,
            LeadStatus.Qualified,
            LeadStatus.QuotationSent,
            LeadStatus.Negotiation,
            LeadStatus.Won
        };

        foreach (var status in path)
        {
            if (lead.Status == target)
                return;

            if (status == target)
            {
                lead.ChangeStatus(status, TestUserId, null);
                return;
            }

            lead.ChangeStatus(status, TestUserId, null);
        }
    }
}
