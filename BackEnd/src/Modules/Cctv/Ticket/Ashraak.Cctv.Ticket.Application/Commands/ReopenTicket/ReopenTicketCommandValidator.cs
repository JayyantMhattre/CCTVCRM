using FluentValidation;
using TicketAggregate = Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket.Ticket;

namespace Ashraak.Cctv.Ticket.Application.Commands.ReopenTicket;

internal sealed class ReopenTicketCommandValidator : AbstractValidator<ReopenTicketCommand>
{
    public ReopenTicketCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        RuleFor(x => x.Reason)
            .NotEmpty()
            .MinimumLength(TicketAggregate.MinReopenReasonLength)
            .WithMessage($"Reopen reason must be at least {TicketAggregate.MinReopenReasonLength} characters.");
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
