using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.LinkVisitAttachment;

internal sealed class LinkVisitAttachmentCommandValidator : AbstractValidator<LinkVisitAttachmentCommand>
{
    public LinkVisitAttachmentCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.AttachmentType).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Title).MaximumLength(200);
    }
}
