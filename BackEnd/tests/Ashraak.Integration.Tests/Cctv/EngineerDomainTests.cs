using Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer;
using Ashraak.Cctv.Engineer.Domain.Enums;
using Ashraak.Cctv.Engineer.Domain.Repositories;
using Ashraak.Cctv.Engineer.Infrastructure.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;
using EngineerAggregate = Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer.Engineer;

namespace Ashraak.Integration.Tests.Cctv;

/// <summary>D1-8 domain tests for engineer lifecycle and number generation.</summary>
public sealed class EngineerDomainTests
{
    private static readonly Guid TestUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public void Create_SetsActiveStatusAndIncrementsRowVersion()
    {
        var engineer = CreateEngineer();

        engineer.Status.Should().Be(EngineerStatus.Active);
        engineer.RowVersion.Should().Be(1);
        engineer.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Update_UpdatesFieldsAndIncrementsRowVersion()
    {
        var engineer = CreateEngineer();

        engineer.Update("Updated Name", "+918888888888", Guid.NewGuid(), TestUserId);

        engineer.Name.Should().Be("Updated Name");
        engineer.Phone.Should().Be("+918888888888");
        engineer.RowVersion.Should().Be(2);
    }

    [Fact]
    public void ChangeStatus_ActiveToInactive_Succeeds()
    {
        var engineer = CreateEngineer();

        engineer.ChangeStatus(EngineerStatus.Inactive, TestUserId);

        engineer.Status.Should().Be(EngineerStatus.Inactive);
        engineer.IsActive.Should().BeFalse();
    }

    [Fact]
    public void ChangeStatus_InactiveToActive_Succeeds()
    {
        var engineer = CreateEngineer();
        engineer.ChangeStatus(EngineerStatus.Inactive, TestUserId);

        engineer.ChangeStatus(EngineerStatus.Active, TestUserId);

        engineer.Status.Should().Be(EngineerStatus.Active);
    }

    [Fact]
    public async Task EngineerNumberGenerator_ProducesExpectedFormat()
    {
        var repository = Substitute.For<IEngineerRepository>();
        repository.GetYearlySequenceAsync(2026, Arg.Any<CancellationToken>()).Returns(3);

        var generator = new EngineerNumberGenerator(repository);
        var number = await generator.GenerateNextAsync(CancellationToken.None);

        number.Should().Be("EN-2026-0004");
    }

    [Fact]
    public async Task EngineerNumberGenerator_FirstOfYear_UsesSequenceOne()
    {
        var repository = Substitute.For<IEngineerRepository>();
        repository.GetYearlySequenceAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(0);

        var generator = new EngineerNumberGenerator(repository);
        var number = await generator.GenerateNextAsync(CancellationToken.None);

        number.Should().MatchRegex(@"^EN-\d{4}-0001$");
    }

    private static EngineerAggregate CreateEngineer() =>
        EngineerAggregate.Create(
            EngineerId.New(),
            "EN-2026-0001",
            "John Engineer",
            "+919999999999",
            null,
            TestUserId);
}
