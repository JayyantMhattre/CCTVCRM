using Ashraak.Cctv.Ticket.Domain.Enums;
using Ashraak.SharedKernel.Contracts.CctvCrm.Enums;
using FluentValidation;

namespace Ashraak.Cctv.Ticket.Application.Commands.UpdateTicketStatus;

internal sealed class UpdateTicketStatusCommandValidator : AbstractValidator<UpdateTicketStatusCommand>
{
    private static readonly HashSet<string> ValidStatuses =
    [
        TicketStatusContract.InProgress,
        TicketStatusContract.Resolved
    ];

    public UpdateTicketStatusCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        RuleFor(x => x.ToStatus)
            .NotEmpty()
            .Must(s => ValidStatuses.Contains(s))
            .WithMessage("Status must be InProgress or Resolved.");
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
