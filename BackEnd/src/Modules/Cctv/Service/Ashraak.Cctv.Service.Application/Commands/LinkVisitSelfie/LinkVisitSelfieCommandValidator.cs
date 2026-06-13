using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.LinkVisitSelfie;

internal sealed class LinkVisitSelfieCommandValidator : AbstractValidator<LinkVisitSelfieCommand>
{
    public LinkVisitSelfieCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
    }
}
