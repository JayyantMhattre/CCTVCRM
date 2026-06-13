using FluentValidation;

namespace Ashraak.Cctv.Ticket.Application.Commands.AssignTicket;

internal sealed class AssignTicketCommandValidator : AbstractValidator<AssignTicketCommand>
{
    public AssignTicketCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        RuleFor(x => x.EngineerId).NotEmpty();
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
