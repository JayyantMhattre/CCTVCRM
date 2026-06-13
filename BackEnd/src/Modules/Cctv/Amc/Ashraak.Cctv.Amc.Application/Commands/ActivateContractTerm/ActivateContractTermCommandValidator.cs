using FluentValidation;

namespace Ashraak.Cctv.Amc.Application.Commands.ActivateContractTerm;

internal sealed class ActivateContractTermCommandValidator : AbstractValidator<ActivateContractTermCommand>
{
    public ActivateContractTermCommandValidator()
    {
        RuleFor(x => x.ContractId).NotEmpty();
        RuleFor(x => x.TermId).NotEmpty();
    }
}
