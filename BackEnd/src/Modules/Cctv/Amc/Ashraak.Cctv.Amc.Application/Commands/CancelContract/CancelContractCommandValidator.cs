using FluentValidation;

namespace Ashraak.Cctv.Amc.Application.Commands.CancelContract;

internal sealed class CancelContractCommandValidator : AbstractValidator<CancelContractCommand>
{
    public CancelContractCommandValidator()
    {
        RuleFor(x => x.ContractId).NotEmpty();
    }
}
