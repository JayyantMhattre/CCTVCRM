using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.LinkVisitSignature;

internal sealed class LinkVisitSignatureCommandValidator : AbstractValidator<LinkVisitSignatureCommand>
{
    public LinkVisitSignatureCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.SignerName).NotEmpty().MaximumLength(200);
    }
}
