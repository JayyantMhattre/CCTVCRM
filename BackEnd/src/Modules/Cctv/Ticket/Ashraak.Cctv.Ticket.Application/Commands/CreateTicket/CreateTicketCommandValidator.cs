using Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket;
using Ashraak.SharedKernel.Contracts.CctvCrm.Enums;
using FluentValidation;
using TicketAggregate = Ashraak.Cctv.Ticket.Domain.Aggregates.Ticket.Ticket;

namespace Ashraak.Cctv.Ticket.Application.Commands.CreateTicket;

internal sealed class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    private static readonly HashSet<string> ValidPriorities =
    [
        TicketPriorityContract.Low,
        TicketPriorityContract.Medium,
        TicketPriorityContract.High,
        TicketPriorityContract.Critical
    ];

    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.SiteId).NotEmpty();
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Priority)
            .NotEmpty()
            .Must(p => ValidPriorities.Contains(p))
            .WithMessage("Priority must be Low, Medium, High, or Critical.");
        RuleFor(x => x.AttachmentFileIds)
            .Must(ids => ids is null || ids.Count <= TicketAggregate.MaxAttachments)
            .WithMessage($"A ticket cannot have more than {TicketAggregate.MaxAttachments} attachments.");
    }
}
