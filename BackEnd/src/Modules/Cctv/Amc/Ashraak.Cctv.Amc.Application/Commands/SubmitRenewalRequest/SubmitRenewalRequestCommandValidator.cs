using FluentValidation;

namespace Ashraak.Cctv.Amc.Application.Commands.SubmitRenewalRequest;

internal sealed class SubmitRenewalRequestCommandValidator : AbstractValidator<SubmitRenewalRequestCommand>
{
    public SubmitRenewalRequestCommandValidator()
    {
        RuleFor(x => x.ContractId).NotEmpty();
        RuleFor(x => x.Message).MaximumLength(1000);
    }
}
