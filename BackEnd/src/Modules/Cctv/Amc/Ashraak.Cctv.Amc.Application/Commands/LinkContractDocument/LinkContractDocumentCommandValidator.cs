using FluentValidation;

namespace Ashraak.Cctv.Amc.Application.Commands.LinkContractDocument;

internal sealed class LinkContractDocumentCommandValidator : AbstractValidator<LinkContractDocumentCommand>
{
    public LinkContractDocumentCommandValidator()
    {
        RuleFor(x => x.ContractId).NotEmpty();
        RuleFor(x => x.FileId).NotEmpty();
        RuleFor(x => x.DocumentType).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}
