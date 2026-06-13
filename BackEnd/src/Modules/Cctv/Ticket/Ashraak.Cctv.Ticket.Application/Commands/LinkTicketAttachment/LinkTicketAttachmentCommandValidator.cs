using FluentValidation;

namespace Ashraak.Cctv.Ticket.Application.Commands.LinkTicketAttachment;

internal sealed class LinkTicketAttachmentCommandValidator : AbstractValidator<LinkTicketAttachmentCommand>
{
    public LinkTicketAttachmentCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.Title).MaximumLength(200).When(x => x.Title is not null);
    }
}
