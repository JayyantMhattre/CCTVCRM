using FluentValidation;

namespace Ashraak.Cctv.Amc.Application.Commands.CreateContractTerm;

internal sealed class CreateContractTermCommandValidator : AbstractValidator<CreateContractTermCommand>
{
    public CreateContractTermCommandValidator()
    {
        RuleFor(x => x.ContractId).NotEmpty();
        RuleFor(x => x.PlanVersionId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
        RuleFor(x => x.TermType).NotEmpty();
    }
}
