using FluentValidation;

namespace Ashraak.Cctv.Customer.Application.Commands.LinkSiteDocument;

internal sealed class LinkSiteDocumentCommandValidator : AbstractValidator<LinkSiteDocumentCommand>
{
    private static readonly string[] AllowedTypes = ["Layout", "Agreement", "Other"];

    public LinkSiteDocumentCommandValidator()
    {
        RuleFor(x => x.SiteId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.DocumentType).NotEmpty().Must(t => AllowedTypes.Contains(t))
            .WithMessage("DocumentType must be Layout, Agreement, or Other.");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
