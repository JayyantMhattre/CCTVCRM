using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.LinkVisitPhoto;

internal sealed class LinkVisitPhotoCommandValidator : AbstractValidator<LinkVisitPhotoCommand>
{
    public LinkVisitPhotoCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.Category).NotEmpty().MaximumLength(32);
        RuleFor(x => x.Caption).MaximumLength(500);
    }
}
