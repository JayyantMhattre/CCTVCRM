using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.Cctv.Ticket.Domain.Enums;
using Ashraak.Cctv.Ticket.Domain.Repositories;
using Ashraak.Cctv.Ticket.Infrastructure.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;
using TicketAggregate = Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket.Ticket;

namespace Ashraak.Integration.Tests.Cctv;

/// <summary>D1-6 domain tests for ticket lifecycle, attachments, and number generation.</summary>
public sealed class TicketDomainTests
{
    private static readonly Guid TestUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid EngineerId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public void AssignEngineer_OpenToAssigned_UpdatesStatus()
    {
        var ticket = CreateTicket();

        ticket.AssignEngineer(TicketAssignmentId.New(), EngineerId, TestUserId);

        ticket.Status.Should().Be(TicketStatus.Assigned);
        ticket.ActiveAssignment.Should().NotBeNull();
        ticket.ActiveAssignment!.EngineerId.Should().Be(EngineerId);
    }

    [Fact]
    public void UpdateStatus_AssignedToInProgress_ToResolved()
    {
        var ticket = CreateTicket();
        ticket.AssignEngineer(TicketAssignmentId.New(), EngineerId, TestUserId);

        ticket.UpdateStatus(TicketStatus.InProgress, TicketAuthorRole.Engineer, TestUserId);
        ticket.Status.Should().Be(TicketStatus.InProgress);

        ticket.UpdateStatus(TicketStatus.Resolved, TicketAuthorRole.Engineer, TestUserId);
        ticket.Status.Should().Be(TicketStatus.Resolved);
        ticket.ResolvedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void UpdateStatus_InvalidTransition_Throws()
    {
        var ticket = CreateTicket();

        var act = () => ticket.UpdateStatus(TicketStatus.Resolved, TicketAuthorRole.Engineer, TestUserId);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Reopen_RequiresMinimumReasonLength()
    {
        var ticket = CreateClosedTicket();

        var act = () => ticket.Reopen("too short", TestUserId);
        act.Should().Throw<InvalidOperationException>().WithMessage("*10*");
    }

    [Fact]
    public void Reopen_IncrementsReopenCount()
    {
        var ticket = CreateClosedTicket();

        ticket.Reopen("Customer reported issue persists on site.", TestUserId);

        ticket.Status.Should().Be(TicketStatus.Reopened);
        ticket.ReopenCount.Should().Be(1);
    }

    [Fact]
    public void Close_OnlyFromResolved()
    {
        var ticket = CreateTicket();

        var act = () => ticket.Close(TestUserId);
        act.Should().Throw<InvalidOperationException>().WithMessage("*resolved*");
    }

    [Fact]
    public void Close_FromResolved_Succeeds()
    {
        var ticket = CreateResolvedTicket();

        ticket.Close(TestUserId);

        ticket.Status.Should().Be(TicketStatus.Closed);
        ticket.ClosedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void LinkAttachment_MaxFiveEnforced()
    {
        var ticket = CreateTicket();

        for (var i = 0; i < TicketAggregate.MaxAttachments; i++)
            ticket.LinkAttachment(TicketAttachmentId.New(), Guid.NewGuid(), null, TestUserId);

        var act = () => ticket.LinkAttachment(TicketAttachmentId.New(), Guid.NewGuid(), null, TestUserId);
        act.Should().Throw<InvalidOperationException>().WithMessage("*5*");
    }

    [Fact]
    public async Task TicketNumberGenerator_ProducesExpectedFormat()
    {
        var repository = Substitute.For<ITicketRepository>();
        repository.GetYearlySequenceAsync(2026, Arg.Any<CancellationToken>()).Returns(7);

        var generator = new TicketNumberGenerator(repository);
        var number = await generator.GenerateNextAsync(CancellationToken.None);

        number.Should().Be("TK-2026-0008");
    }

    private static TicketAggregate CreateTicket() =>
        TicketAggregate.Create(
            TicketId.New(),
            "TK-2026-0001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            null,
            TicketSource.Customer,
            "Camera offline",
            "Main entrance camera not recording since morning.",
            TicketPriority.High,
            TestUserId);

    private static TicketAggregate CreateResolvedTicket()
    {
        var ticket = CreateTicket();
        ticket.AssignEngineer(TicketAssignmentId.New(), EngineerId, TestUserId);
        ticket.UpdateStatus(TicketStatus.InProgress, TicketAuthorRole.Engineer, TestUserId);
        ticket.UpdateStatus(TicketStatus.Resolved, TicketAuthorRole.Engineer, TestUserId);
        return ticket;
    }

    private static TicketAggregate CreateClosedTicket()
    {
        var ticket = CreateResolvedTicket();
        ticket.Close(TestUserId);
        return ticket;
    }
}
