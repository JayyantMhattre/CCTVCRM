using FluentValidation;

namespace Ashraak.Cctv.Customer.Application.Commands.RemoveSiteDocument;

internal sealed class RemoveSiteDocumentCommandValidator : AbstractValidator<RemoveSiteDocumentCommand>
{
    public RemoveSiteDocumentCommandValidator()
    {
        RuleFor(x => x.SiteId).NotEmpty();
        RuleFor(x => x.DocumentId).NotEmpty();
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
