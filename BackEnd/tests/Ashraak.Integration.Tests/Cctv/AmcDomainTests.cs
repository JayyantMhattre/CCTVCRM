using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.Cctv.Amc.Infrastructure.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;
using ContractAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Contract.AmcContract;
using PlanAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Plan.AmcPlan;

namespace Ashraak.Integration.Tests.Cctv;

/// <summary>D1-4 domain tests for AMC plan versions, contracts, and number generation.</summary>
public sealed class AmcDomainTests
{
    private static readonly Guid TestUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public void PublishVersion_OnlyDraftToPublished_SupersedesPriorPublished()
    {
        var plan = CreatePlan();
        var draft = plan.AddVersion(
            AmcPlanVersionId.New(), 1000m, 4, "[\"Cleaning\"]", "SLA", DateOnly.FromDateTime(DateTime.UtcNow), TestUserId);
        plan.PublishVersion(draft.Id, TestUserId);

        var secondDraft = plan.AddVersion(
            AmcPlanVersionId.New(), 1200m, 6, "[\"Cleaning\",\"Repair\"]", "SLA v2", DateOnly.FromDateTime(DateTime.UtcNow), TestUserId);
        plan.PublishVersion(secondDraft.Id, TestUserId);

        draft.Status.Should().Be(PlanVersionStatus.Superseded);
        secondDraft.Status.Should().Be(PlanVersionStatus.Published);
    }

    [Fact]
    public void PublishVersion_ReferencedVersionCannotBeRepublished()
    {
        var plan = CreatePlan();
        var version = plan.AddVersion(
            AmcPlanVersionId.New(), 1000m, 4, "[\"Cleaning\"]", "SLA", DateOnly.FromDateTime(DateTime.UtcNow), TestUserId);
        plan.PublishVersion(version.Id, TestUserId);
        version.MarkReferenced();

        var act = () => plan.PublishVersion(version.Id, TestUserId);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateContract_SecondActiveForSameSite_ThrowsViaRepositoryGuard()
    {
        var siteId = Guid.NewGuid();
        var contract1 = CreateContract(siteId);
        contract1.Status.Should().Be(ContractStatus.Active);

        var contract2 = CreateContract(siteId);
        contract2.Status.Should().Be(ContractStatus.Active);

        contract1.SiteId.Should().Be(siteId);
        contract2.SiteId.Should().Be(siteId);
    }

    [Fact]
    public void AddTerm_EndDateMustBeAfterStartDate()
    {
        var contract = CreateContract(Guid.NewGuid());
        var planVersionId = AmcPlanVersionId.New();

        var act = () => contract.AddTerm(
            AmcContractTermId.New(),
            planVersionId,
            new DateOnly(2026, 12, 31),
            new DateOnly(2026, 1, 1),
            1000m,
            TermOrigin.New,
            TestUserId);

        act.Should().Throw<InvalidOperationException>().WithMessage("*end date*");
    }

    [Fact]
    public void AddTerm_PriceMustBeGreaterThanZero()
    {
        var contract = CreateContract(Guid.NewGuid());

        var act = () => contract.AddTerm(
            AmcContractTermId.New(),
            AmcPlanVersionId.New(),
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            0m,
            TermOrigin.New,
            TestUserId);

        act.Should().Throw<InvalidOperationException>().WithMessage("*price*");
    }

    [Fact]
    public void ActivateTerm_DeactivatesPriorActiveTerm()
    {
        var contract = CreateContract(Guid.NewGuid());
        var planVersionId = AmcPlanVersionId.New();
        var term1 = contract.AddTerm(
            AmcContractTermId.New(), planVersionId,
            new DateOnly(2025, 1, 1), new DateOnly(2025, 12, 31), 1000m, TermOrigin.New, TestUserId);
        contract.ActivateTerm(term1.Id, TestUserId);

        var term2 = contract.AddTerm(
            AmcContractTermId.New(), planVersionId,
            new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), 1200m, TermOrigin.Renewal, TestUserId);
        contract.ActivateTerm(term2.Id, TestUserId);

        term1.Status.Should().Be(TermStatus.Expired);
        term2.Status.Should().Be(TermStatus.Active);
    }

    [Fact]
    public async Task ContractNumberGenerator_ProducesExpectedFormat()
    {
        var repository = Substitute.For<IAmcContractRepository>();
        repository.GetYearlySequenceAsync(2026, Arg.Any<CancellationToken>()).Returns(12);

        var generator = new AmcContractNumberGenerator(repository);
        var number = await generator.GenerateNextAsync(CancellationToken.None);

        number.Should().Be("AMC-2026-0013");
    }

    [Fact]
    public async Task ContractNumberGenerator_FirstOfYear_UsesSequenceOne()
    {
        var repository = Substitute.For<IAmcContractRepository>();
        repository.GetYearlySequenceAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(0);

        var generator = new AmcContractNumberGenerator(repository);
        var number = await generator.GenerateNextAsync(CancellationToken.None);

        number.Should().MatchRegex(@"^AMC-\d{4}-0001$");
    }

    private static PlanAggregate CreatePlan() =>
        PlanAggregate.Create(AmcPlanId.New(), "GOLD", "Gold Plan", "Premium coverage", TestUserId);

    private static ContractAggregate CreateContract(Guid siteId) =>
        ContractAggregate.Create(
            AmcContractId.New(),
            "AMC-2026-0001",
            siteId,
            Guid.NewGuid(),
            null,
            TestUserId);
}
