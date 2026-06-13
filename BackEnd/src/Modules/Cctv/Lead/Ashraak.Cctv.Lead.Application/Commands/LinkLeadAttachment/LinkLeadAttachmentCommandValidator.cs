using FluentValidation;

namespace Ashraak.Cctv.Lead.Application.Commands.LinkLeadAttachment;

internal sealed class LinkLeadAttachmentCommandValidator : AbstractValidator<LinkLeadAttachmentCommand>
{
    public LinkLeadAttachmentCommandValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
    }
}
